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

            var node = ImGui.TreeNodeEx(record.Name, ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.AllowOverlap);
            // Up and down arrows
            {
                ImGui.SameLine();
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X - 50);
                if (j == DataManager.Classes.Count - 1) ImGui.BeginDisabled();
                if (ImGui.ArrowButton("##down" + record.Name, ImGuiDir.Down))
                {
                    var temp = DataManager.Classes[j + 1];
                    DataManager.Classes[j + 1] = record;
                    DataManager.Classes[j] = temp;
                }

                if (j == DataManager.Classes.Count - 1) ImGui.EndDisabled();
                ImGui.SameLine();
                if (j == 0) ImGui.BeginDisabled();
                if (ImGui.ArrowButton("##up" + record.Name, ImGuiDir.Up))
                {
                    var temp = DataManager.Classes[j - 1];
                    DataManager.Classes[j - 1] = record;
                    DataManager.Classes[j] = temp;
                }

                if (j == 0) ImGui.EndDisabled();
            }
            if (node)
            {
                var recordDone = record.Done;
                var recordHours = record.CreditHours;
                var buffer = Encoding.UTF8.GetBytes(record.Name);
                Array.Resize(ref buffer, 40);
                ImGui.Text("Class Name: ");
                ImGui.SameLine();
                if (ImGui.InputText("##name" + record.Name, buffer, 40,
                        ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    // TODO: Code duplication fix
                    record.Name = Encoding.Default.GetString(buffer);
                    record.Name = record.Name.Trim('\0');
                    _lastOpened = record.Code;
                }
                
                buffer = Encoding.UTF8.GetBytes(record.Code);
                Array.Resize(ref buffer, 12);
                ImGui.Text("Class Code: ");
                ImGui.SameLine();
                if (ImGui.InputText("##code" + record.Code, buffer, 12,
                        ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    // TODO: Code duplication fix
                    record.Code = Encoding.Default.GetString(buffer);
                    record.Code = record.Code.Trim('\0');
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
                        Console.WriteLine("Need prereq " + prereq);
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
                    // TODO: Are you sure?
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
        
        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2 - new Vector2(109F, 23F));
        if (ImGui.BeginPopup("SavePopup", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove))
        {
            ImGui.Text("Save Before Exiting?");
            if (ImGui.Button("Save"))
            {
                DataManager.Save();
                Program.Window!.Close();
            }
            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button("Close Anyways"))
            {
                Program.Window!.SetCloseRequestedHandler(() => false);
                Program.Window.Close();
            }
            ImGui.EndPopup();
        }
        
        if (Program.SaveDialog)
        {
            ImGui.OpenPopup("SavePopup");
            Program.SaveDialog = false;
        }
        ImGui.Columns();
        ImGui.End();
        ImGui.PopStyleVar(3);
    }
}