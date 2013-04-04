namespace AutoProxy
{
    public class ScriptFile : File
    {

        public string Content 
        {
            get
            {
                if (string.IsNullOrEmpty(_content))
                    _content = this.Src.ReadFileContent();

                return _content;
            }
            set
            {
                _content = value;
            }
        }

        private string _content;
    }
}
