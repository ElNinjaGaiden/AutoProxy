using System.Collections.Generic;

namespace AutoProxy
{
    public interface ILibrary
    {
        bool SaveFile { get; }

        string Output { get; }

        bool Compress { get; }

        string Namespace { get; set; }

        IEnumerable<IFile> IncludeFiles { get; }

        IEnumerable<IController> Controllers { get; }
    }
}
