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

namespace Nintek.NeuralNetworks.Samples.Wpf.CopyImage
{
    public class MainViewModel : INotifyPropertyChanged
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

        public MainViewModel()
        {
            _sourceBitmap = new Bitmap("C:/sample.jpg");
            _neuralNetworkBitmap = new Bitmap(_sourceBitmap.Width, _sourceBitmap.Height);

            _network = new NeuralNetwork(new List<Layer>
            {
                new Layer
                {
                    Neurons = new List<Neuron> { new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()) }
                },
                new Layer
                {
                    Neurons = new List<Neuron> { new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()) }
                },
                new Layer
                {
                    Neurons = new List<Neuron> { new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()),
                                                 new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()) }
                },
                new Layer
                {
                    Neurons = new List<Neuron> { new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()), new Neuron(Random.NextDouble()) }
                }
            }, new SigmoidFunction());
            
            SourceImage = _sourceBitmap.ToBitmapSource();
            NeuralNetworkImage = _neuralNetworkBitmap.ToBitmapSource();
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

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            NeuralNetworkImage = _neuralNetworkBitmap.ToBitmapSource();
                        }));

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
            for (int y = 0; y < _sourceBitmap.Height; y++)
            {
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

                    if (outputs.Contains(double.NaN))
                    {

                    }

                    var outputColor = Color.FromArgb(sourcePixel.A, (int)outputs[0], (int)outputs[1], (int)outputs[2]);
                    _neuralNetworkBitmap.SetPixel(x, y, outputColor);

                    _network.PropagateBackward(new List<double> { sourcePixel.R, sourcePixel.G, sourcePixel.B });
                }
            }
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
