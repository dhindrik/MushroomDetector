using System;
using Autofac;

namespace MushroomDetector.Droid
{
    public class AndroidBootstrapper : IBootstrapper
    {

        public void Init(ContainerBuilder builder)
        {
            builder.RegisterType<TensorflowClassifier>().Keyed<IClassifier>("OfflineClassifier");
        }
    }
}
