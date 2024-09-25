using Microsoft.EntityFrameworkCore;
using Organic.Application.Dtos.CampaignDtos;
using Organic.Application.Services.Campaigns;
using Organic.Core.Entities;
using Organic.Core.Interfaces;
using Shouldly;

namespace Organic.Tests.Services
{
    public class CampaignServiceShould : IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly CampaignService _campaignService;

		public CampaignServiceShould()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_campaignService = new CampaignService(_context);
		}

		[Fact]
		public async Task CreateCampaignAsync_ShouldCreateCampaign_WhenValidInput()
		{
			var createdById = Guid.NewGuid();
			// Arrange
			var dto = new CreateCampaignDto
			{
				Name = "Test Campaign",
				Description = "A test campaign",
				CreatedById = createdById,
			};

			// Act
			var campaignId = await _campaignService.CreateCampaignAsync(dto);

			// Assert
			var campaign = await _context.Campaigns.FindAsync(campaignId);
			campaign.ShouldNotBeNull();
			campaign.Name.ShouldBe("Test Campaign");
			campaign.Description.ShouldBe("A test campaign");
			campaign.CreatedById.ShouldBe(createdById);
		}

		[Fact]
		public async Task GetCampaignAsync_ShouldReturnCampaign_WhenCampaignExists()
		{
			var id = Guid.NewGuid();
			var createdById = Guid.NewGuid();
			// Arrange
			var campaign = new Campaign
			{
				Id = id,
				Name = "Existing Campaign",
				Description = "Existing campaign description",
				CreatedAt = DateTime.Now,
				CreatedById = createdById
			};
			_context.Campaigns.Add(campaign);
			await _context.SaveChangesAsync();

			// Act
			var result = await _campaignService.GetCampaignAsync(id);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(id);
			result.Name.ShouldBe("Existing Campaign");
		}

		[Fact]
		public async Task GetCampaignAsync_ShouldReturnNull_WhenCampaignDoesNotExist()
		{
			// Act
			var result = await _campaignService.GetCampaignAsync(Guid.NewGuid());

			// Assert
			result.ShouldBeNull();
		}

		[Fact]
		public async Task AddSocialMediaAccountToCampaignAsync_ShouldAddAccount_WhenCampaignExists()
		{
			var id = Guid.NewGuid();
			var createdById = Guid.NewGuid();
			// Arrange
			var campaign = new Campaign
			{
				Id = id,
				Name = "Campaign with Social Media",
				Description = "Campaign description",
				CreatedAt = DateTime.Now,
				CreatedById = createdById,
			};
			_context.Campaigns.Add(campaign);
			await _context.SaveChangesAsync();

			var accountDto = new AddSocialMediaAccountToCampaignDto
			{
				CampaignId = id,
				PlatformName = "Instagram",
				Username = "testuser",
				AccessToken = "access-token",
				RefreshToken = "refresh-token",
				TokenExpiry = DateTime.Now.AddHours(1)
			};

			// Act
			await _campaignService.AddSocialMediaAccountToCampaignAsync(accountDto);

			// Assert
			var socialMediaAccount = await _context.SocialMediaAccounts.FirstOrDefaultAsync();
			socialMediaAccount.ShouldNotBeNull();
			socialMediaAccount.CampaignId.ShouldBe(id);
			socialMediaAccount.PlatformName.ShouldBe("Instagram");
			socialMediaAccount.Username.ShouldBe("testuser");
		}
		[Fact]
		public async Task DeleteCampaignAsync_ShouldDeleteCampaignAndAssociatedSocialMediaAccounts()
		{
			var id = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			var createdById = Guid.NewGuid();
			// Arrange
			var member = new ApplicationUser { Id = createdById, FullName = "Test User", Email = "Test Email" };
			var campaign = new Campaign
			{
				Id = id,
				Name = "Test Campaign",
				Description = "Testing campaign",
				CreatedById = createdById,
				ApplicationUsers = new List<ApplicationUser> { member },
				SocialMediaAccounts = new List<SocialMediaAccount>
				{
					new SocialMediaAccount
					{
						Id = id2,
						PlatformName = "Instagram",
						Username = "testuser",
						AccessToken = "dummy-access-token",   // Provide valid test data
						RefreshToken = "dummy-refresh-token"  // Provide valid test data
					}
				}
			};

			_context.Campaigns.Add(campaign);
			await _context.SaveChangesAsync();

			// Act
			await _campaignService.DeleteCampaignAsync(campaign.Id);

			// Assert
			var deletedCampaign = await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == campaign.Id);
			deletedCampaign.ShouldBeNull();

			var associatedSocialMediaAccount = await _context.SocialMediaAccounts.FirstOrDefaultAsync(sma => sma.Id == id2);
			associatedSocialMediaAccount.ShouldBeNull();

			var associatedApplicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(m => m.Id == createdById);
			associatedApplicationUser.ShouldNotBeNull(); // ApplicationUser should NOT be deleted

			var memberCampaigns = await _context.Campaigns
				.Where(c => c.ApplicationUsers.Any(m => m.Id == createdById))
				.ToListAsync();

			memberCampaigns.ShouldBeEmpty(); // The member should no longer be associated with any campaigns
		}


		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
