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
import ru.pochta.controllers.StackOverflowController;
import ru.pochta.models.stackoverflow.StackOverflowQuestions;
import ru.pochta.models.stackoverflow.StackOverflowUrl;
import ru.pochta.services.StackOverflowService;

/**
 * @author voroningg
 */
public class StackOverflowControllerTest {
    private static final String BASE_URL = "/v1.0/stackoverflow/questions";
    private static final String JAVA_TOPIC = "topic=java";
    private static final String PAGE = "page=";
    private static final String SIZE = "size=";

    @InjectMocks
    private StackOverflowController stackOverflowController;

    @Mock
    private StackOverflowService stackOverflowService;

    private MockMvc mockMvc;

    @BeforeEach
    public void init() throws Exception {
        MockitoAnnotations.openMocks(this);
        Mockito.when(stackOverflowService.getStackOverflowQuestions("java", 0, 1))
                .thenReturn(new StackOverflowQuestions(List.of(new StackOverflowUrl("test"))));
        List<StackOverflowUrl> thousandList = new ArrayList<>();
        for (int i = 0; i < 1000; i++) {
            thousandList.add(new StackOverflowUrl("test"));
        }
        List<StackOverflowUrl> twentyList = new ArrayList<>();
        for (int i = 0; i < 20; i++) {
            twentyList.add(new StackOverflowUrl("test"));
        }
        Mockito.when(stackOverflowService.getStackOverflowQuestions("java", 0, 20))
                .thenReturn(new StackOverflowQuestions(twentyList));
        Mockito.when(stackOverflowService.getStackOverflowQuestions("java", 0, 1000))
                .thenReturn(new StackOverflowQuestions(thousandList));
        mockMvc = MockMvcBuilders.standaloneSetup(stackOverflowController).build();
    }

    @Test
    public void shouldReturnBadRequestOnPageLessThenZero() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s-1&%s1", BASE_URL, JAVA_TOPIC, PAGE, SIZE));
        Assertions.assertEquals(response.getStatus(), HttpStatus.BAD_REQUEST.value());
    }

    @Test
    public void shouldReturnOkResultOnZeroPage() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s0&%s1", BASE_URL, JAVA_TOPIC, PAGE, SIZE));
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        StackOverflowQuestions stackOverflowQuestions = new Gson()
                .fromJson(response.getContentAsString(), StackOverflowQuestions.class);
        Assertions.assertEquals(stackOverflowQuestions.getQuestionsList().size(), 1);
    }

    @Test
    public void shouldReturnOkResultOnEmptyPage() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s1", BASE_URL, JAVA_TOPIC, SIZE));
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        StackOverflowQuestions stackOverflowQuestions = new Gson()
                .fromJson(response.getContentAsString(), StackOverflowQuestions.class);
        Assertions.assertEquals(stackOverflowQuestions.getQuestionsList().size(), 1);
    }

    @Test
    public void shouldReturnBadRequestOnEmptyTopic() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s0&%s1", BASE_URL, PAGE, SIZE));
        Assertions.assertEquals(response.getStatus(), HttpStatus.BAD_REQUEST.value());
    }

    @Test
    public void shouldReturnBadRequestOnSizeLessThenOne() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s0&%s0", BASE_URL, JAVA_TOPIC, PAGE, SIZE));
        Assertions.assertEquals(response.getStatus(), HttpStatus.BAD_REQUEST.value());
    }

    @Test
    public void shouldReturnOkResultOn1Size() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s0&%s1", BASE_URL, JAVA_TOPIC, PAGE, SIZE));
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        StackOverflowQuestions stackOverflowQuestions = new Gson()
                .fromJson(response.getContentAsString(), StackOverflowQuestions.class);
        Assertions.assertEquals(stackOverflowQuestions.getQuestionsList().size(), 1);
    }

    @Test
    public void shouldReturn20QuestionsOnEmptySize() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s0", BASE_URL, JAVA_TOPIC, PAGE));
        StackOverflowQuestions stackOverflowQuestions = new Gson()
                .fromJson(response.getContentAsString(), StackOverflowQuestions.class);
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        Assertions.assertEquals(stackOverflowQuestions.getQuestionsList().size(), 20);
    }

    @Test
    public void shouldReturnBadRequestOnSizeGreaterThan1000() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s0&%s1001", BASE_URL, JAVA_TOPIC, PAGE, SIZE));
        Assertions.assertEquals(response.getStatus(), HttpStatus.BAD_REQUEST.value());
    }

    @Test
    public void shouldReturn1000QuestionsOn1000Size() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s&%s0&%s1000", BASE_URL, JAVA_TOPIC, PAGE, SIZE));
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        StackOverflowQuestions stackOverflowQuestions = new Gson()
                .fromJson(response.getContentAsString(), StackOverflowQuestions.class);
        Assertions.assertEquals(stackOverflowQuestions.getQuestionsList().size(), 1000);
    }

    private MockHttpServletResponse getResponse(String url) throws Exception {
        return mockMvc
                .perform(MockMvcRequestBuilders.get(url))
                .andReturn()
                .getResponse();
    }
}
