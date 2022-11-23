namespace Indra.Net.Dtos {
  public record NewUserDto {
    public string UserName { get; init; }
    public string SaltedPassword { get; init; }
    public string Email { get; init; }
    public bool AcceptedTermsAndConditions { get; init; }
  }
}
