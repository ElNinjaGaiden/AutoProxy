using System.Collections.Generic;

namespace AutoProxy
{
    public interface IMinified
    {
        bool Generate { get; }

        string Name { get; }

        IEnumerable<IFile> RequiredFiles { get; }
    }
}
