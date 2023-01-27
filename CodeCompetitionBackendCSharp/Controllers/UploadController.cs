using CodeCompetitionBackendCSharp.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace CodeCompetitionBackendCSharp.Controllers
{
    [Route("api/upload")]
    [ApiController]
    [Authorize]
    public class UploadController : ControllerBase
    {
		private readonly ILoggerManager _logger;

		public UploadController(ILoggerManager logger)
		{
			_logger = logger;
		}

		[HttpPost]
		[Authorize(Roles = "system administrator, competition administrator, participant")]

		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(500)]
		public IActionResult Upload()
        {
            _logger.LogDebug($"Upload file started.");

            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("StaticFiles", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');

                    if (fileName == null) 
                    {
						_logger.LogError($"400. Bad Request. {ModelState}");
						return BadRequest(ModelState);
					}

                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

					_logger.LogInfo($"200. File added {dbPath}");
					return Ok(dbPath);
                }
                else
                {
					_logger.LogError($"400. Bad Request. {ModelState}");
					return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
				_logger.LogError($"500. {ex.Message}");
				return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
