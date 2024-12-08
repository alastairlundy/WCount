using System.Collections.Generic;
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
        public ulong PostString([FromBody] string s)
        {
            return _charCounter.CountCharacters(s);
        }

        [Route("/strings")]
        [HttpPost]
        public async Task<ulong> PostStrings([FromBody] IEnumerable<string> strings)
        {
            return await _charCounter.CountCharactersAsync(strings);
        }

        //[Route("/file")]
        //[HttpPost]
        //public async Task<ulong> PostInFile( string file)
        //{
            
        //}
    }
}
