using DotnetApi.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace DotnetApi.Application.Authentication.Command;

public class LoginCommandHandler(IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IValidator<LoginCommand> validator): IRequestHandler<LoginCommand, string?>
{
    public async Task<string?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validateResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validateResult.IsValid)
        {
            throw new ValidationException(validateResult.Errors);
        }
        var user = await userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        
        if(user is null)
        {
            throw new ValidationException("User not found");
        }

        if(!passwordHasher.Verify(request.Password, user.Password))
        {
            throw new ValidationException("Password do not mach");
        }
        
        var token = jwtTokenGenerator.GenerateToken(user);
        return token;
    }
}