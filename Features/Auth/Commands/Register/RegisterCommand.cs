using MediatR;
using Microsoft.AspNetCore.Identity;
using MyApp.DTOs.Identity;
using MyApp.Models;

namespace MyApp.Features.Auth.Commands.Register;

public record RegisterCommand(RegisterDto Dto) : IRequest<IdentityResult>;

public class RegisterCommandHandler(UserManager<ApplicationUser> _userManager) 
    : IRequestHandler<RegisterCommand, IdentityResult>
{
    public async Task<IdentityResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var user = new ApplicationUser()
        {
            Email = dto.Email,
            UserName = dto.Email.Split('@')[0]
        };
        return await _userManager.CreateAsync(user, dto.Password);
    }
}
