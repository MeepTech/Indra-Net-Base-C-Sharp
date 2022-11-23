namespace Indra.Net.Data {
  public class ServerInfoDto {

    public string Address {
      get;
      protected set;
    }

    public string DisplayName {
      get;
      protected set;
    }

    public string Description {
      get;
      protected set;
    }

    public string IconUrl {
      get;
      protected set;
    }

    protected internal ServerInfoDto(Server source) {
      Address = source.Address;
      DisplayName = source.DisplayName;
      Description = source.Description;
      IconUrl = source.IconUrl;
    }
  }
}