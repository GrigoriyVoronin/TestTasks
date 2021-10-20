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
import ru.pochta.controllers.WeatherController;
import ru.pochta.models.stackoverflow.StackOverflowQuestions;
import ru.pochta.models.stackoverflow.StackOverflowUrl;
import ru.pochta.models.weather.OutputWeatherData;
import ru.pochta.models.weather.WeatherCity;
import ru.pochta.services.StackOverflowService;
import ru.pochta.services.WeatherService;

/**
 * @author voroningg
 */
public class WeatherControllerTest {
    private static final String BASE_URL = "/v1.0/weather";
    private static final String CITY_LONDON = "city=London";
    private static final String CITY_EMPTY = "city=";
    private static final String CITIES = "city=London, Moscow";


    @InjectMocks
    private WeatherController weatherController;

    @Mock
    private WeatherService weatherService;

    private MockMvc mockMvc;

    @BeforeEach
    public void init() throws Exception {
        MockitoAnnotations.openMocks(this);
        Mockito.when(weatherService.getOutputWeatherData(List.of("London")))
                .thenReturn(new OutputWeatherData(List.of(new WeatherCity("London", 0.0))));
        Mockito.when(weatherService.getOutputWeatherData(List.of()))
                .thenReturn(new OutputWeatherData(List.of()));
        Mockito.when(weatherService.getOutputWeatherData(List.of("London", "Moscow")))
                .thenReturn(new OutputWeatherData(List.of(new WeatherCity("London", 0.0),
                                                          new WeatherCity("Moscow", 0.0))));
        mockMvc = MockMvcBuilders.standaloneSetup(weatherController).build();
    }

    @Test
    public void shouldReturnBadRequestOnEmptyCity() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s", BASE_URL));
        Assertions.assertEquals(response.getStatus(), HttpStatus.BAD_REQUEST.value());
    }

    @Test
    public void shouldReturnOkEmptyResultOnZeroCity() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s", BASE_URL, CITY_EMPTY));
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        OutputWeatherData outputWeatherData = new Gson()
                .fromJson(response.getContentAsString(), OutputWeatherData.class);
        Assertions.assertEquals(outputWeatherData.getWeatherData().size(), 0);
    }

    @Test
    public void shouldReturnOkResultOnOneCity() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s", BASE_URL, CITY_LONDON));
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        OutputWeatherData outputWeatherData = new Gson()
                .fromJson(response.getContentAsString(), OutputWeatherData.class);
        Assertions.assertEquals(outputWeatherData.getWeatherData().size(), 1);
    }

    @Test
    public void shouldReturnOkResultOnManyCity() throws Exception {
        MockHttpServletResponse response = getResponse(
                String.format("%s?%s", BASE_URL, CITIES));
        Assertions.assertEquals(response.getStatus(), HttpStatus.OK.value());
        OutputWeatherData outputWeatherData = new Gson()
                .fromJson(response.getContentAsString(), OutputWeatherData.class);
        Assertions.assertEquals(outputWeatherData.getWeatherData().size(), 2);
    }

    private MockHttpServletResponse getResponse(String url) throws Exception {
        return mockMvc
                .perform(MockMvcRequestBuilders.get(url))
                .andReturn()
                .getResponse();
    }
}
