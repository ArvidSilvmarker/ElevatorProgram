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
            app.Run();
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
    


        public void Run()
        {
            var b = new Building(-3, 10, 2);
            int numberOfElevators = 1;
            b.GenerateElevators(numberOfElevators, 2);
            b.GeneratePeople();
            var m = new Motor(b, 1000);
            m.DrawBuilding();

            m.UpdateOnSpace();

            //while (true)
            //{
            //    Console.SetCursorPosition(0, b.GetTotalHeight() + numberOfElevators + 2);
            //    Console.WriteLine();
            //    m.ClearLines(2);

            //    Console.Write("Flytta hiss nummer: ");
            //    string input = Console.ReadLine();
            //    if (input.Trim().ToLower() == "quit")
            //        break;
            //    int elevatorNumber = Convert.ToInt32(input);

            //    Console.Write("Till våning nummer: ");
            //    int moveToFloor = GetFloorNumber(Console.ReadLine());

            //    m.SetTarget(elevatorNumber, moveToFloor);
            //    m.UpdateUntilAllWaiting();
            //}

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

