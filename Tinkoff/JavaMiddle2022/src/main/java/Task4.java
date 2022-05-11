import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Scanner;
import java.util.stream.Collectors;

public class Task4 {

    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args) {
        int setsCount = parseInt();
        for (int i = 0; i < setsCount; i++) {
            int[] permutation = findMaxPermutation();
            print(permutation);
        }
    }

    private static int[] findMaxPermutation() {
        int setLength = parseInt();
        int[] set = parseIntArr();
        int[] result = new int[setLength];
        boolean[] checked = new boolean[setLength];

        int max = findMax(set);
        int index = placeMaxInResult(set, result, checked, max);
        List<Integer> maxDivisors = findDivisors(max);
        index = placeDivisibleNumbersInResult(set, result, checked, maxDivisors, index);
        placeElseItemsInResult(set, result, checked, index);

        return result;
    }

    private static int findMax(int[] set) {
        int max = 0;
        for (int n : set) {
            if (max < n) {
                max = n;
            }
        }
        return max;
    }

    private static int placeMaxInResult(int[] set, int[] result, boolean[] checked, int max) {
        int index = 0;
        for (int i = 0; i < set.length; i++) {
            if (set[i] == max) {
                result[index++] = max;
                checked[i] = true;
            }
        }
        return index;
    }

    private static List<Integer> findDivisors(int max) {
        List<Integer> divisorsDesc = new ArrayList<>();
        for (int i = max - 1; i > 1; i--) {
            if (max % i == 0) {
                divisorsDesc.add(i);
            }
        }
        return divisorsDesc;
    }

    private static int placeDivisibleNumbersInResult(
            int[] set,
            int[] result,
            boolean[] checked,
            List<Integer> maxDivisors,
            int index)
    {
        for (int divisor : maxDivisors) {
            for (int i = 0; i < set.length; i++) {
                if (checked[i]) {
                    continue;
                }
                int n = set[i];
                if (n % divisor == 0) {
                    result[index++] = n;
                    checked[i] = true;
                }
            }
        }
        return index;
    }

    private static void placeElseItemsInResult(int[] set, int[] result, boolean[] checked, int index) {
        for (int i = 0; i < set.length; i++) {
            if (checked[i]) {
                continue;
            }
            result[index++] = set[i];
        }
    }

    private static void print(int[] arr) {
        System.out.println(Arrays.stream(arr)
                .mapToObj(String::valueOf)
                .collect(Collectors.joining(" ")));
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
