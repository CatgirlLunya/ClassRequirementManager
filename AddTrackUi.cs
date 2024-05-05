using System.Numerics;
using ImGuiNET;

namespace ClassRequirementManager;

public class AddTrackUi
{
    private static string _trackName = "";
    private static bool _opened;
    private static readonly bool[] Flags = new bool[1000];
    private static List<ClassRecord> _classRecords = new();
    public static void Open(ref bool flag)
    {
        var open = true;
        ImGui.SetNextWindowSize(new Vector2(400, 150));
        if (ImGui.BeginPopupModal("Add Track", ref open, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove))
        {
            ImGui.Text("Track Name: ");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(400 - ImGui.CalcTextSize("Track Name:    ").X);
            ImGui.InputText("##track_name", ref _trackName, 40);

            ImGui.SetNextItemWidth(390);
            if (ImGui.BeginCombo("##combo", "Select the classes that are part of this track"))
            {
                foreach (var course in DataManager.Classes)
                {
                    ImGui.Text(course.Name + ": ");
                    ImGui.SameLine();
                    if (ImGui.Checkbox("##checkbox" + course.Code, ref Flags[DataManager.Classes.IndexOf(course)]))
                    {
                        _classRecords.Add(course);
                    }
                    else
                    {
                        if (!_classRecords.Contains(course) || Flags[DataManager.Classes.IndexOf(course)]) continue;
                        
                        _classRecords.Remove(course);
                        Console.Write("Removed " + course.Name);
                    }
                }
                
                ImGui.EndCombo();
            }
            
            if (ImGui.Button("Submit") && _trackName.Length > 0)
            {
                DataManager.Tracks.Add(new Track(_trackName, _classRecords));
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

        if (_opened) return;
        
        Console.WriteLine("Opened Track Addition Popup");
        _opened = true;
        Array.Clear(Flags);
        ImGui.OpenPopup("Add Track");
    }
}