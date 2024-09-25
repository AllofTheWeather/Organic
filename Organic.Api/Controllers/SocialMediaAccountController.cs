using Microsoft.AspNetCore.Mvc;
using Organic.Application.Dtos.SocialMediaAccountDtos;
using Organic.Application.Services.Interfaces;

namespace Organic.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SocialMediaAccountController : ControllerBase
	{
		private readonly ISocialMediaAccountService _socialMediaAccountService;

		public SocialMediaAccountController(ISocialMediaAccountService socialMediaAccountService)
		{
			_socialMediaAccountService = socialMediaAccountService;
		}

		[HttpPost]
		public async Task<IActionResult> CreateSocialMediaAccount([FromBody] CreateSocialMediaAccountDto dto)
		{
			var accountId = await _socialMediaAccountService.CreateSocialMediaAccountAsync(dto);
			return CreatedAtAction(nameof(GetSocialMediaAccountById), new { id = accountId }, null);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetSocialMediaAccountById(Guid id)
		{
			var account = await _socialMediaAccountService.GetSocialMediaAccountByIdAsync(id);
			if (account == null)
			{
				return NotFound();
			}
			return Ok(account);
		}

		[HttpGet("by-campaign/{campaignId}")]
		public async Task<IActionResult> GetSocialMediaAccountsByCampaign(Guid campaignId)
		{
			var accounts = await _socialMediaAccountService.GetSocialMediaAccountsByCampaignAsync(campaignId);
			return Ok(accounts);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateSocialMediaAccount(Guid id, [FromBody] UpdateSocialMediaAccountDto dto)
		{
			dto.Id = id;
			var result = await _socialMediaAccountService.UpdateSocialMediaAccountAsync(dto);
			if (!result)
			{
				return NotFound();
			}
			return Ok("Account updated successfully");
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteSocialMediaAccount(Guid id)
		{
			var result = await _socialMediaAccountService.DeleteSocialMediaAccountAsync(id);
			if (!result)
			{
				return NotFound();
			}
			return Ok("Account deleted successfully");
		}
	}

}
