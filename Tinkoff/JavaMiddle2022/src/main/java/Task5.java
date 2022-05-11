import java.util.*;

public class Task5 {

    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args) {
        int[] input = parseIntArr();
        int trackCount = input[0];
        int barsCount = input[1];
        int samplesCount = input[2];


        TreeMap<Integer, Integer> barsCountMap = new TreeMap<>();
        barsCountMap.put(barsCount, 1);
        int tracksInSetCount = 1;

        for (int i = 0; i < samplesCount; i++) {
            int sampleSize = parseInt();
            tracksInSetCount = placeSample(barsCountMap, trackCount, sampleSize, tracksInSetCount);
        }
    }

    private static int placeSample(
            TreeMap<Integer, Integer> barsCountMap,
            int trackCount,
            int sampleSize,
            int tracksInSetCount)
    {
        Integer max = barsCountMap.lastKey();
        Map.Entry<Integer, Integer> ceil = barsCountMap.ceilingEntry(sampleSize);

        if (ceil == null) {
            print(-1);
            return tracksInSetCount;
        }

        print(ceil.getValue());

        if (ceil.getKey().equals(max) && tracksInSetCount < trackCount) {
            barsCountMap.put(ceil.getKey(), ceil.getValue() + 1);
            tracksInSetCount++;
        } else {
            barsCountMap.remove(ceil.getKey());
        }

        barsCountMap.put(ceil.getKey() - sampleSize, ceil.getValue());

        return tracksInSetCount;
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
