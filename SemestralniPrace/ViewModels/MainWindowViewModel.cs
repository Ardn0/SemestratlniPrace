using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly Window _window;
    private bool _dialogOtevren;

    public MainWindowViewModel(Window window)
    {
        _window = window;
    }

    private Interpreter.Interpreter? _inter;
    private string? _mujKod;
    private string? _vystup;

    public string? MujKod
    {
        get => _mujKod;
        set
        {
            _mujKod = value;
            OnPropertyChanged("MujKod");
        }
    }

    public string? Vystup
    {
        get => _vystup;
        set
        {
            _vystup = value;
            OnPropertyChanged("Vystup");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task Run()
    {
        _inter = new();
        _inter.VymazVystup();
        if (MujKod != null) await _inter.Run(MujKod);
        Vystup = _inter.Vysptup;
        if (_inter.VystupChyba != "")
        {
            Vystup = _inter.VystupChyba;
        }
    }

    private async void Run_Code()
    {
        await Run();
    }


    private async void Save_Code()
    {
        if (_dialogOtevren)
        {
            return;
        }

        _dialogOtevren = true;

        try
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Code save";
            dialog.InitialFileName = "YourCode";
            dialog.DefaultExtension = ".txt";
            FileDialogFilter filter = new FileDialogFilter();
            filter.Name = "Text Files";
            filter.Extensions.Add("txt");
            dialog.Filters.Add(filter);
            var result = await dialog.ShowAsync(_window);


            if (result != null)
            {
                File.WriteAllText(result, MujKod, Encoding.UTF8);
            }
        }
        finally
        {
            _dialogOtevren = false;
        }
    }

    private async void Load_Code()
    {
        if (_dialogOtevren)
        {
            return;
        }

        _dialogOtevren = true;

        try
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter { Name = "Text Files", Extensions = { "txt" } });
            dialog.Title = "Load code";
            var result = await dialog.ShowAsync(_window);

            if (result != null)
            {
                var vstupTxt = string.Join("", result);
                var text = File.ReadAllText(vstupTxt);
                MujKod = text;
            }
        }
        finally
        {
            _dialogOtevren = false;
        }
    }


    private void OdeslatInput_OnClick()
    {
        _inter.Input = Vystup;
        _inter.Pokracuj = true;
        Vystup = "";
    }
}