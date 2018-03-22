using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace ElevatorProgram
{
    class Motor
    {
        public Building Building { get; private set; }
        public List<Person> People { get; private set; }
        public uint UpdateSpeed { get; set; }

        public Motor(Building b) : this(b, new Random().Next(1,7), 500) 
        {
        }

        public Motor(Building b, int numberOfPeople, uint updateSpeed)
        {
            Building = b ?? throw new NullReferenceException("Måste finnas en byggnad.");

            if (numberOfPeople < 0)
                throw new ArgumentException();
            People = new List<Person>();
            for (int i = 0; i < numberOfPeople; i++)
                People.Add(new Person(b));

            if (updateSpeed > 1000000)
                throw new ArgumentException("Woaaa! Alldeles för snabbt.");
            if (updateSpeed <= 0) throw new ArgumentOutOfRangeException(nameof(updateSpeed));
            UpdateSpeed = updateSpeed;
        }

        public void Update()
        {
            while (true)
            {
                Tick();
            }
        }

        public void Update(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Tick();
            }
        }

        public void UpdateUntilAllWaiting()
        {
            while (true)
            {
                Tick();
                bool anybusy = false;
                foreach (var elevator in Building.Elevators)
                    if (elevator.Status == ElevatorState.Busy)
                        anybusy = true;
                if (!anybusy)
                    break;
            }
        }

        public void UpdateRandom(int maxNumberOfConcurrentWaiting)
        {
            while (true)
            {
                Tick();
                if (NumberOfWaiting() > maxNumberOfConcurrentWaiting)
                    SetRandomTarget();
            }


        }


        public void Tick()
        {
            foreach (var elevator in Building.Elevators)
            {
                if (elevator.TargetFloor > elevator.CurrentFloor)
                    elevator.GoUp();
                else if (elevator.TargetFloor < elevator.CurrentFloor)
                    elevator.GoDown();
                else
                    elevator.WaitOnFloor();
            }
            DrawBuilding();
            WriteAllMessages();
            Thread.Sleep(Convert.ToInt32(UpdateSpeed));
        }

        public void DrawBuilding()
        {
            Console.SetCursorPosition(0, 0);
            for (int floor = Building.HighestFloor; floor >= Building.LowestFloor; floor--)
            {
                Console.Write($"{GetFloorString(floor)}  : ");
                var peopleInHallway = Building.GetPeopleInHallway(floor);
                for (int hallwayPos = Building.MaxPeopleInHallway-1; hallwayPos <= 0 ; hallwayPos++)
                {
                    if (peopleInHallway.Count < hallwayPos)
                        Console.Write(" ");
                    else
                    {
                        Console.ForegroundColor = peopleInHallway[hallwayPos].Color;
                        Console.Write($"{peopleInHallway[hallwayPos].TargetFloor}");
                    }


                }
                for (int elevatorPos = 0; elevatorPos < Building.Elevators.Count; elevatorPos++)
                {
                    if (Building.Elevators[elevatorPos].CurrentFloor == floor)
                        Console.Write(" | H | ");
                    else
                        Console.Write(" |   | ");
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
        public void ClearLines(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                Console.WriteLine("                                                                  ");
            }
            Console.SetCursorPosition(0, Console.CursorTop - lines);
        }

        public void WriteMessage(string text)
        {
            Console.SetCursorPosition(0, Building.GetTotalHeight() + 6);
            Console.WriteLine(text);
        }

        public void WriteMessage(string[] texts)
        {
            Console.SetCursorPosition(0, Building.GetTotalHeight() + 6);
            foreach (var text in texts)
            {
                Console.WriteLine(text);
            }
        }

        public void WriteAllMessages()
        {

            Console.SetCursorPosition(0, Building.GetTotalHeight() + 2);
            ClearLines(Building.Elevators.Count);
            foreach (var elevator in Building.Elevators)
                Console.WriteLine(elevator.Message);
        }

        public void SetTarget(int elevatorNumber, int moveToFloor)
        {
            Building.Elevators[elevatorNumber -1].GoToFloor(moveToFloor);
        }

        private void SetRandomTarget()
        {
            var random = new Random();
            SetTarget(random.Next(1,Building.Elevators.Count+1),random.Next(Building.LowestFloor,Building.HighestFloor+1));
        }

        public int NumberOfWaiting()
        {
            int count = 0;
            foreach (var elevator in Building.Elevators)
                if (elevator.Status == ElevatorState.Waiting)
                    count++;
            return count;

        }

    }
}