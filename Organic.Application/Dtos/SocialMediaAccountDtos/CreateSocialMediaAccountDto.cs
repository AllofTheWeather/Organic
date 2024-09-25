using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Application.Dtos.SocialMediaAccountDtos
{
	public class CreateSocialMediaAccountDto
	{
		public string PlatformName { get; set; }
		public string Username { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public DateTime TokenExpiry { get; set; }
		public Guid CampaignId { get; set; }
	}

}
