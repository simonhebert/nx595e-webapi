using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nx595eWebApi.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class OutputController : Nx595eBaseController
    {
        public OutputController(IOptions<AppSettings> optionsAccessor) :
            base(optionsAccessor)
        { }

        // POST: /Output/On/outputNumber
        [HttpPost]
        [Route("On/{outputNumber:int:min(1)}")]
        public async Task<ActionResult> On(int outputNumber)
        {
            return await Output(outputNumber.ToString(), "1");
        }

        // POST: /Output/Off/outputNumber
        [HttpPost]
        [Route("Off/{outputNumber:int:min(1)}")]
        public async Task<ActionResult> Off(int outputNumber)
        {
            return await Output(outputNumber.ToString(), "0");
        }

        /// <summary>
        /// Output On/Off.
        /// </summary>
        /// <param name="onum">Output number starting at index one (1)</param>
        /// <param name="ostate">Output state: 0=Off / 1=On</param>
        /// <returns>Status of the system</returns>
        private async Task<ActionResult> Output(string onum, string ostate)
        {
            using (var client = new HttpClient())
            {
                var settings = AppSettings.Nx595e;
                client.BaseAddress = new Uri(settings.Host);

                var sessionID = await GetSessionID(client);
                var httpContent = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        {"sess", sessionID},
                        {"onum", onum},
                        {"ostate", ostate}
                    }
                );

                var response = await client.PostAsync("/user/output.cgi", httpContent);
                response.EnsureSuccessStatusCode();

                return await JsonStatusResult(client, sessionID);
            }
        }
    }
}
