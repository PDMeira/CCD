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
using System.ServiceModel.Web;
using System.Text;

namespace WebService.REST
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IREST" in both code and config file together.
    [ServiceContract]
    public interface IRest
    {
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json, UriTemplate = "NewMergeRequest/{ruleSet}")]
        string NewMergeRequest(string ruleSet);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml,
            RequestFormat = WebMessageFormat.Xml, UriTemplate = "AddCcd/{mergeId}")]
        void AddCcd(string mergeId, Stream ccd);

        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json, UriTemplate = "GetMergedCcd/{mergeId}")]
        string GetMergedCcd(string mergeId);

    }
}
