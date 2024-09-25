
using Organic.Core.Interfaces;

namespace Organic.Core.Entities
{
	public class Campaign
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreatedAt { get; set; }
		public Guid CreatedById { get; set; }  // The project lead (initially a single user)
		public ApplicationUser CreatedBy { get; set; }

		// Navigation properties
		public ICollection<SocialMediaAccount> SocialMediaAccounts { get; set; }
		public ICollection<ApplicationUser> ApplicationUsers { get; set; }
		public Campaign()
		{
			SocialMediaAccounts = new List<SocialMediaAccount>();
			ApplicationUsers = new List<ApplicationUser>();
		}
	}
}
