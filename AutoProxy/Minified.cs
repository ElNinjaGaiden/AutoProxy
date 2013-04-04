using System.Collections.Generic;

namespace AutoProxy
{
    public class Minified : IMinified
    {
        public bool Generate { get; set; }

        public string Name { get; set; }

        public IEnumerable<IFile> RequiredFiles { get; set; }
    }
}
