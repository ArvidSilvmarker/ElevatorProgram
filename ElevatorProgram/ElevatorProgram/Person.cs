using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ElevatorProgram
{
    public class Person
    {
        public Building Building { get; set; }
        public int TargetFloor { get; set; }
        public int CurrentFloor { get; set; }
        public int Weight { get; set; }
        public ConsoleColor Color { get; set; }
        public Elevator Elevator { get; set; }
        public bool InElevator { get; set; }
        public string Message { get; private set; }


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

        public void Update()
        {
            if (InElevator)
                CurrentFloor = Elevator.CurrentFloor;

            var elevators = Building.AvailableElevators(CurrentFloor, TargetFloor > CurrentFloor ? true : false); //lista med hissar som är tillgängliga att stiga in i och på väg åt rätt håll

            //Ingen tillgänglig hiss på våningen.
            if (!InElevator && elevators.Count == 0)
                WaitOnFloor();

            //Exakt en tillgänglig hiss på våningen.
            else if (!InElevator && elevators.Count == 1)
                GetInElevator(elevators[0]);
                
            //Mer än en tillgänglig hiss på våningen.
            else if (!InElevator && elevators.Count > 1)
                GetInElevator(ChooseElevator(elevators));

            //I en hiss med öppna dörrar.
            else if (InElevator && Elevator.Door == DoorState.Open)
                if (CurrentFloor == TargetFloor)
                    GetOutOfElevator();
                else
                    WaitInElevator(Elevator);

            //I en hiss med icke-öppna dörrar.
                else if (InElevator && Elevator.Door != DoorState.Open)
                 WaitInElevator(Elevator);
        }

        private void WaitInElevator(Elevator elevator)
        {
            AvoidEyeContact();
        }

        private void AvoidEyeContact()
        {
            //Dumdidum...
        }

        private Elevator ChooseElevator(List<Elevator> elevators)
        {
            Elevator roomiestElevator = elevators[0];
            foreach (var elevator in elevators)
            {
                if (elevator.Capacity - elevator.Passengers.Count >
                    roomiestElevator.Capacity - roomiestElevator.Passengers.Count)
                    roomiestElevator = elevator;
            }

            return roomiestElevator;
        }

        private void GetInElevator(Elevator elevator)
        {
            if (elevator.CurrentFloor != CurrentFloor)
                throw new Exception($"Något har fått fel med CurrentFloor för en person och/eller {Elevator.Name}.");
            if (Elevator != null)
                throw new Exception();

            InElevator = true;
            Elevator = elevator;
            elevator.EnterPassenger(this);
            Building.ResetButton(CurrentFloor, (TargetFloor > CurrentFloor) ? ElevatorState.GoingUp : ElevatorState.GoingDown);
        }

        private void GetOutOfElevator()
        {
            if (Elevator == null)
                throw new NullReferenceException();
            if (Elevator.CurrentFloor != CurrentFloor)
                throw new Exception($"Något har fått fel med CurrentFloor för en person och/eller {Elevator.Name}.");

            InElevator = false;
            Elevator.ExitPassenger(this);
            Elevator = null;


        }


        private void WaitOnFloor()
        {
            if (TargetFloor > CurrentFloor && !Building.IsUpButtonPressed(CurrentFloor))
                Building.PressUpButton(CurrentFloor);
            else if (TargetFloor < CurrentFloor && !Building.IsDownButtonPressed(CurrentFloor))
                Building.PressDownButton(CurrentFloor);


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
