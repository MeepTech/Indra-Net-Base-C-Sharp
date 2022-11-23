namespace Indra.Net.Dtos {
  public record FailureResponse(string? Message = null, object? Error = null) 
    : Response(Message, Error, false);
}
