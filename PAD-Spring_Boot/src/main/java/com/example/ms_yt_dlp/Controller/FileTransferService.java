package com.example.ms_yt_dlp.Controller;

import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpMethod;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Service;
import org.springframework.util.MultiValueMap;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.core.io.ByteArrayResource;
import org.springframework.core.io.Resource;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.http.HttpMethod;
import org.springframework.http.client.MultipartBodyBuilder;

import java.io.IOException;


@Service
public class FileTransferService {

    private final RestTemplate restTemplate = new RestTemplate();

    public ResponseEntity<String> sendFile(MultipartFile file) throws IOException {

        // Prepare file data
        MultipartBodyBuilder builder = new MultipartBodyBuilder();
        builder.part("file", new ByteArrayResource(file.getBytes()))
                .header("Content-Disposition", "form-data; name=file; filename=" + file.getOriginalFilename());

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.MULTIPART_FORM_DATA);

        HttpEntity<MultiValueMap<String, HttpEntity<?>>> requestEntity =
                new HttpEntity<>(builder.build(), headers);

        // URL of the ASP.NET Core API
        String url = "http://localhost:5000/api/upload";  // adjust API URL as needed

        // Send request
        ResponseEntity<String> response = restTemplate.exchange(url, HttpMethod.POST, requestEntity, String.class);

        return response;
    }
}
