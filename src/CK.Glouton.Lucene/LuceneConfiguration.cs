using System;

namespace CK.Glouton.Lucene
{
    public class LuceneConfiguration : ILuceneConfiguration
    {
        public int MaxSearch { get; set; }
        public string Path { get; set; }
        public string Directory { get; set; }

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