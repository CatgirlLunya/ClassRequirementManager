using System.Text.Json;

namespace ClassRequirementManager;

public static class DataManager
{
    public static List<ClassRecord> Classes = [];
    private const string Path = "save_data.json";
    private static readonly string BaseDirectory = AppContext.BaseDirectory;
    
    public static bool Load()
    {
        if (!File.Exists(BaseDirectory + "/" + Path)) return false;
        var data = File.ReadAllText(BaseDirectory + "/" + Path);
        Classes = JsonSerializer.Deserialize<List<ClassRecord>>(data)!;
        return true;
    }

    public static void Save()
    {
        Console.WriteLine("Saving");
        var data = JsonSerializer.Serialize(Classes);
        File.WriteAllText(BaseDirectory + "/" + Path, data);
    }
}