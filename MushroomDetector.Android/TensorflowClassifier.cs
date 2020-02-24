using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Java.IO;
using Java.Nio;
using Java.Nio.Channels;
using MushroomDetector.Models;

namespace MushroomDetector.Droid
{
    public class TensorflowClassifier : IClassifier
    {
        public const int FloatSize = 4;
        public const int PixelSize = 3;

        public event EventHandler<ClassificationEventArgs> ClassificationCompleted;

        public async Task Classify(byte[] bytes)
        {
            var mappedByteBuffer = GetModelAsMappedByteBuffer();

            var interpreter = new Xamarin.TensorFlow.Lite.Interpreter(mappedByteBuffer);

            var tensor = interpreter.GetInputTensor(0);

            var shape = tensor.Shape();

            var width = shape[1];
            var height = shape[2];

            var byteBuffer = GetPhotoAsByteBuffer(bytes, width, height);

            var sr = new StreamReader(Application.Context.Assets.Open("labels.txt"));
            var labels = sr.ReadToEnd().Split('\n').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

            var outputLocations = new float[1][] { new float[labels.Count] };

            var outputs = Java.Lang.Object.FromArray(outputLocations);

            interpreter.Run(byteBuffer, outputs);

            var classificationResult = outputs.ToArray<float[]>();

            var result = new List<Classification>();

            for (var i = 0; i < labels.Count; i++)
            {
                var label = labels[i];
                result.Add(new Classification(label, classificationResult[0][i]));
            }

            ClassificationCompleted?.Invoke(this, new ClassificationEventArgs(result));
        }

        private MappedByteBuffer GetModelAsMappedByteBuffer()
        {
            var assetDescriptor = Application.Context.Assets.OpenFd("model.tflite");
            var inputStream = new FileInputStream(assetDescriptor.FileDescriptor);

            var mappedByteBuffer = inputStream.Channel.Map(FileChannel.MapMode.ReadOnly, assetDescriptor.StartOffset, assetDescriptor.DeclaredLength);

            return mappedByteBuffer;
        }

        private ByteBuffer GetPhotoAsByteBuffer(byte[] bytes, int width, int height)
        {
            var modelInputSize = FloatSize * height * width * PixelSize;

            var bitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
            var resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, true);

            var byteBuffer = ByteBuffer.AllocateDirect(modelInputSize);
            byteBuffer.Order(ByteOrder.NativeOrder());

            var pixels = new int[width * height];
            resizedBitmap.GetPixels(pixels, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);

            var pixel = 0;

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var pixelVal = pixels[pixel++];

                    byteBuffer.PutFloat(pixelVal >> 16 & 0xFF);
                    byteBuffer.PutFloat(pixelVal >> 8 & 0xFF);
                    byteBuffer.PutFloat(pixelVal & 0xFF);
                }
            }

            bitmap.Recycle();

            return byteBuffer;
        }
    }
}
