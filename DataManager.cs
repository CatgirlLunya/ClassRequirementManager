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
        Classes = LoadClasses();
        return true;
    }

    public static List<ClassRecord> LoadClasses()
    {
        if (!File.Exists(BaseDirectory + "/" + Path)) return [];
        var data = File.ReadAllText(BaseDirectory + "/" + Path);
        return JsonSerializer.Deserialize<List<ClassRecord>>(data) ?? [];
    }

    public static void Save()
    {
        Console.WriteLine("Saving");
        var data = JsonSerializer.Serialize(Classes);
        File.WriteAllText(BaseDirectory + "/" + Path, data);
    }
}