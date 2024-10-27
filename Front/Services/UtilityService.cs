using Front.Models;
using SharpCompress.Common;

namespace Front.Services
{
    public enum EndpointType
    {
        General_Data,
        Video_Formats,
        Audio_Formats,
        Best_Formats
    }

    public struct FileData
    {
        public string Type;
        public byte[] Data;
        public FileData(string type, byte[] data)
        {
            Type = type;
            Data = data;
        }
    }

    public static class UtilityService
    {
        private const string baseYtdlpURI = "http://ytdlp:8080/api";
        private const string baseFFmpegURI = "http://ffmpeg:8080/api";
        private static HttpClient client = new();

        public static async Task<string> GetResponse(EndpointType endpointType, string ytLink)
        {
            string endpoint = endpointType switch
            {
                EndpointType.General_Data => "endpoint?videoUrl=",
                EndpointType.Video_Formats => "listVideoFormats?videoUrl=",
                EndpointType.Audio_Formats => "listAudioFormats?videoUrl=",
                EndpointType.Best_Formats => "listBestFormats?videoUrl=",
                _ => "error",
            };
            try
            {
                using HttpResponseMessage response = await client.GetAsync($"{baseYtdlpURI}/{endpoint}{ytLink}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static async Task<string> GetResponseDownload(EndpointType endpointType, string ytLink, string id)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync($"{baseYtdlpURI}/downloadFileByID?videoUrl={ytLink}&qID={id}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static async Task<string> GetResponseAudioInfo(string id)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync($"{baseFFmpegURI}/audioInfo/{id}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static async Task<string> GetResponseImageName(string id)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync($"{baseFFmpegURI}/audioWaves/{id}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static async Task<string> GetResponseEffects(Effects effects)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync($"{baseFFmpegURI}/applyEffects/{effects.id}?speed={effects.speed}&volume={effects.volume}&isNorm={effects.isNorm}&format={effects.format}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}