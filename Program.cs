using System;
using Gtk;
using VtHyprset.Helpers;

class Program
{
    static void Main(string[] args)
    {
        Application.Init();

        // Create main window
        var window = new Window("vt-hyprset");
        window.SetDefaultSize(1000, 600);
        window.DeleteEvent += (o, e) => Application.Quit();

        // Horizontal layout container
        var hbox = new Box(Orientation.Horizontal, 8)
        {
            BorderWidth = 10
        };

        // Sidebar
        var sidebar = new Box(Orientation.Vertical, 6)
        {
            WidthRequest = 200
        };

        var btnLoadConfig = new Button("View Config");
        sidebar.PackStart(btnLoadConfig, false, false, 0);

        // TextView inside a scrollable window
        var textView = new TextView
        {
            Editable = false,
            Monospace = true
        };

        var scroll = new ScrolledWindow();
        scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
        scroll.Add(textView);

        // Add widgets to layout
        hbox.PackStart(sidebar, false, false, 0);
        hbox.PackStart(scroll, true, true, 0);
        window.Add(hbox);

        // Button logic to load merged config
        btnLoadConfig.Clicked += (sender, e) =>
        {
            try
            {
                var configText = HyprlandConfigBuilder.BuildFullConfig();
                textView.Buffer.Text = configText;
            }
            catch (Exception ex)
            {
                textView.Buffer.Text = $"Error loading config:\n{ex.Message}";
            }
        };

        // Show everything
        window.ShowAll();
        Application.Run();
    }
}
