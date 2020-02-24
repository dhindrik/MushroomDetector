using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace MushroomDetector.Models
{
    public class PredictionResult
    {
        public List<Prediction> Predictions { get; set; }
    }

    public class Prediction
    {
        public float Probability { get; set; }
        public string TagName { get; set; }

        public Prediction(string tagName, float probability)
        {
            TagName = tagName;
            Probability = probability;
        }
    }
}
