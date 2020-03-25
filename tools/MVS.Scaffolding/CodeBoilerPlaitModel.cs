namespace MVS.Scaffolding
{
    public class CodeBoilerPlaitModel
    {
        private string _commandName;
        private string _class;
        private string _projectPath;
        private string _namespaceMember;
        private string _ext;
        public CodeBoilerPlaitModel(string projectPath, string namespaceMember, string commandName, string @class, string ext)
        {
            _projectPath = projectPath;
            _namespaceMember = namespaceMember;
            _class = @class;
            _ext = ext;
            _commandName = commandName;
        }
        public string @File => $"{_class}.cshtml";

        public string DestinationPath => $"{_projectPath}/{_namespaceMember}/{_commandName}{_class}{_ext}";

    }
}