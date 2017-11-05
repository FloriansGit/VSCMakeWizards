// Based on https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-use-wizards-with-project-templates
//      and https://stackoverflow.com/questions/3882764/issue-with-visual-studio-template-directory-creation

namespace VSCMakeWizards
{
    using System;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TemplateWizard;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

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

        protected void AddTemplate(
            string srcPath,
            string destPath,
            Dictionary<string, string> replacementsDictionary,
            bool bAppend = false)
        {
            if (File.Exists(srcPath))
            {
                // see https://stackoverflow.com/questions/1231768/c-sharp-string-replace-with-dictionary
                Regex re = new Regex(@"(\$\w+\$)", RegexOptions.Compiled);

                string input = File.ReadAllText(srcPath);
                string output = re.Replace(input, match => replacementsDictionary[match.Groups[1].Value]);

                // Don't overwrite
                if (!File.Exists(destPath))
                {
                    File.WriteAllText(destPath, output);
                }
                else if (bAppend)
                {
                    File.AppendAllText(destPath, output);
                }
            }
        }

        protected virtual void AddCMakeCode(string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
        }

        protected virtual void AddSourceFiles(string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
        }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            var desiredNamespace = replacementsDictionary["$safeprojectname$"];
            var templatePath = Path.GetDirectoryName((string)customParams[0]);

            if (replacementsDictionary["$exclusiveproject$"] == Boolean.TrueString)
            {
                var dte = automationObject as DTE2;
                var solution = dte.Solution as EnvDTE100.Solution4;

                if (solution.IsOpen)
                {
                    solution.Close();
                }
            }

            var solutionDir = replacementsDictionary["$solutiondirectory$"];

            // If no solution name give, we take the project's safe name
            if (!replacementsDictionary.ContainsKey("$specifiedsolutionname$"))
            {
                replacementsDictionary.Add("$specifiedsolutionname$", desiredNamespace);
            }
            else if (replacementsDictionary["$specifiedsolutionname$"] == string.Empty)
            {
                replacementsDictionary["$specifiedsolutionname$"] = desiredNamespace;
            }

            AddTemplate(Path.Combine(templatePath, "CMakeLists.txt"),
                        Path.Combine(solutionDir, "CMakeLists.txt"),
                        replacementsDictionary);
            AddTemplate(Path.Combine(templatePath, "CMakeSettings.json"),
                        Path.Combine(solutionDir, "CMakeSettings.json"),
                        replacementsDictionary);

            AddCMakeCode(templatePath, replacementsDictionary);
            AddSourceFiles(templatePath, replacementsDictionary);

            var vsSolution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution7;

            if (vsSolution != null)
            {
                vsSolution.OpenFolder(solutionDir);
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

    public class WizardImplementationExecutable : WizardImplementation
    {
        protected override void AddCMakeCode(string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
            var solutionDir = replacementsDictionary["$solutiondirectory$"];
            var destinationDir = replacementsDictionary["$destinationdirectory$"];

            if (destinationDir == solutionDir)
            {
                AddTemplate(Path.Combine(templatePath, "AddExecutable.cmake"),
                            Path.Combine(solutionDir, "CMakeLists.txt"),
                            replacementsDictionary,
                            true);
            }
            else
            {
                AddTemplate(Path.Combine(templatePath, "AddSubdirectory.cmake"),
                            Path.Combine(solutionDir, "CMakeLists.txt"),
                            replacementsDictionary,
                            true);
                AddTemplate(Path.Combine(templatePath, "AddExecutable.cmake"),
                            Path.Combine(destinationDir, "CMakeLists.txt"),
                            replacementsDictionary);
            }
        }

        protected override void AddSourceFiles(string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
            var destinationDir = replacementsDictionary["$destinationdirectory$"];
            var destinationDirSrc = Path.Combine(destinationDir, "src");

            Directory.CreateDirectory(destinationDirSrc);

            var projectName = replacementsDictionary["$projectname$"];

            AddTemplate(Path.Combine(templatePath, "main.cpp"),
                        Path.Combine(destinationDirSrc, projectName + ".cpp"),
                        replacementsDictionary);
        }
    }

    public class WizardImplementationLibrary : WizardImplementation
    {
        protected override void AddCMakeCode(string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
            var solutionDir = replacementsDictionary["$solutiondirectory$"];
            var destinationDir = replacementsDictionary["$destinationdirectory$"];

            if (destinationDir == solutionDir)
            {
                AddTemplate(Path.Combine(templatePath, "AddLibrary.cmake"),
                            Path.Combine(solutionDir, "CMakeLists.txt"),
                            replacementsDictionary,
                            true);
            }
            else
            {
                AddTemplate(Path.Combine(templatePath, "AddSubdirectory.cmake"),
                            Path.Combine(solutionDir, "CMakeLists.txt"),
                            replacementsDictionary,
                            true);
                AddTemplate(Path.Combine(templatePath, "AddLibrary.cmake"),
                            Path.Combine(destinationDir, "CMakeLists.txt"),
                            replacementsDictionary);
            }
        }

        protected override void AddSourceFiles(string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
            var destinationDir = replacementsDictionary["$destinationdirectory$"];
            var destinationDirSrc = Path.Combine(destinationDir, "src");
            var destinationDirInc = Path.Combine(destinationDir, "inc");

            Directory.CreateDirectory(destinationDirSrc);
            Directory.CreateDirectory(destinationDirInc);

            var projectName = replacementsDictionary["$projectname$"];

            // dont't overwrite
            AddTemplate(Path.Combine(templatePath, "lib.cpp"),
                        Path.Combine(destinationDirSrc, projectName + ".cpp"),
                        replacementsDictionary);
            AddTemplate(Path.Combine(templatePath, "lib.h"),
                        Path.Combine(destinationDirInc, projectName + ".h"),
                        replacementsDictionary);
        }
    }
}
