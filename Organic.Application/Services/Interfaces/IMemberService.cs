using Microsoft.AspNetCore.Identity;
using Organic.Application.Dtos.ApplicationUserDtos;

namespace Organic.Core.Services.Interfaces
{
	public interface IApplicationUserService
	{
		Task<IdentityResult> RegisterApplicationUserAsync(RegisterApplicationUserDto dto);
		Task<SignInResult> LoginApplicationUserAsync(LoginApplicationUserDto dto);
		Task<ApplicationUserDto> GetApplicationUserDetailsAsync(Guid memberId);
		Task<IdentityResult> UpdateApplicationUserDetailsAsync(UpdateApplicationUserDto dto);
	}
}