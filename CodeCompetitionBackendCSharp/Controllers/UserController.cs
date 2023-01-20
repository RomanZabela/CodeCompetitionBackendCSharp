using Auth0.ManagementApi;
using CodeCompetitionBackendCSharp.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using CodeCompetitionBackendCSharp.RequestFeatures;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.Models;
using Newtonsoft.Json;

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
		[ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
		public async Task<IActionResult> GetUsers()
        {
            try
            {
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var users = await _managementApiClient.Users.GetAllAsync(
								new GetUsersRequest(), new PaginationInfo()
							);

				return Ok(users.Select(x => new UserDto
				{
					Id = x.UserId,
					Email = x.Email,
					FirstName = x.FirstName,
					LastName = x.LastName,
					Blocked = x.Blocked ?? false
				}));
			}
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }            

        }


        [HttpGet("withRoles")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<UserDtoExtended>))]
		public async Task<IActionResult> GetUsersWithRoles([FromQuery] UserParameters userParameters)
        {

            try
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

				foreach (var user in userList)
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

				foreach (var user in userList)
				{
					if (userRoles.Exists(ur => ur.UserId == user.Id))
					{
						user.RoleName = userRoles.Where(ur => ur.UserId == user.Id).FirstOrDefault().Name;
						user.RoleId = userRoles.Where(ur => ur.UserId == user.Id).FirstOrDefault().Id;
					}
				}

				var toReturn = Paging.PagedList<UserDtoExtended>.ToPagedList(userList, userParameters.PageNumber, userParameters.PageSize, userParameters.switchOff);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(toReturn.MetaData));
                return Ok(toReturn);
            }
            catch (Exception ex)
            {
				return StatusCode(500, ex.Message);
			}

        }

        [HttpGet("allRoles")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<RoleDto>))]
		public async Task<IActionResult> GetRoles()
        {
            try
            {
				var roles = await _managementApiClient.Roles.GetAllAsync(
				new GetRolesRequest(), new PaginationInfo()
			);

				return Ok(roles.Select(x => new RoleDto
				{
					Id = x.Id,
					Name = x.Name,
					Description = x.Description
				}));
			}
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            

        }

        [HttpGet("userRoles/{Id}")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<RoleDto>))]
		public async Task<IActionResult> GetUserRolesByUserId(string Id)
        {
            try
            {
				var roles = await _managementApiClient.Users.GetRolesAsync(
				Id, new PaginationInfo()
			);

				return Ok(roles.Select(x => new RoleDto
				{
					Id = x.Id,
					Name = x.Name,
					Description = x.Description
				}));
			}
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("deleteRoles/{Id}")]
		[ProducesResponseType(204)]

		public async Task<IActionResult> DeleteUserRolesByUserId(string Id)
        {
            try
            {
                var rolesToDelete = await _managementApiClient.Users.GetRolesAsync(Id, new PaginationInfo());

                var rolesIdToDeleteStr = rolesToDelete.Select(x => x.Id).ToArray();

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
		[ProducesResponseType(200, Type = typeof(string))]
		public async Task<IActionResult> AssignRoleToUser(string Id, [FromBody] string[] roles)
		{
			try
			{
				var rolesToDelete = await _managementApiClient.Users.GetRolesAsync(Id, new PaginationInfo());

				var rolesIdToDeleteStr = rolesToDelete.Select(x => x.Id).ToArray();

				if (rolesIdToDeleteStr.Length > 0)
				{
					// Delete all existing roles
					await _managementApiClient.Users.RemoveRolesAsync(Id, new AssignRolesRequest() { Roles = rolesIdToDeleteStr });
				}
				// Add new role
				await _managementApiClient.Users.AssignRolesAsync(Id, new AssignRolesRequest() { Roles = roles });
				return Ok("Successfully updated");
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
    }
}