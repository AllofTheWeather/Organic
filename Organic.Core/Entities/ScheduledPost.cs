namespace Organic.Core.Entities
{
	public class ScheduledPost
	{
		public Guid Id { get; set; }
		public string Caption { get; set; }
		public string MediaUrl { get; set; }
		public DateTime ScheduledTime { get; set; }
		public bool IsPosted { get; set; }

		public Guid SocialMediaAccountId { get; set; }
		public SocialMediaAccount SocialMediaAccount { get; set; }
		public ICollection<MediaFile> MediaFiles { get; set; }
	}
}
