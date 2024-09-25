using Microsoft.EntityFrameworkCore;
using Moq;
using Organic.Application.Services.Posting;
using Organic.Core.Entities;
using Organic.Core.Interfaces;
using Shouldly;

namespace Organic.Tests.Services
{
	public class PostalServiceShould
	{
		private readonly ApplicationDbContext _context;
		private readonly Mock<ISocialMediaPostService> _instagramPostServiceMock;
		private readonly Mock<IPostServiceFactory> _postServiceFactoryMock;
		private readonly PostalService _postalService;

		public PostalServiceShould()
		{
			// Create a new in-memory database for each test
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique in-memory database for each test
				.Options;

			_context = new ApplicationDbContext(options);

			// Mock the Instagram post service and the factory
			_instagramPostServiceMock = new Mock<ISocialMediaPostService>();
			_postServiceFactoryMock = new Mock<IPostServiceFactory>();

			// Initialize PostalService with the factory mock
			_postalService = new PostalService(_context, _postServiceFactoryMock.Object);
		}

		[Fact]
		public async Task ExecuteScheduledPosts_ShouldPostScheduledPostsAndMarkAsPosted()
		{
			// Arrange
			var now = DateTime.UtcNow;
			var scheduledPost = new ScheduledPost
			{
				Caption = "Test Post",
				MediaUrl = "https://example.com/image.jpg",
				ScheduledTime = now.AddMinutes(-1), // In the past, so it should be posted
				IsPosted = false,
				SocialMediaAccount = new SocialMediaAccount
				{
					PlatformName = "Instagram",
					AccessToken = "test-access-token",
					RefreshToken = "test-refresh-token",
					Username = "test-username",
				}
			};

			_context.ScheduledPosts.Add(scheduledPost);
			await _context.SaveChangesAsync();

			// Set up the factory to return the mock Instagram post service when "Instagram" is requested
			_postServiceFactoryMock
				.Setup(factory => factory.GetPostService("Instagram"))
				.Returns(_instagramPostServiceMock.Object);

			// Act
			await _postalService.ExecuteScheduledPosts();

			// Assert
			_instagramPostServiceMock.Verify(service => service.PostToSocialMedia(It.IsAny<ScheduledPost>()), Times.Once);
			var updatedPost = await _context.ScheduledPosts.FirstOrDefaultAsync(sp => sp.Id == scheduledPost.Id);
			updatedPost.ShouldNotBeNull();
			updatedPost.IsPosted.ShouldBeTrue();
		}

		[Fact]
		public async Task ExecuteScheduledPosts_ShouldNotPostAlreadyPostedPosts()
		{
			// Arrange
			var now = DateTime.UtcNow;
			var scheduledPost = new ScheduledPost
			{
				Caption = "Already Posted Post",
				MediaUrl = "https://example.com/image.jpg",
				ScheduledTime = now.AddMinutes(-1), // In the past
				IsPosted = true, // Already posted
				SocialMediaAccount = new SocialMediaAccount
				{
					PlatformName = "Instagram",
					AccessToken = "test-access-token",
					RefreshToken = "test-refresh-token",
					Username = "test-username",
				}
			};

			_context.ScheduledPosts.Add(scheduledPost);
			await _context.SaveChangesAsync();

			// Set up the factory to return the mock Instagram post service
			_postServiceFactoryMock
				.Setup(factory => factory.GetPostService("Instagram"))
				.Returns(_instagramPostServiceMock.Object);

			// Act
			await _postalService.ExecuteScheduledPosts();

			// Assert
			_instagramPostServiceMock.Verify(service => service.PostToSocialMedia(It.IsAny<ScheduledPost>()), Times.Never);
		}

		[Fact]
		public async Task ExecuteScheduledPosts_ShouldHandleMultiplePostsCorrectly()
		{
			// Arrange
			var now = DateTime.UtcNow;
			var scheduledPost1 = new ScheduledPost
			{
				Caption = "Post 1",
				MediaUrl = "https://example.com/image1.jpg",
				ScheduledTime = now.AddMinutes(-2), // Should be posted
				IsPosted = false,
				SocialMediaAccount = new SocialMediaAccount
				{
					PlatformName = "Instagram",
					AccessToken = "test-access-token",
					RefreshToken = "test-refresh-token",
					Username = "test-username",
				}
			};

			var scheduledPost2 = new ScheduledPost
			{
				Caption = "Post 2",
				MediaUrl = "https://example.com/image2.jpg",
				ScheduledTime = now.AddMinutes(10), // In the future, should not be posted
				IsPosted = false,
				SocialMediaAccount = new SocialMediaAccount
				{
					PlatformName = "Instagram",
					AccessToken = "test-access-token",
					RefreshToken = "test-refresh-token",
					Username = "test-username",
				}
			};

			_context.ScheduledPosts.AddRange(scheduledPost1, scheduledPost2);
			await _context.SaveChangesAsync();

			// Set up the factory to return the mock Instagram post service
			_postServiceFactoryMock
				.Setup(factory => factory.GetPostService("Instagram"))
				.Returns(_instagramPostServiceMock.Object);

			// Act
			await _postalService.ExecuteScheduledPosts();

			// Assert
			// Ensure only the past post is posted
			_instagramPostServiceMock.Verify(service => service.PostToSocialMedia(It.Is<ScheduledPost>(sp => sp.Caption == "Post 1")), Times.Once);
			_instagramPostServiceMock.Verify(service => service.PostToSocialMedia(It.Is<ScheduledPost>(sp => sp.Caption == "Post 2")), Times.Never);

			// Ensure that Post 1 is marked as posted, and Post 2 remains unposted
			var updatedPost1 = await _context.ScheduledPosts.FirstOrDefaultAsync(sp => sp.Id == scheduledPost1.Id);
			var updatedPost2 = await _context.ScheduledPosts.FirstOrDefaultAsync(sp => sp.Id == scheduledPost2.Id);

			updatedPost1.ShouldNotBeNull();
			updatedPost1.IsPosted.ShouldBeTrue();

			updatedPost2.ShouldNotBeNull();
			updatedPost2.IsPosted.ShouldBeFalse();
		}
	}

}
