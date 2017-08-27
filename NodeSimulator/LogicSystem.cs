using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeSimulator.Connections;

namespace NodeSimulator
{
    /// <summary>
    /// A collection of interconnected circuits
    /// </summary>
    public class LogicSystem
    {
        public List<ILogicCircuit> Circuits = new List<ILogicCircuit>();

        /// <summary>
        /// Solve one iteration of the circuit.
        /// Multiple iterations may be needed for a circuit to reach an stable state.
        /// Returns true if some circuit value has changed, thus stability condition hasn't been reached yet
        /// </summary>
        public bool Iterate()
        {
            //Compute all circuit values
            foreach (var C in Circuits)
                C.Compute();

            //Propagate voltages:
            var Change = false;
            foreach (var C in Circuits)
                Change |= C.Propagate();
            return Change;
        }


        /// <summary>
        /// Run Update on all pins and return true if any pin value has changed
        /// </summary>
        public static bool PropagateVoltages(Pin[] Pins)
        {
            return PropagateVoltages((IEnumerable<Pin>)Pins);
        }

        /// <summary>
        /// Run Update on all pins and return true if any pin value has changed
        /// </summary>
        public static bool PropagateVoltages(IEnumerable<Pin> Pins)
        {
            //Propagate voltages
            bool Change = false;
            foreach (var P in Pins)
                Change |= P.Update();
            return Change;
        }
    }
}
