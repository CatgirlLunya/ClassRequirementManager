namespace ClassRequirementManager;

public class ClassRecord(string code, int creditHours, List<string> prerequisites, bool done)
{
    public string Code { get; set; } = code;
    public int CreditHours { get; set; } = creditHours;
    public List<string> Prerequisites { get; set; } = prerequisites;
    public bool Done { get; set; } = done;
}