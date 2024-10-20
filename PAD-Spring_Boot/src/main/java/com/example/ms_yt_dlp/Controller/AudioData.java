package com.example.ms_yt_dlp.Controller;


public class AudioData {
    // Fields
    public String id;
    public String ext;
    public String fileSize;
    public String tbr;

    // No-arg constructor (required for Jackson)
    public AudioData() {}

    // Constructor for initialization
    public AudioData(String id, String ext, String fileSize, String tbr) {
        this.id = id;
        this.ext = ext;
        this.fileSize = fileSize;
        this.tbr = tbr;
    }

    // Getters
    public String getId() {
        return id;
    }

    public String getExt() {
        return ext;
    }

    public String getFileSize() {
        return fileSize;
    }

    public String getTBR() {
        return tbr;
    }

    // Setters
    public void setId(String id) {
        this.id = id;
    }

    public void setExt(String ext) {
        this.ext = ext;
    }

    public void setFileSize(String fileSize) {
        this.fileSize = fileSize;
    }

    public void setTBR(String tbr) {
        this.tbr = tbr;
    }
}