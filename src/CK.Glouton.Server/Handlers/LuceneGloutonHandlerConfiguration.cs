using CK.Glouton.Lucene;
using CK.Glouton.Model.Server;
using System;
using Lucene.Net.Index;

namespace CK.Glouton.Server.Handlers
{
    public class LuceneGloutonHandlerConfiguration : IGloutonHandlerConfiguration, ILuceneConfiguration
    {
        public int MaxSearch { get; set; }
        public string Path { get; set; }
        public string Directory { get; set; }

        private string _actualPath;
        public string ActualPath => _actualPath ?? ( _actualPath = GetActualPath() );

        public OpenMode? OpenMode { get; set; }

        private string GetActualPath()
        {
            var path = string.IsNullOrWhiteSpace( Path )
                ? System.IO.Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "Glouton", "Logs" )
                : Path;
            if( !string.IsNullOrEmpty( Directory ) )
                path += "\\" + Directory;
            return path;
        }

        public IGloutonHandlerConfiguration Clone()
        {
            return new LuceneGloutonHandlerConfiguration
            {
                MaxSearch = MaxSearch,
                Path = Path,
                Directory = Directory,
                OpenMode = OpenMode
            };
        }
    }
}