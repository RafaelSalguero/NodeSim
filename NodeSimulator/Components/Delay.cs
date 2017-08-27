using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeSimulator.Connections;


namespace NodeSimulator.Components
{
    /// <summary>
    /// Sets the output equal to the input
    /// </summary>
    public class Buffer : ILogicCircuit
    {
        public Pin Input { get; } = new Pin();
        public Pin Output { get; } = new Pin(SourceValue.Indeterminate);

        public IEnumerable<Pin> Pins
        {
            get
            {
                yield return Input;
                yield return Output;
            }
        }


        public bool Propagate()
        {
            return LogicSystem.PropagateVoltages(Pins);
        }

        public void Compute()
        {
            Output.SourceValue = ToSource(Input.Value);
        }

        public static SourceValue ToSource(LogicValue Value)
        {
            if (Value == LogicValue.Hi)
                return SourceValue.Hi;
            else if (Value == LogicValue.Low)
                return SourceValue.Low;
            else
                return SourceValue.Indeterminate;
        }

        /// <summary>
        /// Convert a source value to a logic value
        /// </summary>
        public static LogicValue ToLogic(SourceValue Source)
        {
            if (Source == SourceValue.Hi)
                return LogicValue.Hi;
            if (Source == SourceValue.Low)
                return LogicValue.Low;
            else
                return LogicValue.Indeterminate;
        }

     
    }
}
