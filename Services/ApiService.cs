using System.Net.Http.Headers;
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

    /// <summary>
    /// Attempt to Login using a given email and password.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    public void Login(string email, string password)
    {
        var login = new LoginRecord(email, password);
        // TODO:  What does the `out` keyword mean?
        /*
         * The naming convention 'Try....()' on method headers
         * Is used to indicate that this method will return a
         * boolean success flag. Try... methods also always
         * include an `out` parameter for the successful result
         */
        /*
         * Hover your mouse over "TryApiCall"
         * What are TIn and TOut?
         * Where does that information come from?
         */
        if (!TryApiCall(HttpMethod.Post, "api/auth", login,
                out _authResponse, out ErrorRecord? error))
        {
            //Login was not Successful
            Console.WriteLine($"{error.type} : {error.error}");
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
        bool authorize = false)
    {
        throw new NotImplementedException("You gotta write this!");
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
        bool authorize = false)
    {
        throw new NotImplementedException("You gotta write this!");
    }
}