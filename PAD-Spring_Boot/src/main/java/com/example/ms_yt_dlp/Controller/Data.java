package com.example.ms_yt_dlp.Controller;

public class Data {
    public String id;
    public String ext;
    public String resolution;
    public String fps;
    public String fileSize;

    // No-arg constructor (required for Jackson)
    public Data() {}

    // Constructor for initialization
    public Data(String id, String ext, String resolution, String fps, String fileSize) {
        this.id = id;
        this.ext = ext;
        this.resolution = resolution;
        this.fps = fps;
        this.fileSize = fileSize;
    }

    // Getters
    public String getId() {
        return id;
    }

    public String getExt() {
        return ext;
    }

    public String getResolution() {
        return resolution;
    }

    public String getFps() {
        return fps;
    }

    public String getFileSize() {
        return fileSize;
    }

    // Optionally, you can also provide setters if needed
    public void setId(String id) {
        this.id = id;
    }

    public void setExt(String ext) {
        this.ext = ext;
    }

    public void setResolution(String resolution) {
        this.resolution = resolution;
    }

    public void setFps(String fps) {
        this.fps = fps;
    }

    public void setFileSize(String fileSize) {
        this.fileSize = fileSize;
    }
}
