package ru.pochta.models.github;

import java.util.List;

/**
 * @author voroningg
 */
public class GitHubInputData {
    private final List<GitHubInputRepository> items;

    public GitHubInputData(List<GitHubInputRepository> items) {
        this.items = items;
    }

    public List<GitHubInputRepository> getItems() {
        return items;
    }
}
