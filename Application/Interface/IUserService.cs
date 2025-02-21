using Application.DTO.Request;
using Application.DTO.Response;
using Domain.Models;

namespace Application.Interface
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserRequest userRequest);
        Task DeleteUserAsync(int id);
        Task<UserResponse> GetUserByIdAsync(int id);
        Task UpdateUserAsync(int id, UserRequest userRequest);
        Task<User> AuthenticateAsync(string username, string password);
    }
}