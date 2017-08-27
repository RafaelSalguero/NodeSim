using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSimulator.Connections
{
    /// <summary>
    /// A voltaje source that can be manipulated by the owner.
    /// Pins can be connected to nodes, which allows them to define its effective value.
    /// Nodes can be simulated as pins with a HiZ output value
    /// </summary>
    public class Pin
    {
        /// <summary>
        /// Create a high impedance pin. Usualy used as an input pin
        /// </summary>
        public Pin() : this(SourceValue.Z) { }

        /// <summary>
        /// Create a pin with a constant logic value
        /// </summary>
        /// <param name="Value"></param>
        public Pin(SourceValue SourceValue)
        {
            this.SourceValue = SourceValue;
        }

        /// <summary>
        /// Pin measured voltage
        /// </summary>
        public LogicValue Value
        {
            get; private set;
        }

        /// <summary>
        /// Returns true the pin is in a hi current state
        /// </summary>
        public bool HiCurrent
        {
            get
            {
                return
                    SourceValue == SourceValue.Hi && Value == LogicValue.Low ||
                    SourceValue == SourceValue.Low && Value == LogicValue.Hi ||
                    (SourceValue != SourceValue.Z) && Value == LogicValue.Indeterminate;
            }
        }

        /// <summary>
        ///  Propagate voltage source values to measured voltages, returns true if the pin value has been changed
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            var NewValue = GetOutput(ParallelPins.Select(x => x.SourceValue));
            if (NewValue != Value)
            {
                Value = NewValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Pin voltage source value
        /// </summary>
        public SourceValue SourceValue
        {
            get; set;
        }

        /// <summary>
        /// Pins that are directly connected to this pin
        /// </summary>
        private List<Pin> Node { get; } = new List<Pin>();

        /// <summary>
        /// Connect this pin to another pin
        /// </summary>
        /// <param name="Other"></param>
        public void Connect(Pin Other)
        {
            this.Node.Add(Other);
            Other.Node.Add(this);
        }

        /// <summary>
        /// Disconect this pin from another
        /// </summary>
        /// <param name="Other"></param>
        public void Disconnect(Pin Other)
        {
            this.Node.Remove(Other);
            Other.Node.Remove(this);
        }

        /// <summary>
        /// Get all pins that are connected in parallel to this pin, including itself
        /// </summary>
        public IEnumerable<Pin> ParallelPins
        {
            get
            {
                var ret = new HashSet<Pin>();
                GetParallelPins(ret);
                return ret;
            }
        }

        /// <summary>
        /// Get all pins that are connected in parallel to this pin, including itself
        /// </summary>
        private void GetParallelPins(HashSet<Pin> result)
        {
            result.Add(this);
            foreach (var N in Node)
            {
                if (!result.Contains(N))
                    N.GetParallelPins(result);
            }
        }

        /// <summary>
        /// Get the effective meassured voltaje for a pin
        /// </summary>
        /// <param name="Inputs"></param>
        /// <returns></returns>
        internal static LogicValue GetOutput(IEnumerable<SourceValue> Inputs)
        {
            //Remove hi impedance inputs:
            Inputs = Inputs.Where(x => x != Connections.SourceValue.Z);

            //If there are no inputs
            if (!Inputs.Any())
                return LogicValue.Indeterminate;

            //All inputs are equal:
            if (Inputs.GroupBy(x => x).Count() == 1)
            {
                var Value = Inputs.First();
                if (Value == Connections.SourceValue.Hi) return LogicValue.Hi;
                else if (Value == Connections.SourceValue.Low) return LogicValue.Low;
                else return LogicValue.Indeterminate;
            }

            //Some with hi, others with low:
            return LogicValue.Indeterminate;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
