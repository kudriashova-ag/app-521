using MediatR;
using Microsoft.AspNetCore.Identity;
using MyApp.DTOs.Identity;
using MyApp.Models;

namespace MyApp.Features.Auth.Commands.Register;

public record LoginRegisterCommand(RegisterDto Dto) : IRequest<LoginResult>;

public class LoginRegisterCommandHandler( ISender _mediator) 
    : IRequestHandler<LoginRegisterCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginRegisterCommand request, CancellationToken ct)
    {
        var dto = request.Dto;
        await _mediator.Send(new RegisterCommand(dto), ct);
        

        var loginResult = await _mediator.Send(new LoginCommand(new LoginDto(dto.Email, dto.Password)), ct);

        return loginResult;
    }
}
