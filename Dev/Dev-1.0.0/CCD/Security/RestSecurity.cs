using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    public static class RestSecurity
    {
        public static bool Authenticate (string key, string privateKey)
        {
            //Fake Security
            return true;
        }

        public static bool Authorize (string key, string privateKey)
        {
            //Fake Security
            return true;
        }
    }
}
