// <copyright file="WizardImplementationClass.cs" company="Florian">
// Copyright (c) Florian. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

// Based on
// https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-use-wizards-with-project-templates
// https://stackoverflow.com/questions/3882764/issue-with-visual-studio-template-directory-creation

namespace VSCMakeWizards
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TemplateWizard;

    /// <summary>
    /// Base class for the custom wizard implementation
    /// </summary>
    /// <seealso cref="Microsoft.VisualStudio.TemplateWizard.IWizard" />
    public class WizardImplementationClass : IWizard
    {
        /// <summary>
        /// Runs custom wizard logic before opening an item in the template.
        /// </summary>
        /// <param name="projectItem">The project item that will be opened.</param>
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">The project that finished generating.</param>
        public void ProjectFinishedGenerating(Project project)
        {
        }

        /// <summary>
        /// Runs custom wizard logic when a project item has finished generating.
        /// </summary>
        /// <param name="projectItem">The project item that finished generating.</param>
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        public void RunFinished()
        {
        }

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">The automation object being used by the template wizard.</param>
        /// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
        /// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind" /> indicating the type of wizard run.</param>
        /// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
        /// <exception cref="WizardCancelledException">This is used to cancel solution creation ("Open Folder" is used instead)</exception>
        public void RunStarted(
            object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind,
            object[] customParams)
        {
            var desiredNamespace = replacementsDictionary["$safeprojectname$"];
            var templatePath = Path.GetDirectoryName((string)customParams[0]);

            if (replacementsDictionary["$exclusiveproject$"] == bool.TrueString)
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

            this.AddTemplate(
                Path.Combine(templatePath, "CMakeLists.txt"),
                Path.Combine(solutionDir, "CMakeLists.txt"),
                replacementsDictionary);
            this.AddTemplate(
                Path.Combine(templatePath, "CMakeSettings.json"),
                Path.Combine(solutionDir, "CMakeSettings.json"),
                replacementsDictionary);

            this.AddCMakeCode(templatePath, replacementsDictionary);
            this.AddSourceFiles(templatePath, replacementsDictionary);

            var vsSolution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution7;

            if (vsSolution != null)
            {
                vsSolution.OpenFolder(solutionDir);
            }

            throw new WizardCancelledException();
        }

        /// <summary>
        /// Indicates whether the specified project item should be added to the project.
        /// </summary>
        /// <param name="filePath">The path to the project item.</param>
        /// <returns>
        /// true if the project item should be added to the project; otherwise, false.
        /// </returns>
        public bool ShouldAddProjectItem(string filePath)
        {
            return false;
        }

        /// <summary>
        /// Adds a template from the template file repository while replacing the keywords given in
        /// the dictionary.
        /// </summary>
        /// <param name="srcPath">The source path.</param>
        /// <param name="destPath">The dest path.</param>
        /// <param name="replacementsDictionary">The replacements dictionary.</param>
        /// <param name="append">
        /// if set to <c>true</c> the source files content is appended to dest file.
        /// </param>
        protected void AddTemplate(
            string srcPath,
            string destPath,
            Dictionary<string, string> replacementsDictionary,
            bool append = false)
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
                else if (append)
                {
                    File.AppendAllText(destPath, output);
                }
            }
        }

        /// <summary>
        /// Generates the templates CMake code.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <param name="replacementsDictionary">The replacements dictionary.</param>
        protected virtual void AddCMakeCode(
            string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
        }

        /// <summary>
        /// Generates the source files.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <param name="replacementsDictionary">The replacements dictionary.</param>
        protected virtual void AddSourceFiles(
            string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
        }
    }
}
