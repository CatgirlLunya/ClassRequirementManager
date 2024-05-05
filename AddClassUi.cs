using System.Numerics;
using ImGuiNET;

namespace ClassRequirementManager;

public static class AddClassUi
{
    private static string _nameBuffer = "";
    private static string _codeBuffer = "";
    private static string _prereqBuffer = "";
    private static int _creditHours = 0;
    private static bool _completed;
    private static bool _opened = false;

    public static void Open(ref bool flag)
    {
        var open = true;
        ImGui.SetNextWindowSize(new Vector2(400, 150));
        if (ImGui.BeginPopupModal("Add Class", ref open, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove))
        {
            ImGui.Text("Class Name: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(400 - ImGui.CalcTextSize("Class Name:    ").X);
            ImGui.InputTextWithHint("##name", "Ex: Calculus I", ref _nameBuffer, 40);
            
            ImGui.Text("Class Code: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(400 - ImGui.CalcTextSize("Class Code:    ").X);
            ImGui.InputTextWithHint("##code", "Ex: ENG 1001 or ENG-1001", ref _codeBuffer, 12);
            
            ImGui.Text("Class Credit Hours: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(400 - ImGui.CalcTextSize("Class Credit Hours:    ").X);
            ImGui.InputInt("##hours", ref _creditHours, 0, 0, ImGuiInputTextFlags.CharsDecimal);
            
            ImGui.Text("Class Prerequisites: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(400 - ImGui.CalcTextSize("Class Prerequisites:    ").X);
            ImGui.InputTextWithHint("##prereq", "Ex: ENG 1001, ENG 1002", ref _prereqBuffer, 64);
            
            ImGui.Text("Completed? ");
            ImGui.SameLine();
            ImGui.Checkbox("##done", ref _completed);
            
            if (ImGui.Button("Submit") && _nameBuffer.Length > 0 && _codeBuffer.Length > 0 && _creditHours > 0)
            {
                var record = new ClassRecord(_nameBuffer, _codeBuffer.Replace(' ', '-'), _creditHours, 
                    _prereqBuffer.Replace(", ", ",").Replace(' ', '-').Split([',', ' ']).ToList().FindAll(s => s != " " && s != ""),
                    _completed);
                
                DataManager.Classes.Add(record);
                ImGui.CloseCurrentPopup();
                _opened = false;
                flag = false;
            }
            
            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                ImGui.CloseCurrentPopup();
                _opened = false;
                flag = false;
            }
            ImGui.EndPopup();
            if (!_opened) return;
        }

        if (!_opened)
        {
            Console.WriteLine("Opened Class Addition Popup");
            _opened = true;
            ImGui.OpenPopup("Add Class");
        }
    }
}