using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintek.NeuralNetworks.Core
{
    public class Neuron
    {
        public double Value { get; set; }
        public double Sum { get; set; }
        public List<Synapse> Inputs { get; set; }
        public List<Synapse> Outputs { get; set; }

        public Neuron()
        {
            Inputs = new List<Synapse>();
            Outputs = new List<Synapse>();
        }

        public Neuron(double value)
            : this()
        {
            Value = value;
        }
    }
}
