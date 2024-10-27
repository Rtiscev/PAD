using SharpCompress.Common;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace MS_FFMpeg.Services
{
    public class FFmpegService
    {
        private Process process;

        public FFmpegService()
        {
            process = new();
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
        }

        public string GetGeneralAudioData(string fileAbsPath)
        {
            process.StartInfo.Arguments = $"-hide_banner -loglevel info -i \"{fileAbsPath}\" ";
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            string substring = "At least one output file must be specified\n";
            if (error.Contains(substring))
                error = error.Replace(substring, "");
            if (!string.IsNullOrEmpty(error))
                return error;

            return output;
        }
        public string GetAudioWavePicID(string fileAbsPath)
        {
            string substringToRemove = "/app/downloads/";
            string fileName = GetOnlyFileName(fileAbsPath);

            process.StartInfo.Arguments = $"-i \"{fileAbsPath}\" -filter_complex \"showwavespic=s=1280x200\" -frames:v 1 \"{substringToRemove + fileName}.png\"";
            process.Start();
            process.WaitForExit();

            // need to check if the image is present
            string filePath = substringToRemove + $"{fileName}.png";
            Console.WriteLine(filePath);
            if (File.Exists(filePath))
                Console.WriteLine(filePath);
            else
                Console.WriteLine("GET ADUIO WAVE PIC FAIL");

            // say that the file has been uploaded or it exists there and give the name again
            return filePath;
        }
        public string ApplyEffects(string fileName, double speed, double volume, bool isNorm, string format)
        {
            string substringToRemove = "/app/downloads/";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(substringToRemove + fileName);

            Console.WriteLine("APPLY EFFECETS:" + fileName + " " + speed + " " + volume + " " + isNorm + " " + format);

            if (isNorm)
            {
                process.StartInfo.Arguments = $"-i \"{substringToRemove}{fileName}\" -filter_complex \"[0:a]atempo={speed},volume={volume},loudnorm[aout]\" -map \"[aout]\" \"{substringToRemove}{fileNameWithoutExtension}.{format}\"";
                Console.WriteLine($"-i \"{substringToRemove}{fileName}\" -filter_complex \"[0:a]atempo={speed},volume={volume},loudnorm[aout]\" -map \"[aout]\" \"{substringToRemove}{fileNameWithoutExtension}.{format}\"");
            }
            else
            {
                process.StartInfo.Arguments = $"-i \"{substringToRemove}{fileName}\" -filter_complex \"[0:a]atempo={speed},volume={volume}[aout]\" -map \"[aout]\" \"{substringToRemove}{fileNameWithoutExtension}.{format}\"";
                Console.WriteLine($"-i \"{substringToRemove}{fileName}\" -filter_complex \"[0:a]atempo={speed},volume={volume}[aout]\" -map \"[aout]\" \"{substringToRemove}{fileNameWithoutExtension}.{format}\"");
            }
            process.Start();
            process.WaitForExit();
            Console.WriteLine("Error:" + process.StandardError.ReadToEnd());
            Console.WriteLine($"{substringToRemove}{fileNameWithoutExtension}.{format}");

            // need to check if the image is present
            if (File.Exists($"{substringToRemove}{fileNameWithoutExtension}.{format}"))
                Console.WriteLine($"{substringToRemove}{fileNameWithoutExtension}.{format}");
            else
                Console.WriteLine("GET ADUIO WAVE PIC FAIL");

            // say that the file has been uploaded or it exists there and give the name again
            return $"{substringToRemove}{fileNameWithoutExtension}.{format}";
        }
        public string GetOnlyFileName(string fileAbsPath)
        {
            // get the extension type
            string extension = "";
            for (int i = fileAbsPath.Length - 1; fileAbsPath[i] != '.'; i--)
                extension += fileAbsPath[i];
            // reverse string because we were going from the end to the start
            // * mbew -> webm ...

            // reverse string
            char[] charArray = extension.ToCharArray();
            Array.Reverse(charArray);
            extension = new string(charArray);
            Console.WriteLine("GetAudioWavePicID extension:" + extension);

            // get the file name only
            string substringToRemove = "/app/downloads/";
            string fileName = fileAbsPath.Replace(substringToRemove, "");
            // remove dot as well since its not being added
            fileName = fileName.Replace($".{extension}", "");
            Console.WriteLine("GetAudioWavePicID filename:" + fileName);

            return fileName;
        }
    }
}
