using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShipBattle
{
  
    class Menu
    {
        public delegate void MenuActionHandler(Menu menu, MenuActionEvent e);
        public event MenuActionHandler handler;
        private Button[] buttons;
        private int selButton = 0;

        public Menu(Button[] newButtons, MenuActionHandler menuHandler)
        {
            buttons = newButtons;
            handler += menuHandler;
            foreach (Button b in buttons)
            {
                b.handler += b_handler;
                b.draw();
            }
            buttons[0].selected = true;
        }

        public void reset()
        {
            foreach (Button b in buttons)
            {
                b.draw();
            }
            buttons[0].selected = true;
        }

        private void b_handler(Button b, KeyPressEvent e) // throw a UI Action
        {
            switch (e.ConsoleKey.Key)
            {
                case ConsoleKey.Enter:
                    MenuActionEvent myEvent = new MenuActionEvent(b.Action);
                    handler(this, myEvent);
                    break;
                case ConsoleKey.UpArrow:
                    buttons[selButton--].selected = false;
                    if (selButton < 0) selButton = buttons.Length - 1;//cycle through buttons
                    buttons[selButton].selected = true;
                    break;
                case ConsoleKey.DownArrow:
                    buttons[selButton++].selected = false;
                    if (selButton >= buttons.Length) selButton = 0;//cycle through buttons
                    buttons[selButton].selected = true;
                    break;
                default:
                    b.selected = true;
                    break;
            }
        }
    }

    class MenuAction
    {
        public int value;
        public MenuAction(int nv)
        {
            value = nv;
        }
    }
}
