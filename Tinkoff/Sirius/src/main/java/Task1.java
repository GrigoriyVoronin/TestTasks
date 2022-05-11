import java.util.Arrays;
import java.util.Scanner;

public class Task1 {
    public static class ExceptionalResource implements AutoCloseable {

        public void processSomething() {
            throw new IllegalArgumentException("Thrown from processSomething()");
        }

        @Override
        public void close() throws Exception {
            throw new NullPointerException("Thrown from close()");
        }
    }
    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args){
        try {
            demoExceptionalResource();
        } catch (Exception e) {
            e.printStackTrace();
        }
        long[] inputArr = getLongArrFromConsole();
        long first = inputArr[0];
        long second = inputArr[1];
        while (first > 0 && second > 0) {
            if (first % 10 + second % 10 >= 10) {
                print("Yes");
                return;
            }
            first/=10;
            second/=10;
        }
        print("No");
    }
    public static void demoExceptionalResource() throws Exception {
        try (ExceptionalResource exceptionalResource = new ExceptionalResource()) {
            exceptionalResource.processSomething();
        }
    }

    private static void print(Object obj) {
        System.out.println(obj.toString());
    }

    private static long[] getLongArrFromConsole() {
        return Arrays.stream(scanner.nextLine().split(" "))
                .mapToLong(Long::parseLong)
                .toArray();
    }
}
