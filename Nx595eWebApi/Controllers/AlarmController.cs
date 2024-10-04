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
    public class AlarmController : Nx595eBaseController
    {
        //public AlarmController(AppSettings appSettings) :
        //    base(appSettings)
        public AlarmController(IOptions<AppSettings> optionsAccessor) :
            base(optionsAccessor)
        { }

        // GET: /Alarm/Status
        [HttpGet]
        [Route("Status")]
        public async Task<ActionResult> Status()
        {
            using (var client = new HttpClient())
            {
                var settings = AppSettings.Nx595e;
                client.BaseAddress = new Uri(settings.Host);

                var sessionID = await GetSessionID(client);

                return await JsonStatusResult(client, sessionID);
            }
        }

        // POST: /Alarm/Arm/armType
        [HttpPost]
        [Route("Arm/{armType:regex((off|away|stay))}")]
        public async Task<ActionResult> Arm(string armType)
        {
            var armTypeCodeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"off", "16"},
                {"away", "17"},
                {"stay", "18"}
            };

            return await Keyfunction(armTypeCodeMap[armType]);
        }

        // POST: /Alarm/Chime
        [HttpPost]
        [Route("Chime")]
        public async Task<ActionResult> Chime()
        {
            return await Keyfunction("1");
        }

        private async Task<ActionResult> Keyfunction(string data2)
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
                        {"comm", "80"}, // Always 80
                        {"data0", "2"}, // Always 2
                        {"data1", "1"}, // Area Number (Mask) - Always 1 for now.
                        {"data2", data2} // Command ID (Button)
                    }
                );

                var response = await client.PostAsync("/user/keyfunction.cgi", httpContent);
                response.EnsureSuccessStatusCode();

                return await JsonStatusResult(client, sessionID, response);
            }
        }
    }
}
