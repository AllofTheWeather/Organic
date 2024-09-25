using Organic.Application.Dtos.CampaignDtos;

namespace Organic.Application.Services.Interfaces
{
    public interface ICampaignService
    {
        Task AddSocialMediaAccountToCampaignAsync(AddSocialMediaAccountToCampaignDto dto);
        Task<Guid> CreateCampaignAsync(CreateCampaignDto dto);
        Task<CampaignDto> GetCampaignAsync(Guid campaignId);
        Task DeleteCampaignAsync(Guid campaignId);
    }
}