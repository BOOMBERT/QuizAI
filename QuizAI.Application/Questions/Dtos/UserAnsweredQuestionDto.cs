﻿namespace QuizAI.Application.Questions.Dtos;

public record UserAnsweredQuestionDto(QuestionWithAnswersDto Question, UserAnswerDto UserAnswer);
