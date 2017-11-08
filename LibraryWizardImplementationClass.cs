// <copyright file="LibraryWizardImplementationClass.cs" company="Florian">
// Copyright (c) Florian. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace VSCMakeWizards
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Generate library project template wizard
    /// </summary>
    /// <seealso cref="VSCMakeWizards.WizardImplementationClass" />
    public class LibraryWizardImplementationClass : WizardImplementationClass
    {
        /// <inheritdoc/>
        protected override void AddCMakeCode(
            string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ArgumentException("message", nameof(templatePath));
            }

            var solutionDir = replacementsDictionary["$solutiondirectory$"];
            var destinationDir = replacementsDictionary["$destinationdirectory$"];

            if (destinationDir == solutionDir)
            {
                this.AddTemplate(
                    Path.Combine(templatePath, "AddLibrary.cmake"),
                    Path.Combine(solutionDir, "CMakeLists.txt"),
                    replacementsDictionary,
                    true);
            }
            else
            {
                this.AddTemplate(
                    Path.Combine(templatePath, "AddSubdirectory.cmake"),
                    Path.Combine(solutionDir, "CMakeLists.txt"),
                    replacementsDictionary,
                    true);
                this.AddTemplate(
                    Path.Combine(templatePath, "AddLibrary.cmake"),
                    Path.Combine(destinationDir, "CMakeLists.txt"),
                    replacementsDictionary);
            }
        }

        /// <inheritdoc/>
        protected override void AddSourceFiles(
            string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
            var destinationDir = replacementsDictionary["$destinationdirectory$"];
            var destinationDirSrc = Path.Combine(destinationDir, "src");
            var destinationDirInc = Path.Combine(destinationDir, "inc");

            Directory.CreateDirectory(destinationDirSrc);
            Directory.CreateDirectory(destinationDirInc);

            var projectName = replacementsDictionary["$projectname$"];

            this.AddTemplate(
                Path.Combine(templatePath, "lib.cpp"),
                Path.Combine(destinationDirSrc, projectName + ".cpp"),
                replacementsDictionary);
            this.AddTemplate(
                Path.Combine(templatePath, "lib.h"),
                Path.Combine(destinationDirInc, projectName + ".h"),
                replacementsDictionary);
        }
    }
}
