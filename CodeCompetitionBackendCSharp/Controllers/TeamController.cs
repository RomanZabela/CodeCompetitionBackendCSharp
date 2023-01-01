using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Interface;
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
    }
}
