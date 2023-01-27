using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Paging;
using CodeCompetitionBackendCSharp.RequestFeatures;
using CodeCompetitionBackendCSharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
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
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(200, Type = typeof(PagedList<TeamDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetTeams([FromQuery] TeamParameters teamParameters)
        {
			
            _logger.LogDebug($"GetTeams {JsonConvert.SerializeObject(teamParameters)} started.");

			try
            {
                var teams = await _teamService.GetTeams(teamParameters);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(teams.MetaData));

				_logger.LogInfo($"200. Teams: {JsonConvert.SerializeObject(teams)}.");
				return Ok(teams);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("extended")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(200, Type = typeof(PagedList<TeamExtendedDto>))]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetTeamsExtended([FromQuery] TeamParameters teamParameters)
        {
            _logger.LogDebug($"GetTeamsExtended {teamParameters} started");

            try
            {
                var teams = await _teamService.GetTeamsExtended(teamParameters);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(teams.MetaData));

				_logger.LogInfo($"200. Teams: {JsonConvert.SerializeObject(teams)}.");
				return Ok(teams);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{teamId}")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(200, Type = typeof(Models.Team))]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetTeam(int teamId)
        {
            _logger.LogDebug($"GetTeam {teamId} started");

            try
            {
                bool isExist = await _teamService.TeamExists(teamId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				if (!isExist)
                {
					_logger.LogError($"404. Team {teamId} couldn't found.");
					return NotFound();
                }

                Team team = await _teamService.GetTeam(teamId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				TeamDto teamDto = _mapper.Map<TeamDto>(team);

				_logger.LogInfo($"200. Team {JsonConvert.SerializeObject(teamDto)}.");
				return Ok(teamDto);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("teamByParticipant/{participantId}")]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(200, Type = typeof(Team))]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]

		public async Task<IActionResult> GetTeamByParticipantId(int participantId)
        {
            _logger.LogDebug($"GetTeamByParticipantId {participantId} started");

            try
            {                
                Team team = await _teamService.GetTeamByParticipantId(participantId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
				}

				TeamDto teamDto = _mapper.Map<TeamDto>(team);

				_logger.LogInfo($"200. Team {JsonConvert.SerializeObject(teamDto)}.");
				return Ok(teamDto);
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
            }   
        }

        [HttpPost]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> CreateTeam([FromBody] TeamDto teamCreate)
        {
            _logger.LogDebug($"CreateTeam {JsonConvert.SerializeObject(teamCreate)} started");

			if (teamCreate == null)
			{
				_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
				return BadRequest(ModelState);
			}

			try
            {
                TeamParameters teamParameters = new TeamParameters();
                teamParameters.SwitchOff = true;
                var teamsRepeated = await _teamService.GetTeams(teamParameters);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				var teamsRepeatedFiltered = teamsRepeated.Where(c => c.TeamName.Trim().ToUpper() == teamCreate.TeamName.TrimEnd().ToUpper()).FirstOrDefault();

                if (teamsRepeatedFiltered != null)
                {
					_logger.LogError($"409. Task with the same name {teamCreate.TeamName} already exist in DB");
					return Conflict("Task with the same name already exist in DB");
				}
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
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

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

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
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] TeamDto updatedTeam)
        {
            _logger.LogDebug($"UpdateTeam {teamId} with {JsonConvert.SerializeObject(updatedTeam)} started");

			if (updatedTeam == null)
			{
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
			}

			if (teamId != updatedTeam.Id)
            {
				_logger.LogError($"400. Bad Request. {ModelState}");
				return BadRequest(ModelState);
			}

            try
            {
				bool isExist = await _teamService.TeamExists(teamId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

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

            var teamMap = _mapper.Map<Team>(updatedTeam);

            teamMap.UpdateDate = DateTime.Now;
            teamMap.UpdateUserId = Enum.TEMP_USER_ID;

            try
            {
                bool isUpdated = await _teamService.UpdateTeam(teamMap);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				if (!isUpdated)
                {
                    ModelState.AddModelError("", "Something went wrong while updating team");

					_logger.LogError($"500. {ModelState}");
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
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
        [ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> DeleteTeam(int teamId)
        {
            _logger.LogDebug($"DeleteTeam {teamId} started");

            try
            {
				bool isExist = await _teamService.TeamExists(teamId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				if (!isExist)
				{
					_logger.LogError($"404. Team {teamId} couldn't found");
					return NotFound();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, ex.Message);
			}

			try
            {
                var teamToDelete = await _teamService.GetTeam(teamId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				};

                DateTime updateDate = DateTime.Now;
                int updateUserId = Enum.TEMP_USER_ID;
                bool isDeleted = await _teamService.DeleteTeam(teamToDelete, Enum.Status.NotActive, updateDate, updateUserId);

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				if (!isDeleted)
                {
                    ModelState.AddModelError("", "Something went wrong while updating team");
					
					_logger.LogError($"500. {ModelState}");
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

				if (!ModelState.IsValid)
				{
					_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
					return BadRequest(ModelState);
				}

				var teamTasksToDelete = teamTasks.Where(tt => tt.TeamId == teamId);

				foreach (var teamTask in teamTasksToDelete)
				{
					await _taskToTeamService.DeleteTeamTask(Enum.TaskStatus.Fail, teamTask);

					if (!ModelState.IsValid)
					{
						_logger.LogError($"400. Bad Request. {ModelState.ToString()}");
						return BadRequest(ModelState);
					}
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
