using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using DatingApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPhotoManagerService _photoService;
        
        public AdminController(DataContext context, UserManager<User> userManager, 
            IDatingRepository repo, 
            IMapper mapper,
            IPhotoManagerService photoService)
        {
            _userManager = userManager;
            _context = context;
            _repo = repo; 
            _mapper = mapper;
            _photoService = photoService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GeUsersWithRoles()
        {

            var userList = await (
                from user in _context.Users
                orderby user.UserName
                select new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Roles = (
                        from userRole in user.UserRoles
                        join role in _context.Roles
                        on userRole.RoleId equals role.Id
                        select role.Name
                    )
                }
            ).ToListAsync();

            // var userList = await _context.Users
            //     .OrderBy(u => u.UserName)
            //     .Select(u => new {
            //         Id = u.Id,
            //         UserName = u.UserName,
            //         Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            //     }).ToListAsync();

            return Ok(userList);
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDto.RoleNames;
            selectedRoles = selectedRoles ?? new string[] {};

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded)
                return BadRequest("Failed to add to roles");
            
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if(!result.Succeeded)
                return BadRequest("Failed to remove the roles");

            return Ok(await _userManager.GetRolesAsync(user));

        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photosForModerateFromRepo = await _repo.GetPhotosForModeration();
            var photosForModerateToReturn = _mapper.Map<List<PhotosForDetailedDto>>(photosForModerateFromRepo);
            return Ok(photosForModerateToReturn);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("photo/{id}/approve")]
        public async Task<IActionResult> ApprovePhoto(int id)
        {
            await _photoService.ApprovePhoto(id);
            return Ok();
        }


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("photo/{id}/reject")]
        public async Task<IActionResult> RejectPhoto(int id)
        {
            await _photoService.DeletePhoto(id, false);
            return Ok();
        }


    }
}