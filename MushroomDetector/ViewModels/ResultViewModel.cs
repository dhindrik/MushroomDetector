using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MushroomDetector.Models;
using TinyMvvm;
using Xamarin.Essentials;

namespace MushroomDetector.ViewModels
{
    public class ResultViewModel : ViewModelBase
    {
        public Mushroom Item { get; set; }

        public async override Task Initialize()
        {
            await base.Initialize();

            Item = (Mushroom)NavigationParameter;

            RaisePropertyChanged(nameof(Item));
        }

        public ICommand Open => new TinyCommand(() =>
        {
            Browser.OpenAsync(Item.WikipediaUrl);
        });
    }
}
