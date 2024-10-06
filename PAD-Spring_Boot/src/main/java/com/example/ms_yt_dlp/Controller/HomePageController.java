package com.example.ms_yt_dlp.Controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;


@RestController
@RequestMapping("/api")
// Исправлено на @RequestMapping
public class HomePageController {


//    @GetMapping(value = "/home/home")
//    @ResponseBody
//    public String home(Model model) {
//        return "index";
//    }
//
//    @GetMapping(value = "/login")
//    @ResponseBody
//    public String login(Model model) {
//        return "login";
//    }
//

//    @GetMapping("/receiveString")
//    public String receiveString(@RequestParam String inputString) {
//        return "Received string: " + inputString;
//    }
    @GetMapping("/receiveString")
    public ResponseEntity<String> receiveString() {
        return ResponseEntity.ok("String received");
    }
}
