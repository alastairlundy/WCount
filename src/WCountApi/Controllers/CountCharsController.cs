using Microsoft.AspNetCore.Mvc;
using AlastairLundy.WCountLib.Abstractions.Counters;
using Microsoft.AspNetCore.RateLimiting;


namespace WCountApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountCharsControllers : ControllerBase
{
    private readonly ICharacterCounter _characterCounter;

    public CountCharsControllers(ICharacterCounter characterCounter)
    {
        _characterCounter = characterCounter;
    }


        [HttpPost]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length == 0)
                return BadRequest("Text string was empty or null.");
            
            int result = _characterCounter.CountCharacters(text, Encoding.Default);

            return Ok(result);
        }
}