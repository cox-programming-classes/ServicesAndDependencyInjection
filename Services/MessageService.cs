using System.Text;
using ServicesAndDependencyInjection.Models;

namespace ServicesAndDependencyInjection.Services;

public class MessageService
{
    private readonly ApiService api;

    public MessageService(ApiService api)
    {
        this.api = api;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="content">The text content of your message</param>
    /// <param name="recipients">Who are you sending</param>
    public async Task SendMessageAsync(string content, List<string> recipients)
    {
        var rawData = Encoding.UTF8.GetBytes(content); //convert to data in UTF8 encoding
        CreateMessage newMessage = new(rawData, "text/utf8", recipients);
        var success = await api.SendOnlyAsync(
            HttpMethod.Post, "api/messages/send", newMessage);
        if (!success)
        {
            // ahhhhhh
        }
    }

    public async Task<List<RecievedMessageStub>> CheckForMessagesAsync()
    {
        var myInbox = await api.ApiCallAsync<object, List<RecievedMessageStub>>(
            HttpMethod.Get, "api/messages/inbox");
        
        //what if it went wrong???????? oh god

        return myInbox!;
    }

    public async Task<RecievedMessageRecord> OpenAMessageAsync(string id)
    {
        var message = await api.ApiCallAsync<object, RecievedMessageRecord>(
            HttpMethod.Get, $"api/messages/inbox/{id}");
        
        // no error handling, don't even worry about it bruh

        return message;
    }

    public async Task DeleteAMessage(string id)
    {
        
    }
}