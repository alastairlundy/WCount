using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;


namespace WCountApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountCharsControllers : ControllerBase
{
    private readonly ISegmentCharacterCounter _characterCounter;

    public CountCharsControllers(ISegmentCharacterCounter characterCounter)
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

        IEnumerable<StringSegment> segments = new StringTokenizer(text, [' ']);

        int result = _characterCounter.CountCharacters(segments, Encoding.Default);

        return Ok(result);
    }
}