using System.Data.SqlClient;
using System.Diagnostics;

string serverName = "";
string login = "";
string password = "";
string exportPath = @"C:\Projects\Informations";
string localServerName = @"(LocalDB)\MSSQLLocalDB";
string sqlPackagePath = GetSqlPackagePath();

string[] databases = new string[]
{
    "database"
};

if (!Directory.Exists(exportPath))
{
    Directory.CreateDirectory(exportPath);
}

foreach (string db in databases)
{
    string bacpacFile = Path.Combine(exportPath, $"{db}.bacpac");
    ExportDatabase(sqlPackagePath, serverName, db, login, password, bacpacFile);
    DropDatabase(localServerName, db);
    ImportDatabase(sqlPackagePath, localServerName, db, bacpacFile);
}

string GetSqlPackagePath()
{
    string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
    foreach (string path in paths)
    {
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "SqlPackage.exe", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                return files[0];
            }
        }
    }
    throw new FileNotFoundException("SqlPackage.exe not found.");
}

void ExportDatabase(string sqlPackagePath, string serverName, string dbName, string login, string password, string bacpacFile)
{
    ProcessStartInfo startInfo = new ProcessStartInfo
    {
        FileName = sqlPackagePath,
        Arguments = $"/Action:Export /ssn:{serverName} /sdn:{dbName} /su:{login} /sp:{password} /tf:{bacpacFile}",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden
    };

    using (Process process = Process.Start(startInfo))
    {
        process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
        process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        if (!process.WaitForExit(600000)) // 10 dakika zaman aşımı
        {
            process.Kill();
            throw new Exception($"Export timed out for {dbName}. Process was killed.");
        }

        if (process.ExitCode != 0)
        {
            throw new Exception($"Export failed for {dbName}. Error: {process.StandardError.ReadToEnd()}");
        }
    }
}

void DropDatabase(string serverName, string dbName)
{
    using (SqlConnection connection = new SqlConnection($"Server={serverName};Integrated Security=true;"))
    {
        connection.Open();
        using (SqlCommand command = new SqlCommand($"IF EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}') DROP DATABASE [{dbName}];", connection))
        {
            command.ExecuteNonQuery();
        }
    }
}

void ImportDatabase(string sqlPackagePath, string serverName, string dbName, string bacpacFile)
{
    ProcessStartInfo startInfo = new ProcessStartInfo
    {
        FileName = sqlPackagePath,
        Arguments = $"/Action:Import /tsn:{serverName} /tdn:{dbName} /sf:{bacpacFile}",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden
    };


    using (Process process = Process.Start(startInfo))
    {
        process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
        process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        if (!process.WaitForExit(600000)) // 10 dakika zaman aşımı
        {
            process.Kill();
            throw new Exception($"Import timed out for {dbName}. Process was killed.");
        }

        if (process.ExitCode != 0)
        {
            throw new Exception($"Import failed for {dbName}. Error: {process.StandardError.ReadToEnd()}");
        }
    }
}