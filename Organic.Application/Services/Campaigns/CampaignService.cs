using Microsoft.EntityFrameworkCore;
using Organic.Application.Dtos.CampaignDtos;
using Organic.Application.Dtos.ApplicationUserDtos;
using Organic.Application.Dtos.SocialMediaAccountDtos;
using Organic.Application.Services.Interfaces;
using Organic.Core.Entities;

namespace Organic.Application.Services.Campaigns
{
    public class CampaignService : ICampaignService
    {
        private readonly ApplicationDbContext _context;

        public CampaignService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateCampaignAsync(CreateCampaignDto dto)
        {
            var campaign = new Campaign
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.Now,
                CreatedById = dto.CreatedById
            };

            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();

            return campaign.Id;
        }

        public async Task<CampaignDto?> GetCampaignAsync(Guid campaignId)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.ApplicationUsers)
                .Include(c => c.SocialMediaAccounts)
                .FirstOrDefaultAsync(c => c.Id == campaignId);

            if (campaign == null)
            {
                return null;
            }

            return new CampaignDto
            {
                Id = campaign.Id,
                Name = campaign.Name,
                Description = campaign.Description,
                CreatedAt = campaign.CreatedAt,
                CreatedById = campaign.CreatedById,
                SocialMediaAccounts = campaign.SocialMediaAccounts.Select(sma => new SocialMediaAccountDto
                {
                    Id = sma.Id,
                    PlatformName = sma.PlatformName,
                    Username = sma.Username
                }).ToList(),
                ApplicationUsers = campaign.ApplicationUsers.Select(m => new ApplicationUserDto
                {
                    Id = m.Id,
                    FullName = m.FullName
                }).ToList()
            };
        }

        public async Task AddSocialMediaAccountToCampaignAsync(AddSocialMediaAccountToCampaignDto dto)
        {
            var campaign = await _context.Campaigns.FindAsync(dto.CampaignId);
            if (campaign == null)
            {
                throw new Exception("Campaign not found");
            }

            var socialMediaAccount = new SocialMediaAccount
            {
                PlatformName = dto.PlatformName,
                Username = dto.Username,
                AccessToken = dto.AccessToken,
                RefreshToken = dto.RefreshToken,
                TokenExpiry = dto.TokenExpiry,
                CampaignId = dto.CampaignId
            };

            _context.SocialMediaAccounts.Add(socialMediaAccount);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCampaignAsync(Guid campaignId)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.ApplicationUsers)
                .Include(c => c.SocialMediaAccounts)
                .FirstOrDefaultAsync(c => c.Id == campaignId);

            if (campaign != null)
            {
                campaign.ApplicationUsers.Clear();

                _context.Campaigns.Remove(campaign);

                await _context.SaveChangesAsync();
            }
        }

    }
}
