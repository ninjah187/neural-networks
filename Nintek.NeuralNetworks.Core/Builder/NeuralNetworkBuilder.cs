using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintek.NeuralNetworks.Core.Builder
{
    public class NeuralNetworkBuilder
    {
        static readonly Random _random = new Random();

        Layer _inputLayer;
        List<Layer> _hiddenLayers;
        Layer _outputLayer;

        public NeuralNetworkBuilder()
        {
        }

        public NeuralNetworkBuilder AddInputLayer(int neuronsCount)
        {
            _inputLayer = new Layer();
            for (int i = 0; i < neuronsCount; i++)
            {
                var neuron = new Neuron(_random.NextDouble());
                _inputLayer.Neurons.Add(neuron);
            }
            return this;
        }

        public NeuralNetworkBuilder AddHiddenLayer(int neuronsCount)
        {
            _hiddenLayers = _hiddenLayers ?? new List<Layer>();
            var layer = new Layer();
            for (int i = 0; i < neuronsCount; i++)
            {
                var neuron = new Neuron();
                layer.Neurons.Add(neuron);
            }
            _hiddenLayers.Add(layer);
            return this;
        }

        public NeuralNetworkBuilder AddOutputLayer(int neuronsCount)
        {
            _outputLayer = new Layer();
            for (int i = 0; i < neuronsCount; i++)
            {
                var neuron = new Neuron();
                _outputLayer.Neurons.Add(neuron);
            }
            return this;
        }

        public NeuralNetwork Build()
        {
            var layers = new List<Layer>();
            layers.Add(_inputLayer);
            layers.AddRange(_hiddenLayers);
            layers.Add(_inputLayer);
            return new NeuralNetwork(layers, new SigmoidFunction());
        }
    }
}
