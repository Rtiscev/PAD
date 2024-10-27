namespace Front.Models
{
    public class BestFormats
    {
        public Audio[] audio { get; set; }
        public Video[] video { get; set; }
    }

    public class Audio
    {
        public string id { get; set; }
        public string ext { get; set; }
        public string tbr { get; set; }
        public string fileSize { get; set; }
    }

    public class Video
    {
        public string id { get; set; }
        public string ext { get; set; }
        public string rezolution { get; set; }
        public string fps { get; set; }
        public string fileSize { get; set; }
    }

}
