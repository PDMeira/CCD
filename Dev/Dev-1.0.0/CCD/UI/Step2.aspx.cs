using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UI
{
    public partial class Step2 : StepBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override int PageNum()
        {
            return 2;
        }
    }
}