using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Interpreter;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    private Interpreter.Interpreter _inter;
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private async Task Run()
    {
        _inter = new();
        _inter.VymazVystup();
        await _inter.Run(VstupPole.Text);
        VystupPole.Text = _inter.Vysptup;
        if (_inter.VystupChyba != "")
        {
            VystupPole.Text = _inter.VystupChyba;
        }
    }

    private async void Run_Code(object? sender, RoutedEventArgs e)
    {
        await Run();
    }
    
    private async void Save_Code(object? sender, RoutedEventArgs e)
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Title = "Code save";
        dialog.InitialFileName = "YourCode";
        dialog.DefaultExtension = ".txt";
        FileDialogFilter filter = new FileDialogFilter();
        filter.Name = "Text Files";
        filter.Extensions.Add("txt");
        dialog.Filters.Add(filter);
        var result = await dialog.ShowAsync(this);

        if (result != null)
        {
            File.WriteAllText(result, VstupPole.Text, Encoding.UTF8);
        }
    }

    private async void Load_Code(object? sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter { Name = "Text Files", Extensions = { "txt" } });
        dialog.Title = "Load code";
        var result = await dialog.ShowAsync(this);

        if (result != null)
        {
            var vstupTxt = string.Join("", result);
            var text = File.ReadAllText(vstupTxt);
            VstupPole.Text = text;
        }
    }


    private void OdeslatInput_OnClick(object? sender, RoutedEventArgs e)
    {
        _inter.Input = VystupPoleInput.Text;
        _inter.Pokracuj = true;
        VystupPoleInput.Text = "";
    }
}