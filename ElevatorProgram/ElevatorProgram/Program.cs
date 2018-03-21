using System;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ElevatorProgram
{
 

    public class ElevatorProgram
    {
        static void Main(string[] args)
        {
            ElevatorProgram app = new ElevatorProgram();
            app.Run();
        }

        public void Demo()
        {
            Console.Write("Sammanlagt antal våningar: ");
            int floors = Convert.ToInt32(Console.ReadLine());
            Console.Write("Antal källarvåningar: ");
            int cellarFloors = Convert.ToInt32(Console.ReadLine());
            Console.Write("Antal hissar: ");
            int numberOfElevators = Convert.ToInt32(Console.ReadLine());

            var b = new Building(-cellarFloors, floors - cellarFloors);
            b.GeneratorElevators(numberOfElevators);

            Console.Clear();
            b.DrawBuilding();

            Random random = new Random();
            while (true)
            {
                Console.WriteLine();
                ClearLines(2);
                int elevatorNumber = random.Next(1, numberOfElevators + 1);
                int moveToFloor = random.Next(b.LowestFloor, b.HighestFloor + 1);

                b.MoveElevator(elevatorNumber, moveToFloor);

            }
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
                int moveToFloor = GetFloorNumber(Console.ReadLine());

                b.MoveElevator(elevatorNumber, moveToFloor);
            }

        }

        private int GetFloorNumber(string input)
        {

            if (new Regex(@"[kK]0*\d+").IsMatch(input))                 //källarvåningar (tex. K01)
                return -1 * Convert.ToInt32(new Regex(@"[1-9]+\d*").Match(input).ToString());
            if (new Regex(@"[bB][vV]").IsMatch(input))                   //BV
                return 0;
            if (new Regex(@"-*\d+").IsMatch(input))                     //bara siffror, inkl negativa
                return Convert.ToInt32(input);
            throw new ArgumentException("Måste skriva en siffra.");
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

    public class Building
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
            if (elevator.LowestFloor <= targetFloor && targetFloor <= elevator.HighestFloor)
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

    public class Elevator
    {
        public int HighestFloor { get; private set; }
        public int LowestFloor { get; private set; }
        public int CurrentFloor { get; private set; }
        public string Name { get; private set; }

        public Elevator(string name, int lowestFloor, int highestFloor, int startFloor)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name kan inte vara null eller tom.");
            if (name.Length > 8)
                throw new ArgumentException("Name får inte vara mer än 8 tecken.");
            Name = name;

            LowestFloor = lowestFloor;

            if (highestFloor <= lowestFloor)
                throw new ArgumentOutOfRangeException("HighestFloor måste ligga över LowestFloor.");
            HighestFloor = highestFloor;

            if (startFloor < lowestFloor || startFloor > highestFloor)
                throw new ArgumentOutOfRangeException("StartFloor måste vara en tillåten våning.");
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
            if (CurrentFloor >= HighestFloor)
                throw new InvalidOperationException("Hissen kan inte åka ovanför översta våningen.");
            CurrentFloor++;
            WriteMessage($"{Name} är på våning {CurrentFloor}");
        }

        public void GoDown()
        {
            if (CurrentFloor <= LowestFloor)
                throw new InvalidOperationException("Hissen kan inte åka under nedersta våningen.");
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

