using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using CodeCompetitionBackendCSharp.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetTeams()
        {
            var teams = _mapper.Map<List<TeamDto>>(_teamService.GetTeams());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(teams);
        }

        [HttpGet("{teamId}")]
        [ProducesResponseType(200, Type = typeof(Models.Team))]
        [ProducesResponseType(400)]
        public IActionResult GetTeam(int teamId)
        {
            if (!_teamService.TeamExists(teamId))
            {
                return NotFound();
            }

            var team = _mapper.Map<TeamDto>(_teamService.GetTeam(teamId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(team);
        }

        [HttpGet("teamByParticipant/{participantId}")]
        [ProducesResponseType(200, Type = typeof(Models.Team))]
        [ProducesResponseType(400)]
        public IActionResult GetTeamByParticipantId(int participantId)
        {
            /* if (!_participantService.IsParticipantExists(participantId))
            {
                return NotFound();
            } */

            var team = _mapper.Map<TeamDto>(_teamService.GetTeamByParticipantId(participantId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(team);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTeam([FromBody] TeamDto teamCreate)
        {

            if (teamCreate == null)
                return BadRequest(ModelState);

            var teams = _teamService.GetTeams()
                .Where(c => c.TeamName.Trim().ToUpper() == teamCreate.TeamName.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (teams != null)
            {
                ModelState.AddModelError("", "Review already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teamMap = _mapper.Map<Team>(teamCreate);

            teamMap.UpdateDate = DateTime.Now;
            teamMap.CreateDate = DateTime.Now;
            teamMap.CreateUserId = Enum.TEMP_USER_ID;
            teamMap.UpdateUserId = Enum.TEMP_USER_ID;
            teamMap.StatusId = (int)Enum.Status.Active;

            if (!_teamService.CreateTeam(teamMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpPut("{teamId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateTeam(int teamId, [FromBody] TeamDto updatedTeam)
        {
            if (updatedTeam == null)
                return BadRequest(ModelState);

            if (teamId != updatedTeam.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_teamService.TeamExists(teamId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teamMap = _mapper.Map<Team>(updatedTeam);

            teamMap.UpdateDate = DateTime.Now;
            teamMap.UpdateUserId = Enum.TEMP_USER_ID;
           
            if (!_teamService.UpdateTeam(teamMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating team");
                return StatusCode(500, ModelState);
            }
            return NoContent(); ;
        }

        [HttpDelete("{teamId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            if (!_teamService.TeamExists(teamId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teamToDelete = _teamService.GetTeam(teamId);

            Enum.Status newStatus = Enum.Status.NotActive;
            DateTime updateDate = DateTime.Now;
            int updateUserId = Enum.TEMP_USER_ID;

            if (!_teamService.DeleteTeam(teamToDelete, newStatus, updateDate, updateUserId))
            {
                ModelState.AddModelError("", "Something went wrong while updating team");
                return StatusCode(500, ModelState);
            }

            var teamTasks = await _taskToTeamService.GetTeamsTasks();
            var teamTasksToDelete = teamTasks.Where(tt => tt.TeamId == teamId);

            foreach(var teamTask in teamTasksToDelete) 
            {
                try
                {
                    await _taskToTeamService.DeleteTeamTask(Enum.TaskStatus.Fail, teamTask);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }

            }

            return NoContent(); 
        }
    }
}
