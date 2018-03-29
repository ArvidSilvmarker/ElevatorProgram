using System;
using System.Collections.Generic;
using System.Text;

namespace ElevatorProgram
{
    public enum Direction
    {
        Up, Down
    }
    public class Command
    {
        public int Floor { get; set; }
        public Direction Direction { get; set; }

        public Command(int floor, Direction direction)
        {
            Floor = floor;
            Direction = direction;
        }
    }
}
