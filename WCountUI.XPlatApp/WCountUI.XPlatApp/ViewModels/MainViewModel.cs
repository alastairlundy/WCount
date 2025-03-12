using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AlastairLundy.WCountLib.Abstractions.Counters;

using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WCountUI.XPlatApp.Localizations;
using WCountUI.XPlatApp.Views;

namespace WCountUI.XPlatApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Dictionary<string, ViewModelBase> _views;
    
    public MainViewModel(EditorViewModel editorViewModel, SettingsViewModel settingsViewModel)
    {
        _views = new Dictionary<string, ViewModelBase>();
        _views.Add("app.editor", editorViewModel);
        _views.Add("app.settings", settingsViewModel);
    }



    



}