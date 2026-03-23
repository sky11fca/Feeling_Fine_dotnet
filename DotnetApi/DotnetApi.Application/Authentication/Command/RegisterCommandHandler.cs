using DotnetApi.Application.Abstractions;
using DotnetApi.Application.Authentication.Command;
using DotnetApi.Domains.Entities;
using FluentValidation;
using MediatR;

namespace DotnetApi.Application.Authentication.Command;

public class RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IValidator<RegisterCommand> validator): IRequestHandler<RegisterCommand, Domains.Entities.User?>
{
    public async Task<Domains.Entities.User?> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var hashedPassword = passwordHasher.Hash(request.Password);
        
        
        var user = Domains.Entities.User.Create(
            request.BusinessId, 
            request.Username, 
            request.Email, 
            hashedPassword,
            request.UserRole
            );

        await userRepository.AddUserAsync(user, cancellationToken);
        return user;
    }
}