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
    && apt-get install -y python3 python3-pip iputils-ping \
    && pip3 install yt-dlp  --break-system-packages

# Set the working directory in the container
WORKDIR /app

# Copy the JAR file from the build stage
COPY --from=build /app/target/*.jar app.jar

# Copy all source files to /src folder in the final image
COPY --from=build /app /src

# Expose port 8080 (the default Spring Boot port)
EXPOSE 8080

# Set the command to run the Spring Boot application
ENTRYPOINT ["java", "-jar", "/app/app.jar"]


