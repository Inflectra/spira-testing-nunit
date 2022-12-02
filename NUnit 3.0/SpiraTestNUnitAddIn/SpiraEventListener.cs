using NUnit.Engine.Extensibility;
using NUnit.Engine;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using System.Collections.Generic;

namespace Inflectra.SpiraTest.AddOns.NUnit
{
    [Extension(Description = "SpiraTest Reporter Extension")]
    public class SpiraEventListener : ITestEventListener
    {

        #region Filters

        /// <summary>
        /// The XML name of a test-suite property
        /// </summary>
        private static XName testSuite = "test-suite";
        /// <summary>
        /// The XML name of a test-case property
        /// </summary>
        private static XName testCase = "test-case";
        private static XName properties = "properties";

        private static XName asserts = "asserts";

        private static XName methodName = "methodname";

        private static XName executionResult = "result";

        //Used to get the working directory of the test suite
        private static XName environment = "environment";
        private static XName workingDirectory = "cwd";

        //used to get the message and stack trace, if applicable
        private static XName failure = "failure";
        private static XName failureMessage = "message";
        private static XName stackTrace = "stack-trace";
        private static JObject configuration;

        #endregion

        public void OnTestEvent(string report)
        {
            // test-run is passed in once execution is finished
            if (report.StartsWith("<test-run"))
            {
                XElement xml = XElement.Parse(report);
                //loop through each test suite recursively
                foreach (XElement e in xml.Elements(testSuite))
                {
                    ProcessTestSuite(e, null);
                }

            }
        }

        /// <summary>
        /// Recursively process a test suite
        /// </summary>
        private void ProcessTestSuite(XElement element, JObject configuration)
        {
            if (configuration == null)
            {
                XElement e = element.Element(environment);
                //the current working directory
                string location = @e.Attribute(workingDirectory).Value;
                //get the SpiraConfig.json file
                location += @"\SpiraConfig.json";
                try
                {
                    configuration = JObject.Parse(File.ReadAllText(location));
                }
                catch(Exception exception)
                {
                    // if the SpiraConfig.json cannot be found / read, we want to try to parse this as the old format
                }
            }

            //process any nested test suites
            foreach (XElement e in element.Elements(testSuite))
            {
                ProcessTestSuite(e, configuration);
            }
            //process any test cases
            foreach (XElement e in element.Elements(testCase))
            {
                ProcessTestCase(e, configuration);
            }

        }

        /// <summary>
        /// Process a single XML test case and send it to Spira
        /// </summary>
        /// <param name="element"></param>
        private void ProcessTestCase(XElement element, JObject configuration)
        {
            int testCaseId = 0;
            //xml has a <properties> element which contains <property> elements
            foreach (XElement attribute in element.Elements(properties))
            {
                foreach (XElement property in attribute.Elements("property"))
                {
                    string propertyName = property.Attribute("name").Value;
                    //Finds the property which has a name of testcaseid (not case sensitive)
                    if (string.Equals(propertyName, "testcaseid", StringComparison.OrdinalIgnoreCase))
                    {
                        //Convert.ToInt32 throws exception if not valid
                        try
                        {
                            testCaseId = Convert.ToInt32(property.Attribute("value").Value);
                        }
                        catch
                        {
                            //leave testcaseid as 0 
                        }
                    }
                }
            }

            SpiraTestRun testRun = new SpiraTestRun();
            //name of the method
            testRun.RunnerTestName = element.Attribute(methodName).Value;
            //number of asserts
            testRun.RunnerAssertCount = int.Parse(element.Attribute(asserts).Value);

            //either Passed, Failed, Inconclusive, or Skipped
            string result = element.Attribute(executionResult).Value;
            //convert result into something Spira understands
            switch (result)
            {
                case "Passed": testRun.ExecutionStatusId = 2; break;
                case "Failed": testRun.ExecutionStatusId = 1; break;
                case "Inconclusive": testRun.ExecutionStatusId = 6; break;
                case "Skipped": testRun.ExecutionStatusId = 3; break;
                default: testRun.ExecutionStatusId = 4; break;
            }

            //optional fields
            int? releaseId = GetSpiraReleaseId(configuration);
            if (releaseId.HasValue)
            {
                testRun.ReleaseId = releaseId.Value;
            }

            int? testSetId = GetSpiraTestSetId(configuration);
            if (testSetId.HasValue)
            {
                testRun.TestSetId = testSetId.Value;
            }
            //if There was not a valid test case Id included as a test run property, check the JSON config file
            if (testCaseId <= 0)
            {
                testRun.TestCaseId = GetSpiraTestCaseId(testRun.RunnerTestName, configuration);
            }
            else
            {
                testRun.TestCaseId = testCaseId;
            }

            XElement fail = element.Element(failure);
            //if we have a message and stack trace from NUnit
            if (fail != null)
            {
                XElement message = fail.Element(failureMessage);
                if (message != null)
                {
                    testRun.RunnerMessage = message.Value;
                }
                else
                {
                    testRun.RunnerMessage = "";
                }

                XElement trace = fail.Element(stackTrace);
                if (trace != null)
                {
                    testRun.RunnerStackTrace = trace.Value;
                }
                else
                {
                    testRun.RunnerStackTrace = "";
                }
            }
            else
            {
                testRun.RunnerMessage = "Success";
                testRun.RunnerStackTrace = "";
            }

            //send the test run to Spira
            testRun.PostTestRun(SpiraUrl(configuration), SpiraUsername(configuration), SpiraToken(configuration), SpiraProjectId(configuration));
        }

        /// <summary>
        /// Get the test case ID of the given method from the configuration
        /// </summary>
        /// <returns></returns>
        private static int GetSpiraTestCaseId(string methodName, JObject configuration)
        {
            JToken testCases = configuration.GetValue("test_cases");
            int? method = testCases.Value<int?>(methodName);
            if(method.HasValue)
            {
                return method.Value;
            }
            
            //return the default if method is not specified
            return testCases.Value<int>("default");
        }

        /// <summary>
        /// Get the release ID in Spira, if specified, null otherwise
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static int? GetSpiraReleaseId(JObject configuration)
        {
            JToken credentials = configuration.GetValue("credentials");
            int? id = credentials.Value<int?>("release_id");

            return id;
        }

        /// <summary>
        /// Get the test set ID in Spira, if specified, null otherwise
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static int? GetSpiraTestSetId(JObject configuration)
        {
            JToken credentials = configuration.GetValue("credentials");
            int? id = credentials.Value<int?>("test_set_id");

            return id;
        }

        #region Spira Credentials
        private static string SpiraUrl(JObject configuration)
        {
            JToken credentials = configuration.GetValue("credentials");
            return credentials.Value<string>("url");
        }
        private static string SpiraUsername(JObject configuration)
        {
            JToken credentials = configuration.GetValue("credentials");
            return credentials.Value<string>("username");
        }
        private static string SpiraToken(JObject configuration)
        {
            JToken credentials = configuration.GetValue("credentials");
            return credentials.Value<string>("token");
        }
        private static int SpiraProjectId(JObject configuration)
        {
            JToken credentials = configuration.GetValue("credentials");
            return credentials.Value<int>("project_id");
        }

        #endregion

    }
}
