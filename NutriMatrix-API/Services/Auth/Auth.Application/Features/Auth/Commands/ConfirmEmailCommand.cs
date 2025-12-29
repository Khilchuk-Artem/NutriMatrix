
using Auth.Application.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Features.Auth.Commands
{
    public class ConfirmEmailCommand : IRequest<bool>
    {
        public RequestConfirmEmailDTO RequestConfirmEmailDTO { get; set; }
    }

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var dto = request.RequestConfirmEmailDTO;
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
            return result.Succeeded;
        }
    }
}
