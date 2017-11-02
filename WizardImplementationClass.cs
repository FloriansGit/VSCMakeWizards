// Based on https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-use-wizards-with-project-templates
//      and https://stackoverflow.com/questions/3882764/issue-with-visual-studio-template-directory-creation

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE80;

namespace VSCMakeWizards
{
    public class WizardImplementation : IWizard
    {
        // This method is called before opening any item that   
        // has the OpenInEditor attribute.  
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        // This method is only called for item templates,  
        // not for project templates.  
        public void ProjectItemFinishedGenerating(ProjectItem
            projectItem)
        {
        }

        // This method is called after the project is created.  
        public void RunFinished()
        {
        }

        private void AddTemplate(string srcPath, 
            string destPath, 
            Dictionary<string, string> replacementsDictionary)
        {
            if (File.Exists(srcPath))
            {
                // see https://stackoverflow.com/questions/1231768/c-sharp-string-replace-with-dictionary
                Regex re = new Regex(@"(\$\w+\$)", RegexOptions.Compiled);

                string input = File.ReadAllText(srcPath);
                string output = re.Replace(input, match => replacementsDictionary[match.Groups[1].Value]);

                File.WriteAllText(destPath, output);
            }
        }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            var destinationDir = replacementsDictionary["$destinationdirectory$"];
            var desiredNamespace = replacementsDictionary["$safeprojectname$"];
            var templatePath = Path.GetDirectoryName((string)customParams[0]);

            var dte = automationObject as DTE2;
            var solution = dte.Solution as EnvDTE100.Solution4;

            if (solution.IsOpen)
            {
                solution.Close();
            }

            AddTemplate(Path.Combine(templatePath, "CMakeLists.txt"),
                        Path.Combine(destinationDir, "CMakeLists.txt"),
                        replacementsDictionary);
            AddTemplate(Path.Combine(templatePath, "CMakeSettings.json"), 
                        Path.Combine(destinationDir, "CMakeSettings.json"),
                        replacementsDictionary);
            // executable template
            AddTemplate(Path.Combine(templatePath, "main.cpp"), 
                        Path.Combine(destinationDir, desiredNamespace + ".cpp"),
                        replacementsDictionary);
            // library template
            AddTemplate(Path.Combine(templatePath, "lib.cpp"),
                        Path.Combine(destinationDir, desiredNamespace + ".cpp"),
                        replacementsDictionary);
            AddTemplate(Path.Combine(templatePath, "lib.h"),
                        Path.Combine(destinationDir, desiredNamespace + ".h"),
                        replacementsDictionary);

            var vsSolution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution7;

            if (vsSolution != null)
            {
                vsSolution.OpenFolder(destinationDir);
            }

            throw new WizardCancelledException();
        }

        // This method is only called for item templates,  
        // not for project templates.  
        public bool ShouldAddProjectItem(string filePath)
        {
            return false;
        }
    }
}
