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
    public class ZoneController : Nx595eBaseController
    {
        //public ZoneController(AppSettings appSettings) :
        //    base(appSettings)
        public ZoneController(IOptions<AppSettings> optionsAccessor) :
            base(optionsAccessor)
        { }

        // POST: /Zone/Bypass/zoneIndex
        [HttpPost]
        [Route("Bypass/{zoneIndex:int:min(0)}")]
        public async Task<ActionResult> Bypass(int zoneIndex)
        {
            return await Zonefunction(zoneIndex.ToString());
        }

        /// <summary>
        /// Zone Bypass (toggle enabled or disabled).
        /// </summary>
        /// <param name="data0">Zone number starting at index zero (0)</param>
        /// <returns>Status of the system</returns>
        private async Task<ActionResult> Zonefunction(string data0)
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
                        {"comm", "82"},
                        {"data0", data0}
                    }
                );

                var response = await client.PostAsync("/user/zonefunction.cgi", httpContent);
                response.EnsureSuccessStatusCode();

                return await JsonStatusResult(client, sessionID);
            }
        }
    }
}
