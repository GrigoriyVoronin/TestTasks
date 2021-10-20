package ru.pochta.models.github;

import com.google.gson.annotations.SerializedName;

/**
 * @author voroningg
 */
public class GitHubInputRepository {
    @SerializedName("html_url")
    private final String url;

    public GitHubInputRepository(String url) {
        this.url = url;
    }

    public String getUrl() {
        return url;
    }
}
