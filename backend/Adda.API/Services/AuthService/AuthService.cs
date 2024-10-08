using Adda.API.Dtos;
using Adda.API.Models;
using Adda.API.Security.TokenGenerator;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Adda.API.Services.AuthService;

public class AuthService(IJwtTokenGenerator jwtTokenGenerator, UserManager<User> userManager,
    SignInManager<User> signInManager) : IAuthService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly UserManager<User> _userManager = userManager;
    public async Task<ErrorOr<AuthResponse>> LoginAsync(AuthRequest request)
    {
        try
        {

            var user = await _userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(
                    u => u.UserName.Equals(
                            request.Username
                    )
                );

            if (user == null)
            {
                return Error.Validation(description: "Invalid username or password!");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                false
            );

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                string token = _jwtTokenGenerator.GenerateToken(user.Id, user.UserName, roles);
                return new AuthResponse(user.Id, user.KnownAs, user.Gender, user.Photos.FirstOrDefault(p => p.IsMain)?.Url, token);
            }
            else
            {
                return Error.Validation(description: "Invalid username or password!");
            }
        }
        catch (Exception ex)
        {
            return Error.Validation(description: ex.Message);
        }
    }
}
