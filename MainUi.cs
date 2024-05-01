using System.Numerics;
using System.Text;
using ImGuiNET;

namespace ClassRequirementManager;

public static class MainUi
{
    private static bool _addClass, _addPrereq;
    private static string _lastOpened = "", _lastClosed = "";
    private static ClassRecord _prereqRecord = null!;

    private static void MenuBar()
    {
        if (ImGui.BeginMenuBar()) {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Save"))
                {
                    DataManager.Save();
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Classes"))
            {
                ImGui.MenuItem("Add Class", string.Empty, ref _addClass);
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Requirements"))
            {
                if (ImGui.BeginMenu("Add Requirement"))
                {
                    ImGui.MenuItem("Strict Requirement");
                    ImGui.MenuItem("Varied Requirement");
                    ImGui.EndMenu();
                }
                ImGui.MenuItem("View Requirements");
                ImGui.MenuItem("Assign Requirement to Major/Minor");
                ImGui.EndMenu();
            }
            ImGui.EndMenuBar();
        }
        
        if (_addClass) AddClassUi.Open(ref _addClass);
    }
    public static void Display()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);

        ImGui.Begin("Window", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.MenuBar);
        ImGui.SetWindowPos(new Vector2(0, 0));
        ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);

        MenuBar();

        ImGui.Columns(2);
        ImGui.Text("Classes Registered: ");
        for (var j = 0; j < DataManager.Classes.Count; j++)
        {
            var record = DataManager.Classes[j];
            if (_lastOpened == record.Code)
            {
                ImGui.SetNextItemOpen(true);
                _lastOpened = "";
            }

            if (_lastClosed == record.Code)
            {
                ImGui.SetNextItemOpen(false);
                _lastClosed = "";
            }
            if (ImGui.TreeNodeEx(record.Code, ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth))
            {
                var recordDone = record.Done;
                var recordHours = record.CreditHours;
                var buffer = Encoding.UTF8.GetBytes(record.Code);
                ImGui.Text("Class Code: ");
                ImGui.SameLine();
                if (ImGui.InputText("##code" + record.Code, buffer, (uint)record.Code.Length + 1,
                        ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    // TODO: Code duplication fix
                    record.Code = Encoding.Default.GetString(buffer);
                    _lastOpened = record.Code;
                }
                    
                ImGui.Text("Credit Hours: ");
                ImGui.SameLine();
                ImGui.InputInt("##hours" + record.Code, ref recordHours, 0, 0, ImGuiInputTextFlags.CharsDecimal);
                record.CreditHours = recordHours;
                    
                ImGui.Text("Pre-requisites: ");
                for (var i = 0; i < record.Prerequisites.Count; i++)
                {
                    var prereq = record.Prerequisites[i];
                    if (ImGui.Button(prereq))
                    {
                        _lastClosed = record.Code;
                        _lastOpened = prereq;
                    }
                    ImGui.SameLine();
                    if (ImGui.SmallButton("-##" + record.Code + prereq))
                    {
                        record.Prerequisites.Remove(prereq);
                        i--;
                    }
                }

                var cond = DataManager.Classes.Count - record.Prerequisites.Count < 2;
                if (cond) ImGui.BeginDisabled();
                if (ImGui.SmallButton("+"))
                {
                    _addPrereq = true;
                    _prereqRecord = record;
                }
                
                if (cond) ImGui.EndDisabled();
                    
                ImGui.Text("Done: ");
                ImGui.SameLine();
                ImGui.Checkbox("##checkbox" + record.Code, ref recordDone);
                record.Done = recordDone;
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetColumnWidth() - ImGui.CalcTextSize("XXX").X);
                if (ImGui.SmallButton("X"))
                {
                    DataManager.Classes.Remove(record);
                    j--;
                    foreach (var record1 in DataManager.Classes)
                    {
                        record1.Prerequisites.Remove(record.Code);
                    }
                }
                ImGui.TreePop();
            }
        }
        
        if (_addPrereq) AddPrereqUi.Open(ref _addPrereq, _prereqRecord);
        ImGui.Columns();
        ImGui.End();
        ImGui.PopStyleVar(3);
    }
}