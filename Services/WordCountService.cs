using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnitTestBackend.Wrappers;

namespace UnitTestBackend.Services
{
	class WordCountService
	{
		private readonly IFileIOWrapper _fileIOWrapper;
		public WordCountService(IFileIOWrapper fileIOWrapper)
		{
			_fileIOWrapper = fileIOWrapper;
		}

		public void DirectoryExists(string path)
		{
			if (!_fileIOWrapper.DirectoryExists(path))
			{
				throw new DirectoryNotFoundException(path);//in controller it returns NotFound()
			}
		}
		public Dictionary<string, int> GetWordCounts(string path)
		{
			if (!_fileIOWrapper.DirectoryExists(path))
			{
				throw new FileNotFoundException(path);//in controller it returns NotFound()
			}

			var wordCounts = _fileIOWrapper.ReadAllText(path)
				.Split()
				.GroupBy(s => s).ToDictionary(word => word.Key, word => word.Count());

			wordCounts.Remove(""); //better than the verbose Split() overload to ignore empties

			return wordCounts;

		}

		public void SaveWordCounts(Dictionary<string, int> wordCounts, string path)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var wordCount in wordCounts)
			{
				sb.AppendLine($"{wordCount.Key}={wordCount.Value}");
			}

			_fileIOWrapper.WriteAllText(path, sb.ToString());
		}

		public bool FindExactText(string searchTerm, string path)
		{
			var textIsFound = _fileIOWrapper.ReadAllText(path).Contains(searchTerm);
			return textIsFound;
		}

		/*public List<string> FindFussionTexts(string searchTerm, string path)
		{
			int termLength = searchTerm.Length - 1;
			var allTexts = _fileIOWrapper.ReadAllText(path);
			var patter = searchTerm[0] + "[\\w0-9-_?+-.,/\\()\\[\\]@$#!\\{\\}=\\*'\\s\\S]{" + termLength + "}";
			var r = Regex.Matches(allTexts, patter, RegexOptions.IgnoreCase);
			List<string> matchTexts = new List<string>();
			foreach (var match in r)
            {
				matchTexts.Add(match.ToString());
            }
			return matchTexts;
		}*/

		public List<string> GetMatchsWith60PercentSimilarity(string searchTerm, string path)
		{
			int termLength = searchTerm.Length - 1;
			var allTexts = _fileIOWrapper.ReadAllText(path);
			var patter = searchTerm[0] + "[\\w0-9-_?+-.,/\\()\\[\\]@$#!\\{\\}=\\*'\\s\\S]{" + termLength + "}";
			var r = Regex.Matches(allTexts, patter, RegexOptions.IgnoreCase);
			var similarity = 0.0;
			List<string> matchTexts = new List<string>();
			foreach (var match in r)
			{
				var count = 0;
				for (int i = 0; i < searchTerm.Length; i++)
				{
					if (searchTerm[i] == match.ToString()[i])
					{
						count++;
					}
				}
				similarity = (double)count / (double)searchTerm.Length;
				if (similarity >= 0.6)
				{
					matchTexts.Add(match.ToString());
				}
			}
			return matchTexts;
		}

		public string FindTitle(string searchTerm, string path)
		{
			var allTexts = _fileIOWrapper.ReadAllText(path);
			var paragraphs = allTexts.Split("\r\n");
			if (paragraphs.Length > 0)
			{
				return paragraphs[0];
			}
			return null;
		}

		public string[] GetFileEntriesInDirectory(string directory)
		{
			if (_fileIOWrapper.DirectoryExists(directory))
			{
				var fileEntries = _fileIOWrapper.GetFilesInDirectory(directory);
				return fileEntries;
			}
			return null;
		}

		public string[] GetDirectories(string directory)
		{
			if (!_fileIOWrapper.DirectoryExists(directory))
			{
				throw new Exception("Directory not found");
			}
			var directories = _fileIOWrapper.GetAllDirectories(directory);
			return directories;
		}

		public int GetNumberOfAllFiles(string directory, int entriesNumber)
		{
			if (!_fileIOWrapper.DirectoryExists(directory))
			{
				throw new Exception("Directory not found");
			}
			var files = _fileIOWrapper.GetFilesInDirectory(directory);
			var subdirectories = _fileIOWrapper.GetAllDirectories(directory);
			if (files != null)
			{
				entriesNumber += files.Count();
			}
			if (subdirectories != null)
			{
				entriesNumber += subdirectories.Count();
			}
			return entriesNumber;
		}

		
	}
}
