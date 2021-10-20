package ru.pochta.controllers;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import ru.pochta.utils.Response;


/**
 * @author voroningg
 */
@RestController
@RequestMapping("/monitoring")
public class MonitoringController {

    @GetMapping("/ping")
    public ResponseEntity<String> ping() {
        return Response.ok();
    }
}
