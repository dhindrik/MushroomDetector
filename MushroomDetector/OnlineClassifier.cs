using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MushroomDetector.Models;
using Newtonsoft.Json;

namespace MushroomDetector
{
    public class OnlineClassifier : IClassifier
    {
        public event EventHandler<ClassificationEventArgs> ClassificationCompleted;

        public async Task Classify(byte[] bytes)
        {
            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 3);

            client.DefaultRequestHeaders.Add("Prediction-Key", "ad104481e3684f849cac04d627ab93e5");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/80c96cf0-45ee-4887-9925-46422af8711d/classify/iterations/Iteration3/image";

            HttpResponseMessage response;

            using (var content = new ByteArrayContent(bytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                var json = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<PredictionResult>(json);

                ClassificationCompleted.Invoke(this, new ClassificationEventArgs(result.Predictions));
            }
        }
    }
}
