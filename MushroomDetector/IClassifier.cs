using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MushroomDetector.Models;

namespace MushroomDetector
{
    public interface IClassifier
    {
        event EventHandler<ClassificationEventArgs> ClassificationCompleted;

        Task Classify(byte[] bytes);
    }

    public class ClassificationEventArgs : EventArgs
    {
        public List<Classification> Predictions { get; private set; }

        public ClassificationEventArgs(List<Classification> predictions)
        {
            Predictions = predictions;
        }
    }
}
