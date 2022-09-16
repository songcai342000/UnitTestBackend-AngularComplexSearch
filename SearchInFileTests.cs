using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnitTestBackend.Services;
using UnitTestBackend.Wrappers;

namespace UnitTestBackend
{
    [TestClass]
    public class SearchInFileTests
    {
        [TestMethod]
        public void Check_DirectoryNotExists_Throw()
        {
            //arrange
            var path = @"F:\repo";

            var mockFileIO = new Mock<IFileIOWrapper>();
            mockFileIO.Setup(t => t.DirectoryExists(path)).Returns(false);

            var wordCountService = new WordCountService(mockFileIO.Object);

            //act and assert
            Assert.ThrowsException<DirectoryNotFoundException>(() => wordCountService.DirectoryExists(path));
        }
        
        [TestMethod]
        public void Get_WordCounts_ReturnsSameNumber()
        {
            //arrange
            var sb = new StringBuilder();
            sb.AppendLine("hello world");
            sb.AppendLine("hello code");

            var exceptedCounts = new Dictionary<string, int>()
            {
                ["hello"] = 2,
                ["world"] = 1,
                ["code"] = 1
            };

            var path = @"F:\book.txt";
            var mockFileIO = new Mock<IFileIOWrapper>();
            mockFileIO.Setup(t => t.DirectoryExists(path)).Returns(true);
            mockFileIO.Setup(t => t.ReadAllText(path)).Returns(sb.ToString());
            var wordCountService = new WordCountService(mockFileIO.Object);
            var wordCounts = wordCountService.GetWordCounts(path);//act
            CollectionAssert.AreEquivalent(exceptedCounts, wordCounts);
        }

        [TestMethod]
        public void Search_ExactTexts_ReturnsTrue()
        {
            
            var sb = new StringBuilder();//arrange
            sb.AppendLine("hello world");
            sb.AppendLine("hello code");

            var expectedFound = true;

            var path = @"F:\book.txt";
            var searchTerm = "hello";
            var mockFileIO = new Mock<IFileIOWrapper>();
            mockFileIO.Setup(t => t.DirectoryExists(path)).Returns(true);
            mockFileIO.Setup(t => t.ReadAllText(path)).Returns(sb.ToString());
            var wordCountService = new WordCountService(mockFileIO.Object);
            var actualFound = wordCountService.FindExactText(searchTerm, path);
            Assert.AreEqual(expectedFound, actualFound);
        }

        [TestMethod]
        public void Check_Is60PercentSimilarity_ReturnsEqualList()
        {
            //arrange
            var sb = new StringBuilder();
            sb.AppendLine("hello world");
            sb.AppendLine("hello code");

            var expectedTexts = new List<string>()
            {
                "hello",
                "hello"
            };

            var path = @"F:\book.txt";
            var searchTerm = "hiklo";
            var mockFileIO = new Mock<IFileIOWrapper>();
            mockFileIO.Setup(t => t.DirectoryExists(path)).Returns(true);
            mockFileIO.Setup(t => t.ReadAllText(path)).Returns(sb.ToString());
            var wordCountService = new WordCountService(mockFileIO.Object);
            var matchedTexts = wordCountService.GetMatchsWith60PercentSimilarity(searchTerm, path);
            CollectionAssert.AreEquivalent(expectedTexts, matchedTexts);
        }

        [TestMethod]
        public void Determine_FirstBlankLine_ReturnsTitle()
        {
            //arrange
            var sb = new StringBuilder();
            sb.AppendLine("hello world");
            sb.AppendLine("\r\n");
            sb.AppendLine("hello code");
            var expectedText = "hello world";
            var path = @"F:\book.txt";
            var searchTerm = "hello";
            var mockFileIO = new Mock<IFileIOWrapper>();
            mockFileIO.Setup(t => t.DirectoryExists(path)).Returns(true);
            mockFileIO.Setup(t => t.ReadAllText(path)).Returns(sb.ToString());
            var wordCountService = new WordCountService(mockFileIO.Object);
            var matchedText = wordCountService.FindTitle(searchTerm, path);
            Assert.AreEqual(expectedText, matchedText);
        }

        [TestMethod]
        public void Get_FilesInDirectories_ReturnsSameNumber()
        {
            string[] files = { "book.txt", "book1.txt" };
            //arrange
            var directoryPath = "F:\\repos\\TestFiles\\Music\\Female";
            var mockFileIO = new Mock<IFileIOWrapper>();
            mockFileIO.Setup(t => t.DirectoryExists(directoryPath)).Returns(true);
            mockFileIO.Setup(t => t.GetFilesInDirectory(directoryPath)).Returns(files);
            var wordCountService = new WordCountService(mockFileIO.Object);
            var actualEntries = wordCountService.GetNumberOfAllFiles(directoryPath, 0);
            Assert.AreEqual(2, actualEntries);
        }

        
    }
 }
