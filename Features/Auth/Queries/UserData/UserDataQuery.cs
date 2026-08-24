using MediatR;
using Microsoft.AspNetCore.Identity;
using MyApp.Models;
using System.Security.Claims;


namespace MyApp.Features.Auth.Commands.Register;

public record UserDataQuery() : IRequest<string>;

public class UserDataQueryHandler(
    UserManager<ApplicationUser> _userManager,
    IHttpContextAccessor _httpContextAccessor
    )
    : IRequestHandler<UserDataQuery, string>
{
    public async Task<string> Handle(UserDataQuery request, CancellationToken cancellationToken)
    {
        var userClaims = _httpContextAccessor.HttpContext!.User;
        var id = userClaims.FindFirstValue("sub");
        var user = await _userManager.FindByIdAsync(id);
        var roles = await _userManager.GetRolesAsync(user);
        return user.Email;
    }
}
