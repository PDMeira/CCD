using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICcdDeDup" in both code and config file together.
    [ServiceContract]
    public interface ICcdDeDup
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "Test1")]
        string TestString();

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "CreateNewMergeRequest?key={key}&secretKey={secretKey}")]
        string CreateNewMergeRequest(string key, string secretKey);

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "AddCcd/{mergeId}?key={key}&secretKey={secretKey}")]
        string AddCcd(string key, string secretKey, string mergeId, Stream data);

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "MergeWithRuleSet/{mergeId}?key={key}&secretKey={secretKey}")]
        string MergeWithRuleSet(string key, string secretKey, string mergeId, Stream data);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "MergeWithDefaultRules/{mergeId}?key={key}&secretKey={secretKey}")]
        string MergeWithDefaultRules(string key, string secretKey, string mergeId);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "MergeStatus/{mergeId}?key={key}&secretKey={secretKey}")]
        string MergeStatus(string key, string secretKey, string mergeId);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "GetMergedCcd/{mergeId}?key={key}&secretKey={secretKey}")]
        Stream GetMergedCcd(string key, string secretKey, string mergeId);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "FireMergeEngine")]
        string FireMergeEngine();
    }
}
