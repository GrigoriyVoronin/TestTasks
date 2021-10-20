package ru.pochta.models.weather;

import java.util.List;

import com.google.gson.annotations.SerializedName;

/**
 * @author voroningg
 */
public class OutputWeatherData {
    @SerializedName("weather_data")
    private final List<WeatherCity> weatherData;

    public OutputWeatherData(List<WeatherCity> weatherData) {
        this.weatherData = weatherData;
    }

    public List<WeatherCity> getWeatherData() {
        return weatherData;
    }
}
