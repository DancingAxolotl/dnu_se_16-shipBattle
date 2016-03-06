using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShipBattle
{
    class Label
    {
        public string text;
        public Point position;
        public ConsoleColor bgColor, fgColor;
        private bool colored;

        public Label(string Text, Point Position, ConsoleColor Bg, ConsoleColor Fg)
        {
            text = Text;
            position = Position;
            bgColor = Bg;
            fgColor = Fg;
            colored = true;
        }

        public Label(string Text, Point Position)
        {
            text = Text;
            position = Position;
            colored = false;
        }

        public void draw()
        {
            Console.SetCursorPosition(position.X, position.Y);
            if (colored)
            {
                ConsoleColor pushBgColor = Console.BackgroundColor;
                ConsoleColor pushFgColor = Console.ForegroundColor;

                Console.BackgroundColor = bgColor;
                Console.ForegroundColor = fgColor;

                Console.Out.Write(text);

                Console.BackgroundColor = pushBgColor;
                Console.ForegroundColor = pushFgColor;
            }
            else
            {
                Console.Out.Write(text);
            }
            Console.SetCursorPosition(position.X, position.Y);
        }
    }


    
    class Button : Label
    {
        public delegate void ButtonHandler(Button b, KeyPressEvent e);
        public event ButtonHandler handler;
        private ConsoleKeyInfo keyPress
        {
            set
            {
                if (!value.Equals(null))
                {
                    KeyPressEvent myEvent = new KeyPressEvent(value);
                    handler(this, myEvent);
                }
            }
        }

        private bool ButtonSelected = false;
        public bool selected {
            set
            {
                if (ButtonSelected != value)
                {
                    invertColors();
                    draw();
                }
                ButtonSelected = value;
                if (ButtonSelected) keyPress = Console.ReadKey(true);
            }
            get
            {
                return selected;
            }
        }
        public MenuAction Action;


        public Button(string Text, Point Position, MenuAction action, ConsoleColor Bg, ConsoleColor Fg)
            : base(Text, Position, Bg, Fg)
        {
            Action = action;
        }

        public Button(string Text, Point Position, MenuAction action)
            : base(Text, Position, Console.BackgroundColor, Console.ForegroundColor)
        {
            Action = action;
        }

        private void invertColors()
        {
            ConsoleColor tmp = bgColor;
            bgColor = fgColor;
            fgColor = tmp;
        }

    }

    class KeyPressEvent : EventArgs
    {
        public ConsoleKeyInfo ConsoleKey;

        public KeyPressEvent(ConsoleKeyInfo newKey)
        {
            ConsoleKey = newKey;
        }
    }

    class MenuActionEvent : EventArgs
    {
        public MenuAction Action;

        public MenuActionEvent(MenuAction newAction)
        {
            Action = newAction;
        }
    }
        
}
