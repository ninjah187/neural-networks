using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintek.NeuralNetworks.Core
{
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; set; }
        public IActivationFunction ActivationFunction { get; set; }

        double _totalError;

        public NeuralNetwork(List<Layer> layers, IActivationFunction activationFunction)
        {
            Layers = layers;
            ActivationFunction = activationFunction;

            ConnectLayers();
        }

        void ConnectLayers()
        {
            for (var i = 0; i < Layers.Count - 1; i++)
            {
                var layer = Layers[i];
                var nextLayer = Layers[i + 1];

                layer.Attach(nextLayer);
            }
        }

        public List<double> Evaluate(List<double> inputs)
        {
            var inputLayer = Layers.First();

            for (int i = 0; i < inputs.Count; i++)
            {
                inputLayer.Neurons[i].Value = inputs[i];
            }

            PropagateForward();

            var outputs = Layers.Last().Neurons.Select(n => n.Value).ToList();
            return outputs;
        }

        public void PropagateForward()
        {
            foreach (var layer in Layers)
            {
                if (!layer.Neurons.SelectMany(n => n.Inputs).Any())
                {
                    continue;
                }

                foreach (var neuron in layer.Neurons)
                {
                    neuron.Sum = neuron.Inputs.Sum(synapse => synapse.Input.Value * synapse.Weight);
                    neuron.Value = ActivationFunction.Evaluate(neuron.Sum);
                }
            }
        }
        
        public void PropagateBackward(List<double> expected)
        {
            var reversedLayers = ((IEnumerable<Layer>)Layers).Reverse().ToArray();
            Layer previousLayer = null;
            foreach (var layer in reversedLayers)
            {
                if (layer == reversedLayers.First())
                {
                    for (int i = 0; i < layer.Neurons.Count; i++)
                    {
                        var neuron = layer.Neurons[i];
                        var expectedNeuronValue = expected[i];

                        var error = expectedNeuronValue - neuron.Value;
                        neuron.Delta = ActivationFunction.Derivative(neuron.Sum) * error;

                        foreach (var synapse in neuron.Inputs)
                        {
                            synapse.OldWeight = synapse.Weight;
                            var deltaWeight = neuron.Delta / synapse.Input.Value;
                            synapse.Weight = synapse.Weight + deltaWeight;
                        }
                    }
                }
                else if (layer == reversedLayers.Last())
                {
                    break;
                }
                else // hidden layer
                {
                    for (int i = 0; i < layer.Neurons.Count; i++)
                    {
                        var neuron = layer.Neurons[i];
                        
                        var error = neuron.Outputs.Sum(synapse => synapse.Weight * synapse.Output.Delta);
                        neuron.Delta = ActivationFunction.Derivative(neuron.Sum) * error;

                        foreach (var synapse in neuron.Inputs)
                        {
                            synapse.OldWeight = synapse.Weight;
                            var deltaWeight = neuron.Delta / synapse.Input.Value;
                            synapse.Weight = synapse.Weight + deltaWeight;
                        }
                    }
                }

                previousLayer = layer;
            }
        }

        List<double> Divide(double value, IEnumerable<double> values)
        {
            var result = new List<double>();
            foreach (var x in values)
            {
                result.Add(value / x);
            }
            return result;
        }
    }
}
