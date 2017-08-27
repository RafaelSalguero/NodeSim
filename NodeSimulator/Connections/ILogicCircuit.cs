using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSimulator.Connections
{
    /// <summary>
    /// An object that have references to pins. ILogicCircuits can encapsulate memory
    /// </summary>
    public interface ILogicCircuit
    {
        /// <summary>
        /// Get all pins
        /// </summary>
        IEnumerable<Pin> Pins { get; }

        /// <summary>
        /// Compute pin source values from pin measured values
        /// </summary>
        void Compute();

        /// <summary>
        /// Propagate pin voltages. Return true if any state has changed
        /// </summary>
        bool Propagate();
    }
}
