package com.example.ms_yt_dlp.Controller;

import com.example.ms_yt_dlp.model.LoadFile;
import com.example.ms_yt_dlp.service.FileService;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.core.io.ByteArrayResource;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.*;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.*;


@RestController
@RequestMapping("/api")
public class DownloadController {


    private String processBuilder(ProcessBuilder processBuilder) {
        processBuilder.redirectErrorStream(true); // Redirect errors to standard output

        // Start the process
        Process process;
        try {
            process = processBuilder.start();
        } catch (IOException e) {
            throw new RuntimeException(e);
        }

        // Read the output from yt-dlp
        StringBuilder output = new StringBuilder();
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()))) {
            String line;
            while ((line = reader.readLine()) != null) {
                output.append(line).append("\n");
            }
        } catch (IOException e) {
            throw new RuntimeException(e);
        }

//        String key = "\"link\":\"";
//        int startIndex = output.indexOf(key) + key.length();
//        int endIndex = output.indexOf("\"", startIndex);
//
//        String link = output.substring(startIndex, endIndex);
//        System.out.println("Link: " + link);

        // Wait for the process to complete
        int exitCode;
        try {
            exitCode = process.waitFor();
        } catch (InterruptedException e) {
            throw new RuntimeException(e);
        }
        if (exitCode != 0) {
            return "oops";
        }

        return output.toString();
    }

    // Метод для создания карты формата для аудио
    private Map<String, Object> createFormatMap(AudioData audioData) {
        Map<String, Object> formatMap = new LinkedHashMap<>(); // Используем LinkedHashMap для сохранения порядка
        formatMap.put("id", audioData.getId());
        formatMap.put("ext", audioData.getExt());
        formatMap.put("tbr", audioData.getTBR());
        formatMap.put("fileSize", audioData.getFileSize());
        return formatMap;
    }


    // Метод для создания карты формата для видео
    private Map<String, Object> createFormatMap(Data videoData) {
        Map<String, Object> formatMap = new LinkedHashMap<>(); // Используем LinkedHashMap для сохранения порядка
        formatMap.put("id", videoData.getId());
        formatMap.put("ext", videoData.getExt());
        formatMap.put("rezolution", videoData.getResolution());
        formatMap.put("fps", videoData.getFps());
        formatMap.put("fileSize", videoData.getFileSize());
        return formatMap;
    }




    @GetMapping("/upload")
    @ResponseBody
    public String uploadFile(@RequestParam String videoName) throws IOException, InterruptedException {

        // Get the current working directory
        String projectDirectory = System.getProperty("user.dir");

        // Output directory for downloads (e.g., "downloads" folder)
        Path outputDirectory = Paths.get(projectDirectory, "downloads");

        Path filePath = Paths.get(outputDirectory.toString(), videoName);

        var asd = Files.exists(filePath);

        File fl = new File(filePath.toString());

        // Path to the yt-dlp binary
        String ytDlpPath = "curl"; // Modify if necessary


        // Create the command to run yt-dlp and retrieve metadata
        ProcessBuilder processBuilder = new ProcessBuilder(ytDlpPath, "-F", "file=@" + filePath, "https://file.io");
        processBuilder.redirectErrorStream(true); // Redirect errors to standard output

        // Start the process
        Process process = processBuilder.start();

        // Read the output from yt-dlp
        StringBuilder output = new StringBuilder();
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()))) {
            String line;
            while ((line = reader.readLine()) != null) {
                output.append(line).append("\n");
            }
        }

        // Find the index of the "link" key
        String key = "\"link\":\"";
        int startIndex = output.indexOf(key) + key.length();
        int endIndex = output.indexOf("\"", startIndex);

        // Extract the link value
        String link = output.substring(startIndex, endIndex);
        System.out.println("Link: " + link);

        // Wait for the process to complete
        int exitCode = process.waitFor();
        if (exitCode != 0) {
            return "{\"error\": \"Failed to retrieve video metadata.\"}";
        }

        return link;
    }

    @GetMapping("/download") // Изменен на GET
    @ResponseBody
    public String downloadVideo(@RequestParam String videoUrl) throws IOException, InterruptedException {
        // Полный путь к бинарному файлу yt-dlp
        String ytDlpPath = "yt-dlp"; // Измените это, если необходимо

        // Получаем текущую рабочую директорию (т.е. корень вашего проекта)
        String projectDirectory = System.getProperty("user.dir");

        // Указываем директорию для вывода (например, папка "downloads")
        Path outputDirectory = Paths.get(projectDirectory, "downloads");

        // Убедимся, что директория для вывода существует
        if (Files.notExists(outputDirectory)) {
            Files.createDirectory(outputDirectory);
        }

        // Создаем шаблон вывода для yt-dlp, сохраняем файлы в указанной папке
        String outputTemplate = outputDirectory + "/%(title)s.%(ext)s";

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
        // Path to the yt-dlp binary
        String ytDlpPath = "yt-dlp"; // Modify if necessary

        // Get the current working directory
        String projectDirectory = System.getProperty("user.dir");

        // Output directory for downloads (e.g., "downloads" folder)
        Path outputDirectory = Paths.get(projectDirectory, "downloads");

        // Ensure the output directory exists
        if (Files.notExists(outputDirectory)) {
            Files.createDirectory(outputDirectory);
        }

        // Create the command to run yt-dlp and retrieve metadata
        ProcessBuilder processBuilder = new ProcessBuilder(ytDlpPath, "--dump-json", videoUrl);
        processBuilder.redirectErrorStream(true); // Redirect errors to standard output

        // Start the process
        Process process = processBuilder.start();

        // Read the output from yt-dlp
        StringBuilder output = new StringBuilder();
        try (BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()))) {
            String line;
            while ((line = reader.readLine()) != null) {
                output.append(line).append("\n");
            }
        }

        // Wait for the process to complete
        int exitCode = process.waitFor();
        if (exitCode != 0) {
            return "{\"error\": \"Failed to retrieve video metadata.\"}";
        }

        // Extract JSON part from the output
        String jsonResponse = output.toString().trim();
        int jsonStartIndex = jsonResponse.indexOf("{"); // Find the first '{' character
        if (jsonStartIndex == -1) {
            return "{\"error\": \"No valid JSON output found.\"}";
        }

        // Parse only the valid JSON part
        jsonResponse = jsonResponse.substring(jsonStartIndex);

        ObjectMapper objectMapper = new ObjectMapper();
        Map<String, Object> metadata = objectMapper.readValue(jsonResponse, Map.class);

        Map<String, Object> result = new HashMap<>();
        try {
            // Create a result map for serialization into JSON
            result.put("title", metadata.get("title"));
            result.put("thumbnail", metadata.get("thumbnail"));
            result.put("description", metadata.get("description"));
            result.put("view_count", metadata.get("view_count"));
            result.put("comment_count", metadata.get("comment_count"));
            result.put("like_count", metadata.get("like_count"));
            result.put("channel", metadata.get("channel"));
            result.put("upload_date", metadata.get("upload_date"));
            result.put("duration_string", metadata.get("duration_string"));
        } catch (Exception e) {
            result.put("error", e.getMessage());
        }

        // Serialize the result to JSON
        String jsonResult = objectMapper.writeValueAsString(result);

        // Save the JSON result to a file
        Path jsonFilePath = outputDirectory.resolve("video_metadata.json");
        Files.writeString(jsonFilePath, jsonResult);

        return jsonResult;
    }

//    @GetMapping("/listVideoFormats") // Изменен на GET
//    @ResponseBody
//    public String GetVideoFormats(@RequestParam String videoUrl) throws IOException, InterruptedException {
//
//        int asd;
//        // Полный путь к бинарному файлу yt-dlp
//        String ytDlpPath = "yt-dlp"; // Измените это, если необходимо
//
//        // Получаем текущую рабочую директорию (т.е. корень вашего проекта)
//        String projectDirectory = System.getProperty("user.dir");
//
//        // Создаем команду для запуска yt-dlp
//        ProcessBuilder processBuilder = new ProcessBuilder(ytDlpPath, "-F", videoUrl);
//        processBuilder.redirectErrorStream(true); // Перенаправляем ошибки в стандартный вывод
//
//        // Запускаем процесс
//        Process process = processBuilder.start();
//
//        // Читаем вывод от yt-dlp
//        StringBuilder output = new StringBuilder();
//        try (BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()))) {
//            String line;
//            while ((line = reader.readLine()) != null) {
//                output.append(line).append("\n");
//            }
//        }
//
//        String line;
//        ArrayList<String> _vs = new ArrayList<String>();
//        String vOnly = "video only";
//
//        Dictionary<String, Data> dic = new Hashtable<>();
//
//        try (BufferedReader reader = new BufferedReader(new StringReader(output.toString()))) {
//            while ((line = reader.readLine()) != null) {
//                if (line.contains(vOnly)) {
//                    _vs.add(line); // Add line for video only
//                }
//            }
//        } catch (IOException e) {
//            e.printStackTrace();
//        }
//
//        System.out.println("Videos:");
//        for (int i = _vs.size() - 1; i >= 0; i--) {
//            LinkedHashSet<String> ggg = new LinkedHashSet<>();
//            Data d = new Data();
//            String[] res = _vs.get(i).split(" ");
//
//            for (String v : res) {
//                if (v.equals("|") || v.equals("~") || v.equals("") || v.equals("2")) {
//                    continue;
//                } else {
//                    ggg.add(v);
//                }
//            }
//            try {
//                String[] copy = ggg.toArray(new String[0]);
//                d.id = copy[0];
//                d.ext = copy[1];
//                d.resolution = copy[2];
//                d.fps = copy[3];
//                d.fileSize = copy[4];
//
//                if (d.ext.equals("webm")) continue;
//
//                String key = d.resolution + d.fps;
//
//                if (dic.get(key) == null) {
//                    dic.put(key, d);
//                }
//            } catch (Exception ex) {
//                // Handle exception if needed
//            }
//        }
//
//
//        // Convert Dictionary to List<Data>
//        List<Data> dataList = new ArrayList<>();
//        Enumeration<String> keys = dic.keys();
//        while (keys.hasMoreElements()) {
//            String key = keys.nextElement();
//            dataList.add(dic.get(key)); // Add the Data objects to the list
//        }
//
//        // Convert List<Data> to JSON using ObjectMapper
//        try {
//            ObjectMapper objectMapper = new ObjectMapper();
//            String json = objectMapper.writerWithDefaultPrettyPrinter().writeValueAsString(dataList);
//
//            // Print the JSON string
//            return json;
////            System.out.println(json);
//        } catch (Exception e) {
//            e.printStackTrace();
//        }
//        return "eh";
//    }

    @GetMapping("/listAudioFormats") // Изменен на GET
    @ResponseBody
    public String GetAudioFormats(@RequestParam String videoUrl) throws IOException, InterruptedException {

        // Полный путь к бинарному файлу yt-dlp
        String ytDlpPath = "yt-dlp"; // Измените это, если необходимо

        // Получаем текущую рабочую директорию (т.е. корень вашего проекта)
        String projectDirectory = System.getProperty("user.dir");

        // Создаем команду для запуска yt-dlp
        ProcessBuilder processBuilder = new ProcessBuilder(ytDlpPath, "-F", videoUrl);
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

        String line;
        ArrayList<String> _vs = new ArrayList<String>();
        String vOnly = "audio only";

        Dictionary<String, Data> dic = new Hashtable<>();

        try (BufferedReader reader = new BufferedReader(new StringReader(output.toString()))) {
            while ((line = reader.readLine()) != null) {
                if (line.contains(vOnly)) {
                    _vs.add(line); // Add line for video only
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        }


//        System.out.println("Videos:");
        ArrayList<AudioData> asdfgew = new ArrayList<AudioData>();

        for (int i = _vs.size() - 1; i >= 0; i--) {
            LinkedHashSet<String> ggg = new LinkedHashSet<>();
            Data d = new Data();
            String[] res = _vs.get(i).split(" ");

            for (String v : res) {
                if (v.equals("|") || v.equals("audio") || v.equals("only") || v.equals("~") || v.equals("") || v.equals("2")) {
                    continue;
                } else {
                    ggg.add(v);
                }
            }
            try {
                String[] copy = ggg.toArray(new String[0]);

                boolean isOk = true;
                for (int j = 0; j < copy.length; j++) {
                    if (copy[j].equals("unknown")) {
                        isOk = false;
                        break;
                    }
                }
                if (isOk) {
                    AudioData a2 = new AudioData();
                    a2.id = copy[0];
                    a2.ext = copy[1];
                    a2.fileSize = copy[2];
                    a2.tbr = copy[3];
                    asdfgew.add(a2);
                }
            } catch (Exception ex) {
                // Handle exception if needed
            }
        }


        // Convert List<Data> to JSON using ObjectMapper
        try {
            ObjectMapper objectMapper = new ObjectMapper();
            String json = objectMapper.writerWithDefaultPrettyPrinter().writeValueAsString(asdfgew);

            // Print the JSON string
            return json;
//            System.out.println(json);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return "eh";
    }

    @GetMapping("/downloadFileByID") // Изменен на GET
    @ResponseBody
    public String GetOnlyFileById(@RequestParam String videoUrl, @RequestParam String qID) throws IOException, InterruptedException {
        // Полный путь к бинарному файлу yt-dlp
        String ytDlpPath = "yt-dlp"; // Измените это, если необходимо

        // Получаем текущую рабочую директорию (т.е. корень вашего проекта)
        String projectDirectory = System.getProperty("user.dir");

        // Указываем директорию для вывода (например, папка "downloads")
        Path outputDirectory = Paths.get(projectDirectory, "downloads");

        // ------- CHECK FILES THAT ALREADY EXIST
        ArrayList<File> files = new ArrayList<File>();
        File folder = new File(String.valueOf(outputDirectory));
        File[] listOfFiles = folder.listFiles();

        if (listOfFiles != null) {
            for (File file : listOfFiles) {
                if (file.isFile()) {
                    files.add(file);
                }
            }
        }

        // Убедимся, что директория для вывода существует
        if (Files.notExists(outputDirectory)) {
            Files.createDirectory(outputDirectory);
        }

        // Создаем шаблон вывода для yt-dlp, сохраняем файлы в указанной папке
        String outputTemplate = outputDirectory + "/%(title)s.%(ext)s";

        // Создаем команду для запуска yt-dlp
        ProcessBuilder processBuilder = new ProcessBuilder(ytDlpPath, "-f",qID, "-o", outputTemplate, videoUrl);
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
            return "yt-dlp fail";
        }

        // ------- CHECK FILES THAT ALREADY EXIST
        ArrayList<File> files2 = new ArrayList<File>();
        File[] listOfFiles2 = folder.listFiles();

        for (File file : listOfFiles2) {
            if (file.isFile()) {
                files2.add(file);
            }
        }

        HashSet<Object> set2 = new HashSet<>(Arrays.asList(files2.toArray()));
        Set<Object> difference = new HashSet<>(set2);
        if (!files.isEmpty()) {
            // Convert arrays to sets
            HashSet<Object> set1 = new HashSet<>(Arrays.asList(files.toArray()));

            // Difference (set1 - set2)
            difference.removeAll(set1);
        }

        String result;
        if (difference.isEmpty()) {
            result = "file exists!";
        } else {
            File found = (File) difference.stream().findFirst().get();
            String ID = fileService.addSimpleFile(found.getAbsolutePath());
            // Возвращаем вывод команды
            result = ID;
//            result = found.getAbsolutePath() + " " + ID;
        }
        return result;

        // Возвращаем вывод команды
//        return output.toString();
    }

    @GetMapping("/downloadSimple") // Изменен на GET
    @ResponseBody
    public String DownloadSimple(@RequestParam String videoUrl) throws IOException, InterruptedException {

        // Полный путь к бинарному файлу yt-dlp
        String ytDlpPath = "yt-dlp"; // Измените это, если необходимо

        // Получаем текущую рабочую директорию (т.е. корень вашего проекта)
        String projectDirectory = System.getProperty("user.dir");

        // Указываем директорию для вывода (например, папка "downloads")
        Path outputDirectory = Paths.get(projectDirectory, "downloads");

        // ------- CHECK FILES THAT ALREADY EXIST
        ArrayList<File> files = new ArrayList<File>();
        File folder = new File(String.valueOf(outputDirectory));
        File[] listOfFiles = folder.listFiles();

        if (listOfFiles != null) {
            for (File file : listOfFiles) {
                if (file.isFile()) {
                    files.add(file);
                }
            }
        }


        // Убедимся, что директория для вывода существует
        if (Files.notExists(outputDirectory)) {
            Files.createDirectory(outputDirectory);
        }

        // Создаем шаблон вывода для yt-dlp, сохраняем файлы в указанной папке
        String outputTemplate = outputDirectory + "/%(title)s.%(ext)s";

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

        // ------- CHECK FILES THAT ALREADY EXIST
        ArrayList<File> files2 = new ArrayList<File>();
        File[] listOfFiles2 = folder.listFiles();

        for (File file : listOfFiles2) {
            if (file.isFile()) {
                files2.add(file);
            }
        }

        HashSet<Object> set2 = new HashSet<>(Arrays.asList(files2.toArray()));
        Set<Object> difference = new HashSet<>(set2);
        if (!files.isEmpty()) {
            // Convert arrays to sets
            HashSet<Object> set1 = new HashSet<>(Arrays.asList(files.toArray()));

            // Difference (set1 - set2)
            difference.removeAll(set1);
        }

        String result;
        if (difference.isEmpty()) {
            result = "file already exists!";
        } else {
            File found = (File) difference.stream().findFirst().get();
            String ID = fileService.addSimpleFile(found.getAbsolutePath());
            // Возвращаем вывод команды
            result = found.getAbsolutePath() + " " + ID;
        }
        return result;
    }

    @Autowired
    private FileService fileService;

    public String upload(MultipartFile file) throws IOException {
        return fileService.addFile(file);
    }

    @GetMapping("/downloadFileLocally") // Изменен на GET
    public ResponseEntity<ByteArrayResource> download(@RequestParam String id) throws IOException, InterruptedException {
        LoadFile loadFile = fileService.downloadFile(id);

        return ResponseEntity.ok()
                .contentType(MediaType.parseMediaType(loadFile.getFileType()))
                .header(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"" + loadFile.getFilename() + "\"")
                .body(new ByteArrayResource(loadFile.getFile()));
    }


    @GetMapping("/listBestFormats")
    @ResponseBody
    public ResponseEntity<String> getBestFormats(@RequestParam String videoUrl) {
        try {
            // Вызываем метод GetAudioFormats и получаем строку JSON
            String audioFormatsJson = GetAudioFormats(videoUrl);
            // Вызываем метод GetVideoFormats и получаем строку JSON
            String videoFormatsJson = GetVideoFormats(videoUrl);

            // Создаем ObjectMapper для работы с JSON
            ObjectMapper objectMapper = new ObjectMapper();

            // Десериализуем JSON строку в список объектов AudioData
            List<AudioData> audioDataList = objectMapper.readValue(
                    audioFormatsJson,
                    objectMapper.getTypeFactory().constructCollectionType(List.class, AudioData.class)
            );

            // Десериализуем JSON строку в список объектов VideoData
            List<Data> videoDataList = objectMapper.readValue(
                    videoFormatsJson,
                    objectMapper.getTypeFactory().constructCollectionType(List.class, Data.class)
            );

            // Проверяем, что в списке достаточно данных (не менее 2 форматов)
            if (audioDataList.size() < 2 || videoDataList.size() < 2) {
                return ResponseEntity.status(HttpStatus.BAD_REQUEST)
                        .body("{\"error\": \"Less than two audio or video formats found.\"}");
            }

            // Создаем результат в формате JSON
            Map<String, Object> resultMap = new LinkedHashMap<>();

            // Создаем списки для аудио и видео форматов
            List<Map<String, Object>> audioFormats = new ArrayList<>();
            List<Map<String, Object>> videoFormats = new ArrayList<>();

            // Добавляем лучшие аудиоформаты
            audioFormats.add(createFormatMap(audioDataList.get(0)));
            audioFormats.add(createFormatMap(audioDataList.get(1)));

            // Добавляем лучшие видеоформаты
            videoFormats.add(createFormatMap(videoDataList.get(0)));
            videoFormats.add(createFormatMap(videoDataList.get(1)));

            // Заполняем результат
            resultMap.put("audio", audioFormats);
            resultMap.put("video", videoFormats);

            // Преобразуем результат в JSON строку
            String resultJson = objectMapper.writeValueAsString(resultMap);

            // Возвращаем результат как JSON
            return ResponseEntity.ok()
                    .contentType(MediaType.APPLICATION_JSON)
                    .body(resultJson);

        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body("{\"error\": \"Error processing audio or video formats\"}");
        }
    }



    @GetMapping("/listVideoFormats")
    @ResponseBody
    public String GetVideoFormats(@RequestParam String videoUrl) {
        String ytDlpPath = "yt-dlp"; // Измените это, если необходимо
        ProcessBuilder processBuilder = new ProcessBuilder(ytDlpPath, "-F", videoUrl);
        processBuilder(processBuilder);

        ArrayList<String> _vs = new ArrayList<>();
        String vOnly = "video only";
        dictionary(processBuilder, _vs, vOnly);

        Map<String, Data> dic = new HashMap<>();

        for (int i = _vs.size() - 1; i >= 0; i--) {
            List<String> ggg = new ArrayList<>();
            Data d = new Data();
            String[] res = _vs.get(i).split(" ");

            for (String v : res) {
                if (!v.equals("|") && !v.equals("~") && !v.isEmpty() && !v.equals("2")) {
                    ggg.add(v);
                }
            }
            try {
                String[] copy = ggg.toArray(new String[0]);
                d.id = copy[0];
                d.ext = copy[1];
                d.resolution = copy[2];
                d.fps = copy[3];
                d.fileSize = copy[4];

                if (d.ext.equals("webm")) continue;

                String key = d.resolution + d.fps;

                if (dic.get(key) == null) {
                    dic.put(key, d);
                }
            } catch (Exception ex) {
                // Handle exception if needed
            }
        }

        // Convert Dictionary to List<Data>
        List<Data> dataList = new ArrayList<>(dic.values());

        // Sort dataList by resolution in descending order
        dataList.sort((data1, data2) -> {
            int res1 = parseResolution(data1.resolution);
            int res2 = parseResolution(data2.resolution);
            return Integer.compare(res2, res1); // Sort in descending order
        });

        // Convert List<Data> to JSON
        try {
            ObjectMapper objectMapper = new ObjectMapper();
            return objectMapper.writerWithDefaultPrettyPrinter().writeValueAsString(dataList);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return "eh";
    }

    // Helper method to parse resolution strings like "1080p" into integers
    private int parseResolution(String resolution) {
        try {
            return Integer.parseInt(resolution.replaceAll("[^0-9]", ""));
        } catch (NumberFormatException e) {
            return 0; // Fallback if resolution can't be parsed
        }
    }

    private void dictionary(ProcessBuilder processBuilder, ArrayList<String> _vs, String vOnly) {
        String line;
        try (BufferedReader reader = new BufferedReader(new StringReader(processBuilder(processBuilder)))) {
            while ((line = reader.readLine()) != null) {
                if (line.contains(vOnly)) {
                    _vs.add(line);
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

}