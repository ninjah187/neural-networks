using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintek.NeuralNetworks.Core
{
    public class Layer
    {
        static readonly Random Random = new Random();

        public List<Neuron> Neurons { get; set; }

        public Layer()
        {
            Neurons = new List<Neuron>();
        }

        public void Attach(Layer attachedLayer)
        {
            foreach (var neuron in Neurons)
            {
                foreach (var attachedNeuron in attachedLayer.Neurons)
                {
                    var synapse = new Synapse
                    {
                        Input = neuron,
                        Output = attachedNeuron,
                        Weight = Random.NextDouble()
                    };

                    neuron.Outputs.Add(synapse);
                    attachedNeuron.Inputs.Add(synapse);
                }
            }
        }
    }
}
