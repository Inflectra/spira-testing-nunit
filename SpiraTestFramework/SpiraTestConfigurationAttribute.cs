using System;

namespace Inflectra.SpiraTest.AddOns.SpiraTestNUnitAddIn.SpiraTestFramework
{
	/// <summary>
	/// This stores the configuration information needed to access the SpiraTest server
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public sealed class SpiraTestConfigurationAttribute : Attribute
	{
		private string url;
		private string login;
		private string password;
		private int projectId;
		private int releaseId;
		private RunnerName runner;

		#region Enumerations
		
		public enum RunnerName
		{
			NUnit = 1,
			Selenium = 2
		}

		#endregion

		#region Properties
		
		/// <summary>
		/// The URL to the SpiraTest instance 
		/// </summary>
		public string Url
		{
			get
			{
				return this.url;
			}
			set
			{
				this.url = value;
			}
		}

		/// <summary>
		/// A valid SpiraTest login
		/// </summary>
		public string Login
		{
			get
			{
				return this.login;
			}
			set
			{
				this.login = value;
			}
		}

		/// <summary>
		/// A valid SpiraTest password
		/// </summary>
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		/// <summary>
		/// The name of the test runner that should be reported back to SpiraTest
		/// </summary>
		public RunnerName Runner
		{
			get
			{
				return this.runner;
			}
			set
			{
				this.runner = value;
			}
		}

		/// <summary>
		/// The SpiraTest project being tested
		/// </summary>
		public int ProjectId
		{
			get
			{
				return this.projectId;
			}
			set
			{
				this.projectId = value;
			}
		}

		/// <summary>
		/// The SpiraTest release being tested
		/// </summary>
		public int ReleaseId
		{
			get
			{
				return this.releaseId;
			}
			set
			{
				this.releaseId = value;
			}
		}

		#endregion

		/// <summary>
		/// The constructor is called when the attribute is set.
		/// </summary>
		/// <param name="url">The URL to the SpiraTest instance</param>
		/// <param name="login">A valid SpiraTest login</param>
		/// <param name="password">A valid SpiraTest password</param>
		/// <param name="projectId">The SpiraTest project being tested</param>
		public SpiraTestConfigurationAttribute (string url, string login, string password, int projectId) 
		{
			//Update the local member variables
			this.url = url;
			this.login = login;
			this.password = password;
			this.projectId = projectId;
			this.releaseId = -1;
			this.runner = RunnerName.NUnit;
		}

		/// <summary>
		/// The constructor is called when the attribute is set.
		/// </summary>
		/// <param name="url">The URL to the SpiraTest instance</param>
		/// <param name="login">A valid SpiraTest login</param>
		/// <param name="password">A valid SpiraTest password</param>
		/// <param name="projectId">The SpiraTest project being tested</param>
		/// <param name="runner">The name of the runner that should be reported back to SpiraTest</param>
		public SpiraTestConfigurationAttribute (string url, string login, string password, int projectId, RunnerName runner) 
		{
			//Update the local member variables
			this.url = url;
			this.login = login;
			this.password = password;
			this.projectId = projectId;
			this.releaseId = -1;
			this.runner = runner;
		}

		/// <summary>
		/// The constructor is called when the attribute is set.
		/// </summary>
		/// <param name="url">The URL to the SpiraTest instance</param>
		/// <param name="login">A valid SpiraTest login</param>
		/// <param name="password">A valid SpiraTest password</param>
		/// <param name="projectId">The SpiraTest project being tested</param>
		/// <param name="releaseId">The ID of the release being tested</param>
		public SpiraTestConfigurationAttribute (string url, string login, string password, int projectId, int releaseId) 
		{
			//Update the local member variables
			this.url = url;
			this.login = login;
			this.password = password;
			this.projectId = projectId;
			this.releaseId = releaseId;
			this.runner = RunnerName.NUnit;
		}

		/// <summary>
		/// The constructor is called when the attribute is set.
		/// </summary>
		/// <param name="url">The URL to the SpiraTest instance</param>
		/// <param name="login">A valid SpiraTest login</param>
		/// <param name="password">A valid SpiraTest password</param>
		/// <param name="projectId">The SpiraTest project being tested</param>
		/// <param name="releaseId">The ID of the release being tested</param>
		/// <param name="runner">The name of the runner that should be reported back to SpiraTest</param>
		public SpiraTestConfigurationAttribute (string url, string login, string password, int projectId, int releaseId, RunnerName runner) 
		{
			//Update the local member variables
			this.url = url;
			this.login = login;
			this.password = password;
			this.projectId = projectId;
			this.releaseId = releaseId;
			this.runner = RunnerName.NUnit;
			this.runner = runner;
		}
	}
}
