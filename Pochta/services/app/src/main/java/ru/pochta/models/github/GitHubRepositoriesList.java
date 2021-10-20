package ru.pochta.models.github;

import java.util.List;

import com.google.gson.annotations.SerializedName;

/**
 * @author voroningg
 */
public class GitHubRepositoriesList {
    @SerializedName("repositories_list")
    private final List<GitHubRepositoryUrl> repositoriesList;

    public GitHubRepositoriesList(List<GitHubRepositoryUrl> repositoriesList) {
        this.repositoriesList = repositoriesList;
    }

    public List<GitHubRepositoryUrl> getRepositoriesList() {
        return repositoriesList;
    }
}
