using System.Collections.Generic;
using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Abstractions.Counters.Segments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Primitives;

namespace WCountApi.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class CountWordsController : ControllerBase
    {
        private readonly ISegmentWordCounter _wordCounter;

        public CountWordsController(ISegmentWordCounter wordCounter)
        {
           _wordCounter = wordCounter;
        }

        [HttpPost]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length == 0)
                return BadRequest("Text string was empty or null.");

            IEnumerable<StringSegment> segments =  new StringTokenizer(text, [' ']);
            
            int result = _wordCounter.CountWords(segments);

            return Ok(result);
        }
        
    }