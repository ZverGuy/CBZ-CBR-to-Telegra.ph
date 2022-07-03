namespace CBZ_To_Telegraph.Settings
{
    public class ArticleUploaderSettings
    {
        public int MaxParallelThreadsForScans { get; set; }
        public int MaxParallelThreadsForArticles { get; set; }
        public string AuthorName { get; set; }
        public string AccessToken { get; set; }
        
        public int MaxScansPerChapter { get; set; }
        public string ProxyTxtFileName { get; set; }
        public bool EnableProxy { get; set; }
        public double UploadDelay { get; set; }
    }
}