package com.example.ms_yt_dlp.controller;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.web.bind.annotation.*;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("/api")
public class DownloadController {

    @GetMapping("/download") // Изменен на GET
    @ResponseBody
    public String downloadVideo(@RequestParam String videoUrl) throws IOException, InterruptedException {
        // Полный путь к бинарному файлу yt-dlp
        String ytDlpPath = "/usr/local/bin/yt-dlp"; // Измените это, если необходимо

        // Получаем текущую рабочую директорию (т.е. корень вашего проекта)
        String projectDirectory = System.getProperty("user.dir");

        // Указываем директорию для вывода (например, папка "downloads")
        Path outputDirectory = Paths.get(projectDirectory, "downloads");

        // Убедимся, что директория для вывода существует
        if (Files.notExists(outputDirectory)) {
            Files.createDirectory(outputDirectory);
        }

        // Создаем шаблон вывода для yt-dlp, сохраняем файлы в указанной папке
        String outputTemplate = System.getenv("DOWNLOAD_DIR") + "/%(title)s.%(ext)s";

        // Создаем команду для запуска yt-dlp
        ProcessBuilder processBuilder = new ProcessBuilder(ytDlpPath, "-o", outputTemplate, videoUrl);
        processBuilder.redirectErrorStream(true); // Перенаправляем ошибки в стандартный вывод

        // Запускаем процесс
        Process process = processBuilder.start();

        // Читаем вывод от yt-dlp
        StringBuilder output = new StringBuilder();
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()))) {
            String line;
            while ((line = reader.readLine()) != null) {
                output.append(line).append("\n");
            }
        }
        // Ждем завершения процесса
        int exitCode = process.waitFor();
        if (exitCode != 0) {
            return "oops";
        }

        // Возвращаем вывод команды
        return output.toString();
    }

    @GetMapping("/endpoint") // Изменен на GET
    @ResponseBody
    public String GetData(@RequestParam String videoUrl) throws IOException, InterruptedException {
        // Полный путь к бинарному файлу yt-dlp
        String ytDlpPath = "yt-dlp"; // Измените это, если необходимо

        // Получаем текущую рабочую директорию
        String projectDirectory = System.getProperty("user.dir");

        // Указываем директорию для вывода (например, папка "downloads")
        Path outputDirectory = Paths.get(projectDirectory, "downloads");

        // Убедимся, что директория для вывода существует
        if (Files.notExists(outputDirectory)) {
            Files.createDirectory(outputDirectory);
        }

        // Создаем шаблон вывода для yt-dlp
        String outputTemplate = outputDirectory.resolve("%(title)s.%(ext)s").toString();

        // Создаем команду для запуска yt-dlp и получения метаданных
        ProcessBuilder processBuilder = new ProcessBuilder(ytDlpPath, "--dump-json", videoUrl);
        processBuilder.redirectErrorStream(true); // Перенаправляем ошибки в стандартный вывод

        // Запускаем процесс
        Process process = processBuilder.start();

        // Читаем вывод от yt-dlp
        StringBuilder output = new StringBuilder();
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()))) {
            String line;
            while ((line = reader.readLine()) != null) {
                output.append(line).append("\n");
            }
        }

        // Ждем завершения процесса
        int exitCode = process.waitFor();
        if (exitCode != 0) {
            return "{\"error\": \"Failed to retrieve video metadata.\"}";
        }

        // Парсинг JSON-ответа
        String jsonResponse = output.toString().trim();
        ObjectMapper objectMapper = new ObjectMapper();
        Map<String, Object> metadata = objectMapper.readValue(jsonResponse, Map.class);

        // Создаем словарь для сериализации в JSON
        Map<String, Object> result = new HashMap<>();
        result.put("title", metadata.get("title"));
        result.put("thumbnail", metadata.get("thumbnail"));
        result.put("description", metadata.get("description"));
        result.put("view_count", metadata.get("view_count"));
        result.put("comment_count", metadata.get("comment_count"));
        result.put("like_count", metadata.get("like_count"));
        result.put("channel", metadata.get("channel"));
        result.put("upload_date", metadata.get("upload_date"));
        result.put("duration_string", metadata.get("duration_string"));

        // Сериализация результата в JSON
        String jsonResult = objectMapper.writeValueAsString(result);

        // Сохранение JSON в файл
        Path jsonFilePath = outputDirectory.resolve("video_metadata.json");
        Files.writeString(jsonFilePath, jsonResult);

        return jsonResult;
    }
}
