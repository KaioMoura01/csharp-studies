namespace JcmSoft.EFCore.Configurations;

public static class AppConfig
{
    public static string GetConnectionString()
    {
        // return "Data Source=jcmsoft.db";
        return "Host=localhost;Port=5432;Database=jcmsoft;Username=postgres;Password=postgres";
    }
}