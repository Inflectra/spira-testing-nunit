using System;

namespace Inflectra.SpiraTest.AddOns.NUnit
{
    /// <summary>
	/// This stores the SpiraTest Test Case Id for the specific NUnit test
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SpiraTestCaseAttribute: Attribute
    {
        private int testCaseId;

        public SpiraTestCaseAttribute(int testCaseId)
        {
            this.testCaseId = testCaseId;
        }
    }
}
