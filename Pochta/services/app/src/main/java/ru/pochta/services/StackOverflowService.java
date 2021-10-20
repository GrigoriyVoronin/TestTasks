package ru.pochta.services;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

import org.springframework.stereotype.Service;
import ru.pochta.models.stackoverflow.StackOverflowData;
import ru.pochta.models.stackoverflow.StackOverflowQuestions;
import ru.pochta.models.stackoverflow.StackOverflowUrl;
import ru.pochta.utils.Request;

/**
 * @author voroningg
 */
@Service
public class StackOverflowService {
    private static final int STACK_OVERFLOW_PAGE_SIZE = 100;

    public StackOverflowQuestions getStackOverflowQuestions(String topic, int page, int size) throws Exception {
        List<StackOverflowData> stackOverflowDataList = getStackOverflowDataList(topic, page, size);
        return convertToStackOverflowQuestions(stackOverflowDataList);
    }

    private List<StackOverflowData> getStackOverflowDataList(String topic, int page, int size) throws Exception {
        List<StackOverflowData> stackOverflowDataList = new ArrayList<>();
        int currentPage = (int) Math.ceil(((double) (page * size) + size) / STACK_OVERFLOW_PAGE_SIZE);
        for (int i = 0; i < size; i += STACK_OVERFLOW_PAGE_SIZE) {
            int currentSize = Math.min(STACK_OVERFLOW_PAGE_SIZE, size);
            size -= STACK_OVERFLOW_PAGE_SIZE;
            StackOverflowData stackOverflowData = geStackOverflowData(topic, currentPage++, currentSize);
            stackOverflowDataList.add(stackOverflowData);
        }
        return stackOverflowDataList;
    }

    private StackOverflowData geStackOverflowData(
            String topic,
            int currentPage,
            int currentSize)
            throws Exception
    {
        return Request.getZippedWithRetry(getRequestUrl(topic, currentPage, currentSize), StackOverflowData.class);
    }

    private StackOverflowQuestions convertToStackOverflowQuestions(List<StackOverflowData> stackOverflowDataList) {
        List<StackOverflowUrl> stackOverflowUrls = stackOverflowDataList.stream()
                .flatMap(x -> Arrays.stream(x.getItems()))
                .map(x -> new StackOverflowUrl(x.getLink()))
                .collect(Collectors.toList());
        return new StackOverflowQuestions(stackOverflowUrls);
    }

    private String getRequestUrl(String topic, Integer page, Integer size) {
        StringBuilder builder = new StringBuilder();
        builder.append("https://api.stackexchange.com/2.3/questions?");
        builder.append("order=desc&sort=creation");
        if (page != 0) {
            builder.append(String.format("&page=%s", page));
        }
        builder.append(String.format("&pagesize=%s", size));
        builder.append(String.format("&tagged=%s", topic));
        builder.append("&site=stackoverflow");
        return builder.toString();
    }
}
