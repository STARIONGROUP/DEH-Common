# DEHP-Common
Common libraries for reusable components in the frame of Digital Engineering Hub Pathfinder

## Build status

AppVeyor is used to build and test the C# DEHP-Common

Branch | Build Status
------- | :------------
Master |  [![Build Status](https://ci.appveyor.com/api/projects/status/sxq2k1lvqpeijot7/branch/master?svg=true)](https://ci.appveyor.com/project/rheagroup/dehp-common/branch/master)
Development |  [![Build status](https://ci.appveyor.com/api/projects/status/sxq2k1lvqpeijot7/branch/development?svg=true)](https://ci.appveyor.com/project/rheagroup/dehp-common/branch/development)

[![Build history](https://buildstats.info/appveyor/chart/rheagroup/dehp-common)](https://ci.appveyor.com/project/rheagroup/dehp-common)

## SonarQube Status:
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=alert_status)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=security_rating)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=coverage)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=bugs)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=ncloc)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=sqale_index)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_DEHP-Common&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=RHEAGROUP_DEHP-Common)

## Usage:

### IoC:

DEHP Common library has an Inversion of Control Container implementation based on [Autofac](https://github.com/autofac/Autofac).

The container is located under [DEHPCommon.AppContainer.Container;](https://github.com/RHEAGROUP/DEHP-Common/blob/development/DEHPCommon/AppContainer.cs#L39)

It is necessary to build the DEHP IoC container before using DEHPCommon

```Csharp

public App(ContainerBuilder containerBuilder = null)
{
    containerBuilder ??= new ContainerBuilder();
    //Registering the DST adapter specifics dependencies
    containerBuilder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>().SingleInstance();
    //Building the Container 
    DEHPCommon.AppContainer.BuildContainer(containerBuilder);
}
```

### Navigation:

DEHP Common library provides a way to resolve Data Context of views by resolving automatically the view model and its dependencies.
The service is called ```INavigationService``` and is registered in the IoC container.

```Csharp 
void Show<TView>() where TView : Window, new();
```

Usage of the previous method relies on the naming convention of the ViewModel:

```Csharp 
class MyView; class MyViewViewModel
```

Otherwise the use of the following overload is necessary:

```Csharp 
void Show<TView, TViewModel>(TViewModel viewModel = null) where TView : Window, new() where TViewModel : class;
```

Example of usage:

```Csharp

protected override void OnStartup(StartupEventArgs e)
{
    //Using the container
    using (var scope = DEHPCommon.AppContainer.Container.BeginLifetimeScope())
    {
        scope.Resolve<INavigationService>().Show<MainWindow>();
    }

    base.OnStartup(e);
}
```


