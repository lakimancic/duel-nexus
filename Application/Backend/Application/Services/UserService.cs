using AutoMapper;
using Backend.Application.DTOs.Auth;
using Backend.Application.DTOs.Users;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Backend.Data.UnitOfWork;
using Backend.Utils.Security;

namespace Backend.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper, PasswordHasher passwordHasher) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly PasswordHasher _passwordHasher = passwordHasher;

    public async Task<bool> ExistsAsync(string email)
    {
        return await _unitOfWork.Users.ExistsAsync(email);
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
        if (user == null || !_passwordHasher.Verify(loginDto.Password, user.Password))
            return null;
        return _mapper.Map<User, UserDto>(user);
    }

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        User user = _mapper.Map<RegisterDto, User>(registerDto);
        var passwordHash = _passwordHasher.Hash(user.Password);
        user.Password = passwordHash;
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<User, UserDto>(user);
    }
}
