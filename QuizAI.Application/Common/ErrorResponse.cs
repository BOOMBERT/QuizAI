using System.Net;

namespace QuizAI.Application.Common;

public record ErrorResponse(string Title, HttpStatusCode Status, object Details, string Path, string TraceIdentifier);
