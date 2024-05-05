using System.Text.Json;

namespace ClassRequirementManager;

public static class DataManager
{
    public static List<ClassRecord> Classes = [];
    public static List<Track> Tracks = [];
    private const string ClassPath = "class_save.json";
    private const string TrackPath = "track_save.json";
    private static readonly string BaseDirectory = AppContext.BaseDirectory;
    
    public static bool Load()
    {
        if (!File.Exists(BaseDirectory + "/" + ClassPath)) return false;
        Classes = LoadClasses();
        if (!File.Exists(BaseDirectory + "/" + TrackPath)) return false;
        Tracks = LoadTracks();
        return true;
    }

    public static List<ClassRecord> LoadClasses()
    {
        if (!File.Exists(BaseDirectory + "/" + ClassPath)) return [];
        var data = File.ReadAllText(BaseDirectory + "/" + ClassPath);
        return JsonSerializer.Deserialize<List<ClassRecord>>(data) ?? [];
    }
    
    public static List<Track> LoadTracks()
    {
        if (!File.Exists(BaseDirectory + "/" + TrackPath)) return [];
        var data = File.ReadAllText(BaseDirectory + "/" + TrackPath);
        return JsonSerializer.Deserialize<List<Track>>(data) ?? [];
    }

    public static void Save()
    {
        Console.WriteLine("Saving");
        var data = JsonSerializer.Serialize(Classes);
        File.WriteAllText(BaseDirectory + "/" + ClassPath, data);
        data = JsonSerializer.Serialize(Tracks);
        File.WriteAllText(BaseDirectory + "/" + TrackPath, data);
    }
}