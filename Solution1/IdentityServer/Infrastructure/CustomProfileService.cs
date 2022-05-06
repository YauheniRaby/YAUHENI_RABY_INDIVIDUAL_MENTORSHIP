using DataAccessLayer.Models;
using DataAccessLayer.Repositories.Abstract;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Infrastructure
{
    public class CustomProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CustomProfileService> _logger;

        public CustomProfileService(IUserRepository userRepository, ILogger<CustomProfileService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var isSuccessParse = int.TryParse(context.Subject.GetSubjectId(), out int result);

            if (isSuccessParse)
            {
                var user = _userRepository.GetByIdAsync(result);

                var claims = new List<Claim>()
                {
                    new Claim(JwtClaimTypes.Name, user.Name),
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.Role, user.Role.ToString())
                };

                context.IssuedClaims = claims;
            }
            else
            {
                _logger.LogError($"Claims unformed. Invalid value for {nameof(User.Id)}");
            }
            return Task.CompletedTask;
        }
    }
}
