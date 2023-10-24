using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ServicesAndDependencyInjection.Models;

namespace ServicesAndDependencyInjection.Services;

public class ApiService
{
    /// <summary>
    /// This is not the ideal way to do this in production
    /// Injecting the HttpClient in this way is ONLY for this
    /// Console Application Example.
    /// </summary>
    private readonly HttpClient _client;

    /// <summary>
    /// Once you are logged in, this will be where to store your
    /// authorization info.
    /// </summary>
    private AuthResponse _authResponse = default;

    /// <summary>
    /// Generate the Authorization Header from your logged in
    /// credentials saved in _authResponse.
    /// </summary>
    private AuthenticationHeaderValue AuthHeader =>
        new("Bearer", _authResponse.jwt);

    /// <summary>
    /// This service depends on having an HttpClient
    /// in this very rudimentary example of Dependency Injection
    /// we are directly injecting the HttpClient into the service
    /// (this is not good practice in production, but I'm hiding
    /// some gorey details that aren't important right now.)
    /// </summary>
    /// <param name="client"></param>
    public ApiService(HttpClient client)
    {
        _client = client;
    }

    private LoginRecord savedCredential = new ("jcox@winsor.edu", "8iwvqcNAj_TXwqi!");
    public async Task<bool> RenewAuth()
    {
        if (string.IsNullOrEmpty(_authResponse.jwt))
        {
            var loginSuccessful = await LoginAsync(savedCredential.email, savedCredential.password);
            return loginSuccessful;
        }

        _authResponse = await ApiCallAsync<object, AuthResponse>(
            HttpMethod.Get, $"api/auth/renew?refreshToken={_authResponse.refreshToken}",
            null, authorize: true);
        
        return _authResponse != default;
    }
    
    public async Task<bool> LoginAsync(string email = "", string password = "") 
    {
        var login = string.IsNullOrEmpty(email) ?
            savedCredential :
            new(email, password);
        
        if (!TryApiCall(HttpMethod.Post, "api/auth", login,
                out _authResponse, out var error, false))
        {
            //Login was not Successful
            Console.WriteLine($"{error.type} : {error.error}");
            return false;
        }

        return true;
    }

    public async Task<TOut?> ApiCallAsync<TIn, TOut>(
        HttpMethod method, string endpoint,
        TIn? content = default, bool authorize = true)
    {
        var request = new HttpRequestMessage(method, endpoint);
        bool hasRetried = false;
        RetryRequest:
        
        if (authorize)
            request.Headers.Authorization = AuthHeader;

        if (content is not null)
        {
            var inJson = JsonSerializer.Serialize(content);
            request.Content = new StringContent(inJson, Encoding.UTF8, "application/json");
        }

        var response = await _client.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized && !hasRetried)
        {
            var successful = await RenewAuth();
            if (!successful)
            {
                Debug.WriteLine("Failed to renew authorization!");
                return default;
            }

            hasRetried = true;
            goto RetryRequest;
        }
        // If the token is valid, but still Unauthorized
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            Debug.WriteLine("You're not allowed to do that!");
            return default;
        }
        
        // I have a non-unauthorized response.
        var json = await response.Content.ReadAsStringAsync();
        // check for more errors.
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = JsonSerializer.Deserialize<ErrorRecord>(json)!;
                Debug.WriteLine($"{error.type} : {error.error}");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Something went wrong, but I didn't get an error message.");
                Debug.WriteLine(e);
            }

            return default;
        }
        
        // The response was successful! Yay~!
        try
        {
            var result = JsonSerializer.Deserialize<TOut>(json);
            return result;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return default;
        }
    }
    
    /// <summary>
    /// Attempt to make an API Call (that does not have body content)
    /// If the call is successful, deserialize the `result` and null `error`
    /// If the call is NOT successful, null `result` and deserialize the `error`
    /// </summary>
    /// <param name="method">What HTTP Verb?</param>
    /// <param name="endpoint">relative URL that you are accessing</param>
    /// <param name="result">If the call is successful, store the result here.</param>
    /// <param name="error">If the call is NOT successful, store the error here.</param>
    /// <param name="authorize">does this API call require authorization?</param>
    /// <typeparam name="TOut">The DataType of the expected result</typeparam>
    /// <returns>True if and only if result is not null.</returns>
    public bool TryApiCall<TOut>(
        HttpMethod method, string endpoint, 
        out TOut? result, out ErrorRecord? error,
        bool authorize = true)
    {
        var request = new HttpRequestMessage(method, endpoint);
        if (authorize)
            request.Headers.Authorization = AuthHeader;

        var response = _client.Send(request);
        var json = response.Content.ReadAsStringAsync().Result;
        if (response.IsSuccessStatusCode)
        {
            result = JsonSerializer.Deserialize<TOut>(json);
            error = null;
            return true;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Handle Gracefully!
        }
        
        try
        {
            error = JsonSerializer.Deserialize<ErrorRecord>(json);
        }
        catch
        {
            error = new("Unknown Error", json);
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Try to make an API Call that Has Body Content.
    /// Serialize the `content` object using JsonSerializer
    /// and add it to the request as StringContent.
    ///
    /// If the call is successful, deserialize the `result` and null `error`
    /// If the call is NOT successful, null `result` and deserialize the `error`
    /// </summary>
    /// <param name="method">POST or PUT only!</param>
    /// <param name="endpoint">relative URL of the endpoint</param>
    /// <param name="content">The data that will be placed in the request Content.</param>
    /// <param name="result">If the call is successful, store the result here.</param>
    /// <param name="error">If the call is NOT successful, store the error here.</param>
    /// <param name="authorize">does this API call require authorization?</param>
    /// <typeparam name="TIn">DataType of the request Content</typeparam>
    /// <typeparam name="TOut">DataType of the response Content on success.</typeparam>
    /// <returns>True if and only if `result` is not null.</returns>
    public bool TryApiCall<TIn, TOut>(
        HttpMethod method, string endpoint,
        TIn content,
        out TOut? result, out ErrorRecord? error,
        bool authorize = true)
    {
        
        var request = new HttpRequestMessage(method, endpoint);
        if (authorize)
            request.Headers.Authorization = AuthHeader;

        var inJson = JsonSerializer.Serialize(content);
        request.Content = new StringContent(inJson, Encoding.UTF8, "application/json");
        
        var response = _client.Send(request);
        var json = response.Content.ReadAsStringAsync().Result;
        if (response.IsSuccessStatusCode)
        {
            result = JsonSerializer.Deserialize<TOut>(json);
            error = null;
            return true;
        }

        try
        {
            error = JsonSerializer.Deserialize<ErrorRecord>(json);
        }
        catch
        {
            error = new("Unknown Error", json);
        }

        result = default;
        return false;
    }
}