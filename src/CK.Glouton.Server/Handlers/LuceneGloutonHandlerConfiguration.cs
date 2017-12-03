﻿using System;
using CK.Glouton.Model.Server;

namespace CK.Glouton.Server.Handlers
{
    public class LuceneGloutonHandlerConfiguration : IGloutonHandlerConfiguration
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

        public IGloutonHandlerConfiguration Clone()
        {
            return new LuceneGloutonHandlerConfiguration
            {
                MaxSearch = MaxSearch,
                Path = Path,
                Directory = Directory
            };
        }
    }
}