/*
    WCountLib
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using Microsoft.AspNetCore.RateLimiting;

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

        IEnumerable<StringSegment> segments = new StringTokenizer(text, [' ']);

        int result = _wordCounter.CountWords(segments);

        return Ok(result);
    }
}