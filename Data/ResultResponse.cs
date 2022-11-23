namespace Indra.Net.Dtos {
  public record ResultResponse<T>(T? Result, string? Message = null, bool Success = true)
    : Response(Message, Result, Success);
}
