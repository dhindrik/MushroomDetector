using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace MushroomDetector.Models
{
    public class PredictionResult
    {
        public List<Classification> Predictions { get; set; }
    }

    public class Classification
    {
        public float Probability { get; set; }
        public string TagName { get; set; }

        public Classification(string tagName, float probability)
        {
            TagName = tagName;
            Probability = probability;
        }
    }
}
