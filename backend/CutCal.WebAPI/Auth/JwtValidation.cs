using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CutCal.WebAPI.Auth;

public static class JwtValidation
{
    /// <summary>HttpContext.Items key under which the signature-validated token is exposed to controllers.</summary>
    public const string ValidatedTokenItem = "ValidatedAccessToken";

    /// <summary>Rejects tokens that were revoked by logout, even though their signature and lifetime are still valid.</summary>
    public static async Task OnTokenValidated(TokenValidatedContext context)
    {
        var token = context.SecurityToken;
        context.HttpContext.Items[ValidatedTokenItem] = token;

        if (string.IsNullOrEmpty(token.Id))
        {
            return;
        }

        var revocation = context.HttpContext.RequestServices.GetRequiredService<ITokenRevocationService>();
        if (await revocation.IsRevokedAsync(token.Id))
        {
            context.Fail("This session has been logged out.");
        }
    }
}
