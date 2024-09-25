using Microsoft.AspNetCore.Mvc;
using Organic.Application.Dtos.ApplicationUserDtos;
using Organic.Core.Services.Interfaces;

namespace Organic.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ApplicationUserAccountController : ControllerBase
	{
		private readonly IApplicationUserService _memberService;

		public ApplicationUserAccountController(IApplicationUserService memberService)
		{
			_memberService = memberService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterApplicationUserDto dto)
		{
			var result = await _memberService.RegisterApplicationUserAsync(dto);
			if (result.Succeeded)
			{
				return Ok("Registration successful");
			}
			return BadRequest(result.Errors);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginApplicationUserDto dto)
		{
			var result = await _memberService.LoginApplicationUserAsync(dto);
			if (result.Succeeded)
			{
				return Ok("Login successful");
			}
			return Unauthorized("Invalid login attempt");
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetApplicationUserDetails(Guid id)
		{
			var member = await _memberService.GetApplicationUserDetailsAsync(id);
			if (member == null)
			{
				return NotFound("ApplicationUser not found");
			}
			return Ok(member);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateApplicationUserDetails(Guid id, [FromBody] UpdateApplicationUserDto dto)
		{
			dto.Id = id;
			var result = await _memberService.UpdateApplicationUserDetailsAsync(dto);
			if (result.Succeeded)
			{
				return Ok("Update successful");
			}
			return BadRequest(result.Errors);
		}
	}
}
