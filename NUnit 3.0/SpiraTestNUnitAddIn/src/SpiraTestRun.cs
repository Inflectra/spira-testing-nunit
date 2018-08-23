using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Inflectra.SpiraTest.AddOns.NUnit
{
    /// <summary>
    /// A test run in Spira
    /// </summary>
    class SpiraTestRun
    {

        #region Variables
        [JsonProperty("TestRunFormatId")]
        public int TestRunFormatId { get; set;}

        [JsonProperty("StartDate")]
        public string StartDate { get; set; }

        [JsonProperty("RunnerName")]
        public string RunnerName { get; set; }

        [JsonProperty("RunnerTestName")]
        public string RunnerTestName { get; set; }

        [JsonProperty("RunnerMessage")]
        public string RunnerMessage { get; set; }

        [JsonProperty("RunnerStackTrace")]
        public string RunnerStackTrace { get; set; }

        [JsonProperty("TestCaseId")]
        public int TestCaseId { get; set; }

        [JsonProperty("ExecutionStatusId")]
        public int ExecutionStatusId { get; set; }

        // Properties below are optional

        [JsonProperty("ReleaseId")]
        public int ReleaseId { get; set; }

        [JsonProperty("TestSetId")]
        public int TestSetId { get; set; }

        #endregion 

        public SpiraTestRun()
        {

        }

        /// <summary>
        /// Post this test run with this objects properties to Spira
        /// </summary>
        public void PostTestRun(string url, string username, string token, int projectId)
        {
            //create the url and body
            url = url + "/Services/v5_0/RestService.svc/projects/" + projectId + "/test-runs/record?username=" + username + "&api-key=" + token;
            string json = JsonConvert.SerializeObject(this);

            //post the new test run to the server
            httpPOST(url, json);
        }

        /// <summary>
        /// Perform an HTTP POST request on the given url
        /// </summary>
        private static string httpPOST(string url, string body)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //add neccessary headers
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.UserAgent = "Spira NUnit 3.x";

            request.Method = "POST";
            //send the data to the server
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(body);
            }

            //read the server response
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string html = "";
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    html += reader.ReadToEnd();
                }
            }

            return html;
        }


    }
}
