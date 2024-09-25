using Organic.Core.Entities;

namespace Organic.Core.Interfaces
{
	public interface ISocialMediaPostService
	{
		Task PostToSocialMedia(ScheduledPost post);
	}

}