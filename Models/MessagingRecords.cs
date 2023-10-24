namespace ServicesAndDependencyInjection.Models;

/// <summary>
/// Use this to create a new Message
/// POST api/messages/send
/// </summary>
/// <param name="messageContent">raw binary data of the message</param>
/// <param name="contentType">mime content type! (look it up)</param>
/// <param name="recipients">List of email addresses to send this to</param>
/// <param name="selfDestruct">optional parameter!</param>
public record CreateMessage(byte[] messageContent, string contentType,
    List<string> recipients, DateTime? selfDestruct = null);
    

/// <summary>
/// This is the response you get when you GET a sent message.
/// GET api/messages/sent/{id}
/// </summary>
/// <param name="id"></param>
/// <param name="sender"></param>
/// <param name="messageContent"></param>
/// <param name="contentType"></param>
/// <param name="sent"></param>
/// <param name="recipients"></param>
public record struct SentMessageRecord(string id, string sender, byte[] messageContent, string contentType, DateTime sent,
    IEnumerable<string> recipients);

/// <summary>
/// When you GET all your sent messages, you get a List of these.
/// GET api/messages/sent
/// </summary>
/// <param name="id"></param>
/// <param name="sender"></param>
/// <param name="contentType"></param>
/// <param name="contentLength"></param>
/// <param name="sent"></param>
/// <param name="recipients"></param>
public record struct SentMessageStub(string id, string sender, string contentType, int contentLength, DateTime sent,
    IEnumerable<string> recipients)
{
    public static implicit operator SentMessageStub(SentMessageRecord message) =>
        new(message.id, message.sender, message.contentType, message.messageContent.Length, message.sent, message.recipients);
}

/// <summary>
/// When you GET a specific message from your Inbox, this is what you receive.
/// GET api/messages/inbox/{id}
/// </summary>
/// <param name="id"></param>
/// <param name="sender"></param>
/// <param name="messageContent"></param>
/// <param name="contentType"></param>
/// <param name="sent"></param>
/// <param name="recipient"></param>
/// <param name="read"></param>
/// <param name="hidden"></param>
public record struct RecievedMessageRecord(string id, string sender, byte[] messageContent, string contentType, DateTime sent, 
    string recipient, DateTime read, bool hidden);

/// <summary>
/// When you GET your inbox, you get a List of these.
/// GET api/messages/inbox
/// </summary>
/// <param name="id"></param>
/// <param name="sender"></param>
/// <param name="contentType"></param>
/// <param name="contentLength"></param>
/// <param name="sent"></param>
/// <param name="unread"></param>
/// <param name="hidden"></param>
public record struct RecievedMessageStub(string id, string sender, 
    string contentType, int contentLength, DateTime sent, bool unread, bool hidden)
{
    public static implicit operator RecievedMessageStub(RecievedMessageRecord message) =>
        new(message.id, message.sender, message.contentType, message.messageContent.Length,
            message.sent, message.read == default, message.hidden);
}