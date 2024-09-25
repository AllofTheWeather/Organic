using Organic.Application.Services.Interfaces;
using Organic.Core.Entities;
using Organic.Core.Interfaces;
using System.Text.Json;

namespace Organic.Application.Services.SocialMedia
{
    public class InstagramPostService : ISocialMediaPostService
    {
        private readonly HttpClient _httpClient;

        public InstagramPostService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async virtual Task PostToSocialMedia(ScheduledPost post)
        {
            var accessToken = post.SocialMediaAccount.AccessToken;

            var mediaId = await UploadMedia(post, accessToken);
            if (mediaId == null) throw new Exception("Failed to upload media");

            var success = await PublishPost(post, mediaId, accessToken);
            if (!success) throw new Exception("Failed to publish post");
        }

        private async Task<string> UploadMedia(ScheduledPost post, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://graph.instagram.com/{post.SocialMediaAccount.Username}/media")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "image_url", post.MediaUrl },
                    { "access_token", accessToken }
                })
            };

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            return json.RootElement.GetProperty("id").GetString();  // Assuming "id" is returned
        }

        private async Task<bool> PublishPost(ScheduledPost post, string mediaId, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://graph.instagram.com/{post.SocialMediaAccount.Username}/media_publish")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "creation_id", mediaId },
                    { "access_token", accessToken }
                })
            };

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }

}
