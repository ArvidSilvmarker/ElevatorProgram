using System;
using System.Threading;
using System.Collections.Generic;

namespace ElevatorProgram
{
 

    class ElevatorProgram
    {
        static void Main(string[] args)
        {
            ElevatorProgram app = new ElevatorProgram();
            app.Run();
        }

        public void Run()
        {
            var b = new Building(-3, 10);
            b.GeneratorElevators(3);
            b.DrawBuilding();

            while (true)
            {
                Console.WriteLine();
                ClearLines(2);

                Console.Write("Flytta hiss nummer: ");
                string input = Console.ReadLine();
                if (input.Trim().ToLower() == "quit")
                    break;
                int elevatorNumber = Convert.ToInt32(input);

                Console.Write("Till våning nummer: ");
                int moveToFloor = Convert.ToInt32(Console.ReadLine());

                b.MoveElevator(elevatorNumber, moveToFloor);
            }

        }

        public void ClearLines(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                Console.WriteLine("                                          ");
            }

            Console.SetCursorPosition(0, Console.CursorTop - lines);
        }

    }

    class Building
    {
        public int HighestFloor { get; private set; }
        public int LowestFloor { get; private set; }

        public List<Elevator> Elevators { get; private set; }

        public Building(int lowestFloor, int highestFloor)
        {
            if (highestFloor > lowestFloor)
                HighestFloor = highestFloor;
            else
                HighestFloor = lowestFloor + 1;
            LowestFloor = lowestFloor;
        }

        public void GeneratorElevators(int numberOfElevators)
        {
            Elevators = new List<Elevator>();
            int startFloor = LowestFloor > 0 ? LowestFloor : 0;
            for (int i = 0; i < numberOfElevators; i++)
            {
                Elevators.Add(new Elevator($"Hiss{i + 1}", LowestFloor, HighestFloor, startFloor));
            }
        }

        public void DrawBuilding()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = HighestFloor; i >= LowestFloor; i--)
            {
                Console.Write($"{GetFloorString(i)}");
                for (int j = 0; j < Elevators.Count; j++)
                {
                    if (Elevators[j].CurrentFloor == i)
                        Console.Write(" |H| ");
                    else
                        Console.Write(" | | ");
                }
                Console.WriteLine();
            }

        }

        private string GetFloorString(int i)
        {
            if (i > 9 && i < 100)                               //tvåsiffrigt
                return $" {i}";
            else if (i > 0 && i < 10)                           //ensiffrigt
                return $"  {i}";
            else if (i == 0)
                return $" BV";
            else if (i < 0 && i > -10)                          //ensiffrigt negativt
                return $" K{Math.Abs(i)}";
            else if (i < -9 && i > -100)                        //tvåsiffrigt negativt
                return $"K{Math.Abs(i)}";
            else
                return i.ToString();
        }

        public void MoveElevator(int elevatorNumber, int targetFloor)
        {
            var elevator = Elevators[elevatorNumber - 1];
            while (true)
            {
                if (targetFloor < elevator.CurrentFloor)
                    elevator.GoDown();
                else if (targetFloor > elevator.CurrentFloor)
                    elevator.GoUp();
                else
                    break;
                DrawBuilding();
                Thread.Sleep(500);

            }
        }
    }

    class Elevator
    {
        public int HighestFloor { get; private set; }
        public int LowestFloor { get; private set; }
        public int CurrentFloor { get; private set; }
        public string Name { get; private set; }

        public Elevator(string name, int lowestFloor, int highestFloor, int startFloor)
        {
            if (name == null)
                throw new NullReferenceException("Namn kan inte vara null.");
            else
                Name = name;
            LowestFloor = lowestFloor;
            if (highestFloor > lowestFloor)
                HighestFloor = highestFloor;
            else
                HighestFloor = lowestFloor + 1;
            if (startFloor >= lowestFloor && startFloor <= highestFloor)
                CurrentFloor = startFloor;
        }

        public Elevator(string name, int lowestFloor, int highestFloor) : this(name, lowestFloor, highestFloor,
            lowestFloor > 0 ? lowestFloor : 0)
        {
        }

        public void WriteMessage(string text)
        {
            Console.SetCursorPosition(0, Count() + 6);
            Console.WriteLine(text);
        }

        public void GoUp()
        {
            if (CurrentFloor < HighestFloor)
                CurrentFloor++;
            WriteMessage($"{Name} är på våning {CurrentFloor}");
        }

        public void GoDown()
        {
            if (CurrentFloor > LowestFloor)
                CurrentFloor--;
            WriteMessage($"{Name} är på våning {CurrentFloor}");
        }

        public void OpenDoor()
        {
            WriteMessage($"{Name} öppnar dörrarna på våning {CurrentFloor}");
        }

        public int Count()
        {
            return (1 + HighestFloor - LowestFloor);
        }

        public void Report()
        {
            WriteMessage($"{Name} är på våning {CurrentFloor}. Hisschaktet har {Count()} våningar.");
        }
    }
}

