using CSharpFunctionalExtensions;
using OpenCnpj.Core;

namespace OpenCnpj.Application.Application.Commands;
public class PostManualApplicationScrapperCommand : IRequest<Result>
{
    public int? BatchMonth { get; set; }
    public int? BatchYear { get; set; }
}
