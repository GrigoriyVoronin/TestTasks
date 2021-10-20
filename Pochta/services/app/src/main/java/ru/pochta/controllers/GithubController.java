package ru.pochta.controllers;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import ru.pochta.models.github.GitHubRepositoriesList;
import ru.pochta.services.GithubService;
import ru.pochta.utils.CheckParam;
import ru.pochta.utils.Response;

/**
 * @author voroningg
 */
@RestController
@RequestMapping("/v1.0/github")
public class GithubController {
    private final Logger logger = LoggerFactory.getLogger(GithubController.class);
    private final GithubService githubService;

    public GithubController(GithubService githubService) {
        this.githubService = githubService;
    }

    @GetMapping("/repositories")
    public ResponseEntity<String> getRepos(
            @RequestParam String topic,
            @RequestParam String filter,
            @RequestParam(defaultValue = "20") int count)
    {
        try {
            CheckParam.inRange(count, 0, 100);
            GitHubRepositoriesList gitHubRepositoriesList = githubService
                    .getGithubRepositories(topic, filter, count);
            return Response.ok(gitHubRepositoriesList);
        } catch (IllegalArgumentException ex) {
            logger.warn(ex.getMessage());
            return Response.badRequest();
        } catch (Exception ex) {
            logger.error(ex.getMessage());
            return Response.internalServerError();
        }
    }
}
