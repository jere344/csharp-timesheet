using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using wisecorp.Models;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;


namespace wisecorp.ViewModels;

public class VMSettings : ObservableObject
{
    private ModelSettings _modelSettings { get; set; }
    public string Language
    {
        get => _modelSettings.Language;
        set
        {
            _modelSettings.Language = value;
            OnPropertyChanged();
        }
    }
    public string Theme
    {
        get => _modelSettings.Theme;
        set
        {
            _modelSettings.Theme = value;
            OnPropertyChanged();
        }
    }
    public string PrimaryColor
    {
        get => _modelSettings.PrimaryColor;
        set
        {
            _modelSettings.PrimaryColor = value;
            OnPropertyChanged();
        }
    }


    public List<string> Languages => _modelSettings.Languages;
    public List<string> Themes => _modelSettings.Themes;
    public List<string> PrimaryColors => _modelSettings.PrimaryColors;

    public VMSettings()
    {
        _modelSettings = new();
    }
}