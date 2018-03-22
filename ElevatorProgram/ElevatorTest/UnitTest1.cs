using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElevatorProgram;
using System;

namespace ElevatorTest
{
    [TestClass]
    public class ElevatorTest
    {
        //private ElevatorProgram.Elevator elevator = new ElevatorProgram.Elevator("Hiss1", -3, 10, 0);

        [TestMethod]
        public void StartFloorOverBound()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Elevator("Hiss1", 0, 1, 2, 6, 500));
        }

        [TestMethod]
        public void StartFloorUnderBound()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Elevator("Hiss1", 0, 1, -1, 6, 600));
        }

    }
}
