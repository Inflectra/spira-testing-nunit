using NUnit.Framework.Interfaces;
using System;
using NUnit.Framework.Internal.Commands;
using NUnit.Engine.Extensibility;
using NUnit.Engine;
using System.Xml.Linq;
using System.Xml;

namespace Inflectra.SpiraTest.AddOns.NUnit
{
    [Extension(Description = "SpiraTest Reporter Extension")]
    public class SpiraEventListener: ITestEventListener
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

        private static XName methodName = "methodname";

        private static XName executionResult = "result";

        #endregion

        public void OnTestEvent(string report)
        {
            // test-run is passed in once execution is finished
            if (report.StartsWith("<test-run"))
            {
                XElement xml = XElement.Parse(report);
                /*foreach(XAttribute a in xml.Attributes())
                {
                    Console.WriteLine(a);
                }*/

                //loop through each test suite recursively
                foreach(XElement e in xml.Elements(testSuite))
                {
                    ProcessTestSuite(e);
                }

            }
        }

        /// <summary>
        /// Recursively process a test suite
        /// </summary>
        /// <param name="testSuite"></param>
        private void ProcessTestSuite(XElement element)
        {
            //process any nested test suites
            foreach(XElement e in element.Elements(testSuite))
            {
                ProcessTestSuite(e);
            }
            //process any test cases
            foreach(XElement e in element.Elements(testCase))
            {
                ProcessTestCase(e);
            }

        }

        /// <summary>
        /// Process a single XML test case and send it to Spira
        /// </summary>
        /// <param name="element"></param>
        private void ProcessTestCase(XElement element)
        {
            SpiraTestRun testRun = new SpiraTestRun();
            //name of the method
            testRun.RunnerTestName = element.Attribute(methodName).Value;
            //either Passed, Failed, Inconclusive, or Skipped
            string result = element.Attribute(executionResult).Value;
            

        }
    }

}
