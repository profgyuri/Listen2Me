using System.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Settings;
using Serilog;

namespace Listen2Me.MVVM.ViewModels;

public abstract class SyncableSettingsViewModel<TSettings> : ViewModelBase
    where TSettings : JsonSettingsMemory
{
    protected readonly TSettings Settings;
    
    private readonly Dictionary<string, Action> _settingsSyncMap = new();
    
    protected virtual string SaveErrorMessage => "Save settings";
    
    protected SyncableSettingsViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        TSettings settings) 
        : base(errorHandler, logger, messenger)
    {
        Settings = settings;
    }
    
    /// <summary>
    /// Syncs the property with the settings. To be called in <see cref="InitializeAsync"/>.
    /// </summary>
    /// <param name="propertyName">Name of the property to sync.</param>
    /// <param name="setValue">Action to set the value.</param>
    protected void SyncProperty(string propertyName, Action setValue) =>
        _settingsSyncMap[propertyName] = setValue;
    
    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName is not { Length: > 0 } || !IsInitialized) return;
        if (!_settingsSyncMap.TryGetValue(e.PropertyName, out var setValue)) return;

        setValue();
        await ExecuteSafeAsync(async ct => await Settings.SaveAsync(ct), SaveErrorMessage);
    }
}