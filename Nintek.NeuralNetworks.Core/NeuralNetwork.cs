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
                    var sum = neuron.Inputs.Sum(synapse => synapse.Input.Value * synapse.Weight);
                    neuron.Value = ActivationFunction.Evaluate(sum);
                }
            }
        }
    }
}
