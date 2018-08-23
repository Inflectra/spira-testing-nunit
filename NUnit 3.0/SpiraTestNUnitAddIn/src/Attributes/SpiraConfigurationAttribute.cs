using System;

namespace Inflectra.SpiraTest.AddOns.NUnit
{
    /// <summary>
	/// This stores the configuration information needed to access the SpiraTest server
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SpiraConfigurationAttribute: Attribute
    {
        private string url;
        private string login;
        private string token;
        private int projectId;
        private Nullable<int> releaseId;
        private Nullable<int> testSetId;



        public SpiraConfigurationAttribute(string url, string login, string token, int projectId, int releaseId, int testSetId)
        {
            this.url = url;
            this.login = login;
            this.token = token;
            this.projectId = projectId;
            this.releaseId = releaseId;
            this.testSetId = testSetId;
        }
    }
}
