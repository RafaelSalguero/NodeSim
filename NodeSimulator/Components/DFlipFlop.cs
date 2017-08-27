using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeSimulator.Connections;


namespace NodeSimulator.Components
{
    /// <summary>
    /// A D flip flop implemented in code
    /// </summary>
    public class DFlipFlop : ILogicCircuit
    {
        public Pin D { get; } = new Pin();
        public Pin Q { get; } = new Pin();
        public Pin Clock { get; } = new Pin();

        public IEnumerable<Pin> Pins
        {
            get
            {
                yield return D;
                yield return Q;
                yield return Clock;
            }
        }

        public void Compute()
        {
         
        }

        public bool Propagate() => LogicSystem.PropagateVoltages(Pins);
    }
}
