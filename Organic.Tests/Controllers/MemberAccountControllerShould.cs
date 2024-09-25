using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Organic.Api.Controllers;
using Organic.Application.Dtos.ApplicationUserDtos;
using Organic.Core.Services.Interfaces;
using Shouldly;

namespace Organic.Tests.Controllers
{
	public class ApplicationUserAccountControllerShould
	{
		private readonly Mock<IApplicationUserService> _memberServiceMock;
		private readonly ApplicationUserAccountController _controller;

		public ApplicationUserAccountControllerShould()
		{
			_memberServiceMock = new Mock<IApplicationUserService>();
			_controller = new ApplicationUserAccountController(_memberServiceMock.Object);
		}

		[Fact]
		public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
		{
			// Arrange
			var dto = new RegisterApplicationUserDto
			{
				Email = "testuser@example.com",
				FullName = "Test User",
				Password = "Test@1234"
			};

			_memberServiceMock.Setup(s => s.RegisterApplicationUserAsync(dto)).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _controller.Register(dto);

			// Assert
			var okResult = result.ShouldBeOfType<OkObjectResult>();
			okResult.Value.ShouldBe("Registration successful");
			_memberServiceMock.Verify(s => s.RegisterApplicationUserAsync(dto), Times.Once);
		}

		[Fact]
		public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
		{
			// Arrange
			var dto = new RegisterApplicationUserDto
			{
				Email = "testuser@example.com",
				FullName = "Test User",
				Password = "Test@1234"
			};

			var identityResult = IdentityResult.Failed(new IdentityError { Description = "Error during registration" });

			_memberServiceMock.Setup(s => s.RegisterApplicationUserAsync(dto)).ReturnsAsync(identityResult);

			// Act
			var result = await _controller.Register(dto);

			// Assert
			var badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
			badRequestResult.Value.ShouldBe(identityResult.Errors);
			_memberServiceMock.Verify(s => s.RegisterApplicationUserAsync(dto), Times.Once);
		}

		[Fact]
		public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
		{
			// Arrange
			var dto = new LoginApplicationUserDto
			{
				Email = "testuser@example.com",
				Password = "Test@1234",
				RememberMe = false
			};

			_memberServiceMock.Setup(s => s.LoginApplicationUserAsync(dto)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

			// Act
			var result = await _controller.Login(dto);

			// Assert
			var okResult = result.ShouldBeOfType<OkObjectResult>();
			okResult.Value.ShouldBe("Login successful");
			_memberServiceMock.Verify(s => s.LoginApplicationUserAsync(dto), Times.Once);
		}

		[Fact]
		public async Task Login_ShouldReturnUnauthorized_WhenLoginFails()
		{
			// Arrange
			var dto = new LoginApplicationUserDto
			{
				Email = "testuser@example.com",
				Password = "WrongPassword",
				RememberMe = false
			};

			_memberServiceMock.Setup(s => s.LoginApplicationUserAsync(dto)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

			// Act
			var result = await _controller.Login(dto);

			// Assert
			var unauthorizedResult = result.ShouldBeOfType<UnauthorizedObjectResult>();
			unauthorizedResult.Value.ShouldBe("Invalid login attempt");
			_memberServiceMock.Verify(s => s.LoginApplicationUserAsync(dto), Times.Once);
		}

		[Fact]
		public async Task GetApplicationUserDetails_ShouldReturnOk_WhenApplicationUserExists()
		{
			Guid memberId = Guid.NewGuid();

			var memberDto = new ApplicationUserDto
			{
				Id = memberId,
				FullName = "John Doe",
				Email = "johndoe@example.com",
				DateJoined = System.DateTime.Now
			};

			_memberServiceMock.Setup(s => s.GetApplicationUserDetailsAsync(memberId)).ReturnsAsync(memberDto);

			// Act
			var result = await _controller.GetApplicationUserDetails(memberId);

			// Assert
			var okResult = result.ShouldBeOfType<OkObjectResult>();
			okResult.Value.ShouldBe(memberDto);
			_memberServiceMock.Verify(s => s.GetApplicationUserDetailsAsync(memberId), Times.Once);
		}

		[Fact]
		public async Task GetApplicationUserDetails_ShouldReturnNotFound_WhenApplicationUserDoesNotExist()
		{
			// Arrange
			var memberId = Guid.NewGuid();
			_memberServiceMock.Setup(s => s.GetApplicationUserDetailsAsync(memberId)).ReturnsAsync((ApplicationUserDto)null);

			// Act
			var result = await _controller.GetApplicationUserDetails(memberId);

			// Assert
			var notFoundResult = result.ShouldBeOfType<NotFoundObjectResult>();
			notFoundResult.Value.ShouldBe("ApplicationUser not found");
			_memberServiceMock.Verify(s => s.GetApplicationUserDetailsAsync(memberId), Times.Once);
		}

		[Fact]
		public async Task UpdateApplicationUserDetails_ShouldReturnOk_WhenUpdateIsSuccessful()
		{
			// Arrange
			var memberId = Guid.NewGuid();
			var dto = new UpdateApplicationUserDto
			{
				FullName = "Updated User",
				Email = "updated@example.com"
			};

			_memberServiceMock.Setup(s => s.UpdateApplicationUserDetailsAsync(It.Is<UpdateApplicationUserDto>(m => m.Id == memberId)))
							  .ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _controller.UpdateApplicationUserDetails(memberId, dto);

			// Assert
			var okResult = result.ShouldBeOfType<OkObjectResult>();
			okResult.Value.ShouldBe("Update successful");
			_memberServiceMock.Verify(s => s.UpdateApplicationUserDetailsAsync(It.Is<UpdateApplicationUserDto>(m => m.Id == memberId)), Times.Once);
		}

		[Fact]
		public async Task UpdateApplicationUserDetails_ShouldReturnBadRequest_WhenUpdateFails()
		{
			// Arrange
			var memberId = Guid.NewGuid();
			var dto = new UpdateApplicationUserDto
			{
				FullName = "Updated User",
				Email = "updated@example.com"
			};

			var identityResult = IdentityResult.Failed(new IdentityError { Description = "Update failed" });

			_memberServiceMock.Setup(s => s.UpdateApplicationUserDetailsAsync(It.Is<UpdateApplicationUserDto>(m => m.Id == memberId)))
							  .ReturnsAsync(identityResult);

			// Act
			var result = await _controller.UpdateApplicationUserDetails(memberId, dto);

			// Assert
			var badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
			badRequestResult.Value.ShouldBe(identityResult.Errors);
			_memberServiceMock.Verify(s => s.UpdateApplicationUserDetailsAsync(It.Is<UpdateApplicationUserDto>(m => m.Id == memberId)), Times.Once);
		}
	}
}
