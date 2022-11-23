namespace Indra.Net.Dtos {

  public record Response(string? Message, object? Value = null, bool Success = true);
}
