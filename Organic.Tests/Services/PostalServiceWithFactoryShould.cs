using Microsoft.EntityFrameworkCore;
using Moq;
using Organic.Application.Services.Factories;
using Organic.Application.Services.Posting;
using Organic.Application.Services.SocialMedia;
using Organic.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Tests.Services
{
    public class PostalServiceWithFactoryShould
	{
		private readonly ApplicationDbContext _context;
		private readonly Mock<InstagramPostService> _instagramPostServiceMock;
		private readonly Mock<TwitterPostService> _twitterPostServiceMock;
		private readonly PostServiceFactory _postServiceFactory;
		private readonly PostalService PostalService;

		public PostalServiceWithFactoryShould()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_instagramPostServiceMock = new Mock<InstagramPostService>(new Mock<HttpClient>().Object);
			_twitterPostServiceMock = new Mock<TwitterPostService>(new Mock<HttpClient>().Object);

			_postServiceFactory = new PostServiceFactory(_instagramPostServiceMock.Object, _twitterPostServiceMock.Object);
			PostalService = new PostalService(_context, _postServiceFactory);
		}

		[Fact]
		public async Task ExecuteScheduledPosts_ShouldUseCorrectServiceBasedOnPlatform()
		{
			// Arrange
			var now = DateTime.UtcNow;
			var instagramPost = new ScheduledPost
			{
				Caption = "Instagram Post",
				MediaUrl = "https://example.com/image.jpg",
				ScheduledTime = now.AddMinutes(-1),
				IsPosted = false,
				SocialMediaAccount = new SocialMediaAccount
				{
					PlatformName = "Instagram",
					AccessToken = "instagram-access-token",
					RefreshToken = "test-refresh-token",
					Username = "test-username"
				}
			};

			var twitterPost = new ScheduledPost
			{
				Caption = "Twitter Post",
				MediaUrl = "https://example.com/image.jpg",
				ScheduledTime = now.AddMinutes(-1),
				IsPosted = false,
				SocialMediaAccount = new SocialMediaAccount
				{
					PlatformName = "Twitter",
					AccessToken = "twitter-access-token",
					RefreshToken = "test-refresh-token",
					Username = "test-username"
				}
			};

			_context.ScheduledPosts.AddRange(instagramPost, twitterPost);
			await _context.SaveChangesAsync();

			// Act
			await PostalService.ExecuteScheduledPosts();

			// Assert
			_instagramPostServiceMock.Verify(service => service.PostToSocialMedia(It.IsAny<ScheduledPost>()), Times.Once);
			_twitterPostServiceMock.Verify(service => service.PostToSocialMedia(It.IsAny<ScheduledPost>()), Times.Once);
		}
	}

}
