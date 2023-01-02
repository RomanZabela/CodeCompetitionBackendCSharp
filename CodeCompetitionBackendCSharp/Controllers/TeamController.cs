using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
using CodeCompetitionBackendCSharp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public TeamController(ITeamService teamService, IMapper mapper)
        {
            _mapper = mapper;
            _teamService = teamService; 
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
            if (!_teamService.IsTeamExists(teamId))
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
    }
}
