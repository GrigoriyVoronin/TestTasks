package ru.pochta.utils;

/**
 * @author voroningg
 */
public class CheckParam {
    public static void inRange(int param, int min, int max) {
        if (param < min || param > max) {
            throw new IllegalArgumentException();
        }
    }

    public static void greaterThan(int param, int min) {
        if (param < min) {
            throw new IllegalArgumentException();
        }
    }
}
