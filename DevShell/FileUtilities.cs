using System;
using System.IO;
using System.Linq;

namespace DevShell
{
    public static class FileUtilities
    {
        public static string WhereSearch(string filename)
        {
            var paths = new[] { Environment.CurrentDirectory }
                .Concat(Environment.GetEnvironmentVariable("PATH").Split(';'));
            var extensions = new[] { String.Empty }
                .Concat(Environment.GetEnvironmentVariable("PATHEXT").Split(';')
                    .Where(e => e.StartsWith(".")));
            var combinations = paths.SelectMany(x => extensions,
                (path, extension) => System.IO.Path.Combine(path, filename + extension));
            return combinations.FirstOrDefault(File.Exists);
        }
    }
}