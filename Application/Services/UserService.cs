using Application.DTO.Request;
using Application.DTO.Response;
using Application.Interface;
using Domain.Data;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<UserResponse> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
                throw new Exception("User not found.");

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surename = user.Surename,
                BirthDate = user.BirthDate,
                CreatedDate = user.CreatedDate,
                Email = user.Email
            };

            return userResponse;
        }
        public async Task<UserResponse> CreateUserAsync(UserRequest userRequest)
        {

            if (await _userRepository.UserExistsByNameAsync(userRequest.Name))
            {
                throw new Exception("User with this name already exist.");
            }
                var user = new User
            {
                Name = userRequest.Name,
                Surename = userRequest.Surename,
                BirthDate = userRequest.BirthDate,
                CreatedDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Email = userRequest.Email,
                Role = Enum.Parse<RoleCategory>(userRequest.Role)
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, userRequest.Password);

            await _userRepository.AddAsync(user);

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surename = user.Surename,
                BirthDate = user.BirthDate,
                CreatedDate = user.CreatedDate,
                Email = user.Email
            };

            return userResponse;
        }
        public async Task UpdateUserAsync(int id, UserRequest userRequest)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
                throw new Exception("User not found.");

            user.Name = userRequest.Name;
            user.Surename = userRequest.Surename;
            user.BirthDate = userRequest.BirthDate;
            user.Email = userRequest.Email;
            user.Role = Enum.Parse<RoleCategory>(userRequest.Role);

            await _userRepository.UpdateAsync(user);
        }
        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByName(username);
            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }
    }
}
