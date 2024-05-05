namespace ClassRequirementManager;

public class Track(string name, List<ClassRecord> records)
{
    public static bool operator ==(Track a, Track b)
    {
        return string.Join(',', a.Records) == string.Join(',', b.Records) && a.Name == b.Name;
    }

    public static bool operator !=(Track a, Track b)
    {
        return !(a == b);
    }
    
    public string Name { get; set; } = name;
    public List<ClassRecord> Records { get; set; } = records;
}