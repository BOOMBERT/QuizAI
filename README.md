# 🔹 Description
__QuizAI__ is a fully functional __API__ designed according to the principles of __Clean Architecture__ and the __CQRS__ pattern. 
It enables seamless __quiz management__ and includes __versioning__, providing access to __previous quiz attempts__ while ensuring that active attempts are __never interrupted__.

The API supports __three types of questions__ and includes a built-in __permission management system__, allowing __collaborative quiz creation__ as well as the ability to __grant access to private quizzes__ for selected users.

QuizAI supports __images__ that are __optimizing__, with their __storage and serving adjusted__ to the __quiz's privacy__ level.
It also offers __AI-powered__ functionalities for __verifying answer accuracy__ and __generating questions__.

With its __optimized structure__, QuizAI __avoids data duplication and unnecessary storage__, ensuring both __efficiency and performance__. 
It also provides __full validation__ and __error logging__.

# 🔹 Technologies
- Backend: .NET 8, ASP.NET Web API, MediatR
- Database: Microsoft SQL Server, Microsoft.EntityFrameworkCore, AspNetCore.Identity
- Logging and Validation: Serilog, FluentValidation
- Mapping and AI: AutoMapper, OpenAI
- Image Processing and Hashing: SixLabors.ImageSharp, Shipwreck.Phash.Bitmaps
