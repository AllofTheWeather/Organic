using Microsoft.EntityFrameworkCore;
using Organic.Application.Dtos.ScheduledPostDtos;
using Organic.Application.Services.Interfaces;
using Organic.Core.Entities;

namespace Organic.Application.Services.Posting
{
    public class ScheduledPostService : IScheduledPostService
    {
        private readonly ApplicationDbContext _context;

        public ScheduledPostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScheduledPostDto>> GetAllPostsAsync()
        {
            return await _context.ScheduledPosts
                .Select(post => new ScheduledPostDto
                {
                    Id = post.Id,
                    Caption = post.Caption,
                    MediaUrl = post.MediaUrl,
                    ScheduledTime = post.ScheduledTime,
                    IsPosted = post.IsPosted
                }).ToListAsync();
        }

        public async Task<Guid> CreatePostAsync(CreateScheduledPostDto dto)
        {
            var post = new ScheduledPost
            {
                Caption = dto.Caption,
                MediaUrl = dto.MediaUrl,
                ScheduledTime = dto.ScheduledTime,
                IsPosted = false,
                SocialMediaAccountId = dto.SocialMediaAccountId
            };

            _context.ScheduledPosts.Add(post);
            await _context.SaveChangesAsync();

            return post.Id;
        }

        // Add more methods as needed
    }
}
