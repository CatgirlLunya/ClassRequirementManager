using System.Numerics;
using System.Text;
using ImGuiNET;

namespace ClassRequirementManager;

public static class MainUi
{
    private static bool _addClass, _addPrereq, _addTrack;
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

                if (ImGui.MenuItem("Load Saved Data"))
                {
                    DataManager.Load();
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Classes"))
            {
                ImGui.MenuItem("Add Class", string.Empty, ref _addClass);
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Tracks"))
            {
                ImGui.MenuItem("Add Track", string.Empty, ref _addTrack);
                ImGui.EndMenu();
            }
            ImGui.EndMenuBar();
        }
        
        if (_addClass) AddClassUi.Open(ref _addClass);
        if (_addTrack) AddTrackUi.Open(ref _addTrack);
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
        
        // Class column
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
                if (ImGui.ArrowButton("##down" + record.Code, ImGuiDir.Down))
                {
                    
                    (DataManager.Classes[j + 1], DataManager.Classes[j]) =
                        (DataManager.Classes[j], DataManager.Classes[j + 1]);
                }

                if (j == DataManager.Classes.Count - 1) ImGui.EndDisabled();
                ImGui.SameLine();
                if (j == 0) ImGui.BeginDisabled();
                if (ImGui.ArrowButton("##up" + record.Code, ImGuiDir.Up))
                {
                    (DataManager.Classes[j - 1], DataManager.Classes[j]) =
                        (DataManager.Classes[j], DataManager.Classes[j - 1]);
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
                    ImGui.PushStyleColor(ImGuiCol.Text,
                        DataManager.Classes.Find(r => r.Code == prereq)!.Done
                            ? new Vector4(0, 255, 0, 255)
                            : new Vector4(255, 0, 0, 255));
                    if (ImGui.Button(prereq))
                    {
                        _lastClosed = record.Code;
                        _lastOpened = prereq;
                        Console.WriteLine("Need prereq " + prereq);
                    }
                    ImGui.PopStyleColor();
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
        
        // Track column
        ImGui.NextColumn();
        ImGui.Text("Tracks Registered: ");
        foreach (var track in DataManager.Tracks)
        {
            var node = ImGui.TreeNodeEx(track.Name, ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.AllowOverlap);
            if (!node) continue;
            
            ImGui.Text("Track Name: ");
            var buffer = Encoding.UTF8.GetBytes(track.Name);
            Array.Resize(ref buffer, 40);
            ImGui.SameLine();
            if (ImGui.InputText("##track_name" + track.Name, buffer, 40,
                    ImGuiInputTextFlags.EnterReturnsTrue))
            {
                // TODO: Code duplication fix
                track.Name = Encoding.Default.GetString(buffer);
                track.Name = track.Name.Trim('\0');
            }
                    
            foreach (var req in track.Records)
            {
                if (ImGui.Button(req.Name))
                {
                    _lastOpened = req.Code;
                }

                ImGui.SameLine();
                ImGui.Text("Done: ");
                var done = DataManager.Classes.Find(r => r.Code == req.Code)!.Done;
                ImGui.SameLine();
                ImGui.BeginDisabled();
                ImGui.Checkbox("##checkbox" + req.Code, ref done);
                ImGui.EndDisabled();
            }
        }
        
        // Save popup and end
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