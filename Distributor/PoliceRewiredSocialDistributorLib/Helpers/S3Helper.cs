using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace PoliceRewiredSocialDistributorLib.Helpers
{
    public class S3Helper : IDisposable
    {
        Action<string> log;
        AmazonS3Client s3;
        string bucket;

        public S3Helper(string bucket, Action<string> log, RegionEndpoint region = null)
        {
            this.s3 = new AmazonS3Client(region ?? RegionEndpoint.EUWest2);
            this.bucket = bucket;
            this.log = log;
        }

            public void Dispose()
        {
            s3.Dispose();
            s3 = null;
        }

        private void Log(string msg)
        {
            log(msg);
        }

        public async Task<Data> ReadObjectFromJsonAsync<Data>(string key)
        {
            Log(string.Format("Get: {0}:{1}", bucket, key));
            var request = new GetObjectRequest
            {
                BucketName = bucket,
                Key = key,
            };

            try
            {
                var response = await s3.GetObjectAsync(request);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.ResponseStream))
                    {
                        var json = await reader.ReadToEndAsync();
                        Log(string.Format("Json: {0}", json));
                        return JsonConvert.DeserializeObject<Data>(json);
                    }
                }
                else
                {
                    Log("Unable to get object. Response code: " + response.HttpStatusCode.ToString());
                    return default(Data);
                }
            }
            catch (Exception e)
            {
                Log(string.Format("Cannot retrieve object. {0}: {1}", e.GetType().Name, e.Message));
                return default(Data);
            }
        }

        public async Task<bool> WriteObjectToJsonAsync<Data>(string key, Data data)
        {
            Log(string.Format("Put: {0}:{1}", bucket, key));
            var json = JsonConvert.SerializeObject(data);
            Log(string.Format("Json: {0}", json));

            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                ContentBody = json
            };
            var response = await s3.PutObjectAsync(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                Log("Unable to put object. Response code: " + response.HttpStatusCode.ToString());
                return false;
            }
        }

    }
}
