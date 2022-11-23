using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Indra.Net {
  internal interface IHaveDto {
    public Task<object> ToDto(IActor forActor, [NotNull] DbContext dbContext);
  }

  internal interface IHaveDto<T> : IHaveDto where T : class {
    async Task<object> IHaveDto.ToDto(IActor forActor, [NotNull] DbContext dbContext)
      => await ToDto(forActor, dbContext);

    public new Task<T> ToDto(IActor forActor, [NotNull] DbContext dbContext);
  }
}