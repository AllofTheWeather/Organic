using Organic.Application.Dtos.ApplicationUserDtos;
using Organic.Application.Dtos.SocialMediaAccountDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Application.Dtos.CampaignDtos
{
	public class CampaignDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreatedAt { get; set; }
		public Guid CreatedById { get; set; } // The name of the member who created the campaign


		public List<SocialMediaAccountDto> SocialMediaAccounts { get; set; } // Associated social media accounts
		public List<ApplicationUserDto> ApplicationUsers { get; set; } // ApplicationUsers involved in the campaign
	}
}
