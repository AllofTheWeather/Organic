using Organic.Application.Services.Interfaces;
using Organic.Core.Entities;
using Organic.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Organic.Application.Services.SocialMedia
{
    public class TwitterPostService : ISocialMediaPostService
    {
        private readonly HttpClient _httpClient;

        public TwitterPostService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public virtual async Task PostToSocialMedia(ScheduledPost post)
        {
            var accessToken = post.SocialMediaAccount.AccessToken;

            var tweetId = await PostTweet(post, accessToken);
            if (tweetId == null) throw new Exception("Failed to post tweet");
        }

        private async Task<string> PostTweet(ScheduledPost post, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/2/tweets")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "status", post.Caption },  // The tweet text
                { "media_ids", post.MediaUrl },  // Assuming media is uploaded separately and media ID is available
                { "access_token", accessToken }
            })
            };

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            return json.RootElement.GetProperty("id").GetString();  // Assuming "id" is returned
        }
    }

}
