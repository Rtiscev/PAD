# Stage 1: Build the application with Maven
FROM eclipse-temurin:17-jdk as build
WORKDIR /app

# Install Maven (for Maven projects, skip if using Gradle)
RUN apt-get update && apt-get install -y maven

# Copy Maven project files
COPY pom.xml .
COPY src ./src

# Download dependencies and build the Spring Boot application
RUN mvn clean package -DskipTests

# Stage 2: Create the final image with Java and Python on Debian
FROM eclipse-temurin:17-jdk

# Install Python3 and pip3 using apt-get
RUN apt-get update \
    && apt-get install -y python3 python3-pip \
    && pip3 install yt-dlp  --break-system-packages

# Set the working directory in the container
WORKDIR /app

# Copy the JAR file from the build stage
COPY --from=build /app/target/*.jar app.jar

# Expose port 8080 (the default Spring Boot port)
EXPOSE 8080

# Set the command to run the Spring Boot application
ENTRYPOINT ["java", "-jar", "/app/app.jar"]


## Stage 1: Build the application with Maven
#FROM eclipse-temurin:17-jdk-alpine as build
#WORKDIR /app
#
## Install Maven (for Maven projects, skip if using Gradle)
#RUN apk add --no-cache maven
#
## Copy Maven project files
#COPY pom.xml .
#COPY src ./src
#
## Download dependencies and build the Spring Boot application
#RUN mvn clean package -DskipTests
#
## Stage 2: Create the final image with Java and Python
#FROM eclipse-temurin:17-jdk-alpine
#
## Install Python (using apk, the Alpine package manager)
#RUN apk add --no-cache python3 py3-pip
#
## Set the working directory in the container
#WORKDIR /app
#
## Install Python3 and yt-dlp in the base image
#USER root
#RUN apt-get update
#RUN apt-get install -y python3 pip
#RUN pip install yt-dlp --break-system-packages
#
#
## Copy the JAR file from the build stage
#COPY --from=build /app/target/*.jar app.jar
#
## Expose port 8080 (the default Spring Boot port)
#EXPOSE 8080
#
## Set the command to run the Spring Boot application
#ENTRYPOINT ["java", "-jar", "/app/app.jar"]



## Используйте официальный образ Eclipse Temurin JRE
#FROM docker.io/eclipse-temurin:17.0.7_7-jre
#
## Установите рабочую директорию в контейнере
#WORKDIR /app
#
## Установите необходимые зависимости
#USER root
#RUN apt-get update && apt-get install -y python3 python3-pip \
#    && pip install --upgrade pip setuptools wheel \
#    && pip install yt-dlp
#
## Создайте директорию для загрузок
#RUN mkdir -p /app/downloads
#
## Копируем весь проект в рабочую директорию контейнера
##COPY . .
#
## Скопируйте JAR файл приложения в рабочую директорию
#COPY target/MS_YT_DLP-0.0.1-SNAPSHOT.jar /app/MS_YT_DLP-0.0.1-SNAPSHOT.jar
#
## Установите переменные окружения для пути загрузки
#ENV DOWNLOAD_DIR=/app/downloads
#
## Запустите приложение
#CMD ["java", "-XX:+UseG1GC", "-jar", "MS_YT_DLP-0.0.1-SNAPSHOT.jar"]
