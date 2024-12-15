using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WCountLib.Abstractions.Counters;

namespace WCountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharCountController : ControllerBase
    {
        private readonly ICharCounter _charCounter;

        public CharCountController(ICharCounter charCounter)
        {
            _charCounter = charCounter;
        }

        [Route("/string")]
        [HttpPost]
        public int PostString([FromBody] string s)
        {
            return _charCounter.CountCharacters(s, Encoding.Default);
        }

        [Route("/strings")]
        [HttpPost]
        public async Task<ulong> PostStrings([FromBody] IEnumerable<string> strings)
        {
            return await _charCounter.CountCharactersAsync(strings, Encoding.Default);
        }

        //[Route("/file")]
        //[HttpPost]
        //public async Task<ulong> PostInFile( string file)
        //{
            
        //}
    }
}
