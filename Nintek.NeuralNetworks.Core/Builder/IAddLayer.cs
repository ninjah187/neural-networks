using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintek.NeuralNetworks.Core.Builder
{
    public interface IAddLayer
    {
        IAddLayer AddHiddenLayer();

    }
}
