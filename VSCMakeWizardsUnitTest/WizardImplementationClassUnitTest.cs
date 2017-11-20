using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TemplateWizard;
using System.IO.Abstractions.TestingHelpers;
using System.Collections.Generic;
using VSCMakeWizards;

namespace VSCMakeWizardsUnitTest
{
    using XFS = MockUnixSupport;

    [TestClass]
    public class WizardImplementationClassUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string path = XFS.Path(@"c:\something\demo.txt");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {path, new MockFileData("Demo text content")}
            });

            WizardImplementationClass wizard = new WizardImplementationClass(fileSystem);
            Dictionary<string, string> replacementDictionary = new Dictionary<string, string>();
            string[] customParameters = { @"TemplatePath\MyTemplate.vstemplate" };

            try
            {
                wizard.RunStarted(null, replacementDictionary, WizardRunKind.AsNewProject, customParameters);
                Assert.Fail(); // never should get here
            }
            catch (WizardCancelledException)
            {
            }
        }
    }
}
