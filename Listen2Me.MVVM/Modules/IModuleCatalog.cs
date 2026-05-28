namespace Listen2Me.MVVM.Modules;

/// <summary>
/// Provides discovered modules for application startup.
/// </summary>
public interface IModuleCatalog
{
    /// <summary>
    /// Loads modules in startup order.
    /// </summary>
    /// <returns>A read-only module list.</returns>
    IReadOnlyList<IModule> LoadModules();
}