
# DEHP Common library (https://github.com/RHEAGROUP/DEHP-Common#dehp-common)
--------------------------------------------------------------------------

## IoC:
--------

DEHP Common library has an Inversion of Control Container implementation based on [Autofac](https://github.com/autofac/Autofac).

The container is located under [DEHPCommon.AppContainer.Container;](https://github.com/RHEAGROUP/DEHP-Common/blob/development/DEHPCommon/AppContainer.cs#L39)

It is necessary to build the DEHP IoC container before using DEHPCommon

//public App(ContainerBuilder containerBuilder = null)
//{
//    containerBuilder ??= new ContainerBuilder();
//    //Registering the DST adapter specifics dependencies
//    containerBuilder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>().SingleInstance();
//    //Building the Container 
//    DEHPCommon.AppContainer.BuildContainer(containerBuilder);
//}
--------------------------------------------------------------------------

## Navigation:
---------------

DEHP Common library provides a way to resolve Data Context of views by resolving automatically the view model and its dependencies.
The service is called INavigationService and is registered in the IoC container.
 
// void Show<TView>() where TView : Window, new();
Usage of the previous method relies on the naming convention of the ViewModel:
// class MyView; class MyViewViewModel

Otherwise the use of the following overload is necessary:
// void Show<TView, TViewModel>(TViewModel viewModel = null) where TView : Window, new() where TViewModel : class;


Example of usage:

//protected override void OnStartup(StartupEventArgs e)
//{
//    //Using the container
//    using (var scope = DEHPCommon.AppContainer.Container.BeginLifetimeScope())
//    {
//        scope.Resolve<INavigationService>().Show<MainWindow>();
//    }
//
//    base.OnStartup(e);
//}
