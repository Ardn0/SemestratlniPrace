using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaApplication1.Interpreter;
using SemInterpreter;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private Task Run()
    {
        Parser.Vystup = "";
        Parser.VystpuChyba = "";
        Interpreter.Interpreter inter = new Interpreter.Interpreter(VstupPole.Text);
        Dispatcher.UIThread.Post(() =>  VystupPole.Text = Parser.Vystup , DispatcherPriority.Background);
        if (Parser.VystpuChyba != "")
        {
            Dispatcher.UIThread.Post(() =>  VystupPole.Text = Parser.VystpuChyba , DispatcherPriority.Background);
        }

        return Task.CompletedTask;
    }
    

    private void Run_Code(object? sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            Run();
        });
    }

    private async void Save_Code(object? sender, RoutedEventArgs e)
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Title = "Code save";
        dialog.InitialFileName = "YourCode";
        dialog.DefaultExtension = ".txt";
        var result = await dialog.ShowAsync(this);
        string vstupTxt = "";
        foreach (var VARIABLE in result)
        {
            vstupTxt += VARIABLE;
        }

        File.WriteAllText(vstupTxt, VstupPole.Text);
    }

    private async void Load_Code(object? sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Title = "Load code";
        var result = await dialog.ShowAsync(this);
        string vstupTxt = "";
        foreach (var VARIABLE in result)
        {
            vstupTxt += VARIABLE;
        }

        StreamReader reader = new StreamReader(vstupTxt);
        string text = reader.ReadToEnd();
        VstupPole.Text = text;
    }

    private void OdeslatInput_OnClick(object? sender, RoutedEventArgs e)
    {
        Parser.Input = VystupPoleInput.Text;
        Parser.Pokracuj = true;
        VystupPoleInput.Text = "";
    }
}