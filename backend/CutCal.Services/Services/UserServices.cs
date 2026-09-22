using CutCal.Common.Services;
using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Base;
using CutCal.Services.Database;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface IUserService : IBaseCRUDService<UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest>
{
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task<UserResponse> SetAvatarUrlAsync(int userId, string profileImageUrl);
    Task<User?> GetByUsernameAsync(string username);
}

public class UserService : BaseCRUDService<User, UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest>, IUserService
{
    private readonly ICryptoService _crypto;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public UserService(CutCalDbContext context, ICryptoService crypto, IAuthenticatedUserAccessor userAccessor) : base(context)
    {
        _crypto = crypto;
        _userAccessor = userAccessor;
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return Context.Users
            .Include(x => x.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        OwnershipGuard.EnsureIsSelfOrAdmin(userId, _userAccessor);

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            throw new ClientException("New password and confirmation do not match.");
        }

        var user = await Context.Users.FindAsync(userId) ?? throw new ClientException("User not found.");

        if (!_crypto.VerifyPassword(request.OldPassword, user.PasswordHash))
        {
            throw new ClientException("Old password is incorrect.");
        }

        user.PasswordHash = _crypto.HashPassword(request.NewPassword);
        await Context.SaveChangesAsync();
    }

    public async Task<UserResponse> SetAvatarUrlAsync(int userId, string profileImageUrl)
    {
        OwnershipGuard.EnsureIsSelfOrAdmin(userId, _userAccessor);
        var user = await Context.Users.FindAsync(userId) ?? throw new ClientException("User not found.");
        user.ProfileImageUrl = profileImageUrl;
        await Context.SaveChangesAsync();
        return user.Adapt<UserResponse>();
    }

    /// <summary>
    /// Non-admins only ever see/update/delete their own account; GetPagedAsync (the list), GetByIdAsync
    /// and the entity lookup UpdateAsync/DeleteAsync use internally all go through this, so it cannot be
    /// bypassed by fetching a specific id directly.
    /// </summary>
    protected override IQueryable<User> AddSecurityFilter(IQueryable<User> query)
    {
        if (_userAccessor.IsInRole(RoleNames.Admin))
        {
            return query;
        }
        return query.Where(x => x.Id == _userAccessor.UserId);
    }

    protected override IQueryable<User> AddInclude(IQueryable<User> query)
    {
        return query.Include(x => x.UserRoles).ThenInclude(ur => ur.Role);
    }

    protected override IQueryable<User> AddFilter(IQueryable<User> query, UserSearchObject search)
    {
        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query = query.Where(x => (x.FirstName + " " + x.LastName).Contains(search.Name));
        }
        if (!string.IsNullOrWhiteSpace(search.Email))
        {
            query = query.Where(x => x.Email.Contains(search.Email));
        }
        if (!string.IsNullOrWhiteSpace(search.Username))
        {
            query = query.Where(x => x.Username.Contains(search.Username));
        }
        if (search.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == search.IsActive.Value);
        }
        return query;
    }

    protected override User MapInsertToEntity(UserInsertRequest request)
    {
        return new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = _crypto.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    protected override async Task AfterInsert(User entity, UserInsertRequest request)
    {
        var role = await Context.Roles.FirstOrDefaultAsync(x => x.Name == request.Role)
            ?? throw new ClientException($"Role '{request.Role}' does not exist.");
        Context.UserRoles.Add(new UserRole { UserId = entity.Id, RoleId = role.Id });
        await Context.SaveChangesAsync();
    }
}
