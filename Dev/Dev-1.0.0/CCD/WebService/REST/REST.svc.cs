// CCD Consolidation and De-duplication Engine
// Copyright (C) 2013  CreateIT Healthcare Solutions, Inc.

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see {http://www.gnu.org/licenses/}.

// Contact Info:
//     Jonathan Meade
//     CreateIT Healthcare Solutions, Inc.
//     516 SW 15th Street
//     Richmond, IN 47374
//     jmeade@createit-inc.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using Data;

namespace WebService.REST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "REST" in code, svc and config file together.
    public class Rest : IRest
    {
 
        public string NewMergeRequest(string ruleSet)
        {
            if (WebService.Security.Authenticate())
            return Guid.NewGuid().ToString();

        }

        public void AddCcd(string mergeId, Stream ccd)
        {
            //Get from stream??
            var ccdXml = XDocument.Parse("<empty />");
            var ccdStore = CcdStorage.ObjCcdStorage;
            ccdStore.MergeJobs.Add(new MergeJob(){MergeId = Guid.Parse(mergeId), Ccd = ccdXml});
        }

        public string GetMergedCcd(string mergeId)
        {
            return "Test";
        }
    }
}
