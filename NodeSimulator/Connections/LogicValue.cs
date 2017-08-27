using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSimulator.Connections
{
    /// <summary>
    /// Possible values for a digital pin
    /// </summary>
    public enum LogicValue
    {
        /// <summary>
        /// When the logic value is not known to be Hi or Low
        /// </summary>
        Indeterminate,
    
        /// <summary>
        /// Hi voltage, logic 1
        /// </summary>
        Hi,
        /// <summary>
        /// Low voltage, logic 0
        /// </summary>
        Low,
    }

    public enum SourceValue
    {
        /// <summary>
        /// The voltage source is not known to be Hi or Low
        /// </summary>
        Indeterminate,

        /// <summary>
        /// Hi voltage, logic 1
        /// </summary>
        Hi,

        /// <summary>
        /// Low voltage, logic 0
        /// </summary>
        Low ,


        /// <summary>
        /// The voltage source is disconected from the circuit
        /// </summary>
        Z,
    }
}
