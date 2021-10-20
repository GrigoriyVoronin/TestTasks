package ru.pochta.utils;

/**
 * @author voroningg
 */
public class RetryPolicies {

    public static <T> T get(RetryGetFunction<T> function) throws Exception {
        Exception exception = null;
        for (int i = 0; i < 3; i++) {
            try {
                return function.apply();
            } catch (Exception ex) {
                exception = ex;
            }
        }
        throw exception;
    }

    public static void run(RetryRunFunction function) throws Exception {
        Exception exception = null;
        for (int i = 0; i < 3; i++) {
            try {
                function.apply();
                return;
            } catch (Exception ex) {
                exception = ex;
            }
        }
        throw exception;
    }

    @FunctionalInterface
    public interface RetryGetFunction<T> {
        T apply();
    }

    @FunctionalInterface
    public interface RetryRunFunction {
        void apply();
    }
}
