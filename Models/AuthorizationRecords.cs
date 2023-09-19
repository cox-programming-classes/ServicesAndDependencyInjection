using System.Text.Json;

namespace ServicesAndDependencyInjection.Models;

public record struct LoginRecord(string email, string password)
{
    /// <summary>
    /// Really the only reason you'll ever want to convert this
    /// to a String is to use it as Json Content.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => JsonSerializer.Serialize(this);
}
public record struct AuthResponse(string userId, string jwt,
    DateTime expires, string refreshToken);