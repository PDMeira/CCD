using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace AwsInterface
{
    public class S3Interface
    {
        private const string AwsAccessKey = "AKIAID7PMFMHFT67WIQA";
        private const string AwsSecretKey = "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";
        private const string CcdBucket = "CCDHolding";

        public static string AddCcdRecord(Guid mergeId, string ccd, string clientKey)
        {
            var s3Key = Guid.NewGuid();
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretKey, RegionEndpoint.USEast1))
            {
                var req = new PutObjectRequest
                              {
                                  BucketName = CcdBucket,
                                  Key = mergeId + "/" + s3Key,
                                  ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                                  ContentBody = ccd,
                                  ContentType = "text/xml"
                              };
                req.WithMetaData("cit-client-id", clientKey);
                req.WithMetaData("cit-merge-id", mergeId.ToString());
                req.WithMetaData("cit-merge-date", DateTime.Now.ToShortDateString());
                req.WithMetaData("cit-data-type", "CCD");

                var res = client.PutObject(req);

            }

            return s3Key.ToString();
        }

        public static void AddMergedZip(Guid mergeId, Stream zipFile)
        {
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretKey, RegionEndpoint.USEast1))
            {
                var req = new PutObjectRequest();
                req.BucketName = CcdBucket;
                req.Key = mergeId + "/MergeSet.zip";
                req.ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256;
                req.InputStream = zipFile;
                req.ContentType = "application/zip";

                client.PutObject(req);
            }
        }

        public static string AddRuleSet(Guid mergeId, string ruleSet, string clientKey, bool useDefault = false)
        {
            var s3Key = useDefault ? "/DefaultRules" : "/RuleSet" + Guid.NewGuid().ToString();
            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretKey, RegionEndpoint.USEast1))
            {
                var req = new PutObjectRequest
                              {
                                  BucketName = CcdBucket,
                                  Key = mergeId + s3Key,
                                  ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                                  ContentBody = ruleSet,
                                  ContentType = "application/json"
                              };
                req.WithMetaData("cit-client-id", clientKey);
                req.WithMetaData("cit-merge-id", mergeId.ToString());
                req.WithMetaData("cit-merge-date", DateTime.Now.ToShortDateString());
                req.WithMetaData("cit-data-type", "RuleSet");


                var res = client.PutObject(req);

            }

            return s3Key;
        }

        public static string GetMergeStatus(Guid mergeId)
        {
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
            }

            if (objList.Contains(mergeId + "/MergeSet.zip"))
                return "Complete";

            if (objList.Count(x => x.Contains(mergeId + "/MergeSet")) > 0)
                return "Merge In Process";

            return "Merge Waiting";
        }

        public static Dictionary<string,string> GetCompletedMerges()
        {
            //var objList = new List<string>();

            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretKey, RegionEndpoint.USEast1))
            {
                var req = new ListObjectsRequest();
                req.BucketName = CcdBucket;

                using (var res = client.ListObjects(req))
                {
                    var objList = res.S3Objects;
                    var completedMerges = objList.Where(x => x.Key.Contains("MergeSet")).ToDictionary(x => x.LastModified, x => x.Key.Substring(0, x.Key.IndexOf('/')));

                    return completedMerges;
                }
            }

        }

        public static Stream GetMergeSet(Guid mergeId, string key)
        {
            var objList = new List<string>();
            Stream retVal;
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
                    //if (getRes.Metadata["cit-client-id"] != key)
                    //    throw new UnauthorizedAccessException();

                    retVal = getRes.ResponseStream;
                }


            }

            return retVal;
        }

        public static List<string> GetReadyToMerge()
        {
            var objList = new List<string>();

            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretKey, RegionEndpoint.USEast1))
            {
                var req = new ListObjectsRequest();
                req.BucketName = CcdBucket;

                using (var res = client.ListObjects(req))
                {
                    objList = res.S3Objects.Select(x => x.Key).ToList();
                }
            }


            var completedMerges = objList.Where(x => x.Contains("MergeSet")).Select(x => x.Substring(0, x.IndexOf('/'))).Distinct();
            var mergeKeyList = objList.Where(x => x.Contains("RuleSet") || x.Contains("DefaultRules"))
                                      .Select(x => x.Substring(0, x.IndexOf('/')))
                //                      .Where(x => !completedMerges.Contains(x))
                .Distinct();
            
            return mergeKeyList.ToList();

        }

        public static List<string> GetMergeSetCcds(Guid mergeId)
        {
            var objList = new List<string>();
            var retVal = new List<string>();

            using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretKey, RegionEndpoint.USEast1))
            {
                var req = new ListObjectsRequest { BucketName = CcdBucket, Prefix = mergeId.ToString() };

                using (var res = client.ListObjects(req))
                {
                    objList = res.S3Objects.Select(x => x.Key).ToList();
                }

                foreach (var k in objList.Where(x => !x.Contains("RuleSet") && !x.Contains("DefaultRules")))
                {
                    var getReq = new GetObjectRequest { BucketName = CcdBucket, Key = k };

                    using (var res = client.GetObject(getReq))
                    {
                        var sr = new StreamReader(res.ResponseStream);
                        retVal.Add(sr.ReadToEnd());
                    }
                }

            }

            return retVal;
        }

        public static string GetMergeSetRules(Guid mergeId)
        {
            var objList = new List<string>();

            using (
                var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretKey,
                                                                          RegionEndpoint.USEast1))
            {
                var req = new ListObjectsRequest {BucketName = CcdBucket, Prefix = mergeId.ToString()};

                using (var res = client.ListObjects(req))
                {
                    objList = res.S3Objects.Select(x => x.Key).ToList();
                }

                foreach (var k in objList.Where(x => x.Contains("RuleSet") || x.Contains("DefaultRules")))
                {
                    var getReq = new GetObjectRequest {BucketName = CcdBucket, Key = k};

                    using (var res = client.GetObject(getReq))
                    {
                        var sr = new StreamReader(res.ResponseStream);
                        return sr.ReadToEnd();
                    }
                }
            }
            return string.Empty;
        }

    }
}
