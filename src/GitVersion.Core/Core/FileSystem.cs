using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitVersion.Helpers;

namespace GitVersion
{
    public class FileSystem : IFileSystem
    {
        public void Copy(string from, string to, bool overwrite) => File.Copy(from, to, overwrite);

        public void Move(string from, string to) => File.Move(from, to);

        public bool Exists(string file) => File.Exists(file);

        public void Delete(string path) => File.Delete(path);

        public string ReadAllText(string path) => File.ReadAllText(path);

        public void WriteAllText(string? file, string fileContents)
        {
            // Opinionated decision to use UTF8 with BOM when creating new files or when the existing
            // encoding was not easily detected due to the file not having an encoding preamble.
            var encoding = EncodingHelper.DetectEncoding(file) ?? Encoding.UTF8;
            WriteAllText(file, fileContents, encoding);
        }

        public void WriteAllText(string? file, string fileContents, Encoding encoding) =>
            File.WriteAllText(file, fileContents, encoding);

        public IEnumerable<string> DirectoryEnumerateFiles(string? directory, string searchPattern, SearchOption searchOption) =>
            Directory.EnumerateFiles(directory, searchPattern, searchOption);

        public Stream OpenWrite(string path) => File.OpenWrite(path);

        public Stream OpenRead(string path) => File.OpenRead(path);

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);

        public bool DirectoryExists(string path) => Directory.Exists(path);

        public long GetLastDirectoryWrite(string path) => new DirectoryInfo(path)
                .GetDirectories("*.*", SearchOption.AllDirectories)
                .Select(d => d.LastWriteTimeUtc)
                .DefaultIfEmpty()
                .Max()
                .Ticks;

        public bool PathsEqual(string? path, string? otherPath) =>
            string.Equals(
                Path.GetFullPath(path).TrimEnd('\\').TrimEnd('/'),
                Path.GetFullPath(otherPath).TrimEnd('\\').TrimEnd('/'),
                StringComparerUtils.OsDependentComparison);
    }
}
