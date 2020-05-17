using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class LoggingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly StreamWriter _logStream;

  public LoggingMiddleware(RequestDelegate next)
  {
    _next = next;
    _logStream = new StreamWriter(Directory.GetCurrentDirectory() + "/requestsLog.txt");
    _logStream.AutoFlush = true;
  }

  public async Task InvokeAsync(HttpContext httpContext)
  {
    var req = httpContext.Request;
    string reqBody;
    using (var reader = new StreamReader(req.Body))
    {
      reqBody = await reader.ReadToEndAsync();
    }
    _logStream.WriteLine(req.Method + " " + req.Path.Value + " " + reqBody + " " + req.QueryString.Value);
    await _next(httpContext);
  }
}