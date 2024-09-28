package test;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.util.Scanner;

class YtDlpConsoleApp {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        // Запрашиваем у пользователя URL-адрес YouTube
        System.out.print("Enter YouTube URL: ");
        String ytLink = scanner.nextLine();

        // Указываем путь для сохранения видео
        String downloadPath = "D:\\"; // Замените на нужный путь

        // Формируем команду
        String command = "yt-dlp -o \"" + downloadPath + "%(title)s.%(ext)s\" " + ytLink;

        try {
            // Создаем процесс с командой
            ProcessBuilder processBuilder = new ProcessBuilder(command.split(" "));
            processBuilder.redirectErrorStream(true); // Объединяем стандартный вывод и ошибки
            Process process = processBuilder.start();

            // Чтение вывода процесса
            BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()));
            String line;
            while ((line = reader.readLine()) != null) {
                System.out.println(line);
            }

            // Ожидаем завершения процесса
            int exitCode = process.waitFor();
            if (exitCode == 0) {
                System.out.println("Download successful");
            } else {
                System.err.println("Error: Download failed with exit code " + exitCode);
            }

        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            scanner.close();
        }
    }
}
