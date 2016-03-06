/* Handlers for game logic and other game elements.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShipBattle
{
    class Ship
    {
        public Point[] decks;
        private int deckCount;
        private Bearing bearing;
        private int hits = 0;
        private bool destroyed = false;

        public Ship(int DeckCount, Bearing Bearing, Point frontDeck)
        {
            deckCount = DeckCount;
            bearing = Bearing;
            decks = new Point[deckCount];

            int dx = 0, dy = 0;
            switch (Bearing)
            {
                case Bearing.Up:
                    dy = 1;
                    break;
                case Bearing.Down:
                    dy = -1;
                    break;
                case Bearing.Left:
                    dx = 1;
                    break;
                case Bearing.Right:
                    dx = -1;
                    break;
                default:
                    break;
            }

            for (int c = 0; c < deckCount; c++)
            {
                decks[c] = new Point(frontDeck.X + dx * c, frontDeck.Y + dy * c);
            }
        }

        public Boolean isHit(Point hit)
        {
            foreach (Point deck in decks)
            {
                if (deck.Equals(hit))
                {
                    hits++;
                    if (hits >= deckCount) destroyed = true;
                    return true;
                }
            }
            return false;
        }

        public Boolean isDestroyed()
        {
            return destroyed;
        }
    }

    class Field
    {
        private static Random random = new Random();
        private CellState[,] Cells;
        public Point fieldPos;
        public int width = 10, height = 10;
        public int margin = 1;

        public Field(Point fieldPos, int width, int height, int margin)
        {
            this.fieldPos = fieldPos;
            this.width = width;
            this.height = height;
            this.margin = margin;

            Cells = new CellState[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cells[x, y] = CellState.Blank;
                }
            }
        }

        public bool inBounds(Point point)
        {
            if (point.X >= 0 && point.X < width && point.Y >= 0 && point.Y < height) return true;
            return false;
        }

        public CellState getCell(Point point)
        {
            return Cells[point.X, point.Y];
        }

        public void setCell(Point point, CellState newState)
        {
            Cells[point.X, point.Y] = newState;
        }

        public void draw()
        {
            int x = 0, y = 0;
            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    Console.SetCursorPosition(x + margin * (x + 1) + fieldPos.X, y + margin * (y + 1) + fieldPos.Y);
                    Console.Write((char)(int)Cells[x, y]);
                }
            }
            Console.SetCursorPosition(fieldPos.X, y + margin * (y + 1) + fieldPos.Y + 1);
        }


        /* Responsible for populating the field with ships.
         */
        public static Ship[] populate(ShipList list, Field field)
        {
            bool[] freeSpace = new bool[field.width * field.height];
            for (int c = 0; c < field.width * field.height; c++)
            {
                freeSpace[c] = true;
            }

            Ship[] ships = new Ship[list.total];
            int currentType = list.countDecks.Length;

            for (int s = 0; s < list.total; s++)
            {
                //Select next type of ship to place
                while (list.countDecks[currentType - 1] <= 0)
                {
                    int newType = currentType - 1;
                    currentType = newType;
                }

                //start placing
                bool placed = false;
                while (!placed)
                {
                    //pick a free point
                    Point newFront = new Point(random.Next(0, field.width), random.Next(0, field.height));
                    while (!freeSpace[newFront.Y * field.width + newFront.X])
                    {
                        newFront.X = random.Next(0, field.width);
                        newFront.Y = random.Next(0, field.height);
                    }

                    //check all bearings
                    bool foundRoom = false;
                    Bearing firstBearing = (Bearing)random.Next(0, 4);
                    int b = (int)firstBearing;
                    do
                    {
                        if (b > 3) b = 0; //There are four bearings. Duh!
                        ships[s] = new Ship(currentType, (Bearing)b, newFront);

                        bool bounds = true, other = true;
                        foreach (Point deck in ships[s].decks)
                        {
                            bounds = field.inBounds(deck);
                            if (bounds) other = freeSpace[deck.Y * field.width + deck.X];
                        }
                        foundRoom = bounds && other;
                        b++;
                    } while (ships[s].decks.Length != 1 && (b != (int)firstBearing && !foundRoom));

                    //Ship fits, so we place it
                    if (foundRoom)
                    {
                        foreach (Point deck in ships[s].decks)
                        {
                            //field.setCell(deck, CellState.Ship);
                            for (int y = -1; y <= 1; y++) for (int x = -1; x <= 1; x++)
                                {
                                    int space = (deck.Y + y) * field.width + (deck.X + x);
                                    if (space >= 0 && space < field.width * field.height) freeSpace[space] = false;
                                }
                        }
                        placed = true;
                    }

                }
                list.countDecks[(int)currentType - 1]--;

            }
            return ships;
        }

    }
}