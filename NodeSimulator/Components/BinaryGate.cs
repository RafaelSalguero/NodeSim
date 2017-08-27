using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeSimulator.Connections;


namespace NodeSimulator.Components
{
    /// <summary>
    /// A binary gate logic circuit from a boolean logic function taking care of indeterminate inputs
    /// </summary>
    public class BinaryGate : ILogicCircuit
    {
        /// <summary>
        /// Create a binary gate from a boolean function
        /// </summary>
        /// <param name="Function"></param>
        public BinaryGate(Func<bool, bool, bool> Function)
        {
            this.Function = Function;
        }

        private readonly Func<bool, bool, bool> Function;

        public Pin A { get; } = new Pin();
        public Pin B { get; } = new Pin();
        public Pin Output { get; } = new Pin(SourceValue.Indeterminate);

        public IEnumerable<Pin> Pins
        {
            get
            {
                yield return A;
                yield return B;
                yield return Output;
            }
        }


        public bool Propagate()
        {
            return LogicSystem.PropagateVoltages(Pins);
        }

        private static bool? ToBool(LogicValue  Value)
        {
            if (Value == LogicValue.Hi) return true;
            else if (Value == LogicValue.Low) return false;
            else return null;
        }

        private static SourceValue FromBool(bool Value)
        {
            return Value ? SourceValue.Hi : SourceValue.Low;
        }

        public void Compute()
        {
            this.Output.SourceValue = GetValue();
        }

        private SourceValue GetValue()
        {
            var A = ToBool(this.A.Value);
            var B = ToBool(this.B.Value);

            if (A.HasValue && B.HasValue)
            {
                return FromBool(Function(A.Value, B.Value));
            }
            else if (A == null && B.HasValue)
            {
                //If a is indeterminate, test all two A combinations
                var O1 = Function(false, B.Value);
                var O2 = Function(true, B.Value);

                if (O1 == O2)
                    return FromBool(O1);
                else
                    return SourceValue.Indeterminate;
            }
            else if (A.HasValue && B == null)
            {
                //If b is indeterminate, test all two B combinations
                var O1 = Function(A.Value, false);
                var O2 = Function(A.Value, true);

                if (O1 == O2)
                    return FromBool(O1);
                else
                    return SourceValue.Indeterminate;
            }
            else
            {
                //If a and b are indeterminate, test all 4 combinations
                var O1 = Function(false, false);
                var O2 = Function(false, true);
                var O3 = Function(true, false);
                var O4 = Function(true, true);

                if (O1 == O2 && O2 == O3 && O3 == O4)
                    return FromBool(O1);
                else
                    return SourceValue.Indeterminate;
            }
        }

    }
}
