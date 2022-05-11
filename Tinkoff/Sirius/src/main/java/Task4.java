import java.util.*;

public class Task4 {

    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args) {
        int pointsCount = getIntFromConsole();
        SortedMap<Integer, SortedSet<Integer>> xAxis = new TreeMap<>();
        SortedMap<Integer, SortedSet<Integer>> yAxis = new TreeMap<>();
        for (int i =0; i< pointsCount; i++) {
            int[] point = getIntArrFromConsole();
            int x = point[0];
            int y = point[1];
            xAxis.computeIfAbsent(x, k -> new TreeSet<>()).add(y);
            yAxis.computeIfAbsent(y, k -> new TreeSet<>()).add(x);
        }
        long counter = 0;
        for (Map.Entry<Integer, SortedSet<Integer>> xWithYCoordinates : xAxis.entrySet()) {
            int x = xWithYCoordinates.getKey();
            SortedSet<Integer> yCoordinates = xWithYCoordinates.getValue();
            for (Integer y1 : yCoordinates) {
                SortedSet<Integer> xCoordinates1 = yAxis.get(y1).tailSet(x + 1);
                for (Integer y2 : yCoordinates.tailSet(y1 + 1)) {
                    SortedSet<Integer> xCoordinates2 = yAxis.get(y2).tailSet(x + 1);
                    for (Integer x2 : xCoordinates2) {
                        if (xCoordinates1.contains(x2)) {
                            counter++;
                        }
                    }
                }
            }
        }
        print(counter);
    }

    private static void print(Object obj) {
        System.out.println(obj.toString());
    }

    private static int getIntFromConsole() {
        return Integer.parseInt(scanner.nextLine());
    }

    private static int[] getIntArrFromConsole() {
        return Arrays.stream(scanner.nextLine().split(" "))
                .mapToInt(Integer::parseInt)
                .toArray();
    }
}
