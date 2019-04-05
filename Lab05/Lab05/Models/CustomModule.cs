namespace Lab05.Models
{
    class CustomModule
    {
        private string _name;
        private string _filePath;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        public CustomModule(string name, string filePath)
        {
            _name = name;
            _filePath = filePath;
        }
    }
}
