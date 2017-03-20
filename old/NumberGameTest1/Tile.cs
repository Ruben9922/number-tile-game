using System;

namespace NumberGameTest1
{
    class Tile : ICloneable
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
            Number = number;
            Colour = colour;
        }

        public Tile Clone()
        {
            return new Tile(Number, Colour);
        }

        // Based on StackOverflow answer: http://stackoverflow.com/a/536362/3806231
        // Author: Marc Gravell (http://stackoverflow.com/users/23354/marc-gravell)
        object ICloneable.Clone()
        {
            return Clone();
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

        public override string ToString()
        {
            string colourString;
            switch (Colour) // Adding trailing spaces to colours so numbers are aligned
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
            return string.Format("{0} {1}", colourString, numberString); // Could use different colours
        }
    }
}
