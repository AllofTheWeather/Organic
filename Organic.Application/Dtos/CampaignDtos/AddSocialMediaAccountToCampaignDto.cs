using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Application.Dtos.CampaignDtos
{
	public class AddSocialMediaAccountToCampaignDto
	{
		public Guid CampaignId { get; set; } // The ID of the campaign to which the account will be added
		public string PlatformName { get; set; } // The name of the platform (e.g., "Instagram")
		public string Username { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public DateTime TokenExpiry { get; set; }
	}
}
