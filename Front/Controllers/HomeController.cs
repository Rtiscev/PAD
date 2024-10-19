using Front.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Front.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private string NAME;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<PartialViewResult> GetD([FromBody] string ytLink)
        {
            string response = await GetResponse(ytLink);

            VideoMetadata videoMetadata;

            // Parse the JSON
            using (JsonDocument doc = JsonDocument.Parse(response))
            {
                JsonElement root = doc.RootElement;

                videoMetadata = new VideoMetadata
                {
                    Title = root.GetProperty("title").GetString(),
                    Thumbnail = root.GetProperty("thumbnail").GetString(),
                    DurationString = root.GetProperty("duration_string").GetString(),
                    Channel = root.GetProperty("channel").GetString(),
                    UploadDate = root.GetProperty("upload_date").GetString()
                };

                // Safely get the comment count
                if (root.TryGetProperty("comment_count", out JsonElement commentCountElement) && commentCountElement.ValueKind != JsonValueKind.Null)
                {
                    videoMetadata.CommentCount = commentCountElement.GetInt32();
                }

                // Safely get the like count
                if (root.TryGetProperty("like_count", out JsonElement likeCountElement) && likeCountElement.ValueKind != JsonValueKind.Null)
                {
                    videoMetadata.LikeCount = likeCountElement.GetInt32();
                }

                // Safely get the view count
                if (root.TryGetProperty("view_count", out JsonElement viewCountElement) && viewCountElement.ValueKind != JsonValueKind.Null)
                {
                    videoMetadata.ViewCount = viewCountElement.GetInt32();
                }
            }
            return PartialView("_YoutubeData", videoMetadata);
        }

        [HttpPost]
        public async Task<PartialViewResult> GetVideoFormats([FromBody] string ytLink)
        {
            string response = await GetResponse2(ytLink);

            JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;


            List<VideoData> formats = new();
            foreach (JsonElement element in root.EnumerateArray())
            {
                string id = element.GetProperty("id").GetString();
                string ext = element.GetProperty("ext").GetString();
                string resolution = element.GetProperty("resolution").GetString();
                string fps = element.GetProperty("fps").GetString();
                string fileSize = element.GetProperty("fileSize").GetString();

                formats.Add(new() { Id = id, Ext = ext, Resolution = resolution, Fps = fps, FileSize = fileSize });
                Console.WriteLine($"{id} | {ext} | {resolution} | {fps} | {fileSize}");
            }
            ViewBag.type = "Video";
            return PartialView("_AvailableFormats", formats);
        }

        [HttpPost]
        public async Task<PartialViewResult> GetAudioFormats([FromBody] string ytLink)
        {
            string response = await GetResponse3(ytLink);

            JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;


            List<AudioData> formats = new();
            foreach (JsonElement element in root.EnumerateArray())
            {
                string id = element.GetProperty("id").GetString();
                string ext = element.GetProperty("ext").GetString();
                string tbr = element.GetProperty("tbr").GetString();
                string fileSize = element.GetProperty("fileSize").GetString();

                formats.Add(new() { Id = id, Ext = ext, TBR = tbr, FileSize = fileSize });
                Console.WriteLine($"{id} | {ext} | {fileSize} | {tbr}");
            }

            ViewBag.type = "Audio";
            return PartialView("_AvailableFormats", formats);
        }


        private async Task<string> GetResponse55533(string ytLink)
        {
            try
            {
                HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/endpoint?videoUrl={ytLink}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Response = ex.Message;
                return ex.Message;
            }
        }

        private async Task<string> GetResponse(string ytLink)
        {
            try
            {
                HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/endpoint?videoUrl={ytLink}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Response = ex.Message;
                return ex.Message;
            }
        }

        private async Task<string> GetResponse2(string ytLink)
        {
            try
            {
                HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/listVideoFormats?videoUrl={ytLink}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Response = ex.Message;
                return ex.Message;
            }
        }

        private async Task<string> GetResponse3(string ytLink)
        {
            try
            {
                HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/listAudioFormats?videoUrl={ytLink}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Response = ex.Message;
                return ex.Message;
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetVideoById([FromBody] VDInfo data)
        {
            try
            {
                HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/downloadByVideoID?videoUrl={data.ytLink}&vQualityID={data.selectedValue}");
                var data1 = await response.Content.ReadAsStringAsync();

                // get 9th
                string asdasdas = "[download] Destination: /app/downloads/";
                int index = data1.IndexOf(asdasdas);

                if (index != -1)
                {
                    // Find the index of the substring starting from startIndex
                    int index1 = data1.IndexOf('\n', index);
                    string ggg = NAME = data1[(index + asdasdas.Length)..index1];
                    Console.WriteLine(ggg);


                    /////////////
                    HttpClient client2 = new();
                    using HttpResponseMessage response3 = await client.GetAsync($"http://ytdlp:8080/api/upload?videoName={NAME}");
                    var data3 = await response3.Content.ReadAsStringAsync();
                    /////////////



                    using HttpResponseMessage response1 = await client.GetAsync($"http://ytdlp:8080/api/upload?videoName={NAME}");
                    await response1.Content.ReadAsStringAsync();

                    // Get the current working directory
                    string projectDirectory = Directory.GetCurrentDirectory();

                    // Output directory for downloads (e.g., "downloads" folder)
                    string outputDirectory = Path.Combine(projectDirectory, "downloads");

                    // Ensure the output directory exists
                    Directory.CreateDirectory(outputDirectory);

                    // Set the video name and file path
                    string videoName = NAME; // Change this to your desired filename
                    string filePath = Path.Combine(outputDirectory, videoName);


                    // Path to the yt-dlp binary
                    string ytDlpPath = "curl"; // Modify if necessary 

                    // Command to run yt-dlp and retrieve metadata
                    string ytDlpArguments = $"-X GET {response.Content} -H \"accept: application/json\" --output \"{filePath}.mp4\"";

                    // Create the process to execute the command
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = ytDlpPath,
                        Arguments = ytDlpArguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Start the process
                    using (Process process = new Process { StartInfo = processStartInfo })
                    {
                        process.Start();

                        // Read the output from yt-dlp
                        string output = process.StandardOutput.ReadToEnd();
                        string errorOutput = process.StandardError.ReadToEnd();

                        process.WaitForExit();

                        // Print the outputs
                        Console.WriteLine("Output:\n" + output);
                        if (!string.IsNullOrEmpty(errorOutput))
                        {
                            Console.WriteLine("Error:\n" + errorOutput);
                        }
                    }

                }




                //Console.WriteLine(data1);
                return Ok();
            }
            catch (Exception ex)
            {
                ViewBag.Response = ex.Message;
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAudioById([FromBody] VDInfo data)
        {
            try
            {
                HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/downloadByAudioID?videoUrl={data.ytLink}&aQualityID={data.selectedValue}");
                await response.Content.ReadAsStringAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                ViewBag.Response = ex.Message;
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadFile([FromBody] string fileName)
        {
            try
            {


                HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync($"http://ytdlp:8080/api/upload?videoName={fileName}");
                await response.Content.ReadAsStringAsync();

                // Get the current working directory
                string projectDirectory = Directory.GetCurrentDirectory();

                // Output directory for downloads (e.g., "downloads" folder)
                string outputDirectory = Path.Combine(projectDirectory, "downloads");

                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Set the video name and file path
                string videoName = fileName; // Change this to your desired filename
                string filePath = Path.Combine(outputDirectory, videoName);


                // Path to the yt-dlp binary
                string ytDlpPath = "curl"; // Modify if necessary 

                // Command to run yt-dlp and retrieve metadata
                string ytDlpArguments = $"-X GET {response.Content} -H \"accept: application/json\" --output \"{filePath}.mp4\"";

                // Create the process to execute the command
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = ytDlpPath,
                    Arguments = ytDlpArguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the process
                using (Process process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();

                    // Read the output from yt-dlp
                    string output = process.StandardOutput.ReadToEnd();
                    string errorOutput = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    // Print the outputs
                    Console.WriteLine("Output:\n" + output);
                    if (!string.IsNullOrEmpty(errorOutput))
                    {
                        Console.WriteLine("Error:\n" + errorOutput);
                    }
                }

                //"curl - X GET \"https://file.io/9tC2edSTmXMY\" - H \"accept: application/json\"--output filename.ext";
                return Ok();
            }
            catch (Exception ex)
            {
                ViewBag.Response = ex.Message;
                return BadRequest();
            }



        }
    }
}
