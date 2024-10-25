namespace Front.Services
{
    public enum EndpointType
    {
        General_Data,
        Video_Formats,
        Audio_Formats,
    }

    public static class UtilityService
    {
        public static async Task<string> GetResponse(EndpointType endpointType, string ytLink)
        {
            const string basicURI = "http://ytdlp:8080/api";
            string endpoint = endpointType switch
            {
                EndpointType.General_Data => "endpoint?videoUrl=",
                EndpointType.Video_Formats => "listVideoFormats?videoUrl=",
                EndpointType.Audio_Formats => "listAudioFormats?videoUrl=",
                _ => "error",
            };
            try
            {
                HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync($"{basicURI}{endpoint}{ytLink}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}