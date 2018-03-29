using System;
using System.Collections.Generic;
using System.Text;

namespace ElevatorProgram
{
    public class ButtonQueue : Queue<Command>
    {
        public ButtonQueue Remove(int floor, Direction direction)
        {
            var newQueue = new ButtonQueue();
            foreach (Command command in this)
            {
                if (command.Floor != floor && command.Direction != direction)
                    newQueue.Enqueue(command);
            }

            return newQueue;
        }
    }
}
