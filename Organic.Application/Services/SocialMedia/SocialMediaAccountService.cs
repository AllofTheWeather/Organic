using Microsoft.EntityFrameworkCore;
using Organic.Application.Dtos.SocialMediaAccountDtos;
using Organic.Application.Services.Interfaces;
using Organic.Core.Entities;
using Organic.Core.Interfaces;

namespace Organic.Application.Services.SocialMedia
{
    public class SocialMediaAccountService : ISocialMediaAccountService
    {
        private readonly ApplicationDbContext _context;

        public SocialMediaAccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create Social Media Account
        public async Task<Guid> CreateSocialMediaAccountAsync(CreateSocialMediaAccountDto dto)
        {
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

            return socialMediaAccount.Id;
        }

        // Get Social Media Account by ID
        public async Task<SocialMediaAccountDto> GetSocialMediaAccountByIdAsync(Guid id)
        {
            var account = await _context.SocialMediaAccounts
                .FirstOrDefaultAsync(sma => sma.Id == id);

            if (account == null) return null;

            return new SocialMediaAccountDto
            {
                Id = account.Id,
                PlatformName = account.PlatformName,
                Username = account.Username,
                TokenExpiry = account.TokenExpiry,
                CampaignId = account.CampaignId
            };
        }

        // Get Social Media Accounts by Campaign
        public async Task<IEnumerable<SocialMediaAccountDto>> GetSocialMediaAccountsByCampaignAsync(Guid campaignId)
        {
            return await _context.SocialMediaAccounts
                .Where(sma => sma.CampaignId == campaignId)
                .Select(sma => new SocialMediaAccountDto
                {
                    Id = sma.Id,
                    PlatformName = sma.PlatformName,
                    Username = sma.Username,
                    TokenExpiry = sma.TokenExpiry,
                    CampaignId = sma.CampaignId
                }).ToListAsync();
        }

        // Update Social Media Account
        public async Task<bool> UpdateSocialMediaAccountAsync(UpdateSocialMediaAccountDto dto)
        {
            var account = await _context.SocialMediaAccounts
                .FirstOrDefaultAsync(sma => sma.Id == dto.Id);

            if (account == null) return false;

            account.PlatformName = dto.PlatformName;
            account.Username = dto.Username;
            account.AccessToken = dto.AccessToken;
            account.RefreshToken = dto.RefreshToken;
            account.TokenExpiry = dto.TokenExpiry;
            account.CampaignId = dto.CampaignId;

            _context.SocialMediaAccounts.Update(account);
            await _context.SaveChangesAsync();

            return true;
        }

        // Delete Social Media Account
        public async Task<bool> DeleteSocialMediaAccountAsync(Guid id)
        {
            var account = await _context.SocialMediaAccounts.FirstOrDefaultAsync(sma => sma.Id == id);

            if (account == null) return false;

            _context.SocialMediaAccounts.Remove(account);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
