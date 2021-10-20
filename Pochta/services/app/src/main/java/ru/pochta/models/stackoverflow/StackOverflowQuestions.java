package ru.pochta.models.stackoverflow;

import java.util.List;

import com.google.gson.annotations.SerializedName;


/**
 * @author voroningg
 */
public class StackOverflowQuestions {
    @SerializedName("questions_list")
    private final List<StackOverflowUrl> questionsList;

    public StackOverflowQuestions(List<StackOverflowUrl> questionsList) {
        this.questionsList = questionsList;
    }

    public List<StackOverflowUrl> getQuestionsList() {
        return questionsList;
    }
}
