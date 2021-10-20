package ru.pochta.controllers;


import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import ru.pochta.models.weather.OutputWeatherData;
import ru.pochta.services.WeatherService;
import ru.pochta.utils.Response;

/**
 * @author voroningg
 */
@RestController
@RequestMapping("/v1.0/weather")
public class WeatherController {
    private static final Logger logger = LoggerFactory.getLogger(WeatherController.class);
    private final WeatherService weatherService;

    public WeatherController(WeatherService weatherService) {

        this.weatherService = weatherService;
    }

    @GetMapping
    public ResponseEntity<String> getWeatherData(@RequestParam(name = "city") List<String> cities) {
        try {
            OutputWeatherData outputWeatherData = weatherService.getOutputWeatherData(cities);
            return Response.ok(outputWeatherData);
        } catch (Exception ex) {
            logger.error(ex.getMessage());
            return Response.internalServerError();
        }
    }

}
