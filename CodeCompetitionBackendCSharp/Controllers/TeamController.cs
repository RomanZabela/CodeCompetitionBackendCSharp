using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;
using CodeCompetitionBackendCSharp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly ITaskToTeamService _taskToTeamService;
        private readonly IMapper _mapper;
		private readonly ILoggerManager _logger;

		public TeamController(
            ITeamService teamService,
            ITaskToTeamService taskToTeamService,
            IMapper mapper,
            ILoggerManager logger)
        {
            _mapper = mapper;
			_logger = logger;
			_teamService = teamService;
            _taskToTeamService = taskToTeamService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Team>))]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetTeams([FromQuery] TeamParameters teamParameters)
        {
            try
            {
                var teams = await _teamService.GetTeams(teamParameters);
                var teamsDto = _mapper.Map<ICollection<TeamDto>>(teams);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(teams.MetaData));

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
                }

				_logger.LogInfo($"200. Teams: {teamsDto.Count}.");
				return Ok(teamsDto);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("extended")]
        [ProducesResponseType(200, Type = typeof(PagedList<TeamExtendedDto>))]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetTeamsExtended([FromQuery] TeamParameters teamParameters)
        {
            try
            {
                var teams = await _teamService.GetTeamsExtended(teamParameters);
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(teams.MetaData));

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"200. Teams: {teams.Count}.");
				return Ok(teams);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{teamId}")]
        [ProducesResponseType(200, Type = typeof(Models.Team))]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetTeam(int teamId)
        {
            try
            {
                bool isExist = await _teamService.TeamExists(teamId);
                if (!isExist)
                {
					_logger.LogError($"404. Team {teamId} couldn't found.");
					return NotFound();
                }

                Team team = await _teamService.GetTeam(teamId);
                TeamDto teamDto = _mapper.Map<TeamDto>(team);

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"200. Team {teamDto.Id}.");
				return Ok(teamDto);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("teamByParticipant/{participantId}")]
        [ProducesResponseType(200, Type = typeof(Models.Team))]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> GetTeamByParticipantId(int participantId)
        {
            try
            {
                Team team = await _teamService.GetTeamByParticipantId(participantId);
                TeamDto teamDto = _mapper.Map<TeamDto>(team);

                if (!ModelState.IsValid)
                {
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				_logger.LogInfo($"200. Team {teamDto.Id}.");
				return Ok(teamDto);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }   
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateTeam([FromBody] TeamDto teamCreate)
        {
            if (teamCreate == null)
            {
                _logger.LogError($"400. Bad Request. {ModelState.ToString()}");
                return BadRequest(ModelState);
            }

            try
            {
                TeamParameters teamParameters = new TeamParameters();
                teamParameters.switchOff = true;
                var teamsRepeated = await _teamService.GetTeams(teamParameters);
                var teamsRepeatedFiltered = teamsRepeated.Where(c => c.TeamName.Trim().ToUpper() == teamCreate.TeamName.TrimEnd().ToUpper()).FirstOrDefault();

                if (teamsRepeatedFiltered != null)
                {
                    ModelState.AddModelError("", "Review already exists");
                    return StatusCode(422, ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError($"400. Bad Request. {ModelState.ToString()}");
                return BadRequest(ModelState);
            }

            var teamMap = _mapper.Map<Team>(teamCreate);

            teamMap.UpdateDate = DateTime.Now;
            teamMap.CreateDate = DateTime.Now;
            teamMap.CreateUserId = Enum.TEMP_USER_ID;
            teamMap.UpdateUserId = Enum.TEMP_USER_ID;
            teamMap.StatusId = (int)Enum.Status.Active;

            try
            {
                await _teamService.CreateTeam(teamMap);

                _logger.LogInfo($"200. Team {teamMap.TeamName} created");
                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"500. {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{teamId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] TeamDto updatedTeam)
        {
            if (updatedTeam == null)
            {
				_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
				return BadRequest(ModelState);
			}

            if (teamId != updatedTeam.Id)
            {
				_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
				return BadRequest(ModelState);
			}
            try
            {
                bool isExist = await _teamService.TeamExists(teamId);
                if (!isExist)
                {
                    _logger.LogError($"404. Team {teamId} couldn't found.");
                    return NotFound();
                }
            }
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}

            if (!ModelState.IsValid)
            {
				_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
				return BadRequest(ModelState);
			}

            var teamMap = _mapper.Map<Team>(updatedTeam);

            teamMap.UpdateDate = DateTime.Now;
            teamMap.UpdateUserId = Enum.TEMP_USER_ID;

            try
            {
                bool isUpdated = await _teamService.UpdateTeam(teamMap);
                if (!isUpdated)
                {
                    ModelState.AddModelError("", "Something went wrong while updating team");

					_logger.LogError($"500. {ModelState.ToString()}");
					return StatusCode(500, ModelState);
                }

                _logger.LogInfo($"204. Team {teamId} updated.");
                return NoContent(); 
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
           
        }

        [HttpDelete("{teamId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            if (!ModelState.IsValid)
            {
				_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
				return BadRequest(ModelState);
			}

            try
            {
                bool isExist = await _teamService.TeamExists(teamId);
                if (!isExist)
                {
                    _logger.LogError($"404. Team {teamId} couldn't found");
                    return NotFound();
                }

                var teamToDelete = await _teamService.GetTeam(teamId);

                Enum.Status newStatus = Enum.Status.NotActive;
                DateTime updateDate = DateTime.Now;
                int updateUserId = Enum.TEMP_USER_ID;
                bool isDeleted = await _teamService.DeleteTeam(teamToDelete, newStatus, updateDate, updateUserId);

                if (!isDeleted)
                {
                    ModelState.AddModelError("", "Something went wrong while updating team");
                    _logger.LogError($"500. {ModelState.ToString()}");
                    return StatusCode(500, ModelState);
                }
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }

            try
            {
                var teamTasks = await _taskToTeamService.GetTeamsTasks();
                var teamTasksToDelete = teamTasks.Where(tt => tt.TeamId == teamId);

                foreach (var teamTask in teamTasksToDelete)
                {
                    await _taskToTeamService.DeleteTeamTask(Enum.TaskStatus.Fail, teamTask);
                }

                _logger.LogInfo($"204. Team {teamId} deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }
    }
}
