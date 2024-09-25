namespace Organic.Core.Entities
{
	public class MediaFile
	{
		public Guid Id { get; set; }
		public string FilePath { get; set; }
		public string FileType { get; set; }

		public Guid ScheduledPostId { get; set; }
		public ScheduledPost ScheduledPost { get; set; }
	}
}
