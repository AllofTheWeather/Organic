using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Organic.Application.Dtos.ApplicationUserDtos;
using Organic.Application.Services.Auth;
using Organic.Core.Entities;
using Shouldly;

namespace Organic.Tests.Services
{
    public class ApplicationUserServiceShould
	{
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
		private readonly ApplicationUserService _memberService;

		public ApplicationUserServiceShould()
		{
			_userManagerMock = MockUserManager();
			_signInManagerMock = MockSignInManager(_userManagerMock.Object);
			_memberService = new ApplicationUserService(_userManagerMock.Object, _signInManagerMock.Object);
		}

		#region Helper Methods

		private Mock<UserManager<ApplicationUser>> MockUserManager()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			var userManager = new Mock<UserManager<ApplicationUser>>(
				store.Object,
				Mock.Of<IOptions<IdentityOptions>>(),
				Mock.Of<IPasswordHasher<ApplicationUser>>(),
				new IUserValidator<ApplicationUser>[0],
				new IPasswordValidator<ApplicationUser>[0],
				Mock.Of<ILookupNormalizer>(),
				Mock.Of<IdentityErrorDescriber>(),
				Mock.Of<IServiceProvider>(),
				Mock.Of<ILogger<UserManager<ApplicationUser>>>()
			);

			return userManager;
		}

		private Mock<SignInManager<ApplicationUser>> MockSignInManager(UserManager<ApplicationUser> userManager)
		{
			var contextAccessor = new Mock<IHttpContextAccessor>();
			var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

			var signInManager = new Mock<SignInManager<ApplicationUser>>(
				userManager,
				contextAccessor.Object,
				userPrincipalFactory.Object,
				Mock.Of<IOptions<IdentityOptions>>(),
				Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
				Mock.Of<IAuthenticationSchemeProvider>(),
				Mock.Of<IUserConfirmation<ApplicationUser>>()
			);

			return signInManager;
		}

		#endregion

		#region RegisterApplicationUserAsync Tests

		[Fact]
		public async Task RegisterApplicationUserAsync_ShouldRegisterApplicationUser_WhenDataIsValidAndEmailNotExists()
		{
			// Arrange
			var registerDto = new RegisterApplicationUserDto
			{
				Email = "newuser@example.com",
				FullName = "New User",
				Password = "Password@123"
			};

			_userManagerMock.Setup(u => u.FindByEmailAsync(registerDto.Email))
				.ReturnsAsync((ApplicationUser)null);

			_userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
				.ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _memberService.RegisterApplicationUserAsync(registerDto);

			// Assert
			result.Succeeded.ShouldBeTrue();
			_userManagerMock.Verify(u => u.FindByEmailAsync(registerDto.Email), Times.Once);
			_userManagerMock.Verify(u => u.CreateAsync(It.Is<ApplicationUser>(m =>
				m.Email == registerDto.Email &&
				m.FullName == registerDto.FullName &&
				m.UserName == registerDto.Email &&
				m.DateJoined <= DateTime.Now
			), registerDto.Password), Times.Once);
		}

		[Fact]
		public async Task RegisterApplicationUserAsync_ShouldFail_WhenEmailAlreadyExists()
		{
			var id1 = Guid.NewGuid();
			var id2 = Guid.NewGuid();
			var id3 = Guid.NewGuid();
			// Arrange
			var registerDto = new RegisterApplicationUserDto
			{
				Email = "existinguser@example.com",
				FullName = "Existing User",
				Password = "Password@123"
			};

			var existingApplicationUser = new ApplicationUser
			{
				Id = id1,
				Email = registerDto.Email,
				UserName = registerDto.Email,
				FullName = "Existing User",
				DateJoined = DateTime.Now.AddDays(-10)
			};

			_userManagerMock.Setup(u => u.FindByEmailAsync(registerDto.Email))
				.ReturnsAsync(existingApplicationUser);

			// Act
			var result = await _memberService.RegisterApplicationUserAsync(registerDto);

			// Assert
			result.Succeeded.ShouldBeFalse();
			result.Errors.ShouldContain(e => e.Description == "Email already in use");
			_userManagerMock.Verify(u => u.FindByEmailAsync(registerDto.Email), Times.Once);
			_userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public async Task RegisterApplicationUserAsync_ShouldReturnFailure_WhenUserManagerFailsToCreate()
		{
			// Arrange
			var registerDto = new RegisterApplicationUserDto
			{
				Email = "newuser@example.com",
				FullName = "New User",
				Password = "Password@123"
			};

			_userManagerMock.Setup(u => u.FindByEmailAsync(registerDto.Email))
				.ReturnsAsync((ApplicationUser)null);

			var identityErrors = new IdentityError[] { new IdentityError { Description = "Password too weak" } };
			var identityResult = IdentityResult.Failed(identityErrors);
			_userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
				.ReturnsAsync(identityResult);

			// Act
			var result = await _memberService.RegisterApplicationUserAsync(registerDto);

			// Assert
			result.Succeeded.ShouldBeFalse();
			result.Errors.ShouldContain(e => e.Description == "Password too weak");
			_userManagerMock.Verify(u => u.FindByEmailAsync(registerDto.Email), Times.Once);
			_userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password), Times.Once);
		}

		#endregion

		#region LoginApplicationUserAsync Tests

		[Fact]
		public async Task LoginApplicationUserAsync_ShouldSucceed_WhenCredentialsAreValid()
		{
			var id1 = Guid.NewGuid();
			// Arrange
			var loginDto = new LoginApplicationUserDto
			{
				Email = "validuser@example.com",
				Password = "ValidPassword@123",
				RememberMe = true
			};

			var member = new ApplicationUser
			{
				Id = id1,
				Email = loginDto.Email,
				UserName = loginDto.Email
			};

			_userManagerMock.Setup(u => u.FindByEmailAsync(loginDto.Email))
				.ReturnsAsync(member);

			_signInManagerMock.Setup(s => s.PasswordSignInAsync(
				member,
				loginDto.Password,
				loginDto.RememberMe,
				false))
				.ReturnsAsync(SignInResult.Success);

			// Act
			var result = await _memberService.LoginApplicationUserAsync(loginDto);

			// Assert
			result.Succeeded.ShouldBeTrue();
			_userManagerMock.Verify(u => u.FindByEmailAsync(loginDto.Email), Times.Once);
			_signInManagerMock.Verify(s => s.PasswordSignInAsync(
				member,
				loginDto.Password,
				loginDto.RememberMe,
				false), Times.Once);
		}

		[Fact]
		public async Task LoginApplicationUserAsync_ShouldFail_WhenUserDoesNotExist()
		{
			// Arrange
			var loginDto = new LoginApplicationUserDto
			{
				Email = "nonexistent@example.com",
				Password = "AnyPassword",
				RememberMe = false
			};

			_userManagerMock.Setup(u => u.FindByEmailAsync(loginDto.Email))
				.ReturnsAsync((ApplicationUser)null);

			// Optionally, depending on implementation, you might set up SignInManager to handle null
			_signInManagerMock.Setup(s => s.PasswordSignInAsync(
				It.IsAny<ApplicationUser>(),
				It.IsAny<string>(),
				It.IsAny<bool>(),
				It.IsAny<bool>()))
				.ReturnsAsync(SignInResult.Failed);

			// Act
			var result = await _memberService.LoginApplicationUserAsync(loginDto);

			// Assert
			result.Succeeded.ShouldBeFalse();
			_userManagerMock.Verify(u => u.FindByEmailAsync(loginDto.Email), Times.Once);
			_signInManagerMock.Verify(s => s.PasswordSignInAsync(
				It.IsAny<ApplicationUser>(),
				It.IsAny<string>(),
				It.IsAny<bool>(),
				It.IsAny<bool>()), Times.Never); // Since user doesn't exist
		}

		[Fact]
		public async Task LoginApplicationUserAsync_ShouldFail_WhenPasswordIsIncorrect()
		{
			var id1 = Guid.NewGuid();
			// Arrange
			var loginDto = new LoginApplicationUserDto
			{
				Email = "validuser@example.com",
				Password = "WrongPassword",
				RememberMe = false
			};

			var member = new ApplicationUser
			{
				Id = id1,
				Email = loginDto.Email,
				UserName = loginDto.Email
			};

			_userManagerMock.Setup(u => u.FindByEmailAsync(loginDto.Email))
				.ReturnsAsync(member);

			_signInManagerMock.Setup(s => s.PasswordSignInAsync(
				member,
				loginDto.Password,
				loginDto.RememberMe,
				false))
				.ReturnsAsync(SignInResult.Failed);

			// Act
			var result = await _memberService.LoginApplicationUserAsync(loginDto);

			// Assert
			result.Succeeded.ShouldBeFalse();
			_userManagerMock.Verify(u => u.FindByEmailAsync(loginDto.Email), Times.Once);
			_signInManagerMock.Verify(s => s.PasswordSignInAsync(
				member,
				loginDto.Password,
				loginDto.RememberMe,
				false), Times.Once);
		}

		#endregion

		#region GetApplicationUserDetailsAsync Tests

		[Fact]
		public async Task GetApplicationUserDetailsAsync_ShouldReturnApplicationUserDto_WhenApplicationUserExists()
		{
			// Arrange
			var memberId = Guid.NewGuid();
			var member = new ApplicationUser
			{
				Id = memberId,
				Email = "existing@example.com",
				FullName = "Existing ApplicationUser",
				DateJoined = DateTime.Now.AddMonths(-1)
			};

			_userManagerMock.Setup(u => u.FindByIdAsync(memberId.ToString()))
				.ReturnsAsync(member);

			// Act
			var result = await _memberService.GetApplicationUserDetailsAsync(memberId);

			// Assert
			result.ShouldNotBeNull();
			result.Id.ShouldBe(memberId);
			result.Email.ShouldBe(member.Email);
			result.FullName.ShouldBe(member.FullName);
			result.DateJoined.ShouldBe(member.DateJoined);
			_userManagerMock.Verify(u => u.FindByIdAsync(memberId.ToString()), Times.Once);
		}

		[Fact]
		public async Task GetApplicationUserDetailsAsync_ShouldReturnNull_WhenApplicationUserDoesNotExist()
		{
			// Arrange
			var memberId = Guid.NewGuid();

			_userManagerMock.Setup(u => u.FindByIdAsync(memberId.ToString()))
				.ReturnsAsync((ApplicationUser)null);

			// Act
			var result = await _memberService.GetApplicationUserDetailsAsync(memberId);

			// Assert
			result.ShouldBeNull();
			_userManagerMock.Verify(u => u.FindByIdAsync(memberId.ToString()), Times.Once);
		}

		#endregion

		#region UpdateApplicationUserDetailsAsync Tests

		[Fact]
		public async Task UpdateApplicationUserDetailsAsync_ShouldUpdateApplicationUser_WhenDataIsValidAndEmailNotTaken()
		{
		var memberId = Guid.NewGuid();
			// Arrange
			var updateDto = new UpdateApplicationUserDto
			{
				Id = memberId,
				FullName = "Updated Name",
				Email = "updatedemail@example.com"
			};

			var member = new ApplicationUser
			{
				Id = memberId,
				Email = "oldemail@example.com",
				UserName = "oldemail@example.com",
				FullName = "Old Name",
				DateJoined = DateTime.Now.AddMonths(-2)
			};

			_userManagerMock.Setup(u => u.FindByIdAsync(updateDto.Id.ToString()))
				.ReturnsAsync(member);

			_userManagerMock.Setup(u => u.FindByEmailAsync(updateDto.Email))
				.ReturnsAsync((ApplicationUser)null);

			_userManagerMock.Setup(u => u.UpdateAsync(member))
				.ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _memberService.UpdateApplicationUserDetailsAsync(updateDto);

			// Assert
			result.Succeeded.ShouldBeTrue();
			member.FullName.ShouldBe(updateDto.FullName);
			member.Email.ShouldBe(updateDto.Email);
			member.UserName.ShouldBe(updateDto.Email);
			_userManagerMock.Verify(u => u.FindByIdAsync(updateDto.Id.ToString()), Times.Once);
			_userManagerMock.Verify(u => u.FindByEmailAsync(updateDto.Email), Times.Once);
			_userManagerMock.Verify(u => u.UpdateAsync(member), Times.Once);
		}

		[Fact]
		public async Task UpdateApplicationUserDetailsAsync_ShouldFail_WhenApplicationUserDoesNotExist()
		{
			var memberId = Guid.NewGuid();
			// Arrange
			var updateDto = new UpdateApplicationUserDto
			{
				Id = memberId,
				FullName = "Updated Name",
				Email = "updatedemail@example.com"
			};

			_userManagerMock.Setup(u => u.FindByIdAsync(updateDto.Id.ToString()))
				.ReturnsAsync((ApplicationUser)null);

			// Act
			var result = await _memberService.UpdateApplicationUserDetailsAsync(updateDto);

			// Assert
			result.Succeeded.ShouldBeFalse();
			result.Errors.ShouldContain(e => e.Description == "ApplicationUser not found");
			_userManagerMock.Verify(u => u.FindByIdAsync(updateDto.Id.ToString()), Times.Once);
			_userManagerMock.Verify(u => u.FindByEmailAsync(It.IsAny<string>()), Times.Never);
			_userManagerMock.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
		}

		[Fact]
		public async Task UpdateApplicationUserDetailsAsync_ShouldFail_WhenEmailIsAlreadyTakenByAnotherApplicationUser()
		{
		var memberId = Guid.NewGuid();
		var memberId2 = Guid.NewGuid();
			// Arrange
			var updateDto = new UpdateApplicationUserDto
			{
				Id = memberId,
				FullName = "Updated Name",
				Email = "existingemail@example.com"
			};

			var member = new ApplicationUser
			{
				Id = updateDto.Id,
				Email = "oldemail@example.com",
				UserName = "oldemail@example.com",
				FullName = "Old Name",
				DateJoined = DateTime.Now.AddMonths(-2)
			};

			var anotherApplicationUserWithSameEmail = new ApplicationUser
			{
				Id = memberId2,
				Email = updateDto.Email,
				UserName = updateDto.Email,
				FullName = "Another ApplicationUser",
				DateJoined = DateTime.Now.AddMonths(-1)
			};

			_userManagerMock.Setup(u => u.FindByIdAsync(updateDto.Id.ToString()))
				.ReturnsAsync(member);

			_userManagerMock.Setup(u => u.FindByEmailAsync(updateDto.Email))
				.ReturnsAsync(anotherApplicationUserWithSameEmail);

			// Act
			var result = await _memberService.UpdateApplicationUserDetailsAsync(updateDto);

			// Assert
			result.Succeeded.ShouldBeFalse();
			result.Errors.ShouldContain(e => e.Description == "Email is already in use by another member");
			_userManagerMock.Verify(u => u.FindByIdAsync(updateDto.Id.ToString()), Times.Once);
			_userManagerMock.Verify(u => u.FindByEmailAsync(updateDto.Email), Times.Once);
			_userManagerMock.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
		}

		[Fact]
		public async Task UpdateApplicationUserDetailsAsync_ShouldReturnFailure_WhenUserManagerFailsToUpdate()
		{
			var meberId = Guid.NewGuid();
			// Arrange
			var updateDto = new UpdateApplicationUserDto
			{
				Id = meberId,
				FullName = "Updated Name",
				Email = "updatedemail@example.com"
			};

			var member = new ApplicationUser
			{
				Id = updateDto.Id,
				Email = "oldemail@example.com",
				UserName = "oldemail@example.com",
				FullName = "Old Name",
				DateJoined = DateTime.Now.AddMonths(-2)
			};

			_userManagerMock.Setup(u => u.FindByIdAsync(updateDto.Id.ToString()))
				.ReturnsAsync(member);

			_userManagerMock.Setup(u => u.FindByEmailAsync(updateDto.Email))
				.ReturnsAsync((ApplicationUser)null);

			var identityErrors = new IdentityError[] { new IdentityError { Description = "Failed to update member" } };
			var identityResult = IdentityResult.Failed(identityErrors);
			_userManagerMock.Setup(u => u.UpdateAsync(member))
				.ReturnsAsync(identityResult);

			// Act
			var result = await _memberService.UpdateApplicationUserDetailsAsync(updateDto);

			// Assert
			result.Succeeded.ShouldBeFalse();
			result.Errors.ShouldContain(e => e.Description == "Failed to update member");
			_userManagerMock.Verify(u => u.FindByIdAsync(updateDto.Id.ToString()), Times.Once);
			_userManagerMock.Verify(u => u.FindByEmailAsync(updateDto.Email), Times.Once);
			_userManagerMock.Verify(u => u.UpdateAsync(member), Times.Once);
		}

		#endregion
	}
}
