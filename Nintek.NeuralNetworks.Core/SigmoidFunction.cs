using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintek.NeuralNetworks.Core
{
    public class SigmoidFunction : IActivationFunction
    {
        public double Evaluate(double input)
        {
            var inverted = 1 + Math.Pow(Math.E, -input);
            return 1 / inverted;
        }
    }
}
