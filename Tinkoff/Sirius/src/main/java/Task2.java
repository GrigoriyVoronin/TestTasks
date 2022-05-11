import java.util.Arrays;
import java.util.Scanner;

public class Task2 {

    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args){
        int arrLength = getIntFromConsole();
        int[] inputArr = getIntArrFromConsole();
        long searchingSum = getLongFromConsole();
        long currentSum = 0;
        int currentIndex = 0;
        int arrSumCount = 0;

        while (currentSum <= searchingSum) {
            currentSum+=inputArr[currentIndex++];

            if (currentIndex == arrLength) {
                currentIndex = 0;
                arrSumCount++;
            }
        }
        print(currentIndex + arrSumCount * arrLength);
    }

    private static void print(Object obj) {
        System.out.println(obj.toString());
    }

    private static int getIntFromConsole() {
        return Integer.parseInt(scanner.nextLine());
    }

    private static long getLongFromConsole() {
        return Long.parseLong(scanner.nextLine());
    }

    private static int[] getIntArrFromConsole() {
        return Arrays.stream(scanner.nextLine().split(" "))
                .mapToInt(Integer::parseInt)
                .toArray();
    }
}
