using System.Collections.Generic;

namespace AutoProxy
{
    public interface ILibrary
    {
        string Output { get; }

        bool Compress { get; }

        string Namespace { get; set; }

        IEnumerable<IFile> IncludeFiles { get; }

        IEnumerable<IController> Controllers { get; }
    }
}
