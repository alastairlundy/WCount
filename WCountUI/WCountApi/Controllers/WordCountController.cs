using System.IO;
using System.Threading.Tasks;
using AlastairLundy.WCountLib.Abstractions.Counters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WCountApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordCountController : ControllerBase
    {
        private readonly IWordCounter _wordCounter;

        public WordCountController(IWordCounter wordCounter)
        {
            _wordCounter = wordCounter;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CountAsync([FromBody] string text)
        {
            if(string.IsNullOrEmpty(text))
                return BadRequest();

            ulong result = await _wordCounter.CountWordsAsync(new StringReader(text));

            return Ok(result);
        }
    }
}
