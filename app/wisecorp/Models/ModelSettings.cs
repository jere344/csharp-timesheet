using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Globalization;

namespace wisecorp.Models;

public class ModelSettings
{
    private string _language;
    private string _theme;
    private string _primaryColor;
    public string Language {
        get => _language;
        set
        {
            _language = value;
            ((App)Application.Current).SavedSettings["Language"] = value;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(value);
            ((App)Application.Current).SetLanguageDictionary();
            ((MainWindow)((App)Application.Current).MainWindow).FillLeftDrawer();
        }
    }
    public string Theme {
        get => _theme;
        set
        {
            _theme = value;
            ((App)Application.Current).SavedSettings["Theme"] = value;
            ((App)Application.Current).SetTheme();
            // We also need to set the primary color when the theme changes for the bundle
            ((App)Application.Current).SetPrimaryColor();
            ((MainWindow)((App)Application.Current).MainWindow).FillLeftDrawer();
        }
    }
    public string PrimaryColor {
        get => _primaryColor;
        set
        {
            _primaryColor = value;
            ((App)Application.Current).SavedSettings["PrimaryColor"] = value;
            ((App)Application.Current).SetPrimaryColor();
            ((MainWindow)((App)Application.Current).MainWindow).FillLeftDrawer();
        }
    }


    public List<string> Languages { get; set; } = new List<string> { "en", "fr" };
    public List<string> Themes { get; set; } = new List<string> { "light", "dark" };
    public List<string> PrimaryColors { get; set; } = new List<string> { "Blue", "DeepPurple", "Indigo", "Teal", "Green", "Orange", "Red", "Pink" };
    public ModelSettings()
    {
        _language = ((App)Application.Current).SavedSettings["Language"]?.ToString() ?? Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        _theme = ((App)Application.Current).SavedSettings["Theme"]?.ToString() ?? "light";
        _primaryColor = ((App)Application.Current).SavedSettings["PrimaryColor"]?.ToString() ?? "Blue";
    }
}