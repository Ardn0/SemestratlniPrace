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
    private string _programBezi = "Run";

    public MainWindowViewModel(Window window)
    {
        _window = window;
    }

    private Interpreter.Interpreter? _inter;
    private string? _mujKod;
    private string? _vystup;
    private string? _vstupInput;

    public string ProgramBezi
    {
        get => _programBezi;
        set
        {
            _programBezi = value;
            OnPropertyChanged("ProgramBezi");
        }
    }

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

    public string? VstupInput
    {
        get => _vstupInput;
        set
        {
            _vstupInput = value;
            OnPropertyChanged("VstupInput");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Run_Code()
    {
        if (ProgramBezi != "Stop")
        {
            _inter = new();
            _inter.VymazVystup();

            Task.Run(() =>
            {
                if (MujKod != " ")
                {
                    ProgramBezi = "Stop";
                    _inter.Run(MujKod);
                    if (ProgramBezi == "Stop")
                    {
                        if (_inter.Vystup != "")
                        {
                            Vystup = _inter.Vystup;
                        }

                        if (_inter.VystupChyba != "")
                        {
                            Vystup = _inter.VystupChyba;
                        }
                    }
                }

                ProgramBezi = "Run";
            });
        }
        else
        {
            ProgramBezi = "Run";
            Vystup = "Code canceled";
        }
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
                await File.WriteAllTextAsync(result, MujKod, Encoding.UTF8);
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
                var text = await File.ReadAllTextAsync(vstupTxt);
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
        _inter.Input = VstupInput;
        _inter.Pokracuj = true;
        VstupInput = "";
    }
}