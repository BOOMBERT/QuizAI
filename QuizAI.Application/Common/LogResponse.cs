using System.Net;

namespace QuizAI.Application.Common;

public record LogResponse(string Method, string Path, string Query, string TraceIdentifier, object Details, HttpStatusCode StatusCode);
