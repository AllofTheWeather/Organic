using Microsoft.AspNetCore.Mvc;
using Moq;
using Organic.Api.Controllers;
using Organic.Application.Dtos.ScheduledPostDtos;
using Organic.Application.Services.Interfaces;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Tests.Controllers
{
	public class ScheduledPostControllerShould
	{
		private readonly Mock<IScheduledPostService> _scheduledPostServiceMock;
		private readonly ScheduledPostController _controller;

		public ScheduledPostControllerShould()
		{
			_scheduledPostServiceMock = new Mock<IScheduledPostService>();
			_controller = new ScheduledPostController(_scheduledPostServiceMock.Object);
		}

		[Fact]
		public async Task GetAllScheduledPosts_ShouldReturnOk_WithListOfPosts()
		{
			Guid id1 = Guid.NewGuid();
			Guid id2 = Guid.NewGuid();
			// Arrange
			var posts = new List<ScheduledPostDto>
			{
				new ScheduledPostDto { Id = id1, Caption = "First Post", MediaUrl = "https://example.com/1.jpg", ScheduledTime = System.DateTime.Now, IsPosted = false },
				new ScheduledPostDto { Id = id2, Caption = "Second Post", MediaUrl = "https://example.com/2.jpg", ScheduledTime = System.DateTime.Now, IsPosted = true }
			};

			_scheduledPostServiceMock.Setup(s => s.GetAllPostsAsync()).ReturnsAsync(posts);

			// Act
			var result = await _controller.GetAllScheduledPosts();

			// Assert
			var okResult = result.ShouldBeOfType<OkObjectResult>();
			var responsePosts = okResult.Value.ShouldBeOfType<List<ScheduledPostDto>>();
			responsePosts.Count.ShouldBe(2);
			responsePosts[0].Caption.ShouldBe("First Post");
			responsePosts[1].Caption.ShouldBe("Second Post");
			_scheduledPostServiceMock.Verify(s => s.GetAllPostsAsync(), Times.Once);
		}

		[Fact]
		public async Task CreateScheduledPost_ShouldReturnCreatedAtAction_WhenPostIsCreated()
		{
			Guid id1 = Guid.NewGuid();
			Guid id2 = Guid.NewGuid();
			// Arrange
			var dto = new CreateScheduledPostDto
			{
				Caption = "Test Caption",
				MediaUrl = "https://example.com/media.jpg",
				ScheduledTime = System.DateTime.Now.AddHours(1),
				SocialMediaAccountId = id1
			};

			_scheduledPostServiceMock.Setup(s => s.CreatePostAsync(dto)).ReturnsAsync(id2); // Returning postId 1

			// Act
			var result = await _controller.CreateScheduledPost(dto);

			// Assert
			var createdResult = result.ShouldBeOfType<CreatedAtActionResult>();
			createdResult.ActionName.ShouldBe(nameof(ScheduledPostController.GetAllScheduledPosts));
			createdResult.RouteValues.ShouldContainKeyAndValue("id", id2);
			_scheduledPostServiceMock.Verify(s => s.CreatePostAsync(dto), Times.Once);
		}

		[Fact]
		public async Task CreateScheduledPost_ShouldReturnBadRequest_WhenModelStateIsInvalid()
		{
			Guid id1 = Guid.NewGuid();
			Guid id2 = Guid.NewGuid();
			// Arrange
			var dto = new CreateScheduledPostDto
			{
				Caption = "", // Invalid because it might be required
				MediaUrl = "https://example.com/media.jpg",
				ScheduledTime = System.DateTime.Now.AddHours(1),
				SocialMediaAccountId = id1
			};

			_controller.ModelState.AddModelError("Caption", "Caption is required");

			var result = await _controller.CreateScheduledPost(dto);

			var badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();

			var modelState = badRequestResult.Value.ShouldBeOfType<SerializableError>();

			modelState.ContainsKey("Caption").ShouldBeTrue();
			var captionErrors = modelState["Caption"] as string[];
			captionErrors.ShouldContain("Caption is required");

			_scheduledPostServiceMock.Verify(s => s.CreatePostAsync(It.IsAny<CreateScheduledPostDto>()), Times.Never);
		}
	}
}
