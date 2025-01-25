using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using WCountLib.Counters.Abstractions;

namespace WCountAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ByteCountController : ControllerBase
    {
        private readonly IByteCounter _byteCounter;

        public ByteCountController(IByteCounter byteCounter)
        {
            _byteCounter = byteCounter;
        }

        [Route("/string")]
        [HttpPost]
        public int PostString([FromBody]  string s)
        {
            return _byteCounter.CountBytes(s, System.Text.Encoding.Default);
        }

        [Route("/strings")]
        [HttpPost]
        public async Task<ulong> PostStrings([FromBody] IEnumerable<string> strings)
        {
            return await _byteCounter.CountBytesAsync(strings, System.Text.Encoding.Default);
        }
    }
}
