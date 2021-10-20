package ru.pochta.services;

import java.util.ArrayList;
import java.util.List;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import ru.pochta.models.weather.InputWeatherData;
import ru.pochta.models.weather.OutputWeatherData;
import ru.pochta.models.weather.WeatherCity;
import ru.pochta.utils.Request;

/**
 * @author voroningg
 */
@Service
public class WeatherService {
    private final String apikey;

    public WeatherService(@Value("${weather.apikey}") String apikey) {
        this.apikey = apikey;
    }

    public OutputWeatherData getOutputWeatherData(List<String> cities) throws Exception {
        List<WeatherCity> citiesWeathers = new ArrayList<>();
        for (String city : cities) {
            InputWeatherData inputWeatherData = getInputWeatherData(city);
            citiesWeathers.add(new WeatherCity(inputWeatherData.getCity(), inputWeatherData.getMain().getTemp()));
        }
        return new OutputWeatherData(citiesWeathers);
    }

    private InputWeatherData getInputWeatherData(String city) throws Exception {
        return Request.getWithRetry(getRequestUrl(city), InputWeatherData.class);
    }


    private String getRequestUrl(String city) {
        return String.format("https://api.openweathermap.org/data/2.5/weather?q=%s&appid=%s&units=metric",
                             city, apikey);
    }
}
