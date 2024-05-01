using System.Numerics;
using ImGuiNET;

namespace ClassRequirementManager;

public class AddPrereqUi
{
    private static bool _opened;
    private static ClassRecord _currentItem = null!;
    
    public static void Open(ref bool flag, ClassRecord record)
    {
        var open = true;
        ImGui.SetNextWindowSize(new Vector2(160, 80));
        var list = DataManager.Classes.FindAll(r => r != record && !record.Prerequisites.Contains(r.Code));
        
        if (!_opened)
        {
            Console.WriteLine("Opened Class Prerequisite Popup");
            _opened = true;
            _currentItem = list[0];
            ImGui.OpenPopup("Add Prereq");
        }
        
        if (ImGui.BeginPopupModal("Add Prereq", ref open, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove))
        {
            ImGui.Text("Pick a class to add:");
            if (ImGui.BeginCombo("##PrereqCombo", _currentItem.Code))
            {
                foreach (var classRecord in list)
                {
                    if (ImGui.Selectable(classRecord.Code))
                    {
                        _currentItem = classRecord;
                    }
                }
                ImGui.EndCombo();
            }
            
            if (ImGui.Button("Add"))
            {
                record.Prerequisites.Add(_currentItem.Code);
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
        }
    }
}