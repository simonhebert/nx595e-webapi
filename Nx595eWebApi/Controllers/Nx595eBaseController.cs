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
            // Status

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

            stat0 "Armed Away",
            stat1 "Armed Stay",
            stat2 "Ready",
            stat3 "Fire Alarm",
            stat4 "Burg Alarm",
            stat5 "Panic Alarm",
            stat6 "Medical Alarm",
            stat7 "Exit Delay",
            stat8 "Exit Delay 2",
            stat9 "Entry Delay",
            stat10 "Zone Bypass",
            stat11 "Zone Trouble",
            stat12 "Zone Tamper",
            stat13 "Zone Low Battery",
            stat14 "Zone Supervision",
            stat15 "Chime Enabled"
            */

            HttpResponseMessage statusResponse;

            if (responseMessage == null)
            {
                var statusHttpContent = new FormUrlEncodedContent(
                        new Dictionary<string, string>
                        {
                            {"sess", sessionID},
                            {"arsel", "0"} // Area index 0 (first)
                        }
                    );

                statusResponse = await client.PostAsync("/user/status.xml", statusHttpContent);
                statusResponse.EnsureSuccessStatusCode();
            }
            else
                statusResponse = responseMessage;

            var alarmStatus = new Status();

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

            // Zones

            /*
            POST /user/zstate.xml
            sess=7B37A90F60250A55&state=0
            <response>
                <zstate>0</zstate>
                <zseq>1</zseq>
                <zdat>16,0,0,0</zdat>
            </response>

            POST /user/zones.htm
            var zoneDisplay = new Array(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0);
            var zoneNames = new Array("1%2Dxxx","2%2Dxxx","3%2Dxxx","4%2Dxxx","5%2Dxxx","6%2Dxxx","7%2Dxxx","8%2Dxxx","","","11%2Dxxx","12%2Dxxx","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21","%21");
            var zoneSequence = new Array(105,0,0,54,0,23,0,0,1,0,0,8,0,0);
            var zoneStatus = new Array(new Array(0,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0),new Array(49152,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0),new Array(0,0,0,0));
            */

            var zoneHttpContent = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        {"sess", sessionID}
                    }
                );

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
                        .Where(z => /*z != string.Empty &&*/ z != "!")
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
                                zoneString = zoneStates[zoneDisplay[i]];
                            else if (zoneDisplay[i] == 0)
                                zoneString = "Ready";

                            zoneDisplay[i]++;
                        }

                        alarmStatus.Zones[i] = new Zone
                        {
                            Index = i,
                            Number = i + 1,
                            Name = zoneNames[i],
                            Status = zoneString,
                            IsBypassed = (zoneStatus[3][byteindex] & mask) != 0
                        };
                    }
                }
            }

            // Outputs

            // POST : /user/outstat.xml
            // Request: Sess=
            // Response (XML): 
            /*
                <response>
                    <s1>1</s1>
                    <s2>0</s2>
                </response>
            */
            // Optional: /user/outputs.htm

            var outputHttpContent = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        {"sess", sessionID}
                    }
                );

            HttpResponseMessage outputResponse = await client.PostAsync("/user/outstat.xml", outputHttpContent);
            outputResponse.EnsureSuccessStatusCode();

            using (var responseStream = await outputResponse.Content.ReadAsStreamAsync())
            {
                var response = XDocument.Load(responseStream).Element("response");

                alarmStatus.Outputs = new List<Output>();

                foreach (var element in response.Elements())
                {
                    alarmStatus.Outputs.Add(new Output() { Number = int.Parse(element.Name.LocalName.Substring(1)), Name = element.Name.LocalName, IsStateOn = int.Parse(element.Value) != 0 });
                }
            }

            return new JsonResult(alarmStatus);
        }
    }
}
