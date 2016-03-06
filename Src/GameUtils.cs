/*
 * Utility classes and enumerations for ShipBattle game.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShipBattle
{

    struct Settings
    {
        public string gameGrid;
        public int moveCount;
        public Point inputLocation, gridLocation;
        public int consoleHeight;
        public Point fieldPos;
        public int field_Width, field_Height, field_Margin;
        public int[] listParams;
        public Point scoreBoard;
    }

    /* A CellState corresponds to a character on the game field.
     * 32 = " ";
     * 15 = "☼";
     * 46 = ".";
     * 42 = "*";
     * 88 = "X";
     */
    enum CellState : byte
    {
        Blank = 32, Miss = 46, Hit = 42, Kill = 88, Ship = 15, Ship_Hidden = 46
    }

    /* Possible ships bearing.
     * i.e. where does the front of the ship face, so the tail goes the opposite direction.
     */
    enum Bearing : byte
    {
        Up, Down, Left, Right
    }

    /* A list of ships. Duh!
     */
    struct ShipList
    {
        public int[] countDecks;
        public int total;

        public ShipList(int[] decks)
        {
            total = 0;
            countDecks = new int[decks.Length];
            for (int i = 0; i < decks.Length; i++)
            {
                countDecks[i] = decks[i];
                total += decks[i];
            }
        }

    }

    /* Point represents a point on a game field.
     * int X, Y - coordinates
     */
    class Point
    {
        public int X, Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point() : this(0, 0) { }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            if (X == ((Point)obj).X && Y == ((Point)obj).Y) return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p2.X - p1.X, p2.Y - p1.Y);
        }

    }

}