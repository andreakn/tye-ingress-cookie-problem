using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieServer.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {

        [HttpGet]
        public ActionResult Get()
        {
            return Redirect("/cookies");
        }
    }
}
