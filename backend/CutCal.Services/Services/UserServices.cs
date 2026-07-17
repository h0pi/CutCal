using CutCal.Common.Services;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Base;
using CutCal.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface IUserService : IBaseCRUDService<UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest>
{
    Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task<User?> GetByUsernameAsync(string username);
}

public class UserService : BaseCRUDService<User, UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest>, IUserService
{
    private readonly ICryptoService _crypto;

    public UserService(CutCalDbContext context, ICryptoService crypto) : base(context)
    {
        _crypto = crypto;
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return Context.Users
            .Include(x => x.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
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
