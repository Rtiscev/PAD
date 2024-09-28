package Controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.util.Map;

@RestController
@RequestMapping("/api")
public class DownloadController {

    @PostMapping("/download")
    public ResponseEntity<String> downloadVideo(@RequestBody Map<String, String> request) {
        String ytLink = request.get("yt_link");

        // Укажите путь для сохранения видео
        String downloadPath = "D:\\\\";

        try {
            // Команда для загрузки видео через yt-dlp
            String command = "D:\\\\Present\\\\PAD\\\\MS_YT_DLP\\\\tools\\\\yt-dlp -o " + downloadPath + "%(title)s.%(ext)s " + ytLink;

            // Запуск процесса в терминале
            ProcessBuilder processBuilder = new ProcessBuilder(command.split(" "));
            processBuilder.redirectErrorStream(true); // объединяем стандартный вывод и ошибки
            Process process = processBuilder.start();

            // Чтение вывода процесса
            BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()));
            StringBuilder output = new StringBuilder();
            String line;
            while ((line = reader.readLine()) != null) {
                output.append(line).append("\n");
            }

            int exitCode = process.waitFor();
            if (exitCode == 0) {
                return ResponseEntity.ok("Download successful\n" + output.toString());
            } else {
                return ResponseEntity.status(500).body("Download failed with code " + exitCode + "\n" + output.toString());
            }

        } catch (Exception e) {
            return ResponseEntity.status(500).body("Error: " + e.getMessage());
        }
    }
}



