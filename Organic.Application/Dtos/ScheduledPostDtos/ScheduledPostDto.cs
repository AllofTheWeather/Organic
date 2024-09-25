namespace Organic.Application.Dtos.ScheduledPostDtos
{
    public class ScheduledPostDto
    {
        public Guid Id { get; set; }
        public string Caption { get; set; }
        public string MediaUrl { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool IsPosted { get; set; }
    }

    public class CreateScheduledPostDto
    {
        public string Caption { get; set; }
        public string MediaUrl { get; set; }
        public DateTime ScheduledTime { get; set; }
        public Guid SocialMediaAccountId { get; set; }
    }
}
