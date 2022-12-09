using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransJobAPI.Controllers
{
    [Route("api/SubmitAnswerAndGetNextQuestion")]
    [ApiController]
    public class SubmitAnswerAndGetNextQuestionController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {

            return Ok();
        }
    }
}
