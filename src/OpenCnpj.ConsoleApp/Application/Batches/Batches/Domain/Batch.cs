using CSharpFunctionalExtensions;
using OpenCnpj.ConsoleApp.Constants;

namespace OpenCnpj.ConsoleApp.Application.Batches.Batches.Domain;
public class Batch
{
    public int ID { get; private set; }
    public string Identifier { get; private set; }
    public string Status { get; private set; }
    public string? Directory { get; private set; }

    private Batch(string identifier)
    {
        Identifier = identifier;
        Status = "Created";
    }

    public void SetID(int id)
        => ID = id; 

    public static Batch Create()
        => new(DateTime.Now.ToString("yyyy-MM"));

    public void Update(string newStatus)
    {
        Status = newStatus;
    }

    public Result CreateBatchDirectory()
    {
        try
        {
            var directory = Path.Combine(Paths.GovDataFolder, Identifier);

            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            Directory = directory;

            return Result.Success();
        }
        catch(Exception ex)
        {
            return Result.Failure(string.Format("An error occurred while trying to create the batch directory for the files. Exception: {0}", ex.ToString()));
        }
    }

    public Result DeleteBatchDirectory()
    {
        try
        {
            var verificationResult = VerifyIfDirectoryExists();
            if (verificationResult.IsFailure)
                return verificationResult;

            if (System.IO.Directory.Exists(Directory))
                return Result.Failure("The application could not find the directory of the batch files.");

            System.IO.Directory.Delete(Directory!);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(string.Format("An error occurred while trying to delete the batch directory. Exception: {0}", ex.ToString()));
        }
    }

    public Result VerifyIfDirectoryExists()
    {
        if (string.IsNullOrEmpty(Directory))
            return Result.Failure("The directory of the batch files doesn't exists.");

        return Result.Success();
    }
}
