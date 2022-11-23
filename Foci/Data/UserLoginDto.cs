namespace Indra.Net.Dtos {
  public record UserLoginDto {
    public string UserName { get; init; }
    public string Email { get; init; }
    public string SaltedPassword { get; init; }
    public bool RememberMe { get; init; }
  }
}
