using Auth0.ManagementApi;
using CodeCompetitionBackendCSharp.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;
using Newtonsoft.Json;
using CodeCompetitionBackendCSharp.Interface;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IManagementApiClient _managementApiClient;
		private readonly ILoggerManager _logger;

		public UserController(
            IManagementApiClient managementApiClient,
            ILoggerManager logger)
        {
            _managementApiClient = managementApiClient;
			_logger = logger;
		}

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUsers()
        {
			_logger.LogDebug($"GetUsers started.");

            try
            {
                var users = await _managementApiClient.Users.GetAllAsync(
                    new GetUsersRequest(),
                    new PaginationInfo());

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

                var result = users.Select(x => new UserDto
                {
                    Id = x.UserId,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Blocked = x.Blocked ?? false
                });

				_logger.LogInfo($"200. Users: {JsonConvert.SerializeObject(result)}");
				return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }

        }


        [HttpGet("withRoles")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<UserDtoExtended>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetUsersWithRoles([FromQuery] UserParameters userParameters)
        {
			_logger.LogDebug($"GetUsersWithRoles UserParameters: {JsonConvert.SerializeObject(userParameters)} started.");

            try
            {
				var users = await _managementApiClient.Users.GetAllAsync(
				    new GetUsersRequest(),
                    new PaginationInfo()
			        );

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

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

					if (!ModelState.IsValid)
					{
						_logger.LogError($"400. Bad Request. {ModelState}");
						return BadRequest(ModelState);
					}

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

				_logger.LogInfo($"200. Users: {JsonConvert.SerializeObject(toReturn)}");
				return Ok(toReturn);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}

        }

        [HttpGet("allRoles")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<UserDtoExtended>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetRoles()
        {
			_logger.LogDebug($"GetRoles started.");

            try
            {
				var roles = await _managementApiClient.Roles.GetAllAsync(
				new GetRolesRequest(),
				new PaginationInfo()
				);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				var result = roles.Select(x => new RoleDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                });

				_logger.LogInfo($"200. Roles: {JsonConvert.SerializeObject(result)}");
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}

		}

        [HttpGet("userRoles/{Id}")]
		[ProducesResponseType(200, Type = typeof(IEnumerable<UserDtoExtended>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetUserRolesByUserId(string Id)
        {
			_logger.LogDebug($"GetUserRolesByUserId ID: {Id}");

            try
            {
				var roles = await _managementApiClient.Users.GetRolesAsync(
				Id, new PaginationInfo()
				);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				var result = roles.Select(x => new RoleDto
				{
					Id = x.Id,
					Name = x.Name,
					Description = x.Description
				});

				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}

		}

        [HttpDelete("deleteRoles/{Id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]

		public async Task<IActionResult> DeleteUserRolesByUserId(string Id)
        {
			_logger.LogDebug($"DeleteUserRolesByUserId ID: Id");

            try
            {
				var rolesToDelete = await _managementApiClient.Users.GetRolesAsync(
				Id, new PaginationInfo()
				);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				var rolesIdToDeleteStr = rolesToDelete.Select(x => x.Id).ToArray();

				await _managementApiClient.Users.RemoveRolesAsync(
                    Id, new AssignRolesRequest() { Roles = rolesIdToDeleteStr }
                );

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"204. UserRoles for userId {Id} deleted.");
				return NoContent();
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("assignRoles/{Id}")]
		[ProducesResponseType(200, Type = typeof(string))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]

		public async Task<IActionResult> AssignRoleToUser(string Id, [FromBody] string[] roles)
        {
			_logger.LogDebug($"AssignRoleToUser ID: {Id}, Roles: {JsonConvert.SerializeObject(roles)}");

			try
			{
				var rolesToDelete = await _managementApiClient.Users.GetRolesAsync(
								Id, new PaginationInfo()
							);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				var rolesIdToDeleteStr = rolesToDelete.Select(x => x.Id).ToArray();

				// Delete all existing roles
				if (rolesIdToDeleteStr.Length > 0)
				{
					await _managementApiClient.Users.RemoveRolesAsync(
						Id, new AssignRolesRequest() { Roles = rolesIdToDeleteStr }
						);

					if (!ModelState.IsValid)
					{
						_logger.LogError($"400. Bad Request. {ModelState}");
						return BadRequest(ModelState);
					}

				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}

			// Add new role
			try
            {
                await _managementApiClient.Users.AssignRolesAsync(Id, new AssignRolesRequest() { Roles = roles } );

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				return Ok("Successfully updated");
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }
    }
}