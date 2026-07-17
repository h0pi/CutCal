namespace CutCal.Common.Services;

public interface ICryptoService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
