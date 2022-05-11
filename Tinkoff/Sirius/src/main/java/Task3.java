import java.util.Arrays;
import java.util.HashSet;
import java.util.Scanner;
import java.util.Set;

public class Task3 {

    private static final Scanner scanner = new Scanner(System.in);

    public static void main(String[] args){
        int[] childrenAndPairsCount = getIntArrFromConsole();
        int pairsCount = childrenAndPairsCount[1];
        Set<Integer> childrenWithFreeLeft = new HashSet<>();
        Set<Integer> childrenWithFreeRight = new HashSet<>();
        Set<Integer> childrenWithoutFreeSides = new HashSet<>();
        for (int i = 0; i< pairsCount; i++) {
            int[] pair = getIntArrFromConsole();
            if (!processPair(pair, childrenWithFreeLeft, childrenWithFreeRight, childrenWithoutFreeSides)) {
                print("No");
                return;
            }
        }
        print("Yes");
    }

    private static boolean processPair(
            int[] pair,
            Set<Integer> childrenWithFreeLeft,
            Set<Integer> childrenWithFreeRight,
            Set<Integer> childrenWithoutFreeSides)
    {
        int first = pair[0];
        int second = pair[1];
        if (childrenWithoutFreeSides.contains(first) || childrenWithoutFreeSides.contains(second)) {
            return false;
        }
        boolean isLeftFreeSideFirst = childrenWithFreeLeft.contains(first);
        boolean isLeftFreeSideSecond = childrenWithFreeLeft.contains(second);
        boolean isRightFreeSideFirst = childrenWithFreeRight.contains(first);
        boolean isRightFreeSideSecond = childrenWithFreeRight.contains(second);
        if ((isLeftFreeSideFirst && isRightFreeSideSecond) || (isRightFreeSideFirst && isLeftFreeSideSecond)) {
            return false;
        }
        if (isLeftFreeSideFirst) {
            setPair(first, second, childrenWithFreeLeft, childrenWithoutFreeSides);
        } else if (isLeftFreeSideSecond) {
            setPair(second, first, childrenWithFreeLeft, childrenWithoutFreeSides);
        } else if (isRightFreeSideFirst) {
            setPair(first, second, childrenWithFreeRight, childrenWithoutFreeSides);
        } else if (isRightFreeSideSecond) {
            setPair(second, first, childrenWithFreeRight, childrenWithoutFreeSides);
        } else {
            childrenWithFreeLeft.add(first);
            childrenWithFreeRight.add(second);
        }
        return true;
    }

    private static void setPair(
            int oldChild,
            int newChild,
            Set<Integer> freeSideSet,
            Set<Integer> withoutFreeSideSet)
    {
        freeSideSet.add(newChild);
        freeSideSet.remove(oldChild);
        withoutFreeSideSet.add(oldChild);
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
