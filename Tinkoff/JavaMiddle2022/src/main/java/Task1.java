import java.util.Arrays;
import java.util.Scanner;

public class Task1 {

    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args) {
        int[] xAndYCount = parseIntArr();
        int x = xAndYCount[0];
        int y = xAndYCount[1];
        String result = x < y
                ? buildResultString(x, y, 'X', 'Y')
                : buildResultString(y, x, 'Y', 'X');
        print(result);

    }

    private static String buildResultString(int minTypeCount, int maxTypeCount, char minType, char maxType) {
        int diff = maxTypeCount - minTypeCount;
        StringBuilder resultBuilder = new StringBuilder(minTypeCount + maxTypeCount);
        for (int i = 0; i < minTypeCount; i++) {
            resultBuilder.append(maxType);
            resultBuilder.append(minType);
        }
        for (int i = 0; i < diff; i++) {
            resultBuilder.append(maxType);
        }

        return resultBuilder.toString();
    }

    private static void print(Object obj) {
        System.out.println(obj.toString());
    }

    private static int[] parseIntArr() {
        return Arrays.stream(scanner.nextLine()
                        .split(" "))
                .mapToInt(Integer::parseInt)
                .toArray();
    }
}
