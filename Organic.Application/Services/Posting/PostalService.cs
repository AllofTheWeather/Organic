using Hangfire;
using Microsoft.EntityFrameworkCore;
using Organic.Application.Services.Factories;
using Organic.Application.Services.Interfaces;
using Organic.Core.Entities;
using Organic.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Application.Services.Posting
{
    public class PostalService : IPostalService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPostServiceFactory _postServiceFactory;

        public PostalService(ApplicationDbContext context, IPostServiceFactory postServiceFactory)
        {
            _context = context;
            _postServiceFactory = postServiceFactory;
        }

        [AutomaticRetry(Attempts = 3)] // Hangfire attribute to retry the job in case of failure
        public async Task ExecuteScheduledPosts()
        {
            var now = DateTime.UtcNow;

            var scheduledPosts = await _context.ScheduledPosts
                .Include(sp => sp.SocialMediaAccount)
                .Where(sp => sp.ScheduledTime <= now && !sp.IsPosted)
                .ToListAsync();

            foreach (var post in scheduledPosts)
            {
                try
                {
                    // Post to the social media platform
                    var platformService = _postServiceFactory.GetPostService(post.SocialMediaAccount.PlatformName);
                    await platformService.PostToSocialMedia(post);

                    // Mark post as posted
                    post.IsPosted = true;
                    _context.ScheduledPosts.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Handle the failure (log, retry, etc.)
                    // You could add a PostFailed flag or retry mechanism
                    Console.WriteLine($"Failed to post {post.Id}: {ex.Message}");
                }
            }
        }
    }

}
