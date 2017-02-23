using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using Nintek.NeuralNetworks.Core;

namespace Nintek.NeuralNetworks.Samples.Wpf.CopyImage
{
    public class MainViewModel : INotifyPropertyChanged
    {
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
                    Neurons = new List<Neuron> { new Neuron(), new Neuron(), new Neuron() }
                },
                new Layer
                {
                    Neurons = new List<Neuron> { new Neuron(), new Neuron(), new Neuron(), new Neuron() }
                },
                new Layer
                {
                    Neurons = new List<Neuron> { new Neuron(), new Neuron(), new Neuron(), new Neuron(),
                                                 new Neuron(), new Neuron(), new Neuron(), new Neuron() }
                },
                new Layer
                {
                    Neurons = new List<Neuron> { new Neuron(), new Neuron(), new Neuron() }
                }
            }, new SigmoidFunction());
            
            SourceImage = _sourceBitmap.ToBitmapSource();
            NeuralNetworkImage = _neuralNetworkBitmap.ToBitmapSource();
        }

        public async Task RunAsync()
        {
            var refreshFrequency = 100;
            var refreshCounter = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    if (refreshCounter == 1)
                    {
                        NeuralNetworkImage = _neuralNetworkBitmap.ToBitmapSource();
                    }

                    if (refreshCounter == refreshFrequency)
                    {
                        refreshCounter = 0;
                    }

                    CopyImageAndLearn();

                    refreshCounter++;
                }
            });
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
                        ConvertRange(0, 255, 0, 1, sourcePixel.R),
                        ConvertRange(0, 255, 0, 1, sourcePixel.G),
                        ConvertRange(0, 255, 0, 1, sourcePixel.B)
                    };

                    var outputs = _network.Evaluate(inputs).Select(o => ConvertRange(0, 1, 0, 255, o)).ToArray();

                    var outputColor = Color.FromArgb(sourcePixel.A, (int)outputs[0], (int)outputs[1], (int)outputs[2]);
                    _neuralNetworkBitmap.SetPixel(x, y, outputColor);

                    _network.PropagateBackward(inputs);
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
