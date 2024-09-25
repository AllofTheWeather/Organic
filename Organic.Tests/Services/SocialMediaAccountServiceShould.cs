using Microsoft.EntityFrameworkCore;
using Organic.Application.Dtos.SocialMediaAccountDtos;
using Organic.Application.Services.SocialMedia;
using Organic.Core.Entities;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Tests.Services
{
    public class SocialMediaAccountServiceShould
	{
		private readonly ApplicationDbContext _context;
		private readonly SocialMediaAccountService _socialMediaAccountService;

		public SocialMediaAccountServiceShould()
		{
			// Setup the in-memory database
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_socialMediaAccountService = new SocialMediaAccountService(_context);
		}

		[Fact]
		public async Task CreateSocialMediaAccount_ShouldCreateAccountSuccessfully()
		{
			var id1 = Guid.NewGuid();
			// Arrange
			var createDto = new CreateSocialMediaAccountDto
			{
				PlatformName = "Instagram",
				Username = "testuser",
				AccessToken = "access-token",
				RefreshToken = "refresh-token",
				TokenExpiry = DateTime.UtcNow.AddDays(30),
				CampaignId = id1
			};

			// Act
			var accountId = await _socialMediaAccountService.CreateSocialMediaAccountAsync(createDto);

			// Assert
			var createdAccount = await _context.SocialMediaAccounts.FindAsync(accountId);
			createdAccount.ShouldNotBeNull();
			createdAccount.PlatformName.ShouldBe("Instagram");
			createdAccount.Username.ShouldBe("testuser");
			createdAccount.AccessToken.ShouldBe("access-token");
			createdAccount.RefreshToken.ShouldBe("refresh-token");
			createdAccount.CampaignId.ShouldBe(id1);
		}

		[Fact]
		public async Task GetSocialMediaAccountById_ShouldReturnAccount_WhenAccountExists()
		{
			var id1 = Guid.NewGuid();
			// Arrange
			var account = new SocialMediaAccount
			{
				PlatformName = "Twitter",
				Username = "testuser2",
				AccessToken = "token",
				RefreshToken = "refresh-token",
				TokenExpiry = DateTime.UtcNow.AddDays(60),
				CampaignId = id1
			};

			_context.SocialMediaAccounts.Add(account);
			await _context.SaveChangesAsync();

			// Act
			var accountDto = await _socialMediaAccountService.GetSocialMediaAccountByIdAsync(account.Id);

			// Assert
			accountDto.ShouldNotBeNull();
			accountDto.Id.ShouldBe(account.Id);
			accountDto.PlatformName.ShouldBe("Twitter");
			accountDto.Username.ShouldBe("testuser2");
			accountDto.CampaignId.ShouldBe(id1);
		}

		[Fact]
		public async Task GetSocialMediaAccountById_ShouldReturnNull_WhenAccountDoesNotExist()
		{
			// Act
			var accountDto = await _socialMediaAccountService.GetSocialMediaAccountByIdAsync(Guid.NewGuid());

			// Assert
			accountDto.ShouldBeNull();
		}

		[Fact]
		public async Task GetSocialMediaAccountsByCampaign_ShouldReturnAccountsForCampaign()
		{
			// Arrange
			var id1 = Guid.NewGuid();
			var account1 = new SocialMediaAccount
			{
				PlatformName = "Instagram",
				Username = "instauser",
				AccessToken = "access1",
				RefreshToken = "refresh1",
				TokenExpiry = DateTime.UtcNow.AddDays(30),
				CampaignId = id1
			};
			var account2 = new SocialMediaAccount
			{
				PlatformName = "Twitter",
				Username = "twitteruser",
				AccessToken = "access2",
				RefreshToken = "refresh2",
				TokenExpiry = DateTime.UtcNow.AddDays(60),
				CampaignId = id1
			};

			_context.SocialMediaAccounts.AddRange(account1, account2);
			await _context.SaveChangesAsync();

			// Act
			var accounts = await _socialMediaAccountService.GetSocialMediaAccountsByCampaignAsync(id1);

			// Assert
			accounts.Count().ShouldBe(2);
			accounts.ShouldContain(a => a.PlatformName == "Instagram");
			accounts.ShouldContain(a => a.PlatformName == "Twitter");
		}

		[Fact]
		public async Task UpdateSocialMediaAccount_ShouldUpdateSuccessfully_WhenAccountExists()
		{
		var id1 = Guid.NewGuid();
		var id2 = Guid.NewGuid();
			// Arrange
			var account = new SocialMediaAccount
			{
				PlatformName = "Instagram",
				Username = "olduser",
				AccessToken = "old-access-token",
				RefreshToken = "old-refresh-token",
				TokenExpiry = DateTime.UtcNow.AddDays(10),
				CampaignId = id1
			};

			_context.SocialMediaAccounts.Add(account);
			await _context.SaveChangesAsync();

			var updateDto = new UpdateSocialMediaAccountDto
			{
				Id = account.Id,
				PlatformName = "Instagram",
				Username = "updateduser",
				AccessToken = "new-access-token",
				RefreshToken = "new-refresh-token",
				TokenExpiry = DateTime.UtcNow.AddDays(30),
				CampaignId = id2
			};

			// Act
			var result = await _socialMediaAccountService.UpdateSocialMediaAccountAsync(updateDto);

			// Assert
			result.ShouldBeTrue();

			var updatedAccount = await _context.SocialMediaAccounts.FindAsync(account.Id);
			updatedAccount.ShouldNotBeNull();
			updatedAccount.Username.ShouldBe("updateduser");
			updatedAccount.AccessToken.ShouldBe("new-access-token");
			updatedAccount.RefreshToken.ShouldBe("new-refresh-token");
			updatedAccount.TokenExpiry.ShouldBe(updateDto.TokenExpiry);
		}

		[Fact]
		public async Task UpdateSocialMediaAccount_ShouldReturnFalse_WhenAccountDoesNotExist()
		{
			// Arrange
			var updateDto = new UpdateSocialMediaAccountDto
			{
				Id = Guid.NewGuid(),
				PlatformName = "Instagram",
				Username = "nonexistentuser",
				AccessToken = "new-access-token",
				RefreshToken = "new-refresh-token",
				TokenExpiry = DateTime.UtcNow.AddDays(30),
				CampaignId = Guid.NewGuid()
			};

			// Act
			var result = await _socialMediaAccountService.UpdateSocialMediaAccountAsync(updateDto);

			// Assert
			result.ShouldBeFalse();
		}

		[Fact]
		public async Task DeleteSocialMediaAccount_ShouldDeleteSuccessfully_WhenAccountExists()
		{
			var campaignId = Guid.NewGuid();
			// Arrange
			var account = new SocialMediaAccount
			{
				PlatformName = "Facebook",
				Username = "deleteuser",
				AccessToken = "access-token",
				RefreshToken = "refresh-token",
				TokenExpiry = DateTime.UtcNow.AddDays(15),
				CampaignId = campaignId
			};

			_context.SocialMediaAccounts.Add(account);
			await _context.SaveChangesAsync();

			// Act
			var result = await _socialMediaAccountService.DeleteSocialMediaAccountAsync(account.Id);

			// Assert
			result.ShouldBeTrue();

			var deletedAccount = await _context.SocialMediaAccounts.FindAsync(account.Id);
			deletedAccount.ShouldBeNull();
		}

		[Fact]
		public async Task DeleteSocialMediaAccount_ShouldReturnFalse_WhenAccountDoesNotExist()
		{
			// Act
			var result = await _socialMediaAccountService.DeleteSocialMediaAccountAsync(Guid.NewGuid());

			// Assert
			result.ShouldBeFalse();
		}
	}
}
