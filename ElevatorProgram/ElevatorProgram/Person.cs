using System;
using System.Collections.Generic;
using System.Text;

namespace ElevatorProgram
{
    public class Person
    {
        public Building Building { get; set; }
        public int TargetFloor { get; set; }
        public int CurrentFloor { get; set; }
        public int Weight { get; set; }
        public ConsoleColor Color { get; set; }
        public int ElevatorNumber { get; set; }
        public bool InElevator { get; set; }

        public Person(Building b)
        {
            Building = b ?? throw new NullReferenceException("Byggnad får inte vara null.");
            RandomizePerson();
        }

        public Person(Building b, int targetFloor, int currentFloor, int weight, ConsoleColor color)
        {
            Building = b ?? throw new NullReferenceException("Byggnad får inte vara null.");
            if (targetFloor < b.LowestFloor || targetFloor > b.HighestFloor)
                throw new ArgumentException();
            TargetFloor = targetFloor;

            if (currentFloor < b.LowestFloor || currentFloor > b.HighestFloor)
                throw new ArgumentException();
            CurrentFloor = currentFloor;

            if (weight < 0)
                throw new ArgumentException();
            Weight = weight;

            Color = color;

        }

        public void RandomizePerson()
        {
            var random = new Random();

            if (TargetFloor == CurrentFloor)
            {
                while (TargetFloor == CurrentFloor)
                {
                    CurrentFloor = FlipCoin()
                        ? Building.GroundFloor
                        : random.Next(Building.LowestFloor, Building.HighestFloor + 1);

                    TargetFloor = CurrentFloor != Building.GroundFloor
                        ? FlipCoin(3) 
                            ? random.Next(Building.LowestFloor, Building.HighestFloor + 1)
                            :Building.GroundFloor
                        :random.Next(Building.LowestFloor, Building.HighestFloor + 1);
                }

            }

            if (Weight == 0)
            {
                Weight = 30;
                for (int i = 0; i < 4; i++)
                    Weight += random.Next(0, 25);                   //Weight = 30 + 4xD24. Avg = 80, Stnd deviation ~12
            }

            if (Color == ConsoleColor.Black)                        //default of ConsoleColor is Black.
                Color = (ConsoleColor)random.Next(9, 16);
        }

        public bool FlipCoin()
        {
            Random random = new Random();
            if (random.Next(0, 2) == 0)
                return true;
            return false;
        }

        public bool FlipCoin(int i)
        {
            Random random = new Random();
            if (random.Next(0, i) == 0)
                return true;
            return false;
        }

    }
}
