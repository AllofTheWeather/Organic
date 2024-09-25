using Organic.Application.Dtos.ScheduledPostDtos;

namespace Organic.Application.Services.Interfaces
{
    public interface IScheduledPostService
	{
		Task<IEnumerable<ScheduledPostDto>> GetAllPostsAsync();
		Task<Guid> CreatePostAsync(CreateScheduledPostDto dto);
		// Add more methods as needed
	}
}
