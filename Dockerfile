# Используйте официальный образ Eclipse Temurin JRE
FROM docker.io/eclipse-temurin:17.0.7_7-jre

# Установите рабочую директорию в контейнере
WORKDIR /app

# Install Python3 and yt-dlp in the base image
USER root
RUN apt-get update
RUN apt-get install -y python3 pip
RUN pip install yt-dlp --break-system-packages

# Скопируйте JAR файл приложения в рабочую директорию
COPY target/MS_YT_DLP-0.0.1-SNAPSHOT.jar /app/MS_YT_DLP-0.0.1-SNAPSHOT.jar

# Запустите приложение
CMD ["java", "-XX:+UseG1GC", "-jar", "MS_YT_DLP-0.0.1-SNAPSHOT.jar"]
