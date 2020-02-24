using System;
using System.Reflection;
using Autofac;
using MushroomDetector.ViewModels;
using MushroomDetector.Views;
using TinyCacheLib;
using TinyCacheLib.FileStorage;
using TinyMvvm.Autofac;
using TinyMvvm.IoC;
using TinyNavigationHelper;
using TinyNavigationHelper.Forms;
using Xamarin.Forms;

namespace MushroomDetector
{
    public static class Bootstrapper
    {
        public static IBootstrapper Platform { get; set; }

        public static void Init(Application app)
        {
            var builder = new ContainerBuilder();

            Platform?.Init(builder);

            var navigation = new FormsNavigationHelper(app);
            navigation.RegisterViewsInAssembly(Assembly.GetExecutingAssembly());

            builder.RegisterType<MainView>();
            builder.RegisterType<MushroomListView>();
            builder.RegisterType<ResultView>();

            builder.RegisterType<FormsNavigationHelper>().As<INavigationHelper>();

            builder.RegisterType<MainViewModel>();
            builder.RegisterType<MushroomListViewModel>();
            builder.RegisterType<ResultViewModel>();

            builder.RegisterType<OnlineClassifier>().As<IClassifier>();

            var container = builder.Build();

            Resolver.SetResolver(new AutofacResolver(container));

            TinyMvvm.Forms.TinyMvvm.Initialize();

            var cache = TinyCacheHandler.Create("FileCache");

            var fileStorage = new FileStorage();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            fileStorage.Initialize(path);

            cache.SetCacheStore(fileStorage);
        }
    }

    public interface IBootstrapper
    {
        void Init(ContainerBuilder builder);
    }
}
