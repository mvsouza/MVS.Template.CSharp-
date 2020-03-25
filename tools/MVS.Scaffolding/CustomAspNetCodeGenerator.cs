using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;

namespace MVS.Scaffolding
{
    [Alias("command")]
    public class CommandCodeGenerator : ICodeGenerator
    {


        private string _commandName;
        private const string generatedFileExtension = ".cs";
        private IProjectContext _projectContext;
        private IApplicationInfo _applicationInfo;
        private ICodeGeneratorActionsService _codeGeneratorActionsService;
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        private string[] _baseTemplateFolders = { "Command" };

        public CommandCodeGenerator(
            IProjectContext projectContext,
            IApplicationInfo applicationInfo,
            ICodeGeneratorActionsService codeGeneratorActionsService,
            IServiceProvider serviceProvider,
            ILogger logger)
        {
            _projectContext = projectContext ?? throw new ArgumentNullException(nameof(projectContext));
            _applicationInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo));
            _codeGeneratorActionsService = codeGeneratorActionsService ?? throw new ArgumentNullException(nameof(codeGeneratorActionsService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task GenerateCode(CommandLineModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrEmpty(model.CommandName))
                throw new ArgumentException("Please specify the name of the command using --commandName");
            
            var files = GetFiles(model.CommandName);
            foreach (var code in files)
                if (File.Exists(code.DestinationPath) && !model.Force)
                    throw new InvalidOperationException($"File already exists '{code.DestinationPath}' use -f to force over write.");
            
            var commandModel = new CommandModel()
            {
                CommandClassName = model.CommandName,
                Namespace = _applicationInfo.ApplicationName.ReplaceLast(".Infrastructure", "")
            };
            foreach (var code in files)
            {
                await _codeGeneratorActionsService.AddFileFromTemplateAsync(code.DestinationPath, code.File, TemplateFolders, commandModel);
                _logger.LogMessage($"Added: {code.DestinationPath.Substring(InfrastructurePath.Length)}");
            }
        }
        
        private IEnumerable<CodeBoilerPlaitModel> GetFiles(string commandName)
        {
            string appPath = ApplicationPath,
                   testPath = UnitTestPath;
            yield return new CodeBoilerPlaitModel(appPath, "Command", commandName, "Command", generatedFileExtension);
            yield return new CodeBoilerPlaitModel(appPath, "Command", commandName, "CommandHandler", generatedFileExtension);
            yield return new CodeBoilerPlaitModel(appPath, "Validation", commandName, "CommandValidation", generatedFileExtension);
            yield return new CodeBoilerPlaitModel(testPath, "Validation", commandName, "CommandValidationTests", generatedFileExtension);
            yield return new CodeBoilerPlaitModel(testPath, "Command", commandName, "CommandHandlerTests", generatedFileExtension);
        }
        
        public string InfrastructurePath => _applicationInfo.ApplicationBasePath;
        public string ApplicationPath => InfrastructurePath.ReplaceLast("Infrastructure", "Application");
        public string UnitTestPath => InfrastructurePath.Replace("src","test").ReplaceLast("Infrastructure", "UnitTest/Application");
        protected IEnumerable<string> TemplateFolders => 
            TemplateFoldersUtilities.GetTemplateFolders(
                    containingProject: this.GetType().GetTypeInfo().Assembly.GetName().Name,
                    applicationBasePath: InfrastructurePath,
                    baseFolders: _baseTemplateFolders,
                    projectContext: _projectContext);
    }
}
