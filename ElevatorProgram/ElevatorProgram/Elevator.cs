using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;

namespace ElevatorProgram
{
    public enum ElevatorState
    {
        GoingUp, GoingDown, Waiting, GoingTo
    }

    public enum DoorState
    {
        Closed, Closing, Open, Opening
    }

    public class Elevator
    {
        public int HighestFloor { get; private set; }
        public int LowestFloor { get; private set; }
        public int CurrentFloor { get; private set; }
        public DoorState Door { get; private set; }
        public string Name { get; private set; }
        public uint MaximumWeight { get; private set; }
        public uint Capacity { get; private set; }
        public List<int> TargetList { get; private set; }
        public int GoToFloor { get; set; }
        public Direction GoToDirection { get; set; }
        public string Message { get; set; }
        public ElevatorState Status { get; set; }
        public List<Person> Passengers { get; private set; }
        public Building Building { get; private set; }
        public DecisionMaker Logic { get; set; }




        public Elevator(Building building, DecisionMaker logic, string name, int lowestFloor, int highestFloor,
            int startFloor, uint capacity,
            uint maximumWeight)
        {
            Building = building ?? throw new NullReferenceException("Hissen måste ha en byggnad.");
            Logic = logic ?? throw new NullReferenceException("Hissen måste ha en AI.");

            if (string.IsNullOrWhiteSpace(name))
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

            if (maximumWeight == 0)
                throw new ArgumentException("Hissen måste ta mer än 0kg i vikt.");
            MaximumWeight = maximumWeight;

            if (capacity <= 0 || capacity > 8)
                throw new ArgumentException("Måste måste ta 1-8 personer.");
            Capacity = capacity;

            Door = DoorState.Closed;
            Status = ElevatorState.Waiting;
            TargetList = new List<int>();
            Passengers = new List<Person>();

        }



        public void GoUp()
        {
            if (Door != DoorState.Closed)
                throw new Exception("Dörrarna måste vara stängda.");
            if (CurrentFloor == HighestFloor)
                throw new Exception("Kan inte åka till himlen.");

            CurrentFloor++;
            Message = $"{Name} är på våning {CurrentFloor}.";

        }

        public void GoDown()
        {
            if (Door != DoorState.Closed)
                throw new Exception("Dörrarna måste vara stängda.");
            if (CurrentFloor == LowestFloor)
                throw new Exception("Kan inte åka till helvetet.");
            CurrentFloor--;
            Message = $"{Name} är på våning {CurrentFloor}.";

        }


        public void CycleDoor()
        {
            if (Door == DoorState.Closed)
            {
                Message = $"{Name} har öppnat dörrarna på våning {CurrentFloor}.";
                Door = DoorState.Open;
            }
            else if (Door == DoorState.Open)
            {
                Message = $"{Name} har stängt dörrarna på våning {CurrentFloor}.";
                Door = DoorState.Closed;
            }


        }

        public int Count()
        {
            return (1 + HighestFloor - LowestFloor);
        }

        public string Report()
        {
            return
                $"{Name} är på våning {CurrentFloor}. Hisschaktet har {Count()} våningar. Dörrarna är {Door.ToString()}.";
        }


        public void WaitOnFloor()
        {
            Message = $"{Name} är på våning {CurrentFloor}. "; //Dörrarna är {(DoorIsOpen ? "öppna" : "stängda")}.";
        }

        public void Update()
        {
            Move nextMove = Logic.GetNextMove(this);
            switch (nextMove)
            {
                case Move.CycleDoor:
                    CycleDoor();
                    break;
                case Move.GoUp:
                    GoUp();
                    Status = ElevatorState.GoingUp;
                    break;
                case Move.GoDown:
                    GoDown();
                    Status = ElevatorState.GoingDown;
                    break;
                case Move.GoTo:
                    GoToTarget();
                    break;
                default:
                    WaitOnFloor();
                    Status = ElevatorState.Waiting;
                    break;
            }

        }

        private void GoToTarget()
        {
            if (GoToFloor == CurrentFloor)
            {
                if (GoToDirection == Direction.Down)
                {
                    Status = ElevatorState.GoingDown;
                    CycleDoor();
                }
                else
                {
                    Status = ElevatorState.GoingUp; 
                    CycleDoor();
                }
            }
            else if (GoToFloor > CurrentFloor)
                GoUp();
            else if (GoToFloor < CurrentFloor)
                GoDown();
        }

        public void EnterPassenger(Person person)
        {
            if (person == null)
                throw new NullReferenceException("Person får inte vara null.");
            if (Passengers.Count >= Capacity)
                throw new Exception($"Något har gått fel med Capacity för {Name}.");
            if (person.TargetFloor == CurrentFloor)
                throw new Exception($"Personen har gått in i en hiss, fast den är på sin TargetFloor.");

            Passengers.Add(person);
            TargetList.Add(person.TargetFloor);
        }

        public bool IsCurrentFloor(int floor)
        {
            return CurrentFloor == floor;
        }

        public void ExitPassenger(Person person)
        {
            Passengers.Remove(person);
            TargetList.Remove(CurrentFloor);
        }

        public void SetGoToTarget(Command nextTarget)
        {
            Status = ElevatorState.GoingTo;
            GoToDirection = nextTarget.Direction;
            GoToFloor = nextTarget.Floor;
        }
    }

}
