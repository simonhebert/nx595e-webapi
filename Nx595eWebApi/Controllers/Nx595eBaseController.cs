using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nx595eWebApi.Extensions;
using Nx595eWebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nx595eWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Nx595eBaseController : ControllerBase
    {
        protected AppSettings AppSettings { get; }

        //public Nx595eBaseController(AppSettings appSettings)
        public Nx595eBaseController(IOptions<AppSettings> optionsAccessor)
        {
            AppSettings = optionsAccessor.Value;
            //AppSettings = appSettings;
        }

        protected async Task<string> GetSessionID(HttpClient client)
        {
            var settings = AppSettings.Nx595e;
            var httpContent = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    {"lgname", settings.LoginName},
                    {"lgpin", settings.Password}
                }
            );

            var response = await client.PostAsync("/login.cgi", httpContent);
            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(responseStream))
                {
                    reader.SkipLines(28);
                    var sessionline = reader.ReadLine();
                    return sessionline.Split('"')[1];
                }
            }
        }

        protected async Task<JsonResult> JsonStatusResult(HttpClient client, string sessionID, HttpResponseMessage responseMessage = null)
        {
            HttpResponseMessage statusResponse;

            if (responseMessage == null)
            {
                var statusHttpContent = new FormUrlEncodedContent(
                        new Dictionary<string, string>
                        {
                            {"sess", sessionID},
                            {"arsel", "0"}
                        }
                    );

                statusResponse = await client.PostAsync("/user/status.xml", statusHttpContent);
                statusResponse.EnsureSuccessStatusCode();
            }
            else
                statusResponse = responseMessage;

            var alarmStatus = new Status();
            /*
            <response>
              <abank>0</abank>
              <aseq>247</aseq>
              <stat0>0</stat0>
              <stat1>0</stat1>
              <stat2>1</stat2>
              <stat3>0</stat3>
              <stat4>0</stat4>
              <stat5>0</stat5>
              <stat6>0</stat6>
              <stat7>0</stat7>
              <stat8>0</stat8>
              <stat9>0</stat9>
              <stat10>0</stat10>
              <stat11>0</stat11>
              <stat12>0</stat12>
              <stat13>0</stat13>
              <stat14>0</stat14>
              <stat15>0</stat15>
              <stat16>0</stat16>
              <sysflt>No System Faults</sysflt>
             </response>
            */

            using (var responseStream = await statusResponse.Content.ReadAsStreamAsync())
            {
                var response = XDocument.Load(responseStream).Element("response");

                // stat0 = away
                // stat1 = stay
                // stat2 = system ready
                // stat3 = fire alarm
                // stat4 intrusion alarm
                // stat7 = exit time delay
                // stat9 entry time delay
                // stat10 = zone(s) with bypass enabled
                // stat15 = chime enabled
                // sysflt = system status message
                alarmStatus.ArmType =
                    int.Parse(response.Element("stat0").Value) != 0 ? "away" :
                    int.Parse(response.Element("stat1").Value) != 0 ? "stay" :
                    "off";

                alarmStatus.IsSystemReady = int.Parse(response.Element("stat2").Value) != 0;
                alarmStatus.IsFireAlarm = int.Parse(response.Element("stat3").Value) != 0;
                alarmStatus.IsIntrusionAlarm = int.Parse(response.Element("stat4").Value) != 0;
                alarmStatus.IsExitTimeDelay = int.Parse(response.Element("stat7").Value) != 0;
                alarmStatus.IsEntryTimeDelay = int.Parse(response.Element("stat9").Value) != 0;
                alarmStatus.IsZoneBypassEnabled = int.Parse(response.Element("stat10").Value) != 0;
                alarmStatus.IsChimeEnabled = int.Parse(response.Element("stat15").Value) != 0;

                alarmStatus.SystemStatus = response.Element("sysflt").Value;
            }

            var zoneHttpContent = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        {"sess", sessionID}
                    }
                );

            /*
            POST /user/zstate.xml
            sess=7B37A90F60250A55&state=0
            <response>
                <zstate>0</zstate>
                <zseq>1</zseq>
                <zdat>16,0,0,0</zdat>
            </response>
            */
            var zoneResponse = await client.PostAsync("/user/zones.htm", zoneHttpContent);
            zoneResponse.EnsureSuccessStatusCode();

            using (var responseStream = await zoneResponse.Content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(responseStream))
                {
                    reader.SkipLines(14);

                    // 24
                    var zoneDisplay = reader.ReadLine()
                        .Split('(', ')')[1]
                        .Split(',')
                        .Select(d => int.Parse(d))
                        .ToArray();

                    reader.ReadLine();

                    // 48
                    var zoneNames = reader.ReadLine()
                        .Split('(', ')')[1]
                        .Split(',')
                        .Select(z => WebUtility.UrlDecode(z.Trim('"')))
                        .Where(z => z != string.Empty && z != "!")
                        .ToArray();

                    reader.ReadLine();

                    // 14
                    var zoneStatusLine = reader.ReadLine();
                    var zoneStatus = zoneStatusLine
                        .Replace("var zoneStatus = new Array(", string.Empty)
                        .Replace(");", string.Empty)
                        .Split(new[] { "new Array(" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => s != string.Empty)
                        .Select(s => s.Replace(")", string.Empty)
                            .Split(',')
                            .Take(4)
                            .Select(st => int.Parse(st))
                            .ToArray()
                        )
                        .ToArray();

                    var zoneStates = new[] {
                            "Not Ready",
                            "Tamper",
                            "Trouble",
                            "Bypass",
                            "Inhibited",
                            "Alarm",
                            "Low Battery",
                            "Supervision Fault"
                        };

                    alarmStatus.Zones = new Zone[zoneNames.Length];

                    // The zoneNames length not always equal to zoneDisplay or zoneStatus length in the web page.
                    // Take the shorter array available for looping to prevent "index out of range exception"...
                    for (var i = 0; i < Math.Min(zoneNames.Length, Math.Min(zoneDisplay.Length, zoneStatus.Length)); i++)
                    {
                        var byteindex = i / 16;
                        var mask = (0x01 << (i % 16));
                        var zoneString = string.Empty;

                        while (zoneString == string.Empty)
                        {
                            if (zoneDisplay[i] >= zoneStates.Length)
                                zoneDisplay[i] = 0;

                            var st = zoneStatus[zoneDisplay[i]][byteindex];

                            if ((st & mask) != 0)
                            {
                                zoneString = zoneStates[zoneDisplay[i]];
                            }
                            else if (zoneDisplay[i] == 0)
                            {
                                zoneString = "Ready";
                            }

                            zoneDisplay[i]++;
                        }

                        alarmStatus.Zones[i] = new Zone
                        {
                            Index = i,
                            Name = zoneNames[i],
                            Status = zoneString,
                            IsBypassed = (zoneStatus[3][byteindex] & mask) != 0
                        };
                    }

                    return new JsonResult(alarmStatus);
                }
            }
        }
    }
}