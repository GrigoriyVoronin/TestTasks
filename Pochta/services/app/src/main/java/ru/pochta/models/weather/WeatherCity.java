package ru.pochta.models.weather;

import com.google.gson.annotations.SerializedName;

/**
 * @author voroningg
 */
public class WeatherCity {
    @SerializedName("city_name")
    private final String cityName;

    @SerializedName("degrees_celsius")
    private final double degreesCelsius;

    public WeatherCity(String cityName, double degreesCelsius) {
        this.cityName = cityName;
        this.degreesCelsius = degreesCelsius;
    }

    public String getCityName() {
        return cityName;
    }

    public double getDegreesCelsius() {
        return degreesCelsius;
    }
}
