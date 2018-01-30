using System;
using System.Text.RegularExpressions;

namespace CK.Glouton.Common
{
    public static class StringExtensions
    {
        private static readonly Regex EnvironmentRegex = new Regex( @"%[A-Za-z0-9\(\)]*%" );

        public static string GetPathWithSpecialFolders( this string @this )
        {
            var regexMatch = EnvironmentRegex.Match( @this );

            return regexMatch.Success
                ? @this.Replace( regexMatch.Value, Environment.GetEnvironmentVariable( regexMatch.Value.Replace( "%", "" ) ) )
                : @this;

        }
    }
}
