# Listen2Me WPF + MVVM Template

This folder is a `dotnet new` solution template containing:
- `Listen2Me.MVVM` (CommunityToolkit.MVVM + navigation/module scaffolding)
- `Listen2Me.WPF` (WPF host with Serilog, DI, automatic module discovery)

## Install template locally

```powershell
dotnet new install .
```

Run this command from this folder:
- `templates/listen2me-wpf-mvvm-template`

## Create a new solution

```powershell
dotnet new wpf-mvvm -n MyProduct
```

This generates:
- `MyProduct.sln`
- `MyProduct.MVVM`
- `MyProduct.WPF`

## Uninstall template

```powershell
dotnet new uninstall Listen2Me.WPFMvvm.SolutionTemplate
```

