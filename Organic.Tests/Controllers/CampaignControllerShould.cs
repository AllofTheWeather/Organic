using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organic.Api.Controllers;
using Organic.Application.Dtos.CampaignDtos;
using Organic.Application.Services.Campaigns;
using Organic.Application.Services.Interfaces;
using Organic.Core.Entities;
using Shouldly;

namespace Organic.Tests.Controllers
{
    public class CampaignControllerShould : IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly CampaignService _campaignService;
		private readonly CampaignController _campaignController;

		public CampaignControllerShould()
		{
			// Setup in-memory database with a unique name for each test case
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_campaignService = new CampaignService(_context);
			_campaignController = new CampaignController(_campaignService);
		}

		[Fact]
		public async Task CreateCampaign_ShouldReturnOkResult_WhenCampaignIsCreated()
		{
			Guid createdByID = Guid.NewGuid();
			// Arrange
			var dto = new CreateCampaignDto
			{
				Name = "Test Campaign",
				Description = "A test campaign",
				CreatedById = createdByID,
			};

			// Act
			var result = await _campaignController.CreateCampaign(dto);

			// Assert
			var okResult = result.ShouldBeOfType<OkObjectResult>();
			var campaignResponse = okResult.Value.ShouldBeOfType<CreateCampaignResponseDto>();
			var campaignId = campaignResponse.CampaignId;
			var campaign = await _context.Campaigns.FindAsync(campaignId);
			campaign.ShouldNotBeNull();
			campaign.Name.ShouldBe("Test Campaign");
		}

		[Fact]
		public async Task GetCampaign_ShouldReturnOkResult_WhenCampaignExists()
		{
			Guid campaignId = Guid.NewGuid();
			Guid createdById = Guid.NewGuid();
			// Arrange
			var campaign = new Campaign
			{
				Id = campaignId,
				Name = "Existing Campaign",
				Description = "Existing campaign description",
				CreatedAt = DateTime.Now,
				CreatedById = createdById
			};
			_context.Campaigns.Add(campaign);
			await _context.SaveChangesAsync();

			// Act
			var result = await _campaignController.GetCampaign(campaignId);

			// Assert
			var okResult = result.ShouldBeOfType<OkObjectResult>();
			var returnValue = okResult.Value.ShouldBeOfType<CampaignDto>();
			returnValue.Id.ShouldBe(campaignId);
			returnValue.Name.ShouldBe("Existing Campaign");
		}

		[Fact]
		public async Task GetCampaign_ShouldReturnNotFound_WhenCampaignDoesNotExist()
		{
			// Act
			var result = await _campaignController.GetCampaign(Guid.NewGuid());

			// Assert
			result.ShouldBeOfType<NotFoundResult>();
		}

		[Fact]
		public async Task AddSocialMediaAccountToCampaign_ShouldReturnOkResult_WhenAccountIsAdded()
		{
			Guid campaignId = Guid.NewGuid();
			Guid createdById = Guid.NewGuid();
			// Arrange
			var campaign = new Campaign
			{
				Id = campaignId,
				Name = "Campaign with Social Media",
				Description = "Campaign description",
				CreatedAt = DateTime.Now,
				CreatedById = createdById,
			};
			_context.Campaigns.Add(campaign);
			await _context.SaveChangesAsync();

			var accountDto = new AddSocialMediaAccountToCampaignDto
			{
				CampaignId = campaignId,
				PlatformName = "Instagram",
				Username = "testuser",
				AccessToken = "access-token",
				RefreshToken = "refresh-token",
				TokenExpiry = DateTime.Now.AddHours(1)
			};

			// Act
			var result = await _campaignController.AddSocialMediaAccountToCampaign(accountDto);

			// Assert
			result.ShouldBeOfType<OkObjectResult>();
			var socialMediaAccount = await _context.SocialMediaAccounts.FirstOrDefaultAsync();
			socialMediaAccount.ShouldNotBeNull();
			socialMediaAccount.CampaignId.ShouldBe(campaignId);
			socialMediaAccount.PlatformName.ShouldBe("Instagram");
			socialMediaAccount.Username.ShouldBe("testuser");
		}
		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
