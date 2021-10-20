package ru.pochta.models.weather;

/**
 * @author voroningg
 */
public class InputWeatherMain {
    private final double temp;

    public InputWeatherMain(double temp) {
        this.temp = temp;
    }

    public double getTemp() {
        return temp;
    }
}
