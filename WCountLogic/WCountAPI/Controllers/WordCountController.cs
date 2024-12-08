using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WCountAPI.Logic;

using WCountLib.Abstractions.Counters;

namespace WCountAPI.Controllers
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [Route("/string")]
        [HttpPost]
        public async Task<ulong> CountWordsAsync([FromBody] string s)
        {
            return await _wordCounter.CountWordsAsync(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        [Route("/strings")]
        [HttpPost]
        public async Task<ulong> CountWordsAsync([FromBody] IEnumerable<string> strings)
        {
            return await _wordCounter.CountWordsAsync(strings);
        }

        
        //[Route("/file")]
        //[HttpPost]
        //public async Task<ulong> UploadFile(IFormFile file)
        //{
        //    string fileName = string.Empty;

        //    bool wasUploadSuccessful;
        //    try
        //    {
        //        fileName = UploadHandler.UploadFile(file);
        //        wasUploadSuccessful = true;

        //        Response.StatusCode = 200;
        //    }
        //    catch
        //    {
        //        wasUploadSuccessful = false;

        //        Response.StatusCode = 400;
        //    }

        //    ulong result = 0;

        //    if(wasUploadSuccessful == true)
        //    {
        //        result = await _wordCounter.CountWordsInFileAsync(fileName);
        //    }
        //    else
        //    {
        //        result = 0;
        //    }

        //    return result; 
        // }

    }
}
