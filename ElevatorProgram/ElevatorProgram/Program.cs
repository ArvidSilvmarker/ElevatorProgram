using System;
using System.CodeDom.Compiler;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ElevatorProgram
{
 

    public class ElevatorProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till hissprogrammet.");
            Console.WriteLine("(1) Kör en hiss i ett litet hus.");
            Console.WriteLine("(2) Kör flera hissar i ett stort hus.");
            Console.WriteLine("(3) Välj parametrar och kör hissar.");
            Console.WriteLine("(4) Låt hissarna köra själva.");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    RunSmall();
                    break;
                case "2":
                    RunLarge();
                    break;
                case "3":
                    RunOptions();
                    break;
                case "4":
                    RunAutomatic();
                    break;
            }

        }

        private static void RunAutomatic()
        {
            
            Console.Clear();
            Console.Write("Antal våningar: ");
            var floors = Convert.ToInt32(Console.ReadLine());
            Console.Write("Varav källarvåningar: ");
            var cellarFloors = Convert.ToInt32(Console.ReadLine());
            Console.Write("Antal hissar: ");
            var elevators = Convert.ToInt32(Console.ReadLine());
            Console.Write("Kapacitet i hissen: ");
            var elevatorCapacity = Convert.ToUInt32(Console.ReadLine());
            Console.Write("Antal människor: ");
            var people = Convert.ToInt32(Console.ReadLine());
            Console.Write("Kapacitet i hallen: ");
            var hallwayCapacity = Convert.ToInt32(Console.ReadLine());
            Console.Clear();


            var b = new Building(-cellarFloors, floors - cellarFloors - 1, hallwayCapacity, "Automatic");
            b.GenerateElevators(elevators, elevatorCapacity);
            b.GeneratePeople(people);
            var m = new Motor(b, 1000);

            m.DrawBuilding();
            m.Update();

            
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


        public static void RunOptions()
        {
            Console.Clear();
            Console.Write("Antal våningar: ");
            var floors = Convert.ToInt32(Console.ReadLine());
            Console.Write("Varav källarvåningar: ");
            var cellarFloors = Convert.ToInt32(Console.ReadLine());
            Console.Write("Antal hissar: ");
            var elevators = Convert.ToInt32(Console.ReadLine());
            Console.Write("Kapacitet i hissen: ");
            var elevatorCapacity = Convert.ToUInt32(Console.ReadLine());
            Console.Write("Antal människor: ");
            var people = Convert.ToInt32(Console.ReadLine());
            Console.Write("Kapacitet i hallen: ");
            var hallwayCapacity = Convert.ToInt32(Console.ReadLine());
            Console.Clear();

            var b = new Building(-cellarFloors, floors-cellarFloors-1, hallwayCapacity, "Manual");
            b.GenerateElevators(elevators, elevatorCapacity);
            b.GeneratePeople(people);
            var m = new Motor(b, 100);

            m.DrawBuilding();
            m.UpdateManual();

        }

        public static void RunLarge()
        {
            Console.Clear();
            var b = new Building(-5, 15, 4, "Manual");
            int numberOfElevators = 3;
            b.GenerateElevators(numberOfElevators, 8);
            b.GeneratePeople(8);
            var m = new Motor(b, 100);
            while (true)
            {
                m.DrawBuilding();
                try
                {
                    m.UpdateManual();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }
        }

        public static void RunSmall()
        {
            Console.Clear();
            var b = new Building(-3, 10, 2, "Manual");
            int numberOfElevators = 1;
            b.GenerateElevators(numberOfElevators, 2);
            b.GeneratePeople();
            var m = new Motor(b, 100);
            while (true)
            {
                m.DrawBuilding();
                try
                {
                    m.UpdateManual();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
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

        public static Move AskUserForNextMove(Elevator e)
        {
            Console.SetCursorPosition(0, e.Count() + 4);
            ClearLines(4);
            Console.WriteLine($"Ange nästa handling för {e.Name}:");
            Console.WriteLine($"(u)pp - (n)er - öppna/stäng (d)örr");
            var input = Console.ReadKey();
            switch (input.Key)
            {
                case ConsoleKey.U:
                case ConsoleKey.UpArrow:
                    return Move.GoUp;
                case ConsoleKey.N:
                case ConsoleKey.DownArrow:
                    return Move.GoDown;
                case ConsoleKey.O:
                case ConsoleKey.D:
                case ConsoleKey.S:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    return Move.CycleDoor;
                default:
                    Console.WriteLine(input.Key);
                    return Move.Wait;
            }
        }

        public static void ClearLines(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                Console.WriteLine("                                                                  ");
            }
            Console.SetCursorPosition(0, Console.CursorTop - lines);
        }

        public static void PrintText(string s)
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.SetCursorPosition(50, 25);
            Console.WriteLine("                                                     ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(50, 25);
            Console.WriteLine(s);
            Console.ResetColor();
            Console.SetCursorPosition(left, top);
        }
    }

   

    
}

