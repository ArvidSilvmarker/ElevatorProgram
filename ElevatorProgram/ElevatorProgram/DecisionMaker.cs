using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ElevatorProgram
{
    public enum Move
    {
        CycleDoor, GoUp, GoDown, GoTo, Wait
    }
    public interface DecisionMaker
    {
        Queue<int> TargetUp { get; set; }
        Queue<int> TargetDown { get; set; }
        Building Building { get; set; }
        Move GetNextMove(Elevator e);
    }



    class DecisionManual : DecisionMaker
    {
        public Queue<int> TargetUp { get; set; }
        public Queue<int> TargetDown { get; set; }
        public Building Building { get; set; }

        public Move GetNextMove(Elevator e)
        {
            var move = ElevatorProgram.AskUserForNextMove(e);
            if (move == Move.CycleDoor)
                e.Building.ResetButton(e.CurrentFloor, ElevatorState.Waiting);
            return move;
        }
    }

    class DecisionClassic : DecisionMaker
    {
        public Queue<int> TargetUp { get; set; }
        public Queue<int> TargetDown { get; set; }
        public Building Building { get; set; }

        public DecisionClassic(Building b)
        {
            Building = b;
            TargetUp = new Queue<int>();
            TargetDown = new Queue<int>();
        }

        public Move GetNextMove(Elevator e)
        {
            //Input
            var status = e.Status;                                  //Visar vad hissen håller på med: GoingUp, GoingDown, Waiting, GoingTo (dvs åk direkt utan att stanna).
            var door = e.Door;                                      //Visar om dörren är öppen eller stängd.
            var currentFloor = e.CurrentFloor;                      //Visar var hissen är just nu.
            var targetList = e.TargetList;                          //Folk inne i hissen som har tryckt på knappar där de vill av.
            var buttonPressed = Building.ButtonPressed;             //Folk utanför hissen som har tryckt på "åka uppåt" eller "åka neråt"-knappen


            // 1. Om hissen åker direkt till en våning, låt den fortsätta med det
            if (status == ElevatorState.GoingTo)
                return Move.GoTo;

            // 2. Om dörren är öppen, stäng den
            if (door == DoorState.Open)
                return Move.CycleDoor;

            // 3. Om hissen är på en våning där den ska öppna dörren, gör det
            if (targetList.Contains(currentFloor) ||
                (status == ElevatorState.GoingDown && Building.IsDownButtonPressed(currentFloor)) ||
                (status == ElevatorState.GoingUp && Building.IsUpButtonPressed(currentFloor)))
                return Move.CycleDoor;

            // 4. Om det finns fler mål i hissens riktning, åk dit
            if (status == ElevatorState.GoingDown && targetList.Any(floor => floor < currentFloor))
                return Move.GoDown;

            if (status == ElevatorState.GoingUp && targetList.Any(floor => floor > currentFloor))
                return Move.GoUp;

            // 5. Annars vänd och kolla igen
            if (status == ElevatorState.GoingDown && targetList.Any(floor => floor > currentFloor))
            {
                e.Status = ElevatorState.GoingUp;
                return Move.GoUp;
            }

            if (status == ElevatorState.GoingUp && targetList.Any(floor => floor < currentFloor))
            {
                e.Status = ElevatorState.GoingDown;
                return Move.GoDown;
            }
                
            // 6. Om det inte finns fler interna mål, fråga byggnaden om någon har tryckt på knappen och åk direkt dit
            if (targetList.Count == 0 && buttonPressed.Count > 0)
            {
                var nextTarget = GetNextButtonPress();
                e.SetGoToTarget(nextTarget);
                return Move.GoTo;
            }

            // 7. Vänta
            return Move.Wait;


        }

        private Command GetNextButtonPress()
        {
            Command c = Building.ButtonPressed[0];
            Building.ButtonPressed.RemoveAt(0);
            return c;
        }
    }
}
