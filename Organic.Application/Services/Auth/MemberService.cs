using Microsoft.AspNetCore.Identity;
using Organic.Application.Dtos.ApplicationUserDtos;
using Organic.Core.Entities;
using Organic.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organic.Application.Services.Auth
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ApplicationUserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IdentityResult> RegisterApplicationUserAsync(RegisterApplicationUserDto dto)
        {
            // Check if a member with this email already exists
            var existingApplicationUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingApplicationUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already in use" });
            }

            var member = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                DateJoined = DateTime.Now
            };

            return await _userManager.CreateAsync(member, dto.Password);
        }

        public async Task<SignInResult> LoginApplicationUserAsync(LoginApplicationUserDto dto)
        {
            var member = await _userManager.FindByEmailAsync(dto.Email);
            if (member == null)
            {
                return SignInResult.Failed;
            }

            return await _signInManager.PasswordSignInAsync(member, dto.Password, dto.RememberMe, lockoutOnFailure: false);
        }

        public async Task<ApplicationUserDto> GetApplicationUserDetailsAsync(Guid memberId)
        {
            var member = await _userManager.FindByIdAsync(memberId.ToString());
            if (member == null)
            {
                return null; // Alternatively, throw an exception or return a NotFound DTO
            }

            return new ApplicationUserDto
            {
                Id = member.Id,
                FullName = member.FullName,
                Email = member.Email,
                DateJoined = member.DateJoined
            };
        }
        public async Task<IdentityResult> UpdateApplicationUserDetailsAsync(UpdateApplicationUserDto dto)
        {
            var member = await _userManager.FindByIdAsync(dto.Id.ToString());
            if (member == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "ApplicationUser not found" });
            }

            // Check if the new email is already taken by another user
            var existingApplicationUserWithEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingApplicationUserWithEmail != null && existingApplicationUserWithEmail.Id != member.Id)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email is already in use by another member" });
            }

            member.FullName = dto.FullName;
            member.Email = dto.Email;
            member.UserName = dto.Email; // Update username if email is changed

            return await _userManager.UpdateAsync(member);
        }
    }
}
