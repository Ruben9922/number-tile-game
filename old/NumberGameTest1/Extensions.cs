using System;
using System.Collections.Generic;
using Ruben9922.Utilities.ConsoleUtilities;

namespace NumberGameTest1
{
    static class Extensions
    {
        // Return a deep clone of a list
        // Should probably change so doesn't use ICloneable as it doesn't indicate whether shallow or deep copy is performed
        public static List<T> Clone<T>(this List<T> list) where T : ICloneable
        {
            // Inspired by various answers and comments on a StackOverflow question: http://stackoverflow.com/questions/222598/how-do-i-clone-a-generic-list-in-c
            List<T> newList = new List<T>(list.Count);
            foreach (T item in list)
            {
                newList.Add((T)item.Clone()); // Should probably change - could use a ICloneable<T> interface for now
            }
            return newList;
        }

        // Display list of tiles
        public static void Display(this List<Tile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                Console.WriteLine("({0}) {1}", i, tiles[i].ToString());
            }
        }

        // Returns index of selected item in given list
        public static int ChooseItem<T>(this List<T> list, string prompt = "Item #: ")
        {
            return ConsoleReadUtilities.ReadInt(prompt, 0, list.Count);
        }
    }
}
