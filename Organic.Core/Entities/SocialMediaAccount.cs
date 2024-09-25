using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Core.Entities
{
	public class SocialMediaAccount
	{
		public Guid Id { get; set; }
		public string PlatformName { get; set; } // e.g., "Instagram", "Twitter"
		public string Username { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public DateTime TokenExpiry { get; set; }

		public Guid CampaignId { get; set; }
		public Campaign Campaign { get; set; }
	}
}
