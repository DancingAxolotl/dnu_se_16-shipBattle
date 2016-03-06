using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShipBattle
{
    class Game
    {

        private string GAME_GRID;
        private Ship[] ships;
        private int lastCursorPosition;
        public int moveCount;
        public Point gridLocation;
        public Point inputLocation;
        public int consoleHeight;
        public Field field;
        public ShipList list;
        private bool running = false;

        public Game(Settings newGameSettings)
        {
            consoleHeight = newGameSettings.consoleHeight;
            moveCount = newGameSettings.moveCount;
            GAME_GRID = newGameSettings.gameGrid;
            gridLocation = newGameSettings.gridLocation;
            inputLocation = newGameSettings.inputLocation;

            list = new ShipList(newGameSettings.listParams);
            field = new Field(newGameSettings.fieldPos, newGameSettings.field_Width, newGameSettings.field_Height, newGameSettings.field_Margin);
            ships = Field.populate(list, field);

            Console.SetCursorPosition(gridLocation.X, gridLocation.Y);
            Console.Write(GAME_GRID);
            Console.SetCursorPosition(gridLocation.X, gridLocation.Y);
            field.draw();
            lastCursorPosition = inputLocation.Y;
            Console.SetCursorPosition(inputLocation.X, inputLocation.Y);
        }

        public void run(bool doRun)
        {
            //Console.Clear();
            running = true;

            while (moveCount > 0 && list.total > 0 && doRun)
            {
                step();
            }

            if (doRun) finish();

        }

        public void finish()
        {
            if (running)
            {
                Console.Clear();
                Console.SetCursorPosition(gridLocation.X, gridLocation.Y);
                Console.Write(GAME_GRID);
                Console.SetCursorPosition(gridLocation.X, gridLocation.Y);
                field.draw();
                Console.SetCursorPosition(inputLocation.X, inputLocation.Y);

                if (list.total == 0)
                {
                    Console.WriteLine("Congratulations! You WIN!!!");
                    Console.CursorLeft = inputLocation.X;
                    Console.Write("    ***Press any key***    ");
                }
                else
                {
                    Console.WriteLine("    You lose :c");
                    Console.CursorLeft = inputLocation.X;
                    Console.Write("***Press any key***    ");
                }

                Console.ReadKey();
            }
        }

        public bool step()
        {
            if (running)
            {
                Console.CursorTop = lastCursorPosition;
                Console.CursorLeft = inputLocation.X;
                Console.Write("Choose your target: ");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    return false;
                }
                string target = Console.ReadLine();

                if (lastCursorPosition >= consoleHeight - inputLocation.Y - 2)
                {
                    Console.Clear();
                    Console.SetCursorPosition(gridLocation.X, gridLocation.Y);
                    Console.Write(GAME_GRID);
                    Console.SetCursorPosition(gridLocation.X, gridLocation.Y);
                    field.draw();
                    lastCursorPosition = inputLocation.Y;
                    Console.SetCursorPosition(inputLocation.X, inputLocation.Y);
                    Console.WriteLine("Choose your target: " + target);
                }

                int target_X, target_Y;
                try
                {
                    target_X = (target[0] >= 'A' && target[0] <= 'z') ? (target[0] >= 'a' ? (int)target[0] - 'a' : (int)target[0] - 'A') : -1;
                    if (target.Contains(' '))
                    {
                        target_Y = Convert.ToInt32(target.Remove(0, 2));
                    }
                    else
                    {
                        target_Y = Convert.ToInt32(target.Remove(0, 1));
                    }

                    Point targetPoint = new Point(target_X, target_Y - 1);
                    if (field.inBounds(targetPoint))
                    {
                        if (field.getCell(targetPoint) == CellState.Blank)
                        {
                            field.setCell(targetPoint, CellState.Miss);
                            Console.CursorLeft = inputLocation.X;
                            Console.Write("You miss!");
                            foreach (Ship ship in ships)
                            {
                                if (ship.isHit(targetPoint))
                                {
                                    field.setCell(targetPoint, CellState.Hit);

                                    Console.CursorLeft = inputLocation.X;
                                    Console.Write("You hit a ship!");

                                    if (ship.isDestroyed())
                                    {
                                        foreach (Point deck in ship.decks) for (int y = -1; y <= 1; y++) for (int x = -1; x <= 1; x++)
                                                {
                                                    Point adjasent = new Point((deck.X + x), (deck.Y + y));
                                                    if (field.inBounds(adjasent)) field.setCell(adjasent, CellState.Miss);
                                                }
                                        foreach (Point deck in ship.decks)
                                        {
                                            field.setCell(deck, CellState.Kill);
                                        }
                                        Console.CursorLeft = inputLocation.X;
                                        Console.Write("A ship has been destroyed!");
                                        list.total--;
                                    }
                                    break;
                                }
                            }
                            Console.WriteLine();
                            moveCount--;
                        }
                        else
                        {
                            Console.CursorLeft = inputLocation.X;
                            Console.WriteLine("You have already chosen this target.");
                        }
                    }
                    else
                    {
                        Console.CursorLeft = inputLocation.X;
                        Console.WriteLine("Invalid input!");
                        Console.CursorLeft = inputLocation.X;
                        Console.WriteLine("Please choose a target on the field.");
                    }
                }
                catch
                {
                    Console.CursorLeft = inputLocation.X;
                    Console.WriteLine("Invalid input!");
                    Console.CursorLeft = inputLocation.X;
                    Console.WriteLine("Please choose a valid target, like 'A1'.");
                }
                lastCursorPosition = Console.CursorTop;
                field.draw();
            }
            return true;
        }
    }



}
