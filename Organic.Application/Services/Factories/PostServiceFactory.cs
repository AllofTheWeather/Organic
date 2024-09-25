using Organic.Application.Services.SocialMedia;
using Organic.Core.Interfaces;

namespace Organic.Application.Services.Factories
{
    public class PostServiceFactory : IPostServiceFactory
    {
        private readonly InstagramPostService _instagramPostService;
        private readonly TwitterPostService _twitterPostService;

        public PostServiceFactory(InstagramPostService instagramPostService, TwitterPostService twitterPostService)
        {
            _instagramPostService = instagramPostService;
            _twitterPostService = twitterPostService;
        }

        public ISocialMediaPostService GetPostService(string platformName)
        {
            return platformName switch
            {
                "Instagram" => _instagramPostService,
                "Twitter" => _twitterPostService,
                _ => throw new NotSupportedException($"Platform '{platformName}' is not supported.")
            };
        }
    }

}
