namespace Waves.Api.Models.Messanger;

public class GeeSuccessMessanger
{
    public GeeSuccessMessanger(string result)
    {
        Result = result;
    }

    public string Result { get; }
}
