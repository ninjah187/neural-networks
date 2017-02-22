using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nintek.NeuralNetworks.Core;

namespace Nintek.NeuralNetworks.Samples
{
    class Program
    {
        static NeuralNetwork CreateTestNetwork()
        {
            var inputLayer = new Layer
            {
                Neurons = new List<Neuron>
                {
                    new Neuron { Value = 1 },
                    new Neuron { Value = 1 }
                }
            };

            var hiddenLayer = new Layer
            {
                Neurons = new List<Neuron> { new Neuron(), new Neuron(), new Neuron() }
            };

            var inputHiddenSynapses = new List<Synapse>
            {
                new Synapse { Input = inputLayer.Neurons[0], Output = hiddenLayer.Neurons[0], Weight = 0.8 },
                new Synapse { Input = inputLayer.Neurons[1], Output = hiddenLayer.Neurons[0], Weight = 0.2 },
                new Synapse { Input = inputLayer.Neurons[0], Output = hiddenLayer.Neurons[1], Weight = 0.4 },
                new Synapse { Input = inputLayer.Neurons[1], Output = hiddenLayer.Neurons[1], Weight = 0.9 },
                new Synapse { Input = inputLayer.Neurons[0], Output = hiddenLayer.Neurons[2], Weight = 0.3 },
                new Synapse { Input = inputLayer.Neurons[1], Output = hiddenLayer.Neurons[2], Weight = 0.5 },
            };

            foreach (var neuron in inputLayer.Neurons)
            {
                neuron.Outputs = inputHiddenSynapses.Where(s => s.Input == neuron || s.Output == neuron).ToList();
            }

            foreach (var neuron in hiddenLayer.Neurons)
            {
                neuron.Inputs = inputHiddenSynapses.Where(s => s.Input == neuron || s.Output == neuron).ToList();
            }

            var outputLayer = new Layer
            {
                Neurons = new List<Neuron> { new Neuron() }
            };

            var hiddenOutputSynapses = new List<Synapse>
            {
                new Synapse { Input = hiddenLayer.Neurons[0], Output = outputLayer.Neurons[0], Weight = 0.3 },
                new Synapse { Input = hiddenLayer.Neurons[1], Output = outputLayer.Neurons[0], Weight = 0.5 },
                new Synapse { Input = hiddenLayer.Neurons[2], Output = outputLayer.Neurons[0], Weight = 0.9 },
            };

            foreach (var neuron in hiddenLayer.Neurons)
            {
                neuron.Outputs = hiddenOutputSynapses.Where(s => s.Input == neuron || s.Output == neuron).ToList();
            }

            foreach (var neuron in outputLayer.Neurons)
            {
                neuron.Inputs = hiddenOutputSynapses.Where(s => s.Input == neuron || s.Output == neuron).ToList();
            }

            var layers = new List<Layer> { inputLayer, hiddenLayer, outputLayer };

            var network = new NeuralNetwork(layers, new SigmoidFunction());

            return network;
        }

        static void Main(string[] args)
        {
            var network = CreateTestNetwork();

            network.PropagateForward();

            int i = 0;
            while (i++ < 1000000)
            {
                Console.Clear();
                network.PropagateForward();
                network.PropagateBackward(0);
                var output = network.Layers.Last().Neurons.First().Value;
                Console.WriteLine($"iteration: {i}\noutput: {output}");
            }
        }
    }
}
