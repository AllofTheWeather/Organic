using Microsoft.AspNetCore.Mvc;
using Organic.Application.Dtos.ScheduledPostDtos;
using Organic.Application.Services.Interfaces;

namespace Organic.Api.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class ScheduledPostController : ControllerBase
	{
		private readonly IScheduledPostService _scheduledPostService;

		public ScheduledPostController(IScheduledPostService scheduledPostService)
		{
			_scheduledPostService = scheduledPostService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllScheduledPosts()
		{
			var posts = await _scheduledPostService.GetAllPostsAsync();
			return Ok(posts);
		}

		[HttpPost]
		public async Task<IActionResult> CreateScheduledPost([FromBody] CreateScheduledPostDto dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var postId = await _scheduledPostService.CreatePostAsync(dto);
			return CreatedAtAction(nameof(GetAllScheduledPosts), new { id = postId }, null);
		}

		// Add more actions like Get by ID, Update, Delete as needed
	}
}