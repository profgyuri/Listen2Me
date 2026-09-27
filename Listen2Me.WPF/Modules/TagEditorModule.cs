using Listen2Me.MVVM.Modules;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.System;
using Listen2Me.MVVM.TagEditor;
using Listen2Me.MVVM.ViewModels.Layouts;
using Listen2Me.MVVM.ViewModels.Shells;
using Listen2Me.WPF.Views.Layouts;
using Listen2Me.WPF.Views.Shells;
using Microsoft.Extensions.DependencyInjection;

namespace Listen2Me.WPF.Modules;

public class TagEditorModule : IModule
{
    public string Name { get; } = "TagEditor";
    
    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IFileRenamer, FileRenamer>();
        services.AddScoped<IFilenameToTagsParser, FilenameToTagsParser>();
        services.AddScoped<ITagsToFilenameParser, TagsToFilenameParser>();
        services.AddScoped<IMaskMatcher, MaskMatcher>();
        services.AddScoped<IMaskParser, MaskParser>();

        services.AddSingleton<TagEditorFormula>();
        services.AddSingleton<TagEditorFormulaViewModel>();
    }

    public void RegisterNavigation(INavigationRegistry registry)
    {
        
    }

    public void RegisterViews(IViewRegistry registry)
    {
        registry.Register<TagEditorFormulaViewModel, TagEditorFormula>();
    }
}