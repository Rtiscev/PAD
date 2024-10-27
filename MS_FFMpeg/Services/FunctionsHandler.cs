using MongoDB.Driver;

namespace MS_FFMpeg.Services
{
    public static class FunctionsHandler
    {
        private static MongoService mongoService = new();
        private static FFmpegService ffmpegService = new();
        private static string downloadPath = Path.Combine(Environment.CurrentDirectory, "downloads");

        public static async Task<List<string>> ListItemsAsync()
        {
            return await mongoService.ListAllFilesAsync();
        }
        public static async Task<string> GetAudioInformation(string id)
        {
            //string workingDirectory = Environment.CurrentDirectory;
            //string downloadPath = Path.Combine(workingDirectory, "downloads");

            // find item in mongodb and download item locally 
            string fileName = await mongoService.DownloadFileAsync(new(id), downloadPath);
            return ffmpegService.GetGeneralAudioData(downloadPath + '/' + fileName);
        }
        public static async Task<string> GetAudioWavePic(string id)
        {
            // find item in mongodb and return its name
            string fileName = await mongoService.DownloadFileAsync(new(id), downloadPath);

            // download the file to downloads folder ... a little bit separation of concerns here
            string pngPath = ffmpegService.GetAudioWavePicID(downloadPath + "/" + fileName);
            Console.WriteLine("GETAUDIOWAVEPIC download path:" + downloadPath + "/" + fileName);
            Console.WriteLine("PNTPATH:" + pngPath);
            Console.WriteLine("FILENAME:" + fileName);

            string pngFileName = pngPath.Replace("/app/downloads/", "");
            Console.WriteLine("PNGFILENAME:" + pngFileName);
            // save the item in mongodb (upload it there) 
            var pngID = await mongoService.UploadFile(pngPath, pngFileName);
            Console.WriteLine("PNGID:" + pngID);

            return pngID.ToString();
        }
        public static async Task<string> ApplyEffects(string id, double speed, double volume, bool isNorm, string format)
        {
            // we have these parameters and we have the fileID. what do we need to do with them?
            // if we have the file ID then probably the file exists locally, if not we download it from DB
            // thats what the downloadfileasync() does
            // next up we need to tell ffmpeg to apply these effects (god bless) to the file that already exists (we made sure on previous step)

            // find item in mongodb and return its name
            string fileName = await mongoService.DownloadFileAsync(new(id), downloadPath);

            // apply effects and get the file name of converted file (new file)
            string convertedFileName = ffmpegService.ApplyEffects(fileName, speed, volume, isNorm, format);
            Console.WriteLine("CONVERTED :" + convertedFileName);

            // now upload it to the mongodb
            var uploadedFileID = await mongoService.UploadFile(convertedFileName, convertedFileName.Replace("/app/downloads/", ""));
            Console.WriteLine("Uploaded file:" + uploadedFileID);

            return uploadedFileID.ToString();
        }
    }
}
