using Microsoft.AspNetCore.Mvc;
using Organic.Application.Dtos.CampaignDtos;
using Organic.Application.Services.Interfaces;

namespace Organic.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CampaignController : ControllerBase
	{
		private readonly ICampaignService _campaignService;

		public CampaignController(ICampaignService campaignService)
		{
			_campaignService = campaignService;
		}

		[HttpPost]
		public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto dto)
		{
			var campaignId = await _campaignService.CreateCampaignAsync(dto);
			return Ok(new CreateCampaignResponseDto { CampaignId = campaignId });
		}

		[HttpGet("{campaignId}")]
		public async Task<IActionResult> GetCampaign(Guid campaignId)
		{
			var campaign = await _campaignService.GetCampaignAsync(campaignId);
			if (campaign == null)
			{
				return NotFound();
			}

			return Ok(campaign);
		}

		[HttpPost("add-socialmediaaccount")]
		public async Task<IActionResult> AddSocialMediaAccountToCampaign([FromBody] AddSocialMediaAccountToCampaignDto dto)
		{
			await _campaignService.AddSocialMediaAccountToCampaignAsync(dto);
			return Ok("Social media account added to campaign");
		}
	}
}
