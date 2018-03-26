using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ElevatorProgram
{
    public class Building
    {
        public int HighestFloor { get; private set; }
        public int LowestFloor { get; private set; }
        public int GroundFloor { get; private set; }
        public int MaxPeopleInHallway { get; private set; }
        public List<int> ButtonUpPressed { get; private set; }
        public List<int> ButtonDownPressed { get; private set; }
        public List<Elevator> Elevators { get; private set; }
        public List<Person> People { get; private set; }

        public Building(int lowestFloor, int highestFloor, int maxPeopleInHallway)
        {
            if (highestFloor <= lowestFloor)
                throw new ArgumentException("Översta våningen måste vara ovanför understa våningen.");
            HighestFloor = highestFloor;
            LowestFloor = lowestFloor;
            if (lowestFloor < 0 && highestFloor > 0)
                GroundFloor = 0;
            else
                GroundFloor = lowestFloor;

            MaxPeopleInHallway = maxPeopleInHallway;
            People = new List<Person>();
            ButtonUpPressed = new List<int>();
            ButtonDownPressed = new List<int>();
        }

        public void GenerateElevators(int numberOfElevators)
        {
            GenerateElevators(numberOfElevators, 4);
        }

        public void GenerateElevators(int numberOfElevators, uint capacity)
        {
            uint maxWeight = 500;

            Elevators = new List<Elevator>();
            int startFloor = LowestFloor > 0 ? LowestFloor : 0;
            for (int i = 0; i < numberOfElevators; i++)
            {
                Elevators.Add(new Elevator(this, $"Hiss {i + 1}", LowestFloor, HighestFloor, startFloor, capacity, maxWeight));
            }
        }

        public void GeneratePeople(int numberOfPeople)
        {

            for (int i = 0; i < numberOfPeople; i++)
                People.Add(new Person(this));
        }

        public void GeneratePeople()
        {
            GeneratePeople(MaxPeopleInHallway);
        }

        public int GetTotalHeight()
        {
            return HighestFloor - LowestFloor + 1;
        }

        public List<Person> GetPeopleInHallway(int floor)
        {
            List<Person> peopleInHallway = new List<Person>();
            foreach (Person person in People)
            {
                if (person.CurrentFloor == floor && !person.InElevator)
                    peopleInHallway.Add(person);
            }

            return peopleInHallway;
        }

        public List<Elevator> AvailableElevators(int currentFloor, bool goingUp)
        {

            List<Elevator> availableElevators = new List<Elevator>();
            foreach (var elevator in Elevators)                                         //En hiss är tillgänglig att stiga in i, om
                if (elevator.CurrentFloor == currentFloor                               //... den är på samma våning som personen
                    && elevator.Door == DoorState.Open                                  //... och dörrarna är öppna
                    && elevator.Capacity > elevator.Passengers.Count                    //... och den inte är full
                    && (GoingSameWay(elevator, goingUp)))                               //... och den är på väg åt rätt håll (eller väntar)
                     availableElevators.Add(elevator);
            return availableElevators;
        }

        public bool GoingSameWay(Elevator elevator, bool goingUp)
        {
            if (elevator.Status == ElevatorState.GoingDown && goingUp)
                return false;
            if (elevator.Status == ElevatorState.GoingUp && !goingUp)
                return false;
            return true;
        }

        public bool IsUpButtonPressed(int floor)
        {
            return ButtonUpPressed.Contains(floor);
        }

        public bool IsDownButtonPressed(int floor)
        {
            return ButtonDownPressed.Contains(floor);
        }

        public void PressUpButton(int floor)
        {
            ButtonUpPressed.Add(floor);
            SendElevator(floor, true);
        }
        public void PressDownButton(int floor)
        {
            ButtonDownPressed.Add(floor);
            SendElevator(floor, false);
        }

        public void SendElevator(int floor, bool upButtonPressed)
        {
            SendRandomElevator(floor, upButtonPressed);
        }

        public void SendRandomElevator(int floor, bool upButtonPressed)
        {
            var random = new Random();
            var elevator = Elevators[random.Next(0, Elevators.Count)];
            elevator.AddTarget(floor, upButtonPressed);
        }

    }
}
