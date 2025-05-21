using WebApiHttpExample.Models;

namespace WebApiHttpExample.Repositories;

public interface IUsersRepository
{
    public Task<User> GetUserByIdFromDatabaseAsync(int id);

    public Task<User> CreateUserInDatabaseAsync(User user);

    public Task<bool> UpdateUserInDatabaseAsync(User user);

    public Task<bool> PartiallyUpdateUserInDatabaseAsync(int id, User user);

    public Task<bool> DeleteUserByIdInDatabaseAsync(int id);
}

public class UsersRepository : IUsersRepository
{
    public async Task<User> GetUserByIdFromDatabaseAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<User> CreateUserInDatabaseAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateUserInDatabaseAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> PartiallyUpdateUserInDatabaseAsync(int id, User user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteUserByIdInDatabaseAsync(int id)
    {
        throw new NotImplementedException();
    }
}
