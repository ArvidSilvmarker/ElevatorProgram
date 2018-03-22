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

        public List<Elevator> Elevators { get; private set; }
        public List<Person> People { get; private set; }

        public Building(int lowestFloor, int highestFloor)
        {
            if (highestFloor <= lowestFloor)
                throw new ArgumentException("Översta våningen måste vara ovanför understa våningen.");
            HighestFloor = highestFloor;
            LowestFloor = lowestFloor;
            if (lowestFloor < 0 && highestFloor > 0)
                GroundFloor = 0;
            else
                GroundFloor = lowestFloor;

            MaxPeopleInHallway = 6;
            People = new List<Person>();
        }

        public void GenerateElevators(int numberOfElevators)
        {
            uint maxWeight = 500;
            uint capacity = 4;

            Elevators = new List<Elevator>();
            int startFloor = LowestFloor > 0 ? LowestFloor : 0;
            for (int i = 0; i < numberOfElevators; i++)
            {
                Elevators.Add(new Elevator($"Hiss {i + 1}", LowestFloor, HighestFloor, startFloor, capacity, maxWeight));
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
    }
}
