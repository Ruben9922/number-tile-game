using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberGameTest1
{
    class Set : ICloneable
    {
        public const int MinSize = 3;

        public enum SetTypes
        {
            Invalid, Run, Group
        };
        
        public List<Tile> Tiles { get; set; }
        private int setType = (int)SetTypes.Invalid;
        public int SetType
        {
            get
            {
                return setType;
            }
            private set
            {
                if (value >= 0 && value < 3)
                {
                    setType = value;
                }
            }
        }

        public Set(List<Tile> tiles)
        {
            Tiles = tiles;
            UpdateSetType();
        }

        private Set(List<Tile> tiles, int setType)
        {
            Tiles = tiles;
            SetType = setType;
        }

        public override string ToString() // Will change to Display method that uses List<Tile>.Display
        {
            StringBuilder setStringBuilder = new StringBuilder();
            int tileCount = Tiles.Count();
            for (int i = 0; i < tileCount; i++)
            {
                setStringBuilder.Append(string.Format("({0}) {1}", i, Tiles[i].ToString()));
                if (i != tileCount - 1)
                {
                    setStringBuilder.Append(", ");
                }
            }
            return setStringBuilder.ToString();
        }
        
        public Set Clone()
        {
            return new Set(Tiles.Clone(), SetType);
        }

        // Based on StackOverflow answer: http://stackoverflow.com/a/536362/3806231
        // Author: Marc Gravell (http://stackoverflow.com/users/23354/marc-gravell)
        object ICloneable.Clone()
        {
            return Clone();
        }

        // Determines type of set then sets SetType accordingly
        // Haven't put into Tiles set accessor as Tiles object may be modified and detecting that seems kind of complicated
        public void UpdateSetType()
        {
            // Immediately set as invalid if min. size is non-positive and actual size is zero,
            // or min. size positive and actual size is less than minimum size
            if (Tiles.Count() < Math.Max(1, MinSize))
            {
                SetType = (int)SetTypes.Invalid;
            }
            else
            {
                bool isRun = true;
                bool isGroup = true;

                // Pick first non-smiley tile to compare other tiles to - will probably change to use LINQ
                int tileColour;
                int tileNumber;
                int n = 0;
                do
                {
                    tileColour = Tiles[n].Colour;
                    tileNumber = Tiles[n].Number;
                    n++;
                } while (tileNumber == 0 && n < Tiles.Count());

                // Check whether each remaining tile is one greater than preceding tile and same colour as first tile (for a run)
                // or same number as first tile (for a group)
                for (int i = n; i < Tiles.Count(); i++)
                {
                    Tile tile = Tiles[i];
                    if (tile.Number != 0) // If tile is not a smiley face tile
                    {
                        if (tile.Colour != tileColour || tile.Number != tileNumber + 1 + i - n)
                        {
                            isRun = false;
                        }
                        if (tile.Number != tileNumber)
                        {
                            isGroup = false;
                        }
                    }
                }

                // Then set SetType accordingly
                if (isRun)
                {
                    SetType = (int)SetTypes.Run;
                }
                else if (isGroup)
                {
                    SetType = (int)SetTypes.Group;
                }
                else
                {
                    SetType = (int)SetTypes.Invalid;
                }
            }
        }

        // Returns whether set is valid, updating set type in the process
        public bool IsValid()
        {
            return SetType != (int)SetTypes.Invalid;
        }

        // Returns human-readable string representation of SetType
        public string SetTypeToString()
        {
            switch (SetType)
            {
                case (int)SetTypes.Run:
                    return "run";
                case (int)SetTypes.Group:
                    return "group";
                default:
                    return "invalid";
            }
        }
    }
}
