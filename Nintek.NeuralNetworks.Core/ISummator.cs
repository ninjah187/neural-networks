using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintek.NeuralNetworks.Core
{
    public interface ISummator
    {
        double Sum(IEnumerable<double> inputs);
    }
}
