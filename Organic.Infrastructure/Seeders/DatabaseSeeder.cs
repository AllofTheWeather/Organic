using Organic.Core.Entities;

namespace Organic.Infrastructure.Seeders
{
	public class DatabaseSeeder
	{
		private readonly ApplicationDbContext _context;

		public DatabaseSeeder(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task SeedAsync()
		{
			// Seed members if they don't exist
			if (!_context.ApplicationUsers.Any())
			{
				var members = new List<ApplicationUser>
		{
			new ApplicationUser
			{
				Id = Guid.NewGuid(), // Generate GUID for ApplicationUser Id
                FullName = "John Doe",
				Email = "john@example.com",
				DateJoined = DateTime.UtcNow
			},
			new ApplicationUser
			{
				Id = Guid.NewGuid(), // Generate GUID for ApplicationUser Id
                FullName = "Jane Smith",
				Email = "jane@example.com",
				DateJoined = DateTime.UtcNow
			}
		};

				await _context.ApplicationUsers.AddRangeAsync(members);
				await _context.SaveChangesAsync();
			}

			// Seed campaigns if they don't exist
			if (!_context.Campaigns.Any())
			{
				var campaigns = new List<Campaign>
		{
			new Campaign
			{
				Id = Guid.NewGuid(), // Use GUID for Campaign Id
                Name = "Campaign 1",
				Description = "First campaign",
				CreatedAt = DateTime.UtcNow,
				CreatedById = _context.ApplicationUsers.First(m => m.Email == "john@example.com").Id  // Use the existing member's GUID
            },
			new Campaign
			{
				Id = Guid.NewGuid(), // Use GUID for Campaign Id
                Name = "Campaign 2",
				Description = "Second campaign",
				CreatedAt = DateTime.UtcNow,
				CreatedById = _context.ApplicationUsers.First(m => m.Email == "jane@example.com").Id  // Use the existing member's GUID
            }
		};

				await _context.Campaigns.AddRangeAsync(campaigns);
				await _context.SaveChangesAsync();
			}

			// Seed social media accounts if they don't exist
			if (!_context.SocialMediaAccounts.Any())
			{
				var socialMediaAccounts = new List<SocialMediaAccount>
		{
			new SocialMediaAccount
			{
				Id = Guid.NewGuid(), // Use GUID for SocialMediaAccount Id
                PlatformName = "Instagram",
				Username = "john_insta",
				AccessToken = "access_token_123",
				RefreshToken = "refresh_token_123",
				TokenExpiry = DateTime.UtcNow.AddMonths(1),
				CampaignId = _context.Campaigns.First(c => c.Name == "Campaign 1").Id // Use existing campaign GUID
            },
			new SocialMediaAccount
			{
				Id = Guid.NewGuid(), // Use GUID for SocialMediaAccount Id
                PlatformName = "Twitter",
				Username = "jane_twitter",
				AccessToken = "access_token_456",
				RefreshToken = "refresh_token_456",
				TokenExpiry = DateTime.UtcNow.AddMonths(1),
				CampaignId = _context.Campaigns.First(c => c.Name == "Campaign 2").Id // Use existing campaign GUID
            }
		};

				await _context.SocialMediaAccounts.AddRangeAsync(socialMediaAccounts);
				await _context.SaveChangesAsync();
			}

			// Seed scheduled posts if they don't exist
			if (!_context.ScheduledPosts.Any())
			{
				var scheduledPosts = new List<ScheduledPost>
		{
			new ScheduledPost
			{
				Id = Guid.NewGuid(), // Use GUID for ScheduledPost Id
                Caption = "First Post",
				MediaUrl = "https://example.com/image1.jpg",
				ScheduledTime = DateTime.UtcNow.AddHours(1),
				IsPosted = false,
				SocialMediaAccountId = _context.SocialMediaAccounts.First(sma => sma.Username == "john_insta").Id  // Use existing SocialMediaAccount GUID
            },
			new ScheduledPost
			{
				Id = Guid.NewGuid(), // Use GUID for ScheduledPost Id
                Caption = "Second Post",
				MediaUrl = "https://example.com/image2.jpg",
				ScheduledTime = DateTime.UtcNow.AddHours(2),
				IsPosted = false,
				SocialMediaAccountId = _context.SocialMediaAccounts.First(sma => sma.Username == "jane_twitter").Id  // Use existing SocialMediaAccount GUID
            }
		};

				await _context.ScheduledPosts.AddRangeAsync(scheduledPosts);
				await _context.SaveChangesAsync();
			}
		}
	}
}

