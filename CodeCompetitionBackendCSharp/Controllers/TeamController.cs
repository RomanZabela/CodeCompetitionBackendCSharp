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

        public TeamController(
            ITeamService teamService,
            ITaskToTeamService taskToTeamService,
            IMapper mapper)
        {
            _mapper = mapper;
            _teamService = teamService;
            _taskToTeamService = taskToTeamService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Team>))]
        public async Task<IActionResult> GetTeams([FromQuery] TeamParameters teamParameters)
        {
            try
            {
                var teams = await _teamService.GetTeams(teamParameters);
                var teamsDto = _mapper.Map<ICollection<TeamDto>>(teams);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(teams.MetaData));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(teamsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("extended")]
        [ProducesResponseType(200, Type = typeof(PagedList<TeamExtendedDto>))]
        public async Task<IActionResult> GetTeamsExtended([FromQuery] TeamParameters teamParameters)
        {
            try
            {
                var teams = await _teamService.GetTeamsExtended(teamParameters);
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(teams.MetaData));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(teams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{teamId}")]
        [ProducesResponseType(200, Type = typeof(Models.Team))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTeam(int teamId)
        {
            try
            {
                bool isExist = await _teamService.TeamExists(teamId);
                if (!isExist)
                {
                    return NotFound();
                }

                Team team = await _teamService.GetTeam(teamId);
                TeamDto teamDto = _mapper.Map<TeamDto>(team);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(teamDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("teamByParticipant/{participantId}")]
        [ProducesResponseType(200, Type = typeof(Models.Team))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTeamByParticipantId(int participantId)
        {
            try
            {
                Team team = await _teamService.GetTeamByParticipantId(participantId);
                TeamDto teamDto = _mapper.Map<TeamDto>(team);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(teamDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }   
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateTeam([FromBody] TeamDto teamCreate)
        {
            if (teamCreate == null)
                return BadRequest(ModelState);

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
                return StatusCode(500, ex.Message);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teamMap = _mapper.Map<Team>(teamCreate);

            teamMap.UpdateDate = DateTime.Now;
            teamMap.CreateDate = DateTime.Now;
            teamMap.CreateUserId = Enum.TEMP_USER_ID;
            teamMap.UpdateUserId = Enum.TEMP_USER_ID;
            teamMap.StatusId = (int)Enum.Status.Active;

            try 
            {
                await _teamService.CreateTeam(teamMap);
                return Ok("Successfully created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{teamId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] TeamDto updatedTeam)
        {
            if (updatedTeam == null)
                return BadRequest(ModelState);

            if (teamId != updatedTeam.Id)
            {
                return BadRequest(ModelState);
            }
            try
            {
                bool isExist = await _teamService.TeamExists(teamId);
                if (!isExist)
                {
                    return NotFound();
                }
            }
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}

			if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teamMap = _mapper.Map<Team>(updatedTeam);

            teamMap.UpdateDate = DateTime.Now;
            teamMap.UpdateUserId = Enum.TEMP_USER_ID;

            try
            {
                bool isUpdated = await _teamService.UpdateTeam(teamMap);
                if (!isUpdated)
                {
                    ModelState.AddModelError("", "Something went wrong while updating team");
                    return StatusCode(500, ModelState);
                }
                return NoContent(); ;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }

        [HttpDelete("{teamId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                bool isExist = await _teamService.TeamExists(teamId);
                if (!isExist)
                {
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
                    return StatusCode(500, ModelState);
                }
            }
            catch (Exception ex)
            {
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

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
