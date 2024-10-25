using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using EndpointType = Front.Services.EndpointType;

namespace Front.Controllers
{
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;
        MongoService mongoService;
        private string NAME;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            mongoService = new();
        }

        public async Task<IActionResult> Index()
        {
            Task.Run(() =>
            {
                ConfigureDatabase();
            });

#if DEBUG
            string binPath = AppContext.BaseDirectory;
            Console.WriteLine("Path is:" + binPath);

            // List directories
            string[] directories = Directory.GetDirectories(binPath);
            Console.WriteLine("Directories:");
            foreach (string directory in directories)
            {
                Console.WriteLine(directory);
            }
#endif
            return View();
        }

        private async Task ConfigureDatabase()
        {
            try
            {
                await mongoService.ListAllFilesAsync();
            }
            catch (Exception me)
            {
                Console.WriteLine("Unable to insert due to an error: " + me);
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> GetGeneralData([FromBody] string ytLink)
        {
            string response = await UtilityService.GetResponse(EndpointType.General_Data, ytLink);
            GeneralData? generalData = JsonSerializer.Deserialize<GeneralData>(response);
            return PartialView("_YoutubeData", generalData);
        }

        [HttpPost]
        public async Task<PartialViewResult> GetVideoFormats([FromBody] string ytLink)
        {
            string response = await UtilityService.GetResponse(EndpointType.Video_Formats, ytLink);
            var videoFormats = JsonSerializer.Deserialize<VideoData[]>(response);
            return PartialView("_FormatsData", videoFormats);
        }

        [HttpPost]
        public async Task<PartialViewResult> GetAudioFormats([FromBody] string ytLink)
        {
            string response = await UtilityService.GetResponse(EndpointType.Audio_Formats, ytLink);
            var audioFormats = JsonSerializer.Deserialize<AudioData[]>(response);
            return PartialView("_FormatsData", audioFormats);
        }

        [HttpPost]
        public async Task<PartialViewResult> Get2BestFormats([FromBody] string ytLink)
        {
            List<AudioData> formats = new();

            return PartialView("_FormatsData", formats);
        }

        //struct FileTotalData
        //{
        //    public byte[] fileBytes;
        //    public string contentType;
        //    public string fileName;
        //}

        //private async Task<FileTotalData> GetResponseDownload(string id)
        //{
        //    try
        //    {
        //        HttpClient client = new();
        //        using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/downloadFileLocally?id={id}");

        //        string _contentType = response.Content.Headers.ContentType.ToString();
        //        string _fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('\"') ?? "downloaded_file";
        //        byte[] _fileBytes = await response.Content.ReadAsByteArrayAsync();

        //        FileTotalData fileTotalData = new() { contentType = _contentType, fileBytes = _fileBytes, fileName = _fileName };
        //        return fileTotalData;
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Response = ex.Message;
        //        return new();
        //    }
        //}



        //[HttpPost]
        //public async Task<IActionResult> GetVideoById([FromBody] VDInfo data)
        //{
        //    try
        //    {
        //        HttpClient client = new();
        //        using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/downloadByVideoID?videoUrl={data.ytLink}&vQualityID={data.selectedValue}");
        //        var data1 = await response.Content.ReadAsStringAsync();

        //        // get 9th
        //        string asdasdas = "[download] Destination: /app/downloads/";
        //        int index = data1.IndexOf(asdasdas);

        //        if (index != -1)
        //        {
        //            // Find the index of the substring starting from startIndex
        //            int index1 = data1.IndexOf('\n', index);
        //            string ggg = NAME = data1[(index + asdasdas.Length)..index1];
        //            Console.WriteLine(ggg);


        //            /////////////
        //            HttpClient client2 = new();
        //            using HttpResponseMessage response3 = await client.GetAsync($"http://ytdlp:8080/api/upload?videoName={NAME}");
        //            var data3 = await response3.Content.ReadAsStringAsync();
        //            /////////////



        //            using HttpResponseMessage response1 = await client.GetAsync($"http://ytdlp:8080/api/upload?videoName={NAME}");
        //            await response1.Content.ReadAsStringAsync();

        //            // Get the current working directory
        //            string projectDirectory = Directory.GetCurrentDirectory();

        //            // Output directory for downloads (e.g., "downloads" folder)
        //            string outputDirectory = Path.Combine(projectDirectory, "downloads");

        //            // Ensure the output directory exists
        //            Directory.CreateDirectory(outputDirectory);

        //            // Set the video name and file path
        //            string videoName = NAME; // Change this to your desired filename
        //            string filePath = Path.Combine(outputDirectory, videoName);


        //            // Path to the yt-dlp binary
        //            string ytDlpPath = "curl"; // Modify if necessary 

        //            // Command to run yt-dlp and retrieve metadata
        //            string ytDlpArguments = $"-X GET {response.Content} -H \"accept: application/json\" --output \"{filePath}.mp4\"";

        //            // Create the process to execute the command
        //            ProcessStartInfo processStartInfo = new ProcessStartInfo
        //            {
        //                FileName = ytDlpPath,
        //                Arguments = ytDlpArguments,
        //                RedirectStandardOutput = true,
        //                RedirectStandardError = true,
        //                UseShellExecute = false,
        //                CreateNoWindow = true
        //            };

        //            // Start the process
        //            using (Process process = new Process { StartInfo = processStartInfo })
        //            {
        //                process.Start();

        //                // Read the output from yt-dlp
        //                string output = process.StandardOutput.ReadToEnd();
        //                string errorOutput = process.StandardError.ReadToEnd();

        //                process.WaitForExit();

        //                // Print the outputs
        //                Console.WriteLine("Output:\n" + output);
        //                if (!string.IsNullOrEmpty(errorOutput))
        //                {
        //                    Console.WriteLine("Error:\n" + errorOutput);
        //                }
        //            }
        //        }

        //        //Console.WriteLine(data1);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Response = ex.Message;
        //        return BadRequest();
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> GetAudioById([FromBody] VDInfo data)
        //{
        //    try
        //    {
        //        HttpClient client = new();
        //        using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/downloadByAudioID?videoUrl={data.ytLink}&aQualityID={data.selectedValue}");
        //        await response.Content.ReadAsStringAsync();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Response = ex.Message;
        //        return BadRequest();
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> DownloadFile([FromBody] string fileName)
        //{
        //    try
        //    {
        //        HttpClient client = new();
        //        using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/upload?videoName={fileName}");
        //        await response.Content.ReadAsStringAsync();

        //        // Get the current working directory
        //        string projectDirectory = Directory.GetCurrentDirectory();

        //        // Output directory for downloads (e.g., "downloads" folder)
        //        string outputDirectory = Path.Combine(projectDirectory, "downloads");

        //        // Ensure the output directory exists
        //        Directory.CreateDirectory(outputDirectory);

        //        // Set the video name and file path
        //        string videoName = fileName; // Change this to your desired filename
        //        string filePath = Path.Combine(outputDirectory, videoName);


        //        // Path to the yt-dlp binary
        //        string ytDlpPath = "curl"; // Modify if necessary 

        //        // Command to run yt-dlp and retrieve metadata
        //        string ytDlpArguments = $"-X GET {response.Content} -H \"accept: application/json\" --output \"{filePath}.mp4\"";

        //        // Create the process to execute the command
        //        ProcessStartInfo processStartInfo = new ProcessStartInfo
        //        {
        //            FileName = ytDlpPath,
        //            Arguments = ytDlpArguments,
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };

        //        // Start the process
        //        using (Process process = new Process { StartInfo = processStartInfo })
        //        {
        //            process.Start();

        //            // Read the output from yt-dlp
        //            string output = process.StandardOutput.ReadToEnd();
        //            string errorOutput = process.StandardError.ReadToEnd();

        //            process.WaitForExit();

        //            // Print the outputs
        //            Console.WriteLine("Output:\n" + output);
        //            if (!string.IsNullOrEmpty(errorOutput))
        //            {
        //                Console.WriteLine("Error:\n" + errorOutput);
        //            }
        //        }

        //        //"curl - X GET \"https://file.io/9tC2edSTmXMY\" - H \"accept: application/json\"--output filename.ext";
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Response = ex.Message;
        //        return BadRequest();
        //    }
        //}
    }
}