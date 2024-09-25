using Microsoft.AspNetCore.Identity;
using Organic.Core.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Organic.Core.Entities
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		[Key]
		[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public override Guid Id
		{
			get { return base.Id; }
			set { base.Id = value; }
		}
		public string FullName { get; set; }
		public string Email	{ get; set; }
		public DateTime DateJoined { get; set; }
		public ICollection<Campaign> CreatedCampaigns { get; set; }
		public ICollection<Campaign> Campaigns { get; set; }
	}
}
