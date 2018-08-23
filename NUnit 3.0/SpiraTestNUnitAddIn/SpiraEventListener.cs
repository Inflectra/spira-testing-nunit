using System;
using NUnit.Engine;
using NUnit.Engine.Extensibility;

namespace SpiraTestNUnitAddIn
{
    [Extension(Description = "SpiraTest Reporter Extension")]
    public class SpiraEventListener: ITestEventListener
    { 
        public void OnTestEvent(string report) {
            Console.WriteLine("Inflectra Report: " + report);
        }
    }
}
