package controllers;

import java.util.ArrayList;
import java.util.List;

import com.google.gson.Gson;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.MockitoAnnotations;
import org.springframework.http.HttpStatus;
import org.springframework.mock.web.MockHttpServletResponse;
import org.springframework.test.web.servlet.MockMvc;
import org.springframework.test.web.servlet.request.MockMvcRequestBuilders;
import org.springframework.test.web.servlet.setup.MockMvcBuilders;
import ru.pochta.controllers.GithubController;
import ru.pochta.models.github.GitHubRepositoriesList;
import ru.pochta.models.github.GitHubRepositoryUrl;
import ru.pochta.services.GithubService;

/**
 * @author voroningg
 */
public class GithubControllerTest {
    private static final String BASE_URL = "/v1.0/github/repositories";
    private static final String JAVA_TOPIC = "topic=java";
    private static final String POPULAR_FILTER = "filter=popular";
    private static final String COUNT = "count=";

    @InjectMocks
    private GithubController githubController;

    @Mock
    private GithubService githubService;

    private MockMvc mockMvc;

    @BeforeEach
    public void init() throws Exception {
        MockitoAnnotations.openMocks(this);
        Mockito.when(githubService.getGithubRepositories("java", "popular", 1))
                .thenReturn(new GitHubRepositoriesList(List.of(new GitHubRepositoryUrl("testUrl"))));
        Mockito.when(githubService.getGithubRepositories("java", "popular", 0))
                .thenReturn(new GitHubRepositoriesList(new ArrayList<>()));
        List<GitHubRepositoryUrl> twentyList = new ArrayList<>();
        List<GitHubRepositoryUrl> hundredList = new ArrayList<>();
        for (int i = 0; i < 100; i++) {
            hundredList.add(new GitHubRepositoryUrl("test"));
        }
        for (int i = 0; i < 20; i++) {
            twentyList.add(new GitHubRepositoryUrl("test"));
        }
        Mockito.when(githubService.getGithubRepositories("java", "popular", 20))
                .thenReturn(new GitHubRepositoriesList(twentyList));
        Mockito.when(githubService.getGithubRepositories("java", "popular", 100))
                .thenReturn(new GitHubRepositoriesList(hundredList));
        mockMvc = MockMvcBuilders.standaloneSetup(githubController).build();
    }

    @Test
    public void shouldReturnBadRequestOnCountLessThenZero() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s&%s-1", BASE_URL, JAVA_TOPIC, POPULAR_FILTER, COUNT));
        Assertions.assertEquals(response.getStatus(), HttpStatus.BAD_REQUEST.value());
    }

    @Test
    public void shouldReturnOkEmptyResultOnZeroCount() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s&%s0", BASE_URL, JAVA_TOPIC, POPULAR_FILTER, COUNT));
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        GitHubRepositoriesList gitHubRepositoriesList = new Gson()
                .fromJson(response.getContentAsString(), GitHubRepositoriesList.class);
        Assertions.assertEquals(gitHubRepositoriesList.getRepositoriesList().size(), 0);
    }

    @Test
    public void shouldReturnBadRequestOnEmptyTopic() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s1", BASE_URL, POPULAR_FILTER, COUNT));
        Assertions.assertEquals(response.getStatus(), HttpStatus.BAD_REQUEST.value());
    }

    @Test
    public void shouldReturnBadRequestOnEmptyFilter() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s1", BASE_URL, JAVA_TOPIC, COUNT));
        Assertions.assertEquals(response.getStatus(), HttpStatus.BAD_REQUEST.value());
    }

    @Test
    public void shouldReturn20RepositoriesOnEmptyCount() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s", BASE_URL, JAVA_TOPIC, POPULAR_FILTER));
        GitHubRepositoriesList gitHubRepositoriesList = new Gson()
                .fromJson(response.getContentAsString(), GitHubRepositoriesList.class);
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        Assertions.assertEquals(gitHubRepositoriesList.getRepositoriesList().size(), 20);
    }

    @Test
    public void shouldReturnBadRequestOnCountGreaterThan100() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s&%s101", BASE_URL, JAVA_TOPIC, POPULAR_FILTER, COUNT));
        Assertions.assertEquals(response.getStatus(), HttpStatus.BAD_REQUEST.value());
    }

    @Test
    public void shouldReturn100RepositoriesOn100Count() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s&%s100", BASE_URL, JAVA_TOPIC, POPULAR_FILTER, COUNT));
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        GitHubRepositoriesList gitHubRepositoriesList = new Gson()
                .fromJson(response.getContentAsString(), GitHubRepositoriesList.class);
        Assertions.assertEquals(gitHubRepositoriesList.getRepositoriesList().size(), 100);
    }

    private MockHttpServletResponse getResponse(String url) throws Exception {
        return mockMvc
                .perform(MockMvcRequestBuilders.get(url))
                .andReturn()
                .getResponse();
    }
}
