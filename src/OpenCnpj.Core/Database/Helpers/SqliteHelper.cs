namespace OpenCnpj.Core.Database.Helpers;
public static class SqliteHelper
{
    public static string GetSqliteDbFolder()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        return Path.Combine(appData, "OpenCNPJ");
    }

    public static string GetSqliteDbFile()
        => Path.Combine(GetSqliteDbFolder(), "open_cnpj.db");

    public static string GetSqliteConnectionString()
        => string.Format("Data Source={0};", GetSqliteDbFile());

    public static void InitializeSqliteFolder()
    {
        Directory.CreateDirectory(GetSqliteDbFolder());
    }

}
