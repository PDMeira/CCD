using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Amazon;
using Amazon.S3.Model;
using AwsInterface;


namespace CcdDeDupAsService
{
    public partial class ccd : System.Web.UI.Page
    {
        private const string AwsAccessKey = "AKIAID7PMFMHFT67WIQA";
        private const string AwsSecretKey = "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";
        private const string CcdBucket = "CCDHolding";
        protected void Page_Load(object sender, EventArgs e)
        {

            string mergeId = Request.QueryString["mergeId"];

            var objList = new List<string>();
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretKey, RegionEndpoint.USEast1))
            {
                var req = new ListObjectsRequest();
                req.BucketName = CcdBucket;
                req.Prefix = mergeId.ToString();

                using (var res = client.ListObjects(req))
                {
                    objList = res.S3Objects.Select(x => x.Key).ToList();
                }

                if (!objList.Contains(mergeId + "/MergeSet.zip"))
                    throw new NoFileException();

                var getReq = new GetObjectRequest();
                getReq.BucketName = CcdBucket;
                getReq.Key = mergeId + "/MergeSet.zip";

                using (var getRes = client.GetObject(getReq))
                {
                    var retVal = getRes.ResponseStream;
                    var context = HttpContext.Current;

                    context.Response.ContentType = "application/zip";
                    context.Response.AppendHeader("Content-Disposition",
                                                  "attachment;filename=" + mergeId + "_" + DateTime.Now + ".zip");
                    retVal.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
        }
    }
}