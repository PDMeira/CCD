using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI
{
    public abstract class StepBase : System.Web.UI.Page
    {
        public abstract int PageNum();
        public int MaxPage = 5;
        
    }
}