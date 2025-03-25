using Application.DTO.Request;
using Application.DTO.Response;
using Domain.Models;

namespace Application.Interface
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserRequest userRequest, CancellationToken cancellationToken);
        Task DeleteUserAsync(int id, CancellationToken cancellationToken);
        Task<UserResponse> GetUserByIdAsync(int id, CancellationToken cancellationToken);
        Task UpdateUserAsync(int id, UserRequest userRequest, CancellationToken cancellationToken);
        Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
    }
}