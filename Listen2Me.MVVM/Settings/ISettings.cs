using Listen2Me.MVVM.Settings.Appearance;

namespace Listen2Me.MVVM.Settings;

/// <summary>
/// God interface for all settings.
/// </summary>
public interface ISettings
{
    /// <inheritdoc cref="IAppearanceSettings"/>
    public IAppearanceSettings Appearance { get; set; }
}