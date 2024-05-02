using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ClassRequirementManager;

class Program
{
    public static Sdl2Window? Window;
    private static GraphicsDevice? _gd;
    private static ImGuiController? _controller;
    private static CommandList? _cl;
    private static string _lastOpened = "";
    private static string _lastClosed = "";

    private static bool _addClass;

    private static Vector3 _clearColor = new(0.45f, 0.55f, 0.6f);

    public static bool SaveDialog = false;
    static void Main()
    {
        if (!DataManager.Load())
        {
            DataManager.Classes.Add(new("MAA-1001",3, new List<string>(), true));
            DataManager.Classes.Add(new("MAA-1002",5, new List<string>(["MAA-1001"]), true));
            DataManager.Classes.Add(new("MAA-1003",5, new List<string>(["MAA-1001"]), true));
            DataManager.Classes.Add(new("MAA-1004",5, new List<string>(["MAA-1001", "MAA-1003"]), true));
        }
        
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(50, 50, 800, 800, WindowState.Normal, "Class Requirement Manager"),
            new GraphicsDeviceOptions(false, null, true, ResourceBindingModel.Default, true, true),
            preferredBackend: GraphicsBackend.OpenGL,
            out Window,
            out _gd
        );

        Window.SetCloseRequestedHandler(() =>
        {
            var loaded = DataManager.LoadClasses();
            if (DataManager.Classes.Count != loaded.Count) goto NotEqual;
            for (var i = 0; i < DataManager.Classes.Count; i++)
            {
                if (loaded[i] != DataManager.Classes[i]) goto NotEqual;
            }

            return false;
            
            NotEqual:
            SaveDialog = true;

            return true;
        });

        Window.Resized += () =>
        {
            _gd.MainSwapchain.Resize((uint)Window.Width, (uint)Window.Height);
            _controller.WindowResized(Window.Width, Window.Height);
        };

        _cl = _gd.ResourceFactory.CreateCommandList();
        _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, Window.Width,
            Window.Height);
        
        var stopwatch = Stopwatch.StartNew();
        while (Window.Exists)
        {
            float deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
            stopwatch.Restart();

            InputSnapshot snapshot = Window.PumpEvents();
            if (!Window.Exists) break;
            _controller.Update(deltaTime, snapshot);

            MainUi.Display();
            
            _cl.Begin();
            _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
            _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1));
            _controller.Render(_gd, _cl);
            _cl.End();
            _gd.SubmitCommands(_cl);
            _gd.SwapBuffers(_gd.MainSwapchain);
        }
        
        _gd.WaitForIdle();
        _controller.Dispose();
        _cl.Dispose();
        _gd.Dispose();
    }

    
}