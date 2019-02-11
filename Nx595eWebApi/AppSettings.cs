using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nx595eWebApi
{
    public class AppSettings
    {
        public class Nx595eSettings
        {
            public string Host { get; set; }

            public string LoginName { get; set; }

            public string Password { get; set; }
        }

        public Nx595eSettings Nx595e { get; set; }
    }
}
