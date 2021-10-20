package ru.pochta.services;

import java.util.stream.Collectors;

import org.springframework.stereotype.Service;
import ru.pochta.models.github.GitHubInputData;
import ru.pochta.models.github.GitHubRepositoriesList;
import ru.pochta.models.github.GitHubRepositoryUrl;
import ru.pochta.utils.Request;

/**
 * @author voroningg
 */
@Service
public class GithubService {
    public GitHubRepositoriesList getGithubRepositories(String topic, String filter, int count) throws Exception {
        GitHubInputData inputData = getGithubInputData(topic, filter, count);
        return convertToOutputData(inputData);
    }

    private GitHubInputData getGithubInputData(String topic, String filter, int count) throws Exception {
        return Request.getWithRetry(getRequestUrl(topic, filter, count), GitHubInputData.class);
    }

    private GitHubRepositoriesList convertToOutputData(GitHubInputData inputData) {
        return new GitHubRepositoriesList(
                inputData.getItems()
                        .stream()
                        .map(r -> new GitHubRepositoryUrl(r.getUrl()))
                        .collect(Collectors.toList()));
    }

    private String getRequestUrl(String topic, String filter, int count) {
        return new StringBuilder("https://api.github.com/search/repositories?")
                .append(String.format("q=%s", topic))
                .append(String.format("&sort=%s", filter))
                .append(String.format("&per_page=%s", count))
                .toString();
    }
}
