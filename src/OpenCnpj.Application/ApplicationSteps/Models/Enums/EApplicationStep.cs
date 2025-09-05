using NpgsqlTypes;

namespace OpenCnpj.Application.ApplicationSteps.Models.Enums;

[PgName("application_step")]
public enum EApplicationStep
{
    [PgName("application_step")]
    StartedApplication = 0,
    DownloadingFiles = 1,
    ExtractingFiles = 2,
    ProcessingRawFiles = 3,
    FormattingRawData = 4,
}