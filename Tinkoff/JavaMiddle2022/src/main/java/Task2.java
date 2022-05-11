import java.util.Arrays;
import java.util.HashSet;
import java.util.Scanner;
import java.util.Set;
import java.util.Stack;


public class Task2 {
    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args) {
        int[] countAndLength = parseIntArr();
        int serversCount = countAndLength[0];
        int stepLength = countAndLength[1];
        Set<Integer> secureServers = parseSecureServers();
        boolean isSecurePathExist = findSecurePath(serversCount, stepLength, secureServers);
        print(isSecurePathExist ? "YES" : "NO");
    }

    private static Set<Integer> parseSecureServers() {
        String secureMarks = scanner.nextLine();
        Set<Integer> secureServers = new HashSet<>();
        for (int i = 0; i < secureMarks.length(); i++) {
            if (secureMarks.charAt(i) == '1') {
                secureServers.add(i + 1);
            }
        }
        return secureServers;
    }

    private static boolean findSecurePath(int serversCount, int stepLength, Set<Integer> secureServers) {
        Stack<Integer> serversToCheck = new Stack<>();
        serversToCheck.add(1);
        while (serversToCheck.size() > 0) {
            Integer currentServer = serversToCheck.pop();
            for (int i = 1; i <= stepLength; i++) {
                Integer nextServer = currentServer + i;
                if (serversCount == nextServer) {
                    return true;
                }
                if (secureServers.contains(nextServer)) {
                    serversToCheck.add(nextServer);
                }
            }
        }
        return false;
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
