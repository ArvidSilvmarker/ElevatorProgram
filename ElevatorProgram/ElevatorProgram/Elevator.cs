using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ElevatorProgram
{
    public enum ElevatorState
    {
        Busy, Waiting
    }

    public class Elevator
    {
        public int HighestFloor { get; private set; }
        public int LowestFloor { get; private set; }
        public int CurrentFloor { get; private set; }
        public bool DoorIsOpen { get; private set; }
        public string Name { get; private set; }
        public uint MaximumWeight { get; private set; }
        public uint Capacity { get; private set; }
        public int TargetFloor { get; private set; }
        public string Message { get; set; }
        public ElevatorState Status  { get; private set; }

        public Elevator(string name, int lowestFloor, int highestFloor, int startFloor, uint capacity,
            uint maximumWeight)
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

            if (maximumWeight == 0)
                throw new ArgumentException("Hissen måste ta mer än 0kg i vikt.");
            MaximumWeight = maximumWeight;

            if (capacity <= 0 || capacity > 8)
                throw new ArgumentException("Måste måste ta 1-8 personer.");
            Capacity = capacity;

            DoorIsOpen = false;
            TargetFloor = CurrentFloor;

        }



        public void GoUp()
        {
            if (CurrentFloor >= HighestFloor)
                throw new InvalidOperationException("Hissen kan inte åka ovanför översta våningen.");
            CurrentFloor++;

            if (CurrentFloor != TargetFloor)
            {
                Status = ElevatorState.Busy;
                Message = $"{Name} är på våning {CurrentFloor} på väg mot våning {TargetFloor}";
            }
            else
            {
                WaitOnFloor();
            }
            
        }

        public void GoDown()
        {
            if (CurrentFloor <= LowestFloor)
                throw new InvalidOperationException("Hissen kan inte åka under nedersta våningen.");
            CurrentFloor--;

            if (CurrentFloor != TargetFloor)
            {
                Status = ElevatorState.Busy;
                Message = $"{Name} är på våning {CurrentFloor} på väg mot våning {TargetFloor}";
            }
            else
            {
                WaitOnFloor();
            }
        }

        public void OpenDoor()
        {
            DoorIsOpen = true;
            Message = $"{Name} öppnar dörrarna på våning {CurrentFloor}";
        }

        public void CloseDoor()
        {
            DoorIsOpen = false;
            Message = $"{Name} stänger dörrarna på våning {CurrentFloor}";
        }

        public int Count()
        {
            return (1 + HighestFloor - LowestFloor);
        }

        public string Report()
        {
            return
                $"{Name} är på våning {CurrentFloor}. Hisschaktet har {Count()} våningar. Dörrarna är {(DoorIsOpen ? "öppna" : "stängda")}.";
        }

        public void GoToFloor(int target)
        {
            if (target < LowestFloor || target > HighestFloor)
                throw new ArgumentOutOfRangeException("Hissen kan bara åka till en våning som existerar.");
            TargetFloor = target;
            Message = $"{Name} är på väg mot våning {TargetFloor}.";
        }

        public void WaitOnFloor()
        {
            Status = ElevatorState.Waiting;
            Message = $"{Name} är på våning {CurrentFloor}. ";  //Dörrarna är {(DoorIsOpen ? "öppna" : "stängda")}.";
        }
    }

}
