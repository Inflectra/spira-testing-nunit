using System;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using System.Web.Services;
using System.Web.Services.Protocols;

using NUnit.Core;
using NUnit.Core.Extensibility;

namespace Inflectra.SpiraTest.AddOns.SpiraTestNUnitAddIn
{
	/// <summary>
	/// This class extends the built-in NUnit test case to provide support for logging the execution
	/// results with the configured instance of SpiraTest
	/// </summary>
	public class SpiraTestCase : TestCase
	{
		private const string CLASS_NAME = "SpiraTestCase::";
		protected const string TEST_EXECUTE_WEB_SERVICES_URL = "/Services/TestExecute.asmx";

		protected TestCase testCase;
		protected int testCaseId = 0;

		/// <summary>
		/// The constructor method for the class. Sets the local member variables with the passed in values
		/// </summary>
		/// <param name="testCase">The NUnit Test Case</param>
		/// <param name="testCaseId">The ID of the matching SpiraTest test case</param>
		public SpiraTestCase (TestCase testCase, int testCaseId) : base (testCase.TestName.FullName, testCase.TestName.Name)
		{
			//Set the member variables
			this.testCase = testCase;
			this.testCaseId = testCaseId;
		}

		/// <summary>
		/// Executes the NUnit test case
		/// </summary>
		/// <param name="result">The test case result</param>
		public override void Run(TestCaseResult result)
		{
			const string METHOD_NAME = "Run: ";

			try
			{
				//Call the base method to log the result within NUnit
				testCase.Run (result);

				//Get the URL, Login, Password and ProjectId from the parent test fixture attribute
				Type type = testCase.FixtureType;
				Attribute attribute = Reflect.GetAttribute (type, "Inflectra.SpiraTest.AddOns.SpiraTestNUnitAddIn.SpiraTestFramework.SpiraTestConfigurationAttribute", false);
				if (attribute == null)
				{
					throw new Exception ("Cannot retrieve the SpiraTestConfiguration attribute from the test fixture");
				}

				//Get the various properties from the attribute
				string url = (string) Reflect.GetPropertyValue (attribute, "Url", BindingFlags.Public | BindingFlags.Instance);
				string login = (string) Reflect.GetPropertyValue (attribute, "Login", BindingFlags.Public | BindingFlags.Instance);
				string password = (string) Reflect.GetPropertyValue (attribute, "Password", BindingFlags.Public | BindingFlags.Instance);
				int projectId = (int) Reflect.GetPropertyValue (attribute, "ProjectId", BindingFlags.Public | BindingFlags.Instance);
				int releaseId = -1;
				if (Reflect.GetPropertyValue (attribute, "ReleaseId", BindingFlags.Public | BindingFlags.Instance) != null)
				{
					releaseId = (int) Reflect.GetPropertyValue (attribute, "ReleaseId", BindingFlags.Public | BindingFlags.Instance);
				}
				string runnerName = Reflect.GetPropertyValue (attribute, "Runner", BindingFlags.Public | BindingFlags.Instance).ToString();

				//Now we need to extract the result information
				int executionStatusId = -1;
				if (!result.Executed)
				{
					//Set status to 'Not Run'
					executionStatusId = 3;
				}
				else
				{
					if (result.IsFailure)
					{
						//Set status to 'Failed'
						executionStatusId = 1;
					}
					if (result.IsSuccess)
					{
						//Set status to 'Passed'
						executionStatusId = 2;
					}
				}

				//Extract the other information
				string testCaseName = result.Name;
				string message = result.Message;
				string stackTrace = result.StackTrace;
				int assertCount = result.AssertCount;
				DateTime startDate = DateTime.Now.AddSeconds (-result.Time);
				DateTime endDate = DateTime.Now;

				//Instantiate the web-service proxy class and set the URL from the text box
				bool success = false;
				SpiraTestExecute.TestExecute spiraTestExecuteProxy = new SpiraTestExecute.TestExecute();
				spiraTestExecuteProxy.Url = url + TEST_EXECUTE_WEB_SERVICES_URL;

				//Create a new cookie container to hold the session handle
				CookieContainer cookieContainer = new CookieContainer();
				spiraTestExecuteProxy.CookieContainer = cookieContainer;

				//Attempt to authenticate the user
				success = spiraTestExecuteProxy.Authenticate (login, password);
				if (!success)
				{
					throw new Exception ("Cannot authenticate with SpiraTest, check the URL, login and password");
				}

				//Now connect to the specified project
				success = spiraTestExecuteProxy.ConnectToProject (projectId);
				if (!success)
				{
					throw new Exception ("Cannot connect to the specified project, check permissions of user!");
				}

				//Now actually record the test run itself
				spiraTestExecuteProxy.RecordTestRun (
					-1,
					testCaseId,
					releaseId,
					startDate,
					endDate,
					executionStatusId,
					runnerName,
					testCaseName,
					assertCount,
					message,
					stackTrace
					);

				//Close the SpiraTest connection
				spiraTestExecuteProxy.Disconnect();
			}
			catch (Exception exception)
			{
				//Log error then rethrow
				System.Diagnostics.EventLog.WriteEntry (SpiraTestAddin.SOURCE_NAME, CLASS_NAME + METHOD_NAME + exception.Message, System.Diagnostics.EventLogEntryType.Error);
				throw exception;
			}
		}
	}
}
