# 📝 Description
__QuizAI__ is a fully functional __API__ designed according to the principles of __Clean Architecture__ and the __CQRS__ pattern. 
It enables seamless __quiz management__ and includes __versioning__, providing access to __previous quiz attempts__ while ensuring that active attempts are __never interrupted__.

The API supports __three types of questions__ and includes a built-in __permission management system__, allowing __collaborative quiz creation__ as well as the ability to __grant access to private quizzes__ for selected users.

QuizAI supports __images__ that are __optimizing__, with their __storage and serving adjusted__ to the __quiz's privacy__ level.
It also offers __AI-powered__ functionalities for __verifying answer accuracy__ and __generating questions__.

With its __optimized structure__, QuizAI __avoids data duplication and unnecessary storage__, ensuring both __efficiency and performance__. 
It also provides __full validation__ and __error logging__.

# 🛠️ Technologies
- __Backend__: .NET 8, ASP.NET Web API, MediatR
- __Database__: Microsoft SQL Server, Microsoft.EntityFrameworkCore, AspNetCore.Identity
- __Logging and Validation__: Serilog, FluentValidation
- __Mapping and AI__: AutoMapper, OpenAI
- __Image Processing and Hashing__: SixLabors.ImageSharp, Shipwreck.Phash.Bitmaps

# ✨ Features

## 🔹 Solving Process
### _The quiz-solving process is designed such that:_
- fetching the first question starts an attempt.
- each answer is submitted separately, allowing users to return to previously started quizzes and continue their attempt. 
  Returned information helps indicate whether an attempt is unfinished and whether the quiz can be edited as well.

## 🔹 Versioning
### _Quizzes have versioning, which ensures that:_
- users have full access to their attempts, even if the quiz has been modified or deleted. Everything is retained except for categories (which are not duplicated in the database), the quiz image (question images are still stored), and in some cases, the quiz permissions.
- during quiz solving, there will be no situation where the quiz unexpectedly changes, gets deleted, or its privacy settings change, preventing errors and ensuring a smooth user experience.
- only versions of quizzes that someone has started solving are stored, which means quizzes that haven’t been started and have no attempt history are not versioned.
- if an attempt is made to change or start solving an older version of a quiz, an error will be returned with information about the latest version, if one exists.

## 🔹 Permissions
### _Quiz creators have the ability to manage access to their quizzes, which means that:_
- it’s possible to grant someone permission to edit a quiz or solve it if it's private, using their email address.
- users can collaboratively create quizzes and share them with a limited group of recipients, enabling more flexible quiz creation. 
  Permissions are continuously updated or copied to the latest quiz version, allowing for a smooth editing process.

## 🔹 Images
### _Quizzes and questions may contain images, which:_
- are optimized for file size during upload, and their dimensions are adjusted to meet requirements.
- currently can have the extensions png, jpg, or jpeg, which is checked during validation, where the file size is also verified.
- are separated into two folders (private and public). 
  The public folder contains images for public quizzes, while the private folder contains images for private quizzes. 
  This ensures that images for private quizzes are securely stored and only accessible through the appropriate endpoint, whereas images for public quizzes are readily accessible directly from the browser, as they are served as static files, reducing the number of requests. 
  It is important to note that images are not duplicated (during upload, it is checked whether the image already exists using hashing), meaning that a given image will only have one record in the database, even if it is used multiple times in both folders. 
  An image file may appear once in the private folder and once in the public folder — if this occurs, they will have the same name and refer to the same record in the database. 
  All images are always deleted when no longer needed, ensuring unnecessary files and records are not stored in the database.

# 💾 Database Diagram

![QuizAI-Database-Diagram-Image](https://github.com/user-attachments/assets/dfa86350-1458-4730-83fc-0b9dc513de84)
_It does not include the AspNetCore.Identity tables that are not used in the API_

# 🌐 Endpoints Preview

![QuizAI-SwaggerUI-Overview-Image](https://github.com/user-attachments/assets/db75c8b9-334b-48b5-a0cf-b673cd07c405)

## 🔹 Quizzes

### POST: `/api/quizzes`
- __Parameters:__ `name`, `description`, `isPrivate`, `categories[]`, `image`
- Creates a new quiz using `multipart/form-data` (image upload)

### GET: `/api/quizzes`
- __Parameters:__ `searchPhrase`, `pageNumber`, `pageSize`, `sortBy`, `sortDirection`, `filterByCategories[]`, `filterByCreator`, `filterBySharedQuizzes`, `filterByUnfinishedAttempts`
- Retrieves quizzes with pagination, sorting, and filtering options
- Example output: 
    ```json
    {
        "data": [
            {
                "id": "e91c1c38-604d-4422-5f22-08dd54cff4f5",
                "name": "World Capitals Challenge",
                "description": "Test your knowledge of the world's capitals. Choose the correct answers and see how well you know the capitals of different countries.",
                "creationDate": "2025-02-24T12:39:51.45",
                "hasImage": true,
                "categories": [
                    "geography",
                    "countries",
                    "capitals"
                ],
                "isPrivate": false,
                "isDeprecated": false,
                "latestVersionId": null,
                "questionCount": 3,
                "creatorId": "e1ba0a05-e591-49f6-ba24-4d2853874c58",
                "canEdit": true,
                "hasUnfinishedAttempt": false,
                "publicImageUrl": "/api/uploads/e8c24880-c7cf-423f-99e8-041537d6b200.png"
            }
        ],
        "pagination": {
            "totalItemCount": 1,
            "totalPageCount": 1,
            "pageSize": 5,
            "currentPage": 1
        }
    }
    ```

### GET: `/api/quizzes/{{quizId}}`
- __Parameters:__ `quizId`
- Retrieves detailed information about the specified quiz
- Example output:
    ```json
    {
        "id": "e91c1c38-604d-4422-5f22-08dd54cff4f5",
        "name": "World Capitals Challenge",
        "description": "Test your knowledge of the world's capitals. Choose the correct answers and see how well you know the capitals of different countries.",
        "creationDate": "2025-02-24T12:39:51.45",
        "hasImage": true,
        "categories": [
            "geography",
            "countries",
            "capitals"
        ],
        "isPrivate": false,
        "isDeprecated": false,
        "latestVersionId": null,
        "questionCount": 3,
        "creatorId": "e1ba0a05-e591-49f6-ba24-4d2853874c58",
        "canEdit": true,
        "hasUnfinishedAttempt": false,
        "publicImageUrl": "/api/uploads/e8c24880-c7cf-423f-99e8-041537d6b200.png"
    }
    ```

### PUT: `/api/quizzes/{{quizId}}`
- __Parameters:__ `quizId`, `name`, `description`, `categories[]`
- Updates the details of the specified quiz

### DELETE: `/api/quizzes/{{quizId}}`
- __Parameters:__ `quizId`
- Deletes the specified quiz

### PATCH: `/api/quizzes/{{quizId}}/privacy`
- __Parameters:__ `quizId`, `isPrivate`
- Changes the privacy of the quiz (sets it to either private or public)

### GET: `/api/quizzes/{{quizId}}/attempts/latest`
- __Parameters:__ `quizId`
- Retrieves detailed information about the latest finished attempt for the specified quiz
- Example output:
    ```json
    {
        "userAnsweredQuestions": [
            {
                "question": {
                    "id": 35,
                    "content": "The capital of Australia is Sydney",
                    "type": 1,
                    "order": 1,
                    "hasImage": false,
                    "multipleChoiceAnswers": [],
                    "openEndedAnswer": null,
                    "trueFalseAnswer": {
                        "isCorrect": false
                    },
                    "publicImageUrl": null
                },
                "userAnswer": {
                    "id": "19111156-7d19-45ef-9e21-08dd54d40a0e",
                    "answerText": [
                        "true"
                    ],
                    "isCorrect": false,
                    "answeredAt": "2025-02-24T13:06:23.5366667"
                }
            },
            {
                "question": {
                    "id": 36,
                    "content": "What is the capital of Japan?",
                    "type": 2,
                    "order": 2,
                    "hasImage": false,
                    "multipleChoiceAnswers": [],
                    "openEndedAnswer": {
                        "validContent": [
                            "Tokyo"
                        ],
                        "verificationByAI": true,
                        "ignoreCaseAndSpaces": true
                    },
                    "trueFalseAnswer": null,
                    "publicImageUrl": null
                },
                "userAnswer": {
                    "id": "744ac123-e489-4996-9e22-08dd54d40a0e",
                    "answerText": [
                        "tokyo"
                    ],
                    "isCorrect": true,
                    "answeredAt": "2025-02-24T13:06:45.3233333"
                }
            },
            {
                "question": {
                    "id": 37,
                    "content": "Which of the following are capitals of European countries?",
                    "type": 0,
                    "order": 3,
                    "hasImage": false,
                    "multipleChoiceAnswers": [
                        {
                            "content": "Paris",
                            "isCorrect": true
                        },
                        {
                            "content": "Rome",
                            "isCorrect": true
                        },
                        {
                            "content": "London",
                            "isCorrect": true
                        },
                        {
                            "content": "Madrid",
                            "isCorrect": true
                        },
                        {
                            "content": "Barcelona",
                            "isCorrect": false
                        }
                    ],
                    "openEndedAnswer": null,
                    "trueFalseAnswer": null,
                    "publicImageUrl": null
                },
                "userAnswer": {
                    "id": "e4b33b12-4d24-48a2-9e23-08dd54d40a0e",
                    "answerText": [
                        "Paris",
                        "Rome",
                        "London"
                    ],
                    "isCorrect": false,
                    "answeredAt": "2025-02-24T13:07:23.2133333"
                }
            }
        ],
        "quizAttempt": {
            "id": "12ba9151-9393-4851-983d-08dd54d4033a",
            "quizId": "e91c1c38-604d-4422-5f22-08dd54cff4f5",
            "startedAt": "2025-02-24T13:06:12.08",
            "finishedAt": "2025-02-24T13:07:23.186909",
            "correctAnswerCount": 1,
            "questionCount": 3,
            "quizName": "World Capitals Challenge"
        }
    }
    ```

### GET: `/api/quizzes/{{quizId}}/stats`
- __Parameters:__ `quizId`, `includeDeprecatedVersions`
- Retrieves statistics for the specified quiz, with an option to include deprecated versions in the stats
- Example output:
    ```json
    {
        "quizAttemptsCount": 1,
        "averageCorrectAnswers": 0.3333333333333333,
        "averageTimeSpent": "00:01:11"
    }
    ```

## 🔹 QuizzesImage

### GET: `/api/quizzes/{{quizId}}/image`
- __Parameters:__ `quizId`
- Retrieves the image for the specified private quiz

### PATCH: `/api/quizzes/{{quizId}}/image`
- __Parameters:__ `quizId`, `image`
- Updates the image for the specified quiz

### DELETE: `/api/quizzes/{{quizId}}/image`
- __Parameters:__ `quizId`
- Deletes the image of the specified quiz

## 🔹 Questions

### GET: `/api/quizzes/{{quizId}}/questions`
- __Parameters:__ `quizId`
- Retrieves all questions for the specified quiz
- Example output:
    ```json
    [
        {
            "id": 35,
            "content": "The capital of Australia is Sydney",
            "type": 1,
            "order": 1,
            "hasImage": false,
            "multipleChoiceAnswers": [],
            "openEndedAnswer": null,
            "trueFalseAnswer": {
                "isCorrect": false
            },
            "publicImageUrl": null
        },
        {
            "id": 36,
            "content": "What is the capital of Japan?",
            "type": 2,
            "order": 2,
            "hasImage": false,
            "multipleChoiceAnswers": [],
            "openEndedAnswer": {
                "validContent": [
                    "Tokyo"
                ],
                "verificationByAI": true,
                "ignoreCaseAndSpaces": true
            },
            "trueFalseAnswer": null,
            "publicImageUrl": null
        },
        {
            "id": 37,
            "content": "Which of the following are capitals of European countries?",
            "type": 0,
            "order": 3,
            "hasImage": false,
            "multipleChoiceAnswers": [
                {
                    "content": "Paris",
                    "isCorrect": true
                },
                {
                    "content": "Rome",
                    "isCorrect": true
                },
                {
                    "content": "London",
                    "isCorrect": true
                },
                {
                    "content": "Madrid",
                    "isCorrect": true
                },
                {
                    "content": "Barcelona",
                    "isCorrect": false
                }
            ],
            "openEndedAnswer": null,
            "trueFalseAnswer": null,
            "publicImageUrl": null
        }
    ]
    ```

### GET: `/api/quizzes/{{quizId}}/questions/next`
- __Parameters:__ `quizId`
- Retrieves the current question for the ongoing quiz attempt
- Example output:
    ```json
    {
        "id": 35,
        "content": "The capital of Australia is Sydney",
        "type": 1,
        "order": 1,
        "hasImage": false,
        "multipleChoiceAnswers": [],
        "questionCount": 3,
        "publicImageUrl": null
    }
    ```

### GET: `/api/quizzes/{{quizId}}/questions/order/{{orderNumber}}`
- __Parameters:__ `quizId`, `orderNumber`
- Retrieves detailed information about a specific question in the quiz by its order number
- Example output:
    ```json
    {
        "id": 37,
        "content": "Which of the following are capitals of European countries?",
        "type": 0,
        "order": 3,
        "hasImage": false,
        "multipleChoiceAnswers": [
            {
                "content": "Paris",
                "isCorrect": true
            },
            {
                "content": "Rome",
                "isCorrect": true
            },
            {
                "content": "London",
                "isCorrect": true
            },
            {
                "content": "Madrid",
                "isCorrect": true
            },
            {
                "content": "Barcelona",
                "isCorrect": false
            }
        ],
        "openEndedAnswer": null,
        "trueFalseAnswer": null,
        "publicImageUrl": null
    }
    ```

### PATCH: `/api/quizzes/{{quizId}}/questions/order`
- __Parameters:__ `quizId`, `orderChanges: [questionId, newOrder]`
- Updates the order of questions in the specified quiz

### POST: `api/quizzes/{{quizId}}/questions/answer`
- __Parameters:__ `quizId`, `userAnswer[]`
- Submits an answer for the current question of the ongoing quiz attempt

### DELETE: `api/quizzes/{{quizId}}/questions/{{questionId}}`
- __Parameters:__ `quizId`, `questionId`
- Deletes the specified question from the quiz

## 🔹 QuestionsImage

### GET: `/api/quizzes/{{quizId}}/questions/{{questionId}}/image`
- __Parameters:__ `quizId`, `questionId`
- Retrieves the image for the specified question of the private quiz

### PATCH: `/api/quizzes/{{quizId}}/questions/{{questionId}}/image`
- __Parameters:__ `quizId`, `questionId`, `image`
- Updates the image for the specified question of the quiz

### DELETE: `/api/quizzes/{{quizId}}/questions/{{questionId}}/image`
- __Parameters:__ `quizId`, `questionId`
- Deletes the image of the specified question of the quiz

## 🔹 Identity

### POST: `/api/identity/register`
- __Parameters:__ `email`, `password`
- Registers a new user with the provided details

### POST: `/api/identity/login`
- __Parameters:__ `email`, `password`
- Authenticates a user and returns both access and refresh tokens upon successful login
- Example output:
    ```json
    {
        "tokenType": "Bearer",
        "accessToken": "CfDJ8KYPMFRNa2NGsZYBbRtvUZH7JPxPwlI0QmbI73u...",
        "expiresIn": 3600,
        "refreshToken": "CfDJ8KYPMFRNa2NGsZYBbRtvUZGWdnDvsXfrQgTQe0..."
    }
    ```

### POST: `/api/identity/refresh`
- __Parameters:__ `refreshToken`
- Refreshes both access and refresh tokens using the provided refresh token.
- Example output:
    ```json
    {
        "tokenType": "Bearer",
        "accessToken": "CfDJ8KYPMFRNa2NGsZYBbRtvUZH7JPxPwlI0QmbI73u...",
        "expiresIn": 3600,
        "refreshToken": "CfDJ8KYPMFRNa2NGsZYBbRtvUZGWdnDvsXfrQgTQe0..."
    }
    ```

### GET: `/api/identity/manage/info`
- __Parameters:__ None
- Retrieves the user's email
- Example output:
    ```json
    {
        "email": "test@test.com",
    }
    ```

### POST: `/api/identity/manage/info`
- __Parameters:__ `newPassword`, `oldPassword`
- Updates the user's password

## 🔹 TrueFalseQuestions

### POST: `api/quizzes/{{quizId}}/questions/true-false`
- __Parameters:__ `quizId`, `content`, `isCorrect`
- Creates a new true/false question for the specified quiz

### PUT: `api/quizzes/{{quizId}}/questions/true-false/{{questionId}}`
- __Parameters:__ `quizId`, `questionsId`, `content`, `isCorrect`
- Updates the specified true/false question for the quiz

### GET: `api/quizzes/{{quizId}}/questions/true-false/generate`
- __Parameters:__ `quizId`, `suggestions`
- Generates a true/false question based on specified quiz content and optional suggestions
- Example output:
    ```json
    {
        "questionContent": "The capital of Canada is Ottawa.",
        "isCorrect": true
    }
    ```

## 🔹 MultipleChoiceQuestions

### POST: `api/quizzes/{{quizId}}/questions/multiple-choice`
- __Parameters:__ `quizId`, `content`, `answers: [content, isCorrect]`
- Creates a new multiple choice question for the specified quiz

### PUT: `api/quizzes/{{quizId}}/questions/multiple-choice/{{questionId}}`
- __Parameters:__ `quizId`, `questionId`, `content`, `answers: [content, isCorrect]`
- Updates the specified multiple choice question for the quiz

### GET: `api/quizzes/{{quizId}}/questions/multiple-choice/generate`
- __Parameters:__ `quizId`, `suggestions`
- Generates a multiple choice question based on specified quiz content and optional suggestions
- Example output:
    ```json
    {
        "questionContent": "Which of the following cities is the capital of a South American country?",
        "answers": [
            {
                "content": "Buenos Aires",
                "isCorrect": true
            },
            {
                "content": "Santiago",
                "isCorrect": true
            },
            {
                "content": "Lima",
                "isCorrect": true
            },
            {
                "content": "Rio de Janeiro",
                "isCorrect": false
            },
            {
                "content": "Caracas",
                "isCorrect": true
            },
            {
                "content": "São Paulo",
                "isCorrect": false
            },
            {
                "content": "Bogotá",
                "isCorrect": true
            },
            {
                "content": "La Paz",
                "isCorrect": true
            }
        ]
    }
    ```

## 🔹 OpenEndedQuestions

### POST: `api/quizzes/{{quizId}}/questions/open-ended`
- __Parameters:__ `quizId`, `content`, `answers[]`, `verificationByAI`, `ignoreCaseAndSpaces`
- Creates a new open-ended question for the specified quiz, with optional AI-based answer verification and case/space insensitivity for answers 

### PUT: `api/quizzes/{{quizId}}/questions/open-ended/{{questionId}}`
- __Parameters:__ `quizId`, `content`, `answers[]`, `verificationByAI`, `ignoreCaseAndSpaces`
- Updates the specified open-ended question for the quiz, allowing optional AI-based answer verification and case/space insensitivity

### GET: `api/quizzes/{{quizId}}/questions/open-ended/generate`
- __Parameters:__ `quizId`, `suggestions`
- Generates an open-ended question based on specified quiz content and optional suggestions
- Example output:
```json
{
    "questionContent": "What is the capital of Poland?",
    "validContent": [
        "Warsaw"
    ],
    "verificationByAI": true,
    "ignoreCaseAndSpaces": true
}
```

## 🔹 QuizAttempts

### GET: `/api/quiz-attempts/{{quizAttemptId}}`
- __Parameters:__ `quizAttemptId`
- Retrieves detailed information about the specified quiz attempt
- Example output:
    ```json
    {
        "userAnsweredQuestions": [
            {
                "question": {
                    "id": 35,
                    "content": "The capital of Australia is Sydney",
                    "type": 1,
                    "order": 1,
                    "hasImage": false,
                    "multipleChoiceAnswers": [],
                    "openEndedAnswer": null,
                    "trueFalseAnswer": {
                        "isCorrect": false
                    },
                    "publicImageUrl": null
                },
                "userAnswer": {
                    "id": "19111156-7d19-45ef-9e21-08dd54d40a0e",
                    "answerText": [
                        "true"
                    ],
                    "isCorrect": false,
                    "answeredAt": "2025-02-24T13:06:23.5366667"
                }
            },
            {
                "question": {
                    "id": 36,
                    "content": "What is the capital of Japan?",
                    "type": 2,
                    "order": 2,
                    "hasImage": false,
                    "multipleChoiceAnswers": [],
                    "openEndedAnswer": {
                        "validContent": [
                            "Tokyo"
                        ],
                        "verificationByAI": true,
                        "ignoreCaseAndSpaces": true
                    },
                    "trueFalseAnswer": null,
                    "publicImageUrl": null
                },
                "userAnswer": {
                    "id": "744ac123-e489-4996-9e22-08dd54d40a0e",
                    "answerText": [
                        "tokyo"
                    ],
                    "isCorrect": true,
                    "answeredAt": "2025-02-24T13:06:45.3233333"
                }
            },
            {
                "question": {
                    "id": 37,
                    "content": "Which of the following are capitals of European countries?",
                    "type": 0,
                    "order": 3,
                    "hasImage": false,
                    "multipleChoiceAnswers": [
                        {
                            "content": "Paris",
                            "isCorrect": true
                        },
                        {
                            "content": "Rome",
                            "isCorrect": true
                        },
                        {
                            "content": "London",
                            "isCorrect": true
                        },
                        {
                            "content": "Madrid",
                            "isCorrect": true
                        },
                        {
                            "content": "Barcelona",
                            "isCorrect": false
                        }
                    ],
                    "openEndedAnswer": null,
                    "trueFalseAnswer": null,
                    "publicImageUrl": null
                },
                "userAnswer": {
                    "id": "e4b33b12-4d24-48a2-9e23-08dd54d40a0e",
                    "answerText": [
                        "Paris",
                        "Rome",
                        "London"
                    ],
                    "isCorrect": false,
                    "answeredAt": "2025-02-24T13:07:23.2133333"
                }
            }
        ],
        "quizAttempt": {
            "id": "12ba9151-9393-4851-983d-08dd54d4033a",
            "quizId": "e91c1c38-604d-4422-5f22-08dd54cff4f5",
            "startedAt": "2025-02-24T13:06:12.08",
            "finishedAt": "2025-02-24T13:07:23.186909",
            "correctAnswerCount": 1,
            "questionCount": 3,
            "quizName": "World Capitals Challenge"
        }
    }
    ```


### GET: `/api/quiz-attempts`
- __Parameters:__ `searchPhrase`, `pageNumber`, `pageSize`, `sortBy`, `sortDirection`, `filterByQuizId`, `filterByStartedAtYearAndMonth`, `filterByFinishedAtYearAndMonth`
- Retrieves quiz attempts with pagination, sorting, and filtering options
- Example output:
    ```json
    {
        "data": [
            {
                "id": "12ba9151-9393-4851-983d-08dd54d4033a",
                "quizId": "e91c1c38-604d-4422-5f22-08dd54cff4f5",
                "startedAt": "2025-02-24T13:06:12.08",
                "finishedAt": "2025-02-24T13:07:23.186909",
                "correctAnswerCount": 1,
                "questionCount": 3,
                "quizName": "World Capitals Challenge"
            }
        ],
        "pagination": {
            "totalItemCount": 1,
            "totalPageCount": 1,
            "pageSize": 5,
            "currentPage": 1
        }
    }
    ```

## 🔹 QuizPermissions

### PUT: `/api/quizzes/{{quizId}}/permissions/{{userEmail}}`
- __Parameters:__ `quizId`, `userEmail`, `canEdit`, `canPlay`
- Sets the permissions for the specified user on the given quiz, determining whether they can edit or play it

### GET: `/api/quizzes/{{quizId}}/permissions`
- __Parameters:__ `quizId`
- Retrieves the permissions set for users associated with the specified quiz
- Example output:
    ```json
    [
        {
            "id": "c5c9c8ac-b138-45ef-9924-08dd5511b166",
            "userEmail": "test@test.com",
            "canEdit": true,
            "canPlay": true
        }
    ]
    ```
