using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

namespace MVS.Scaffolding
{
    /// <summary>
    /// This class defines the command line options to be used with the 'customaspnetcodegenerator'.
    /// </summary>
    public class BundleCommandLineModel
    {
        [Option(Name="bundleName", ShortName="n", Description="Name of the bundle to be generated.")]
        public string BundleName { get; set; }

        [Option(Name="force", ShortName="f", Description="Specify this option to overwrite existing files.")]
        public bool Force { get; internal set; }
    }
}