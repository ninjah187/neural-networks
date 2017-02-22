using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintek.NeuralNetworks.Core
{
    public interface IActivationFunction
    {
        double Evaluate(double input);
    }
}
