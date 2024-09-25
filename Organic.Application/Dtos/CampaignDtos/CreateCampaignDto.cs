using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Application.Dtos.CampaignDtos
{
	public class CreateCampaignDto
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public Guid CreatedById { get; set; } // The ID of the member creating the campaign
	}
}
