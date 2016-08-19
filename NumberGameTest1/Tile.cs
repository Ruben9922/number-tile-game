using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberGameTest1
{
    class Tile
    {
        public enum Colours
        {
            Black, Blue, Orange, Red
        };

        private int number;
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                if ((value >= 0) && (value <= 13))
                {
                    number = value;
                }
            }
        }
        private int colour;
        public int Colour
        {
            get
            {
                return colour;
            }
            set
            {
                if ((value >= 0) && (value < 4))
                {
                    colour = value;
                }
            }
        }

        public Tile(int number, int colour)
        {
            this.Number = number;
            this.Colour = colour;
        }

        //public void DisplayTileString()
        //{
        //    Console.BackgroundColor = ConsoleColor.White;
        //    switch (Colour)
        //    {
        //        case (int)Colours.Black:
        //            Console.ForegroundColor = ConsoleColor.Black;
        //            break;
        //        case (int)Colours.Blue:
        //            Console.ForegroundColor = ConsoleColor.Blue;
        //            break;
        //        case (int)Colours.Orange:
        //            Console.ForegroundColor = ConsoleColor.DarkYellow;
        //            break;
        //        case (int)Colours.Red:
        //            Console.ForegroundColor = ConsoleColor.Red;
        //            break;
        //    }
        //    string numberString;
        //    if (Number == 0)
        //    {
        //        numberString = "Smiley :)";
        //    }
        //    else
        //    {
        //        numberString = number.ToString();
        //    }
        //    Console.Write(numberString);
        //    Console.ResetColor();
        //}

        public void DisplayTileString()
        {
            Console.ForegroundColor = ConsoleColor.White;
            string colourString;
            switch (Colour)
            {
                case (int)Colours.Black:
                    colourString = "Black";
                    break;
                case (int)Colours.Blue:
                    colourString = "Blue";
                    break;
                case (int)Colours.Orange:
                    colourString = "Orange";
                    break;
                case (int)Colours.Red:
                    colourString = "Red";
                    break;
                default:
                    colourString = "";
                    break;
            }
            string numberString;
            if (Number == 0)
            {
                numberString = "Smiley :)";
            }
            else
            {
                numberString = number.ToString();
            }
            Console.Write("{0} {1}", colourString, numberString);
            Console.ResetColor();
        }
    }
}
