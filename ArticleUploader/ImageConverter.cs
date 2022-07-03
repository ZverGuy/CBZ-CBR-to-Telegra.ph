using System.IO;
using System.Linq;
using ImageMagick;

namespace CBZ_To_Telegraph.ChapterParserAndExtractor
{
    public class ImageConverter
    {
        public string ConvertImageToJpg(string pathtofile)
        {
            string fileformat = pathtofile.Split(".").Last();
            FileStream newFile = File.Create(pathtofile.Replace(fileformat, "jpg"));
            using (var image = new MagickImage(pathtofile))
            {
                image.SetCompression(CompressionMethod.JPEG);
                image.Quality = 95;
                image.Format = MagickFormat.Jpeg;
                image.Write(newFile);
            }

            string filepath = newFile.Name;
            newFile.Dispose();
            return filepath;
        }
    }
}