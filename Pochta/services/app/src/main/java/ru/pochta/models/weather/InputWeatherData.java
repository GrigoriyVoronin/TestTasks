package ru.pochta.models.weather;

import com.google.gson.annotations.SerializedName;

/**
 * @author voroningg
 */
public class InputWeatherData {
    private final InputWeatherMain main;
    @SerializedName("name")
    private final String city;

    public InputWeatherData(InputWeatherMain main, String city) {
        this.main = main;
        this.city = city;
    }

    public String getCity() {
        return city;
    }

    public InputWeatherMain getMain() {
        return main;
    }
}
