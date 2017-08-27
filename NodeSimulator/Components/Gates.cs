using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeSimulator.Connections;


namespace NodeSimulator.Components
{
    public class And : BinaryGate
    {
        public And() : base((a, b) => a && b) { }
    }

    public class Or : BinaryGate
    {
        public Or() : base((a, b) => a || b) { }
    }

    public class Not : ILogicCircuit
    {
        public Pin A { get; } = new Pin();
        public Pin Output { get; } = new Pin();

        public IEnumerable<Pin> Pins
        {
            get
            {
                yield return A;
                yield return Output;
            }
        }

        public void Compute()
        {
            if (A.Value == LogicValue.Hi)
                Output.SourceValue = SourceValue.Low;
            else if (A.Value == LogicValue.Low)
                Output.SourceValue = SourceValue.Hi;
            else
                Output.SourceValue = SourceValue.Indeterminate;
        }

        public bool Propagate()
        {
            return LogicSystem.PropagateVoltages(Pins);
        }
    }

    public class Xor : BinaryGate
    {
        public Xor() : base((a, b) => (a && !b) || (!a && b)) { }
    }

    public class Nor : BinaryGate
    {
        public Nor() : base((a, b) => !(a || b)) { }
    }

    public class Nand : BinaryGate
    {
        public Nand() : base((a, b) => !(a && b)) { }
    }
}
