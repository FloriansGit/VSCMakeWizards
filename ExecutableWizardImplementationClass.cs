// <copyright file="ExecutableWizardImplementationClass.cs" company="Florian">
// Copyright (c) Florian. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace VSCMakeWizards
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Abstractions;

    /// <summary>
    /// Generate executable project template wizard
    /// </summary>
    /// <seealso cref="VSCMakeWizards.WizardImplementationClass" />
    public class ExecutableWizardImplementationClass : WizardImplementationClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableWizardImplementationClass"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        public ExecutableWizardImplementationClass(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableWizardImplementationClass"/> class.
        /// </summary>
        public ExecutableWizardImplementationClass()
            : this(fileSystem: new FileSystem()) // use default implementation which calls System.IO
        {
        }

        /// <inheritdoc/>
        protected override void AddCMakeCode(
            string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
            var solutionDir = replacementsDictionary["$solutiondirectory$"];
            var destinationDir = replacementsDictionary["$destinationdirectory$"];

            if (destinationDir == solutionDir)
            {
                this.AddTemplate(
                    Path.Combine(templatePath, "AddExecutable.cmake"),
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
                    Path.Combine(templatePath, "AddExecutable.cmake"),
                    Path.Combine(destinationDir, "CMakeLists.txt"),
                    replacementsDictionary);
            }
        }

        /// <inheritdoc/>
        protected override void AddSourceFiles(
            string templatePath,
            Dictionary<string, string> replacementsDictionary)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ArgumentException("message", nameof(templatePath));
            }

            var destinationDir = replacementsDictionary["$destinationdirectory$"];
            var destinationDirSrc = Path.Combine(destinationDir, "src");

            Directory.CreateDirectory(destinationDirSrc);

            var projectName = replacementsDictionary["$projectname$"];

            this.AddTemplate(
                Path.Combine(templatePath, "main.cpp"),
                Path.Combine(destinationDirSrc, projectName + ".cpp"),
                replacementsDictionary);
        }
    }
}
