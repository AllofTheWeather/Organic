using Organic.Application.Dtos.SocialMediaAccountDtos;

namespace Organic.Application.Services.Interfaces
{
	public interface ISocialMediaAccountService
	{
		Task<Guid> CreateSocialMediaAccountAsync(CreateSocialMediaAccountDto dto);
		Task<SocialMediaAccountDto> GetSocialMediaAccountByIdAsync(Guid id);
		Task<bool> UpdateSocialMediaAccountAsync(UpdateSocialMediaAccountDto dto);
		Task<bool> DeleteSocialMediaAccountAsync(Guid id);
		Task<IEnumerable<SocialMediaAccountDto>> GetSocialMediaAccountsByCampaignAsync(Guid campaignId);
	}
}
