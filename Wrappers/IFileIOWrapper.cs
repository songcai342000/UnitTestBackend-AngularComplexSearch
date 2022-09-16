using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestBackend.Wrappers
{
    public interface IFileIOWrapper
    {
        public bool FileExists(string filepath);
        public bool DirectoryExists(string path);
        public string ReadAllText(string path);
        public void WriteAllText(string path, string text);
        public string[] GetFilesInDirectory(string directory);
        public string[] GetAllDirectories(string directory);
    }
}
