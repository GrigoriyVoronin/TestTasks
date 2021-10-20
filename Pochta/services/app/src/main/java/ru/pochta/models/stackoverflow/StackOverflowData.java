package ru.pochta.models.stackoverflow;

/**
 * @author voroningg
 */
public class StackOverflowData {
    private final StackOverflowLink[] items;

    public StackOverflowData(StackOverflowLink[] items) {
        this.items = items;
    }

    public StackOverflowLink[] getItems() {
        return items;
    }
}
