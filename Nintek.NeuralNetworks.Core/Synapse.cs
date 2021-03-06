﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintek.NeuralNetworks.Core
{
    public class Synapse
    {
        public double Weight { get; set; }
        public double OldWeight { get; set; }
        public Neuron Input { get; set; }
        public Neuron Output { get; set; }

        public override string ToString()
            => $"{{ weight: {Weight}, oldWeight: {OldWeight}, input: {Input}, output: {Output}  }}";
    }
}
