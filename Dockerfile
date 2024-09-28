# Используйте официальный образ Eclipse Temurin JRE
FROM docker.io/eclipse-temurin:17.0.7_7-jre

# Установите рабочую директорию в контейнере
WORKDIR /app

# Обновите пакеты и установите Python
RUN apt-get update && \
    apt-get install -y python3 python3-pip

# Проверьте, установлен ли Python, и выведите версию
RUN python3 --version
RUN which python3

# Скопируйте двоичный файл yt-dlp в /app/tools и проверьте, существует ли он
COPY tools/yt-dlp /app/tools/yt-dlp
RUN echo "yt-dlp скопирован в /app/tools. Проверка наличия:" && ls -al /app/tools/yt-dlp

# Проверьте разрешения файла yt-dlp
RUN echo "Проверка разрешений yt-dlp:" && ls -l /app/tools/yt-dlp

# Установите правильные разрешения, чтобы сделать yt-dlp исполняемым
RUN chmod a+rx /app/tools/yt-dlp

# Вставьте код на C# как комментарий
# var ytDlpPath = "/app/tools/yt-dlp";
# string ytDlpPath = "tools/yt-dlp";
#
# // Это получит текущую рабочую директорию (т.е. \bin\Debug)
# string workingDirectory = Environment.CurrentDirectory;
# string conc = Path.Combine(workingDirectory, ytDlpPath);
#
# // Создайте новый процесс для запуска yt-dlp
# Process process = new Process();
# process.StartInfo.FileName = ytDlpPath;
#
# // Аргументы для передачи yt-dlp (в данном случае, URL видео)
# process.StartInfo.Arguments = videoUrl;
#
# // Перенаправьте стандартный вывод и ошибки, чтобы вы могли захватить вывод
# process.StartInfo.RedirectStandardOutput = true;
# process.StartInfo.RedirectStandardError = true;
#
# // Запустите процесс
# process.Start();
#
# // Прочитайте вывод от yt-dlp
# string output = process.StandardOutput.ReadToEnd();
# string error = process.StandardError.ReadToEnd();
#
# // Подождите, пока процесс завершится
# process.WaitForExit();

# Скопируйте JAR файл приложения в рабочую директорию
COPY target/MS_YT_DLP-0.0.1-SNAPSHOT.jar /app/MS_YT_DLP-0.0.1-SNAPSHOT.jar

# Откройте порт 8080 для внешнего доступа
EXPOSE 8080/tcp

# Запустите приложение
CMD ["java", "-XX:+UseG1GC", "-jar", "MS_YT_DLP-0.0.1-SNAPSHOT.jar"]
