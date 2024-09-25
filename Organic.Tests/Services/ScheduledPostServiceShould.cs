using Microsoft.EntityFrameworkCore;
using Organic.Application.Dtos.ScheduledPostDtos;
using Organic.Application.Services.Posting;
using Organic.Core.Entities;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Tests.Services
{
    public class ScheduledPostServiceShould : IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly ScheduledPostService _scheduledPostService;

		public ScheduledPostServiceShould()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_scheduledPostService = new ScheduledPostService(_context);
		}

		[Fact]
		public async Task GetAllPostsAsync_ShouldReturnAllScheduledPosts()
		{
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			var socialMediaAccountId = Guid.NewGuid();
			var socialMediaAccountId2 = Guid.NewGuid();
			// Arrange
			var posts = new List<ScheduledPost>
			{
				new ScheduledPost
				{
					Id = id1,
					Caption = "First post",
					MediaUrl = "https://example.com/media1.jpg",
					ScheduledTime = DateTime.Now.AddHours(1),
					IsPosted = false,
					SocialMediaAccountId = socialMediaAccountId
				},
				new ScheduledPost
				{
					Id = id2,
					Caption = "Second post",
					MediaUrl = "https://example.com/media2.jpg",
					ScheduledTime = DateTime.Now.AddHours(2),
					IsPosted = false,
					SocialMediaAccountId = socialMediaAccountId2
				}
			};

			_context.ScheduledPosts.AddRange(posts);
			await _context.SaveChangesAsync();

			// Act
			var result = await _scheduledPostService.GetAllPostsAsync();

			// Assert
			result.ShouldNotBeNull();
			result.Count().ShouldBe(2);
			result.ShouldContain(post => post.Caption == "First post" && post.MediaUrl == "https://example.com/media1.jpg");
			result.ShouldContain(post => post.Caption == "Second post" && post.MediaUrl == "https://example.com/media2.jpg");
		}
		[Fact]
		public async Task CreatePostAsync_ShouldAddPostAndReturnPostId()
		{
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			var socialMediaAccountId = Guid.NewGuid();
			var socialMediaAccountId2 = Guid.NewGuid();
			// Arrange
			var createPostDto = new CreateScheduledPostDto
			{
				Caption = "New Scheduled Post",
				MediaUrl = "https://example.com/media.jpg",
				ScheduledTime = DateTime.Now.AddDays(1),
				SocialMediaAccountId = socialMediaAccountId
			};

			// Act
			var postId = await _scheduledPostService.CreatePostAsync(createPostDto);

			// Assert
			postId.ShouldNotBe(id2); // Ensure post ID is returned

			var createdPost = await _context.ScheduledPosts.FindAsync(postId);
			createdPost.ShouldNotBeNull();
			createdPost.Caption.ShouldBe("New Scheduled Post");
			createdPost.MediaUrl.ShouldBe("https://example.com/media.jpg");
			createdPost.IsPosted.ShouldBeFalse(); // Ensure IsPosted is set to false
			createdPost.SocialMediaAccountId.ShouldBe(socialMediaAccountId);
		}
		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
