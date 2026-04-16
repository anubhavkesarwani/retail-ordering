using RetailOrdering.Application.DTOs;
using RetailOrdering.Core.Entities;
using RetailOrdering.Core.Interfaces;

namespace RetailOrdering.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public AuthService(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        if (await _userRepository.ExistsAsync(request.Email))
            return null;

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var created = await _userRepository.CreateAsync(user);

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(created),
            RefreshToken = _jwtService.GenerateRefreshToken(),
            User = MapToUserDto(created)
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(user),
            RefreshToken = _jwtService.GenerateRefreshToken(),
            User = MapToUserDto(user)
        };
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        };
    }
}
