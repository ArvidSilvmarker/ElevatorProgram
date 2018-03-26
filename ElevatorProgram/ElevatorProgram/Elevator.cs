using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;

namespace ElevatorProgram
{
    public enum ElevatorState
    {
        GoingUp, GoingDown, Waiting, GoingDirectly
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
        public List<int> TargetUp { get; private set; }
        public List<int> TargetDown { get; private set; }
        public int DirectTargetFloor { get; private set; }
        public string Message { get; set; }
        public ElevatorState Status  { get; private set; }
        public List<Person> Passengers { get; private set; }
        public Building Building { get; private set; }
        public bool GoDirectly { get; set; }


        public Elevator(Building building, string name, int lowestFloor, int highestFloor, int startFloor, uint capacity,
            uint maximumWeight)
        {
            Building = building ?? throw new NullReferenceException("Hissen måste ha en byggnad.");

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
            TargetUp = new List<int>();
            TargetDown = new List<int>();
            Passengers = new List<Person>();

        }



        public void GoingUp()
        {
            int targetFloor = GetNextTarget();

            if (CurrentFloor >= HighestFloor)
                throw new InvalidOperationException("Hissen kan inte åka ovanför översta våningen.");

            if (Building.IsUpButtonPressed(CurrentFloor) || targetFloor == CurrentFloor)
            {
                TargetUp.RemoveAll(IsCurrentFloor);
                Building.ButtonUpPressed.RemoveAll(IsCurrentFloor);
                OpenDoor();
            }
            else
            {
                CurrentFloor++;
                Message = $"{Name} är på våning {CurrentFloor} på väg mot våning {targetFloor}.";
            }  
        }

        public void GoingDown()
        {
            int targetFloor = GetNextTarget();

            if (CurrentFloor <= LowestFloor)
                throw new InvalidOperationException("Hissen kan inte åka under nedersta våningen.");

            if (Building.IsDownButtonPressed(CurrentFloor) || targetFloor == CurrentFloor)
            {
                TargetDown.RemoveAll(IsCurrentFloor);
                Building.ButtonDownPressed.RemoveAll(IsCurrentFloor);
                OpenDoor();
            }
            else
            {
                CurrentFloor--;
                Message = $"{Name} är på våning {CurrentFloor} på väg mot våning {targetFloor}.";
            }
        }

        public void GoToTarget()
        {
           

        }

        public void OpenDoor()
        {
            if (Door == DoorState.Opening)
            {
                Message = $"{Name} har öppnat dörrarna på våning {CurrentFloor}.";
                Door = DoorState.Open;
            }
            else
            {
                Message = $"{Name} öppnar dörrarna på våning {CurrentFloor}.";
                Door = DoorState.Opening;
            }
        }

        public void CloseDoor()
        {
            if (Door == DoorState.Closing)
            {
                Message = $"{Name} har stängat dörrarna på våning {CurrentFloor}.";
                Door = DoorState.Closed;
            }
            else
            {
                Message = $"{Name} stänger dörrarna på våning {CurrentFloor}.";
                Door = DoorState.Closing;
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
            Message = $"{Name} är på våning {CurrentFloor}. ";  //Dörrarna är {(DoorIsOpen ? "öppna" : "stängda")}.";
        }

        public void Update()
        {
            if (Door == DoorState.Closing)
                CloseDoor();
            else if (Door == DoorState.Opening)
                OpenDoor();
            else if (Status == ElevatorState.GoingUp)
                GoingUp();
            else if (Status == ElevatorState.GoingDown)
                GoingDown();
            else if (Status == ElevatorState.GoingDirectly)
                GoToTarget();
            else
                WaitOnFloor();

            Status = ChangeStatus();

        }

        private ElevatorState ChangeStatus()
        {
            if (Status == ElevatorState.GoingDirectly && DirectTargetFloor == CurrentFloor)
                Status = ElevatorState.Waiting;
            if (TargetUp.Count == 0 && TargetDown.Count == 0)
                Status = ElevatorState.Waiting;
            else if (Status == ElevatorState.GoingDown && TargetDown.Count == 0)
                Status = ElevatorState.GoingUp;
            else if (Status == ElevatorState.GoingUp && TargetUp.Count == 0)
                Status = ElevatorState.GoingDown;

        }

        private int GetNextTarget()
        {
            int next = Status == ElevatorState.GoingUp ? HighestFloor : LowestFloor;
            if (Status == ElevatorState.GoingUp)
            {
                for (int floor = HighestFloor; floor >= CurrentFloor; floor--)
                    if (TargetUp.Contains(floor))
                        next = floor;
            }
            else if (Status == ElevatorState.GoingDown)
            {
                for (int floor = LowestFloor; floor <= CurrentFloor; floor++)
                    if (TargetDown.Contains(floor))
                        next = floor;
            }

            return next;

        }


        public void EnterPassenger(Person person)
        {
            if (Passengers.Count >= Capacity)
                throw new Exception($"Något har gått fel med Capacity för {Name}.");

            if (person.TargetFloor == CurrentFloor)
                throw new Exception($"Personen har gått in i en hiss, fast den är på sin TargetFloor.");

            Passengers.Add(person);
            if (person.TargetFloor > CurrentFloor)
            {
                TargetUp.Add(person.TargetFloor);
                if (Status == ElevatorState.Waiting)
                    Status = ElevatorState.GoingUp;
            }

            if (person.TargetFloor < CurrentFloor)
            {
                TargetDown.Add(person.TargetFloor);
                if (Status == ElevatorState.Waiting)
                    Status = ElevatorState.GoingDown;
            }
        }

        public bool IsCurrentFloor(int floor)
        {
            return CurrentFloor == floor;
        }

        public void ExitPassenger(Person person)
        {
            Passengers.Remove(person);
        }

        public void AddTarget(int targetFloor, bool goingUp)
        {
            if (Status == ElevatorState.Waiting && GoDirectly)
            {
                Status = ElevatorState.GoingDirectly;
                DirectTargetFloor = targetFloor;
            }
            else
            { 
                if (goingUp)
                    TargetUp.Add(targetFloor);
                else
                    TargetDown.Add(targetFloor);
            }
        }
    }

}
