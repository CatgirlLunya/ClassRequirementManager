using System.Numerics;
using ImGuiNET;

namespace ClassRequirementManager;

public static class AddClassUi
{
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
            ImGui.InputTextWithHint("", "Ex: ENG 1001 or ENG-1001", ref _codeBuffer, 8);
            
            ImGui.Text("Class Credit Hours: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(400 - ImGui.CalcTextSize("Class Credit Hours:    ").X);
            ImGui.InputInt("##2", ref _creditHours, 0, 0, ImGuiInputTextFlags.CharsDecimal);
            
            ImGui.Text("Class Prerequisites: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(400 - ImGui.CalcTextSize("Class Prerequisites:    ").X);
            ImGui.InputTextWithHint("##3", "Ex: ENG 1001, ENG 1002", ref _prereqBuffer, 64);
            
            ImGui.Text("Completed? ");
            ImGui.SameLine();
            ImGui.Checkbox("##4", ref _completed);
            
            if (ImGui.Button("Submit") && _codeBuffer.Length > 0 && _creditHours > 0)
            {
                var record = new ClassRecord(_codeBuffer.Replace(' ', '-'), _creditHours, 
                    _prereqBuffer.Replace(", ", ",").Replace(' ', '-').Split([',', ' ']).ToList(), _completed);
                
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