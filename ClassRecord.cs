namespace ClassRequirementManager;

public class ClassRecord(string name, string code, int creditHours, List<string> prerequisites, bool done)
{
    public static bool operator ==(ClassRecord a, ClassRecord b)
    {
        return string.Join(',', a.Prerequisites) == string.Join(',', b.Prerequisites) && a.Done == b.Done && a.CreditHours == b.CreditHours &&
               a.Code == b.Code;
    }

    public static bool operator !=(ClassRecord a, ClassRecord b)
    {
        return !(a == b);
    }

    public string Name { get; set; } = name;
    public string Code { get; set; } = code;
    public int CreditHours { get; set; } = creditHours;
    public List<string> Prerequisites { get; set; } = prerequisites;
    public bool Done { get; set; } = done;
}