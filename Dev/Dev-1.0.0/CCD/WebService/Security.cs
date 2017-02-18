using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService
{
    public static class Security
    {
        public static bool Authenticate(string key, string privateKey)
        {
            //Fake Security
            return true;
        }

        public static bool Authorize(string key, string privateKey)
        {
            //Fake Security
            return true;
        }
    }
}