using Front.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using SharpCompress.Common;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Front.Controllers
{
    public class HomeController : Controller
    {
        IMongoCollection<Restaurant> _restaurantsCollection;
        string MongoConnectionString = "mongodb://root:example@mongo:27017/";
        private ILogger<HomeController> _logger;
        //private IMongoDatabase _database;
        //private GridFSBucket _gridFS;


        private string NAME;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //Task.Run(() =>
            //{
            //    ConfigureDatabase();
            //});

            string binPath = AppContext.BaseDirectory;
            Console.WriteLine("Path is:" + binPath);

            // List directories
            string[] directories = Directory.GetDirectories(binPath);
            Console.WriteLine("Directories:");
            foreach (string directory in directories)
            {
                Console.WriteLine(directory);
            }

            return View();
        }

        private async Task ConfigureDatabase()
        {
            try
            {
                Setup();

                Console.WriteLine("Inserting a document...");
                await InsertOneRestaurantAsync();

                // Creates a filter for all documents that have a "name" value of "Mongo's Pizza"
                var filter = Builders<Restaurant>.Filter
                    .Eq(r => r.Name, "Mongo's Pizza");

                // Finds the newly inserted document by using the filter
                var document = _restaurantsCollection.Find(filter).FirstOrDefault();

                // Prints the document
                Console.WriteLine($"Document Inserted: {document.ToBsonDocument()}");

                Cleanup();

                await CreateBigAssFile();

                var database = new MongoClient(MongoConnectionString).GetDatabase("sample_restaurants");

                var fs = new GridFSBucket(database);

                var id = await TryUploading(fs);

                Console.WriteLine("ID IS: " + id);
                // Prints a message if any exceptions occur during the operation    
            }
            catch (Exception me)
            {
                Console.WriteLine("Unable to insert due to an error: " + me);
            }

        }

        private async Task CreateBigAssFile()
        {
            string filePath = "randomContent.txt";
            long targetFileSizeMB = 30;
            long targetFileSizeBytes = targetFileSizeMB * 1024 * 1024; // 30 MB in bytes

            // Create a StreamWriter to write into the file
            using (StreamWriter writer = new StreamWriter(Path.Combine(AppContext.BaseDirectory, filePath), false, Encoding.UTF8))
            {
                Random random = new Random();
                StringBuilder sb = new StringBuilder();

                // Create random content in chunks
                while (new FileInfo(filePath).Length < targetFileSizeBytes)
                {
                    sb.Clear();
                    for (int i = 0; i < 1000; i++) // Adjust 1000 to generate more/less data in each iteration
                    {
                        char randomChar = (char)random.Next(32, 126); // Printable ASCII characters
                        sb.Append(randomChar);
                    }

                    // Write the generated content to the file
                    writer.Write(sb.ToString());
                }
            }

            Console.WriteLine("File generated successfully with size exceeding 30 MB.");
        }

        private async Task<ObjectId> TryUploading(GridFSBucket fs)
        {
            using (var s = System.IO.File.OpenRead(Path.Combine(AppContext.BaseDirectory, "randomContent.txt")))
            {
                var t = Task.Run<ObjectId>(() =>
                {
                    return fs.UploadFromStreamAsync("test.txt", s);
                });

                return t.Result;
            }
        }

        private async Task InsertOneRestaurantAsync()
        {
            Cleanup();

            // start-insert-one-async
            // Generates a new restaurant document
            Restaurant newRestaurant = new()
            {
                Name = "Mongo's Pizza",
                RestaurantId = "12345",
                Cuisine = "Pizza",
                Address = new()
                {
                    Street = "Pizza St",
                    ZipCode = "10003"
                },
                Borough = "Manhattan",
            };

            // Asynchronously inserts the new document into the restaurants collection
            await _restaurantsCollection.InsertOneAsync(newRestaurant);
            // end-insert-one-async
        }

        private void Setup()
        {
            // Allows automapping of the camelCase database fields to models
            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

            // Establishes the connection to MongoDB and accesses the restaurants database
            var mongoClient = new MongoClient(MongoConnectionString);
            var restaurantsDatabase = mongoClient.GetDatabase("sample_restaurants");
            _restaurantsCollection = restaurantsDatabase.GetCollection<Restaurant>("restaurants");
        }

        private void Cleanup()
        {
            var filter = Builders<Restaurant>.Filter
                .Eq(r => r.Name, "Mongo's Pizza");

            _restaurantsCollection.DeleteOne(filter);
        }

        public class Restaurant
        {
            public ObjectId Id { get; set; }

            public string Name { get; set; }

            [BsonElement("restaurant_id")]
            public string RestaurantId { get; set; }

            public string Cuisine { get; set; }

            public Address Address { get; set; }

            public string Borough { get; set; }

            public List<GradeEntry> Grades { get; set; }
        }

        public class Address
        {
            public string Building { get; set; }

            [BsonElement("coord")]
            public double[] Coordinates { get; set; }

            public string Street { get; set; }

            [BsonElement("zipcode")]
            public string ZipCode { get; set; }
        }

        public class GradeEntry
        {
            public DateTime Date { get; set; }

            public string Grade { get; set; }

            public float? Score { get; set; }
        }










        [HttpPost]
        public async Task<PartialViewResult> GetGeneralData([FromBody] string ytLink)
        {
            string response = await GetResponse(ytLink);
            GeneralData? generalData = JsonSerializer.Deserialize<GeneralData>(response);
            return PartialView("_YoutubeData", generalData);
        }

        [HttpPost]
        public async Task<PartialViewResult> GetVideoFormats([FromBody] string ytLink)
        {
            string response = await GetResponse2(ytLink);
            var videoFormats = JsonSerializer.Deserialize<VideoData[]>(response);
            ViewBag.type = "Video";
            return PartialView("_FormatsData", videoFormats);
        }

        [HttpPost]
        public async Task<PartialViewResult> GetAudioFormats([FromBody] string ytLink)
        {
            string response = await GetResponse3(ytLink);
            var audioFormats = JsonSerializer.Deserialize<AudioData[]>(response);
            ViewBag.type = "Audio";
            return PartialView("_FormatsData", audioFormats);
        }

        [HttpPost]
        public async Task<PartialViewResult> Get2BestFormats([FromBody] string ytLink)
        {
            List<AudioData> formats = new();

            return PartialView("_FormatsData", formats);

        }



        //// Method to upload a large file
        //public async Task<ObjectId> UploadFileAsync(string filePath)
        //{
        //    using (var stream = System.IO.File.OpenRead(filePath))
        //    {
        //        // Upload file to GridFS
        //        ObjectId fileId = await _gridFS.UploadFromStreamAsync(Path.GetFileName(filePath), stream);
        //        Console.WriteLine($"File uploaded with Id: {fileId}");
        //        return fileId;
        //    }
        //}

        //// Method to download a large file
        //public async Task DownloadFileAsync(ObjectId fileId, string outputFilePath)
        //{
        //    using (var stream = System.IO.File.Create(outputFilePath))
        //    {
        //        // Download file from GridFS
        //        await _gridFS.DownloadToStreamAsync(fileId, stream);
        //        Console.WriteLine($"File downloaded to: {outputFilePath}");
        //    }
        //}

        //// Method to list all stored files in GridFS
        //public async Task ListFilesAsync()
        //{
        //    using (var cursor = await _gridFS.FindAsync(Builders<GridFSFileInfo>.Filter.Empty))
        //    {
        //        foreach (var fileInfo in await cursor.ToListAsync())
        //        {
        //            Console.WriteLine($"File: {fileInfo.Filename}, Id: {fileInfo.Id}");
        //        }
        //    }
        //}


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
