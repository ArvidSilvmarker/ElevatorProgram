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
            app.RandomPeople();
        }


        public void RandomPeople()
        {
            int count = 0;
            var b = new Building(-3, 10);
            var m = new Motor(b);
            var p = new Person(b);
            for (int i = 0; i < 10; i++)
            {
                var person = new Person(b);
                Console.ForegroundColor = person.Color;
                Console.WriteLine(
                    $"Person {i} är på våning {person.CurrentFloor} och vill till våning {person.TargetFloor}.");
                Console.WriteLine($"Vikt: {person.Weight}kg.");
                Console.WriteLine();
            }


        }


        public void RandomColors()
        {
            Console.WriteLine("Default färg.");

            var random = new Random();
            for (int i = 0; i < 16; i++)
            {
                //int color = random.Next(0, 16);
                int color = i;
                Console.ForegroundColor = (ConsoleColor)color;
                Console.WriteLine($"Denna text är {Enum.GetName(typeof(ConsoleColor),(ConsoleColor)color)}. Nummer {color}.");
                Console.ResetColor();

                //    Denna text är Black. Nummer 0.
                //    Denna text är DarkBlue. Nummer 1.
                //    Denna text är DarkGreen. Nummer 2.
                //    Denna text är DarkCyan. Nummer 3.
                //    Denna text är DarkRed. Nummer 4.
                //    Denna text är DarkMagenta. Nummer 5.
                //    Denna text är DarkYellow. Nummer 6.
                //    Denna text är Gray. Nummer 7.
                //    Denna text är DarkGray. Nummer 8.
                //    Denna text är Blue. Nummer 9.
                //    Denna text är Green. Nummer 10.
                //    Denna text är Cyan. Nummer 11.
                //    Denna text är Red. Nummer 12.
                //    Denna text är Magenta. Nummer 13.
                //    Denna text är Yellow. Nummer 14.
                //    Denna text är White. Nummer 15.

            }



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
            b.GenerateElevators(numberOfElevators);
            var m = new Motor(b);

            Console.Clear();
            m.DrawBuilding();

            Random random = new Random();
            while (true)
            {
                Console.WriteLine();
                m.ClearLines(3);
                m.UpdateRandom(Convert.ToInt32(Math.Floor(numberOfElevators/2.0)));

            }
        }
       

        public void Run()
        {
            var b = new Building(-3, 10);
            b.GenerateElevators(3);
            var m = new Motor(b, 5, 1000);
            m.DrawBuilding();

            while (true)
            {
                Console.WriteLine();
                m.ClearLines(2);

                Console.Write("Flytta hiss nummer: ");
                string input = Console.ReadLine();
                if (input.Trim().ToLower() == "quit")
                    break;
                int elevatorNumber = Convert.ToInt32(input);

                Console.Write("Till våning nummer: ");
                int moveToFloor = GetFloorNumber(Console.ReadLine());

                m.SetTarget(elevatorNumber, moveToFloor);
                m.UpdateUntilAllWaiting();
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



    }

   

    
}

