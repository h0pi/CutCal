using CutCal.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace CutCal.WebAPI.Auth;

public interface ITokenRevocationService
{
    Task<bool> IsRevokedAsync(string tokenId);
}

public class TokenRevocationService : ITokenRevocationService
{
    private readonly CutCalDbContext _context;

    public TokenRevocationService(CutCalDbContext context)
    {
        _context = context;
    }

    public Task<bool> IsRevokedAsync(string tokenId)
    {
        return _context.RevokedTokens.AsNoTracking().AnyAsync(x => x.Jti == tokenId);
    }
}
