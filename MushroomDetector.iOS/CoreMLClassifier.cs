using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreML;
using Foundation;
using ImageIO;
using MushroomDetector.Models;
using Vision;

namespace MushroomDetector.iOS
{
    public class CoreMLClassifier : IClassifier
    {
        public event EventHandler<ClassificationEventArgs> ClassificationCompleted;

        public async Task Classify(byte[] bytes)
        {
            var modelUrl = NSBundle.MainBundle.GetUrlForResource("model", "mlmodel");
            var compiledUrl = MLModel.CompileModel(modelUrl, out var error);
            var compiledModel = MLModel.Create(compiledUrl, out error);

            var vnCoreModel = VNCoreMLModel.FromMLModel(compiledModel, out error);

            var classificationRequest = new VNCoreMLRequest(vnCoreModel, HandleVNRequest);

            var data = NSData.FromArray(bytes);
            var handler = new VNImageRequestHandler(data, CGImagePropertyOrientation.Up, new VNImageOptions());

            handler.Perform(new[] { classificationRequest }, out error);
        }

        private void HandleVNRequest(VNRequest request, NSError error)
        {
            if (error != null)
            {
                ClassificationCompleted?.Invoke(this, new ClassificationEventArgs(new List<Classification>()));
            }

            var result = request.GetResults<VNClassificationObservation>();
            var classifications = result.OrderByDescending(x => x.Confidence).Select(x => new Classification(x.Identifier, x.Confidence)).ToList();

            ClassificationCompleted?.Invoke(this, new ClassificationEventArgs(classifications));
        }
    }
}
