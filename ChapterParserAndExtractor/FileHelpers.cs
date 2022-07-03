using System;
using System.IO;
using System.IO.Compression;

namespace CBZ_To_Telegraph.ChapterParserAndExtractor
{
    internal static class FileHelpers
    {
        internal static FileStream WriteFileFromStream(Stream stream, string directoryname)
        {
            string filePath;
            stream.Position = 0;
            FileStream file;
            try
            {
                filePath = AppDomain.CurrentDomain.BaseDirectory + $"\\{directoryname}\\" + "unpack.cbz";
                file = System.IO.File.Create(filePath);
                stream.CopyTo(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + $"\\{directoryname}");
                filePath = AppDomain.CurrentDomain.BaseDirectory + $"\\{directoryname}\\" + "unpack.cbz";
                file = System.IO.File.Create(filePath);
                stream.CopyTo(file);
            }
            return file;
        }

        internal static DirectoryInfo UnpackZipFileToDirectory(string directoryname)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + $"\\{directoryname}\\" + "unpack.cbz";
            var unpackedDir =
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + $"\\{directoryname}\\unpacked");
            
            ZipFile.ExtractToDirectory(filePath, unpackedDir.FullName);
            return unpackedDir;
        }
    }
}