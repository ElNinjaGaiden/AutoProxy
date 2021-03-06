﻿using System.Collections.Generic;

namespace AutoProxy
{
    public class Library : ILibrary
    {
        public bool SaveFile { get; set; }

        public string Output { get; set; }

        public bool Compress { get; set; }

        public string Namespace { get; set; }

        public IEnumerable<IFile> IncludeFiles { get; set; }

        public IEnumerable<IController> Controllers { get; set; }
    }
}
