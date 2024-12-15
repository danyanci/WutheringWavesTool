namespace Waves.Api.Models.Messanger;

public class LoginMessanger
{
    public LoginMessanger(bool isSuccess, string token, long id)
    {
        IsSuccess = isSuccess;
        Token = token;
        Id = id;
    }

    public bool IsSuccess { get; }
    public string Token { get; }

    public long Id { get; set; }
}
