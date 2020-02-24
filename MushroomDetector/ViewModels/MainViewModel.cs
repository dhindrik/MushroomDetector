using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using MushroomDetector.Models;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using TinyCacheLib;
using TinyMvvm;
using TinyMvvm.IoC;
using Xamarin.Essentials;

namespace MushroomDetector.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IClassifier defaultClassifier;
        private readonly IClassifier offlineClassifier;

        private IEnumerable<Mushroom> mushrooms;

        public MainViewModel(IClassifier defaultClassifier)
        {
            this.defaultClassifier = defaultClassifier;
            offlineClassifier = Resolver.Resolve<IClassifier>("OfflineClassifier");
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            mushrooms = await TinyCacheHandler.Default.RunAsync<IEnumerable<Mushroom>>("mushrooms", async () =>
             {
                 var client = new HttpClient();
                 var json = await client.GetStringAsync("https://mushroom-functions.azurewebsites.net/api/getmushrooms");

                 var result = JsonConvert.DeserializeObject<List<Mushroom>>(json);

                 return result;
             });

        }

        public ICommand PickPhoto => new TinyCommand(async() =>
        {
            var file = await CrossMedia.Current.PickPhotoAsync();

            await HandlePhoto(file);
        });

        public ICommand TakePhoto => new TinyCommand(async () =>
        {
            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                CompressionQuality = 50,
                CustomPhotoSize = 50
            });

            await HandlePhoto(file);
        });

        private async Task HandlePhoto(MediaFile file)
        {
            var stream = file.GetStreamWithImageRotatedForExternalStorage();

            var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            var bytes = memoryStream.ToArray();

            if(Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    defaultClassifier.ClassificationCompleted += Classifier_ClassificationCompleted;

                    await defaultClassifier.Classify(bytes);

                    return;
                }
                catch (Exception ex)
                {

                }
            }

            offlineClassifier.ClassificationCompleted += Classifier_ClassificationCompleted;

            await offlineClassifier.Classify(bytes);
        }

        

        private void Classifier_ClassificationCompleted(object sender, ClassificationEventArgs e)
        {
            var sortedList = e.Predictions.OrderByDescending(x => x.Probability);

            var top = sortedList.First();

            if(top.Probability >= 0.9)
            {
                var mushroom = mushrooms.Single(x => x.LatinName.ToLower() == top.TagName.ToLower());

                Navigation.NavigateToAsync("ResultView", mushroom);
            }

            var classifier = (IClassifier)sender;
            classifier.ClassificationCompleted -= Classifier_ClassificationCompleted;
        }
    }
}
