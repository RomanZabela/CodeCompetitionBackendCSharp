using Auth0.ManagementApi;
using CodeCompetitionBackendCSharp.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IManagementApiClient _managementApiClient;

        public UserController(IManagementApiClient managementApiClient)
        {
            _managementApiClient = managementApiClient;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _managementApiClient.Users.GetAllAsync(
                new GetUsersRequest(), new PaginationInfo()
            );
            return users.Select(x => new UserDto
            {
                Id = x.UserId,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Blocked = x.Blocked ?? false
            });

        }


        [HttpGet("withRoles")]
        public async Task<IEnumerable<UserDtoExtended>> GetUsersWithRoles()
        {
            var users = await _managementApiClient.Users.GetAllAsync(
                new GetUsersRequest(), new PaginationInfo()
            );

            List<UserDtoExtended> userList = users.Select(x => new UserDtoExtended
            {
                Id = x.UserId,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Blocked = x.Blocked ?? false,
                RoleName = "",
                RoleId = ""
            }).ToList();

            var userRoles = new List<RoleDto>();

            foreach(var user in userList)
            {
                var roles = await _managementApiClient.Users.GetRolesAsync(
                    user.Id, new PaginationInfo()
                );

                if (roles.Count > 0) 
                {
                    userRoles.Add(roles.Select(x => new RoleDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        UserId = user.Id
                    }).FirstOrDefault());
                }
            }

            foreach(var user in userList)
            {
                if (userRoles.Exists(ur => ur.UserId == user.Id))
                {
                    user.RoleName = userRoles.Where(ur => ur.UserId == user.Id).FirstOrDefault().Name;
                    user.RoleId = userRoles.Where(ur => ur.UserId == user.Id).FirstOrDefault().Id;
                }
            }

            return userList;
        }

        [HttpGet("allRoles")]
        public async Task<IEnumerable<RoleDto>> GetRoles()
        {
            var roles = await _managementApiClient.Roles.GetAllAsync(
                new GetRolesRequest(), new PaginationInfo()
            );

            return roles.Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            });

        }

        [HttpGet("userRoles/{Id}")]
        public async Task<IEnumerable<RoleDto>> GetUserRolesByUserId(string Id)
        {
            var roles = await _managementApiClient.Users.GetRolesAsync(
                Id, new PaginationInfo()
            );

            return roles.Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            });

        }



        [HttpDelete("deleteRoles/{Id}")]
        public async Task<IActionResult> DeleteUserRolesByUserId(string Id)
        {
            var rolesToDelete = await _managementApiClient.Users.GetRolesAsync(
                Id, new PaginationInfo()
            );

            var rolesIdToDeleteStr = rolesToDelete.Select(x => x.Id).ToArray();

            try
            {
                await _managementApiClient.Users.RemoveRolesAsync(
                    Id, new AssignRolesRequest() { Roles = rolesIdToDeleteStr }
                );
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut("assignRoles/{Id}")]
        public async Task<IActionResult> AssignRoleToUser(string Id, [FromBody] string[] roles)
        {

            var rolesToDelete = await _managementApiClient.Users.GetRolesAsync(
                Id, new PaginationInfo()
            );

            var rolesIdToDeleteStr = rolesToDelete.Select(x => x.Id).ToArray();

            // Delete all existing roles
            if (rolesIdToDeleteStr.Length > 0)
            {
                try
                {
                    await _managementApiClient.Users.RemoveRolesAsync(
                        Id, new AssignRolesRequest() { Roles = rolesIdToDeleteStr }
                    );
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
             
            // Add new role
            try
            {
                await _managementApiClient.Users.AssignRolesAsync(Id, new AssignRolesRequest() { Roles = roles } );
                return Ok("Successfully updated");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    

    }

}
    

