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

        public uint UpdateSpeed { get; set; }

        public Motor(Building b, uint updateSpeed)
        {
            Building = b ?? throw new NullReferenceException("Måste finnas en byggnad.");

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
                    if (elevator.Status != ElevatorState.Waiting)
                        anybusy = true;
                if (!anybusy)
                    break;
            }
        }

        public void UpdateOnSpace()
        {
            while (true)
            {
                Char c = Console.ReadKey(true).KeyChar;

                if (c == ' ')
                    Tick();
                if (c == 'q')
                    break;
            }
        }


        public void Tick()
        {
            foreach (var elevator in Building.Elevators)
                elevator.Update();
            foreach (var person in Building.People)
                person.Update();
            DrawBuilding();
            WriteAllMessages();
            Thread.Sleep(Convert.ToInt32(UpdateSpeed));
        }

        public void DrawBuilding()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine();
            DrawGoingDirection();
            for (int floor = Building.HighestFloor; floor >= Building.LowestFloor; floor--)
            {
                Console.Write($"{GetFloorString(floor)}");
                Console.Write($"{GetButtonString(floor)}");
                DrawPeopleInHallway(Building.GetPeopleInHallway(floor));
                DrawElevators(floor);

                Console.WriteLine();
            }

        }

        private void DrawGoingDirection()
        {
            int numberOfSpace = GetFloorString(Building.HighestFloor).Length + Building.MaxPeopleInHallway * 2 + 1 + GetButtonString(Building.HighestFloor).Length;
            for (int i = 0; i < numberOfSpace; i++)
                Console.Write(" ");
            for (int elevatorPos = 0; elevatorPos < Building.Elevators.Count; elevatorPos++)
            {
                if (Building.Elevators[elevatorPos].TargetUp.Count > 0)
                    Console.Write("^");
                else
                    Console.Write(" ");

                if (Building.Elevators[elevatorPos].TargetDown.Count > 0)
                    Console.Write("v");
                else
                    Console.Write(" ");

                for (int posInElevator = 0;
                    posInElevator < Building.Elevators[elevatorPos].Capacity;
                    posInElevator++)
                    Console.Write("  ");
            }

            Console.WriteLine();
        }

        private void DrawElevators(int floor)
        {
            for (int elevatorPos = 0; elevatorPos < Building.Elevators.Count; elevatorPos++)
            {

                if (Building.Elevators[elevatorPos].CurrentFloor == floor)
                {
                    Console.Write("|H");
                    for (int posInElevator = 0;
                        posInElevator < Building.Elevators[elevatorPos].Capacity;
                        posInElevator++)
                    {

                        if (posInElevator < Building.Elevators[elevatorPos].Passengers.Count -1)
                            DrawPerson(Building.Elevators[elevatorPos].Passengers[posInElevator]);
                        else
                            Console.Write("  ");
                    }
                    Console.Write("|"); 
                }
                else
                {
                    Console.Write("| ");
                    for (int posInElevator = 0;
                        posInElevator < Building.Elevators[elevatorPos].Capacity;
                        posInElevator++)
                        Console.Write("  ");
                    Console.Write("|");
                }

            }
        }

        private void DrawPeopleInHallway(List<Person> peopleInHallway)
        {
            for (int hallwayPos = Building.MaxPeopleInHallway - 1; hallwayPos >= 0; hallwayPos--)
            {
                if (peopleInHallway.Count <= hallwayPos)
                    Console.Write("  ");
                else
                    DrawPerson(peopleInHallway[hallwayPos]);
            }
        }

        private void DrawPerson(Person person)
        {
            Console.ForegroundColor = person.Color;
            if (person.TargetFloor > 0 && person.TargetFloor < 10)
                Console.Write($"{person.TargetFloor:00}");
            else if (person.TargetFloor == 0)
                Console.Write($"BV");
            else
                Console.Write($"{person.TargetFloor}");

            Console.ResetColor();
        }

        private string GetFloorString(int i)
        {
            if (i > 9 && i < 100)                               //tvåsiffrigt
                return $" {i} : ";
            else if (i > 0 && i < 10)                           //ensiffrigt
                return $"  {i} : ";
            else if (i == 0)
                return $" BV : ";
            else if (i < 0 && i > -10)                          //ensiffrigt negativt
                return $" K{Math.Abs(i)} : ";
            else if (i < -9 && i > -100)                        //tvåsiffrigt negativt
                return $"K{Math.Abs(i)} : ";
            else
                return i.ToString();
        }

        private string GetButtonString(int floor)
        {
            string text = "";
            if (Building.IsUpButtonPressed(floor))
                text += ("^");
            else
                text += (" ");

            if (Building.IsDownButtonPressed(floor))
                text += ("v");
            else
                text += (" ");
            text += " ";
            return text;
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