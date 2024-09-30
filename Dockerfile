# Используйте официальный образ Eclipse Temurin JRE
FROM docker.io/eclipse-temurin:17.0.7_7-jre

# Установите рабочую директорию в контейнере
WORKDIR /app

# Установите необходимые зависимости
USER root
RUN apt-get update && apt-get install -y python3 python3-pip \
    && pip install --upgrade pip setuptools wheel \
    && pip install yt-dlp

# Создайте директорию для загрузок
RUN mkdir -p /app/downloads

# Копируем весь проект в рабочую директорию контейнера
COPY . .

# Скопируйте JAR файл приложения в рабочую директорию
COPY target/MS_YT_DLP-0.0.1-SNAPSHOT.jar /app/MS_YT_DLP-0.0.1-SNAPSHOT.jar

# Установите переменные окружения для пути загрузки
ENV DOWNLOAD_DIR=/app/downloads

# Запустите приложение
CMD ["java", "-XX:+UseG1GC", "-jar", "MS_YT_DLP-0.0.1-SNAPSHOT.jar"]
