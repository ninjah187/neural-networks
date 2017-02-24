using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Windows;
using System.Diagnostics;
using Nintek.NeuralNetworks.Core;
using Nintek.NeuralNetworks.Core.Builder;

namespace Nintek.NeuralNetworks.Samples.Wpf.CopyImage
{
    public class SingleNetworkViewModel : INotifyPropertyChanged
    {
        static readonly Random Random = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapSource SourceImage
        {
            get { return _sourceImage; }
            set
            {
                if (_sourceImage != value)
                {
                    _sourceImage = value;
                    OnPropertyChanged();
                }
            }
        }
        BitmapSource _sourceImage;

        public BitmapSource NeuralNetworkImage
        {
            get { return _neuralNetworkImage; }
            set
            {
                if (_neuralNetworkImage != value)
                {
                    _neuralNetworkImage = value;
                    OnPropertyChanged();
                }
            }
        }
        BitmapSource _neuralNetworkImage;

        Bitmap _sourceBitmap;
        Bitmap _neuralNetworkBitmap;

        NeuralNetwork _network;

        public SingleNetworkViewModel()
        {
            _sourceBitmap = new Bitmap("C:/sample.jpg");
            _neuralNetworkBitmap = new Bitmap(_sourceBitmap.Width, _sourceBitmap.Height);

            //_network = new NeuralNetworkBuilder()
            //    .AddInputLayer(3)
            //    .AddHiddenLayer(3)
            //    .AddHiddenLayer(4)
            //    .AddOutputLayer(3)
            //    .Build();

            _network = new NeuralNetworkBuilder()
                .AddInputLayer(3)
                .AddHiddenLayer(4)
                .AddHiddenLayer(4)
                .AddOutputLayer(3)
                .Build();

            SourceImage = _sourceBitmap.ToBitmapSource();
            //NeuralNetworkImage = _neuralNetworkBitmap.ToBitmapSource();
        }

        public async Task RunAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    var i = 0;
                    double totalIterationMs = 0;
                    while (true)
                    {
                        var stopwatch = Stopwatch.StartNew();

                        CopyImageAndLearn();

                        //Application.Current.Dispatcher.Invoke(new Action(() =>
                        //{
                        //    NeuralNetworkImage = _neuralNetworkBitmap.ToBitmapSource();
                        //}));

                        stopwatch.Stop();
                        i++;
                        totalIterationMs += stopwatch.ElapsedMilliseconds;
                        var averageIterationTime = TimeSpan.FromMilliseconds(totalIterationMs / i);

                        Console.WriteLine("----------");
                        Console.WriteLine($"iterations: {i}");
                        Console.WriteLine($"last iteration time: {stopwatch.Elapsed}");
                        Console.WriteLine($"average iteration time: {averageIterationTime}");
                    }
                });
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
        }

        void CopyImageAndLearn()
        {
            var bitmap = (Bitmap) _neuralNetworkBitmap.Clone();

            var lineColors = new List<Color>();

            for (int y = 0; y < _sourceBitmap.Height; y++)
            {
                if (y != 0)
                {
                    for (int x = 0; x < _sourceBitmap.Width; x++)
                    {
                        bitmap.SetPixel(x, y - 1, lineColors[x]);
                    }
                    lineColors = new List<Color>();
                }

                for (int x = 0; x < _sourceBitmap.Width; x++)
                {
                    var sourcePixel = _sourceBitmap.GetPixel(x, y);
                    var inputs = new List<double>
                    {
                        ConvertRange(0, 255, 0.0001, 1, sourcePixel.R),
                        ConvertRange(0, 255, 0.0001, 1, sourcePixel.G),
                        ConvertRange(0, 255, 0.0001, 1, sourcePixel.B)
                    };

                    var outputsRaw = _network.Evaluate(inputs);
                    var outputs = outputsRaw.Select(o => ConvertRange(0, 1, 0, 255, o)).ToArray();
                    //var outputs = _network.Evaluate(inputs).Select(o => ConvertRange(0, 1, 0, 255, o)).ToArray();

                    var outputColor = Color.FromArgb(sourcePixel.A, (int)outputs[0], (int)outputs[1], (int)outputs[2]);
                    lineColors.Add(outputColor);
                    //_neuralNetworkBitmap.SetPixel(x, y, outputColor);
                    bitmap.SetPixel(x, y, Color.Yellow);

                    _network.PropagateBackward(inputs);
                }

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    NeuralNetworkImage = bitmap.ToBitmapSource();
                }));
            }

            var oldBitmap = _neuralNetworkBitmap;
            _neuralNetworkBitmap = bitmap;

            oldBitmap.Dispose();
        }

        public static double ConvertRange(
            double originalStart, double originalEnd, // original range
            double newStart, double newEnd, // desired range
            double value) // value to convert
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (newStart + ((value - originalStart) * scale));
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
