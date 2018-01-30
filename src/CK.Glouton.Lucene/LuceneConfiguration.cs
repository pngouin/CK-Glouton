using CK.Glouton.Common;
using Lucene.Net.Index;
using System;

namespace CK.Glouton.Lucene
{
    public class LuceneConfiguration : ILuceneConfiguration
    {
        private string _path;

        public int MaxSearch { get; set; }
        public string Path
        {
            get => _path;
            set => _path = value.GetPathWithSpecialFolders();
        }
        public string Directory { get; set; }
        public OpenMode? OpenMode { get; set; }

        private string _actualPath;
        public string ActualPath => _actualPath ?? ( _actualPath = GetActualPath() );

        private string GetActualPath()
        {
            var path = string.IsNullOrWhiteSpace( Path )
                ? System.IO.Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "Glouton", "Logs" )
                : Path;
            if( !string.IsNullOrEmpty( Directory ) )
                path += "\\" + Directory;
            return path;
        }
    }
}