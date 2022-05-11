import java.util.Arrays;
import java.util.Scanner;

public class Task3 {

    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args) {
        int peopleCount = parseInt();
        int[][] ticketsCosts = parseTicketsCosts(peopleCount);
        int[] fastestTimeByPerson = findFastestTimeByPerson(ticketsCosts);
        print(fastestTimeByPerson[peopleCount - 1]);
    }

    private static int[][] parseTicketsCosts(int peopleCount) {
        int[][] ticketsCosts = new int[peopleCount][];
        for (int i = 0; i < peopleCount; i++) {
            ticketsCosts[i] = parseIntArr();
        }
        return ticketsCosts;
    }

    private static int[] findFastestTimeByPerson(int[][] ticketsCosts) {
        int[] fastestTimeByPerson = new int[ticketsCosts.length];
        for (int i = 0; i < ticketsCosts.length; i++) {
            int previous = getPrevious(fastestTimeByPerson, i - 1);
            for (int j = 0; j < 3; j++) {
                int index = i + j;
                if (index >= ticketsCosts.length) {
                    break;
                }

                fastestTimeByPerson[index] = getFastest(fastestTimeByPerson[index], previous + ticketsCosts[i][j]);
            }
        }

        return fastestTimeByPerson;
    }

    private static int getPrevious(int[] fastestTimeByPerson, int i) {
        if (i < 0) {
            return 0;
        } else {
            return fastestTimeByPerson[i];
        }
    }

    private static int getFastest(int current, int nextCost) {
        return current == 0
                ? nextCost
                : Math.min(current, nextCost);
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

    private static int parseInt() {
        return Integer.parseInt(scanner.nextLine());
    }
}
