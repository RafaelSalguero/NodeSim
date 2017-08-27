using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeSimulator.Components;
using NodeSimulator.Connections;

namespace NodeSimulator.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PinTest()
        {
            var A = new Pin(SourceValue.Z);
            var B = new Pin(SourceValue.Z);
            var C = new Pin(SourceValue.Hi);

            //Star like connections:
            A.Connect(B);
            A.Connect(C);

            //Propagate voltages:
            A.Update();
            B.Update();
            C.Update();

            //All voltages are Hi:

            Assert.AreEqual(LogicValue.Hi, A.Value);
            Assert.AreEqual(LogicValue.Hi, B.Value);
            Assert.AreEqual(LogicValue.Hi, C.Value);

            //Pins are not in hi current:
            Assert.IsFalse(A.HiCurrent);
            Assert.IsFalse(B.HiCurrent);
            Assert.IsFalse(C.HiCurrent);

            //Simulate a hi current state:
            A.SourceValue = SourceValue.Low;

            //Propagate voltages:
            A.Update();
            B.Update();
            C.Update();

            //Pins A and C are in hi current:
            Assert.IsTrue (A.HiCurrent);
            Assert.IsFalse(B.HiCurrent);
            Assert.IsTrue (C.HiCurrent);
        }

        [TestMethod]
        public void EdgeDetectorTest()
        {
            //The edge detector is the following circuit:

            /*
            A--\-------------|     |
                \            | AND |---
                 \-- (NOT) --|     |
            */

            //The propagation delay caused by the NOT gate 
            //leads to an Hi pulse when the A changes from 0 to 1

            var And = new And();
            var Not = new Not();

            //Make the conections
            And.A.Connect(Not.A);
            Not.Output.Connect(And.B);

            //Init the world simulator
            var Word = new LogicSystem();
            Word.Circuits.Add(And);
            Word.Circuits.Add(Not);

            And.A.SourceValue = SourceValue.Low;

            //Propagate inputs:
            Assert.IsTrue(Word.Iterate());

            //Calculate and:
            Assert.IsTrue(Word.Iterate());
            Assert.AreEqual(LogicValue.Low, And.Output.Value);

            //System become stable
            Assert.IsFalse(Word.Iterate());
            Assert.IsFalse(Word.Iterate());


            //Change output to Hi
            And.A.SourceValue = SourceValue.Hi;

            //Propagate inputs
            Assert.IsTrue(Word.Iterate());

            //Expect a hi pulse:
            Assert.IsTrue(Word.Iterate());
            Assert.AreEqual(LogicValue.Hi, And.Output.Value);

            //And is low again:
            Assert.IsTrue(Word.Iterate());
            Assert.AreEqual(LogicValue.Low, And.Output.Value);

            //System become stable
            Assert.IsFalse(Word.Iterate());
            Assert.IsFalse(Word.Iterate());

            //Still in low:
            Assert.AreEqual(LogicValue.Low, And.Output.Value);


        }

        [TestMethod]
        public void SrLatchGateBasedTest()
        {
            var Nor1 = new Nor();
            var Nor2 = new Nor();
            
            //Connect two nor gates onto a SR latch configuration
            Nor1.Output.Connect(Nor2.A);
            Nor2.Output.Connect(Nor1.B);

            //Simulate the world:
            var W = new LogicSystem();
            W.Circuits.Add(Nor1);
            W.Circuits.Add(Nor2);

            W.Iterate();

            //Nor1 output is Q output from the latch
            Assert.AreEqual(LogicValue.Indeterminate, Nor1.Output.Value);

            //Reset the latch:
            Nor1.A.SourceValue = SourceValue.Hi;
            Nor2.B.SourceValue = SourceValue.Low;

            //Propagate inputs:
            W.Iterate();

            //Latch output still not resolved:
            Assert.AreEqual(LogicValue.Indeterminate, Nor1.Output.Value);

            //Nor 1 is computed:
            W.Iterate();
            Assert.AreEqual(LogicValue.Low, Nor1.Output.Value);

            //Latch is still in transition, since Nor2 output is indeterminate
            Assert.AreEqual(LogicValue.Indeterminate, Nor2.Output.Value);

            //Nor2 is computed, latch transition finished:
            W.Iterate();
            Assert.AreEqual(LogicValue.Hi, Nor2.Output.Value);

            //Set the latch in memory mode:
            Nor1.A.SourceValue = SourceValue.Low;
            Nor2.B.SourceValue = SourceValue.Low;

            //Propagate inputs:
            W.Iterate();
            Assert.AreEqual(LogicValue.Low, Nor1.Output.Value);
            Assert.AreEqual(LogicValue.Hi, Nor2.Output.Value);

            W.Iterate();
            Assert.AreEqual(LogicValue.Low, Nor1.Output.Value);
            Assert.AreEqual(LogicValue.Hi, Nor2.Output.Value);

            //Value is preserved
            W.Iterate();
            Assert.AreEqual(LogicValue.Low, Nor1.Output.Value);
            Assert.AreEqual(LogicValue.Hi, Nor2.Output.Value);

            //Set in forbiden state:
            Nor1.A.SourceValue = SourceValue.Hi;
            Nor2.B.SourceValue = SourceValue.Hi;

            //Propagate inputs:
            W.Iterate();

            //Calculate Nor1 and 2
            W.Iterate();
            W.Iterate();

            Assert.AreEqual(LogicValue.Low, Nor1.Output.Value);
            Assert.AreEqual(LogicValue.Low, Nor2.Output.Value);

            //Race condition:
            Nor1.A.SourceValue = SourceValue.Low;
            Nor2.B.SourceValue = SourceValue.Low;

            //Propagate inputs:
            W.Iterate();

            //Calculate Nor1 and 2
            //We see that the system is unstable

            W.Iterate();
            Assert.AreEqual(LogicValue.Hi, Nor1.Output.Value);
            Assert.AreEqual(LogicValue.Hi, Nor2.Output.Value);

            W.Iterate();
            Assert.AreEqual(LogicValue.Low, Nor1.Output.Value);
            Assert.AreEqual(LogicValue.Low, Nor2.Output.Value);

            W.Iterate();
            Assert.AreEqual(LogicValue.Hi, Nor1.Output.Value);
            Assert.AreEqual(LogicValue.Hi, Nor2.Output.Value);

            W.Iterate();
            Assert.AreEqual(LogicValue.Low, Nor1.Output.Value);
            Assert.AreEqual(LogicValue.Low, Nor2.Output.Value);

            //Set the latch:
            Nor1.A.SourceValue = SourceValue.Low;
            Nor2.B.SourceValue = SourceValue.Hi;

            //Propagate inputs:
            W.Iterate();

            //Calculate 
            W.Iterate();
            W.Iterate();

            //Latch became stable again:
            Assert.AreEqual(LogicValue.Hi, Nor1.Output.Value);
            Assert.AreEqual(LogicValue.Low, Nor2.Output.Value);

        }



        [TestMethod]
        public void NorTest()
        {
            var Nor = new Nor();

            var W = new LogicSystem();
            W.Circuits.Add(Nor);

            Nor.A.SourceValue = SourceValue.Low;
            Nor.B.SourceValue = SourceValue.Low;


            Assert.AreEqual(LogicValue.Indeterminate, Nor.A.Value);
            Assert.AreEqual(LogicValue.Indeterminate, Nor.B.Value);

            //Propagate input values:
            W.Iterate();

            Assert.AreEqual(LogicValue.Low, Nor.A.Value);
            Assert.AreEqual(LogicValue.Low, Nor.B.Value);

            //Nor gate hasn't been computed yet
            Assert.AreEqual(LogicValue.Indeterminate, Nor.Output.Value);

            W.Iterate();

            //Nor gate computed:
            Assert.AreEqual(SourceValue.Hi, Nor.Output.SourceValue);
            Assert.AreEqual(LogicValue.Hi, Nor.Output.Value);


            Nor.A.SourceValue = SourceValue.Low;
            Nor.B.SourceValue = SourceValue.Hi;

            Assert.AreEqual(LogicValue.Hi, Nor.Output.Value);

            //Propagate inputs change:
            W.Iterate();

            Assert.AreEqual(LogicValue.Hi, Nor.Output.Value);

            Assert.AreEqual(LogicValue.Low, Nor.A.Value);
            Assert.AreEqual(LogicValue.Hi, Nor.B.Value);

            //Compute nor gate:
            W.Iterate();

            Assert.AreEqual(LogicValue.Low, Nor.Output.Value);

            //Nor behaves correctly with indeterminate inputs:
            Nor.A.SourceValue = SourceValue.Indeterminate;
            Nor.B.SourceValue = SourceValue.Hi;

            //Propagate inputs:
            W.Iterate();

            //Check that nor output is ow:
            Assert.AreEqual(LogicValue.Low, Nor.Output.Value);
        }

    }
}
