using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Mvc;
using SharpCompress.Common;
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
        private string oldId = "";

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
            ViewBag.type = "Video";
            return PartialView("_FormatsData", videoFormats);
        }
        [HttpPost]
        public async Task<PartialViewResult> GetAudioFormats([FromBody] string ytLink)
        {
            string response = await UtilityService.GetResponse(EndpointType.Audio_Formats, ytLink);
            var audioFormats = JsonSerializer.Deserialize<AudioData[]>(response);
            ViewBag.type = "Audio";
            return PartialView("_FormatsData", audioFormats);
        }
        [HttpPost]
        public async Task<PartialViewResult> Get2BestFormats([FromBody] string ytLink)
        {
            string response = await UtilityService.GetResponse(EndpointType.Best_Formats, ytLink);
            Console.WriteLine(response);
            var formats = JsonSerializer.Deserialize<BestFormats>(response);
            Console.WriteLine(formats);
            return PartialView("_FormatsData", formats);
        }
        [HttpPost]
        public async Task<IActionResult> DownloadFileById([FromBody] DownloadRequest request)
        {
            string response = "nope";
            if (oldId != request.Id)
            {
                oldId = request.Id;
                response = await UtilityService.GetResponseDownload(EndpointType.General_Data, request.YtLink, request.Id);
            }
            return Json(new { response });
        }

        public class DownloadRequest
        {
            public string Id { get; set; }
            public string YtLink { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GetBytesFromFile([FromBody] string fileID)
        {
            var fileData = await mongoService.GetByteArrayFromFile(fileID);
            var base64String = System.Convert.ToBase64String(fileData.Data);
            Console.WriteLine("sucess");

            return Json(new
            {
                bytes = base64String,
                contentType = fileData.Type
            });
        }

        // FFMPEG TIME BOIS
        [HttpPost]
        public async Task<IActionResult> GetAudioInformation([FromBody] string fileID)
        {
            string response = await UtilityService.GetResponseAudioInfo(fileID);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetAudioVisual([FromBody] string fileID)
        {
            Console.WriteLine("GetAudioVisual FILEID:" + fileID);
            // get the file id of uploaded image (Audio waves)
            string response = await UtilityService.GetResponseImageName(fileID);
            Console.WriteLine("GetAudioVisual RESPONSE ID:" + response);

            // get byte array from that file 
            var fileData = await mongoService.GetByteArrayFromFileNEW(response);

            // create the base64string
            var base64String = System.Convert.ToBase64String(fileData.Data);
            Console.WriteLine("sucess");

            return Json(new
            {
                bytes = base64String,
                contentType = fileData.Type
            });
        }
        [HttpPost]
        public async Task<IActionResult> GetAudioVisual2([FromBody] string fileID)
        {
            Console.WriteLine("GetAudioVisual2 FILEID:" + fileID);
            // get the file id of uploaded image (Audio waves)
            string response = await UtilityService.GetResponseImageName(fileID);
            Console.WriteLine("GetAudioVisual2 RESPONSE ID:" + response);

            // get byte array from that file 
            var fileData = await mongoService.GetByteArrayFromFileSimple(response);

            // create the base64string
            var base64String = System.Convert.ToBase64String(fileData);
            Console.WriteLine("sucess");

            return Json(new { bytes = base64String });
        }
        [HttpPost]
        public async Task<IActionResult> ApplyEffects([FromBody] Effects effects)
        {
            //Console.WriteLine("ApplyEffects id:" + effects.id);
            //Console.WriteLine("ApplyEffects volume:" + effects.volume);
            //Console.WriteLine("ApplyEffects speed:" + effects.speed);
            //Console.WriteLine("ApplyEffects formar:" + effects.format);
            //Console.WriteLine("ApplyEffects isnorm:" + effects.isNorm);

            // get the audio file from DB, download it locally on MSFFMpeg
            // apply effects to it, save it locally, then upload it to the DB
            // GetResponseEffects returns id of that file, saved in DB
            string response = await UtilityService.GetResponseEffects(effects);
            Console.WriteLine("GetAudioVisual RESPONSE ID:" + response);

            // ugh...
            // now need to get the audio source
            // and the audiowave image 
            // will be better if this method only gets the file, or not? hm
            // alr
            // pass the converted file id obtained previously
            var fileData = await mongoService.GetByteArrayFromFileSimple(response);
            var base64String = System.Convert.ToBase64String(fileData);
            Console.WriteLine("sucess");

            return Json(new
            {
                bytes = base64String,
                contentType = "audio/mp3",
                id = response
            });
        }
    }
}