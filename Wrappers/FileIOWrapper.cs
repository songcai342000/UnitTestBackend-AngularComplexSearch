using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UnitTestBackend.Wrappers
{
    class FileIOWrapper: IFileIOWrapper
    {
        public bool FileExists(string filepath)
        {
            return File.Exists(filepath);
        }
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        public string[] GetFilesInDirectory(string directory)
        {
            var files = Directory.GetFiles(directory);
            return files;
        }

        public string[] GetAllDirectories(string directory)
        {
            return Directory.GetDirectories(directory);
        }

       

    }
}
