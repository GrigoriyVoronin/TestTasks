package ru.pochta.controllers;


import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import ru.pochta.models.stackoverflow.StackOverflowQuestions;
import ru.pochta.services.StackOverflowService;
import ru.pochta.utils.CheckParam;
import ru.pochta.utils.Response;


/**
 * @author voroningg
 */
@RestController
@RequestMapping("/v1.0/stackoverflow")
public class StackOverflowController {
    private final Logger logger = LoggerFactory.getLogger(StackOverflowController.class);
    private final StackOverflowService stackOverflowService;

    public StackOverflowController(StackOverflowService stackOverflowService) {

        this.stackOverflowService = stackOverflowService;
    }

    @GetMapping("/questions")
    public ResponseEntity<String> getQuestionsByTopic(
            @RequestParam String topic,
            @RequestParam(defaultValue = "0") int page,
            @RequestParam(defaultValue = "20") int size)
    {
        try {
            CheckParam.greaterThan(page, 0);
            CheckParam.inRange(size, 1, 1000);
            StackOverflowQuestions stackOverflowQuestions = stackOverflowService
                    .getStackOverflowQuestions(topic, page, size);
            return Response.ok(stackOverflowQuestions);
        } catch (IllegalArgumentException ex) {
            logger.warn(ex.getMessage());
            return Response.badRequest();
        } catch (Exception ex) {
            logger.error(ex.getMessage());
            return Response.internalServerError();
        }
    }
}
