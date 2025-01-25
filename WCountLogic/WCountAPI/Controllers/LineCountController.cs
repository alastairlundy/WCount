using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using WCountLib.Counters.Abstractions;

namespace WCountAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LineCountController : ControllerBase
{
    private readonly ILineCounter _lineCounter;

    public LineCountController(ILineCounter lineCounter)
    {
       _lineCounter = lineCounter;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    [Route("/string")]
    [HttpPost]
    public int CountLines([FromBody] string s)
    {
        return _lineCounter.CountLines(s);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strings"></param>
    /// <returns></returns>
    [Route("/strings")]
    [HttpPost]
    public int CountLines([FromBody] IEnumerable<string> strings)
    {
        return _lineCounter.CountLines(strings);
    }
}