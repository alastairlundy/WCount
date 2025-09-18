using AlastairLundy.WCountLib.Abstractions.Counters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Primitives;

namespace WCountApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountWordsController : ControllerBase
    {
        private readonly IWordCounter _wordCounter;

        public CountWordsController(IWordCounter wordCounter)
        {
           _wordCounter = wordCounter;
        }

        [HttpPost]
        [EnableRateLimiting("sliding-free")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length == 0)
                return BadRequest("Text string was empty or null.");
            
            int result = _wordCounter.CountWords(text);

            return Ok(result);
        }
        
    }
}
