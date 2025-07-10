using System.Text;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WCountApi.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class CharCountController : ControllerBase
    {
        private readonly ICharacterCounter _charCounter;

        public CharCountController(ICharacterCounter charCounter)
        {
            _charCounter = charCounter;
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CountAsync([FromBody] string text)
        {
            if(string.IsNullOrEmpty(text))
                return BadRequest();

            int result = await _charCounter.CountCharactersAsync(text, Encoding.Default);

            return Ok(result);
        }
    }
