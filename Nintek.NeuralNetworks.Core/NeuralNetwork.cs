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

        public NeuralNetwork(List<Layer> layers, IActivationFunction activationFunction)
        {
            Layers = layers;
            ActivationFunction = activationFunction;

            //ConnectLayers();
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

        public void PropagateBackward(double target)
        {
            var reversedLayers = ((IEnumerable<Layer>) Layers).Reverse().ToArray();
            Layer previousLayer = null;
            foreach (var layer in reversedLayers)
            {
                if (layer == reversedLayers.First())
                {
                    foreach (var neuron in layer.Neurons)
                    {
                        var marginOfError = target - neuron.Value;
                        var delta = ActivationFunction.Derivative(neuron.Sum) * marginOfError;

                        foreach (var synapse in neuron.Inputs)
                        {
                            synapse.OldWeight = synapse.Weight;
                            var deltaWeight = delta / synapse.Input.Value;
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
                    foreach (var neuron in layer.Neurons)
                    {
                        var marginOfError = target - neuron.Value;
                        var delta = ActivationFunction.Derivative(neuron.Sum) * marginOfError;

                        List<double> deltaWeights = null;

                        foreach (var outputSynapse in neuron.Outputs)
                        {
                            var deltaHidden = delta / outputSynapse.OldWeight * ActivationFunction.Derivative(neuron.Sum);
                            var inputs = neuron.Inputs.Select(s => s.Input.Value).ToArray();
                            deltaWeights = Divide(deltaHidden, inputs);
                        }

                        for (int i = 0; i < neuron.Inputs.Count; i++)
                        {
                            var inputSynapse = neuron.Inputs[i];
                            var deltaWeight = deltaWeights[i];
                            inputSynapse.OldWeight = inputSynapse.Weight;
                            inputSynapse.Weight = inputSynapse.Weight + deltaWeight;
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
