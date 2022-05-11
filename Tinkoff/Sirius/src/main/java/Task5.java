import java.util.*;

public class Task5 {

    private static final Scanner scanner = new Scanner(System.in);

    // Понимаю, что решение не очень эффективное и не очень красивое,
    // но то что было лучше, не решало все возможные варианты задачи.
    public static void main(String[] args) {
        Map<Integer, Set<Integer>> graph = getGraph();
        int[] startAndEnd = getIntArrFromConsole();
        int start = startAndEnd[0];
        int end = startAndEnd[1];

        Map<Integer, Set<Integer>> pathsLengthsFromStartToPeaks = new HashMap<>();
        pathsLengthsFromStartToPeaks.computeIfAbsent(start, k -> new HashSet<>()).add(0);
        Map<Integer, Set<Integer>> visitedPeaksBySourcePeak = new HashMap<>();
        processGraph(graph, pathsLengthsFromStartToPeaks, visitedPeaksBySourcePeak, start, end, false);

        int pathFromStartToEndLength = findPathLengthFromStartToEnd(
                pathsLengthsFromStartToPeaks,
                end);

        print(pathFromStartToEndLength);
    }

    private static int findPathLengthFromStartToEnd(Map<Integer, Set<Integer>> pathsLengthsFromStartToPeaks, int end) {
        if (!pathsLengthsFromStartToPeaks.containsKey(end)) {
            return -1;
        }

        Optional<Integer> minimalPath = pathsLengthsFromStartToPeaks.get(end).stream()
                .filter(x -> x %3 ==0)
                .min(Integer::compareTo);
        return minimalPath.orElse(-1);
    }

    private static void processGraph(
            Map<Integer, Set<Integer>> graph,
            Map<Integer, Set<Integer>> pathsLengthsFromStartToPeaks,
            Map<Integer, Set<Integer>> visitedPeaksBySourcePeak,
            int currentPeak,
            int endPeak,
            boolean isHasOneLengthCycle)
    {
        if (currentPeak == endPeak) {
            return;
        }

        Set<Integer> peaksWithPathFromCurrentPeak = graph.getOrDefault(currentPeak, new HashSet<>());
        Set<Integer> lengthsVariantsToCurrentPeak = pathsLengthsFromStartToPeaks
                .getOrDefault(currentPeak, new HashSet<>());

        visitedPeaksBySourcePeak.computeIfAbsent(currentPeak, key -> new HashSet<>());
        isHasOneLengthCycle = isHasOneLengthCycle || checkCycle(peaksWithPathFromCurrentPeak, graph, currentPeak);
        for (Integer peak : peaksWithPathFromCurrentPeak) {
            Set<Integer> visitedByCurrentPeak = visitedPeaksBySourcePeak.get(currentPeak);
            if (visitedByCurrentPeak.contains(peak)) {
                continue;
            }

            pathsLengthsFromStartToPeaks.computeIfAbsent(peak, key -> new HashSet<>());
            Set<Integer> pathsToNewPeak = pathsLengthsFromStartToPeaks.get(peak);
            for (Integer length : lengthsVariantsToCurrentPeak) {
                pathsToNewPeak.add(length + 1);
            }
            visitedByCurrentPeak.add(peak);
            if (peak == endPeak && isHasOneLengthCycle) {
                findNearestDivisibleBy3InCycle(lengthsVariantsToCurrentPeak).ifPresent(pathsToNewPeak::add);
            } else {
                processGraph(graph, pathsLengthsFromStartToPeaks, visitedPeaksBySourcePeak,
                        peak, endPeak, isHasOneLengthCycle);
            }
        }
    }

    private static boolean checkCycle(
            Set<Integer> peaksWithPathFromCurrentPeak,
            Map<Integer, Set<Integer>> graph,
            int currentPeak)
    {
        return peaksWithPathFromCurrentPeak.stream()
                        .anyMatch(peak -> graph.getOrDefault(peak, new HashSet<>()).contains(currentPeak));
    }

    private static Optional<Integer> findNearestDivisibleBy3InCycle(Set<Integer> lengthsToCurrentPeak) {
        return lengthsToCurrentPeak.stream()
                .map(n -> findNearestDivisibleBy3(n + 1))
                .min(Integer::compareTo);
    }

    private static int findNearestDivisibleBy3(int n) {
        while (n % 3 != 0) {
            n+=2;
        }
        return n;
    }

    private static Map<Integer, Set<Integer>> getGraph() {
        int[] peaksAndEdgesCount = getIntArrFromConsole();
        int edgesCount = peaksAndEdgesCount[1];
        Map<Integer, Set<Integer>> graph = new HashMap<>();
        for (int i = 0; i < edgesCount; i++) {
            int[] edgePeaks = getIntArrFromConsole();
            graph.computeIfAbsent(edgePeaks[0], key -> new HashSet<>())
                    .add(edgePeaks[1]);
        }
        return graph;
    }

    private static void print(Object obj) {
        System.out.println(obj.toString());
    }

    private static int[] getIntArrFromConsole() {
        return Arrays.stream(scanner.nextLine().split(" "))
                .mapToInt(Integer::parseInt)
                .toArray();
    }
}