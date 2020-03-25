using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

namespace MVS.Scaffolding
{
    /// <summary>
    /// This class defines the command line options to be used with the 'customaspnetcodegenerator'.
    /// </summary>
    public class CommandLineModel
    {
        [Option(Name="commandName", ShortName="n", Description="Name of the command to be generated.")]
        public string CommandName { get; set; }

        [Option(Name="force", ShortName="f", Description="Specify this option to overwrite existing files.")]
        public bool Force { get; internal set; }
    }
}