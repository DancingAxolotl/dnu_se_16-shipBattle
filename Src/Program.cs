/*
        _____  _      _          ____          _    _    _       
       / ____|| |    (_)        |  _ \        | |  | |  | |      
      | (___  | |__   _  _ __   | |_) |  __ _ | |_ | |_ | |  ___ 
       \___ \ | '_ \ | || '_ \  |  _ <  / _` || __|| __|| | / _ \
       ____) || | | || || |_) | | |_) || (_| || |_ | |_ | ||  __/
      |_____/ |_| |_||_|| .__/  |____/  \__,_| \__| \__||_| \___|
                        | |                                      
                        |_|                                      


                                  I
                                 _T
                                 n|_
                               r_7|Ip,
                              -7niHT--'
                             _,rkjH____,
                              __[]H`/'         //
                            ,(__)]H/    /     //
                           ,[]xxv7L[]_rn_rf-fL/{) ,p
                       _,,r`==,kT{[x]L_`||  `|=[` 'T[__
                     `fIzzrzzzr=f-v-__  /|` ,/  | ,|   |_____
                     |(),  | |---'    |P_,\_|    \== __..--""/__..~~``
             _    |^""`=r"`|  \   \_-"|T    V __--``     __,~""
          -f|0|- "7._ _.| / \  | __,.. -- -''``    __..``
   __,..-` '"='     __,. ..--``""          __..--``
-``  ___,..--``` ""              __,..--```
 \`""                  __,..--```
  \           __,,--```            __ ,,..  ~~  ````
   \,,..--'```        __ .. ~~ ``
        ``~~~ ''' ```
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShipBattle
{

    class Program
    {

        static void Main(string[] args)
        {
            //setup console
            Console.Title = "Ship battle";
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetWindowSize(80, 25);
            Console.Clear();
            Console.CursorVisible = false;

            //default game settings
            Settings defaultSettings = new Settings();
            defaultSettings.gameGrid = loadResources("Res\\field_6x6_alt.ascii");
            defaultSettings.moveCount = 25;
            defaultSettings.consoleHeight = 25;
            defaultSettings.inputLocation = new Point(35, 8);
            defaultSettings.scoreBoard = new Point(20, 2);
            defaultSettings.gridLocation = new Point(0, 0);
            defaultSettings.fieldPos = new Point(10, 10);
            defaultSettings.field_Width = 6;
            defaultSettings.field_Height = 6;
            defaultSettings.field_Margin = 1;
            defaultSettings.listParams = new int[2] { 0, 3 };

            IntroState introState = new IntroState();
            MainMenuState menuState = new MainMenuState();
            GameState gameState = new GameState(defaultSettings);

            ProgramState currentState = introState;
            ProgramAction action = ProgramAction.INTRO;
            do
            {
                action = currentState.getAction();

                switch (action)
                {
                    case ProgramAction.PLAY:
                        currentState = gameState;
                        break;
                    case ProgramAction.MAIN:
                        currentState = menuState;
                        break;
                    default:
                        break;
                }
            } while (action != ProgramAction.QUIT);

        }

        public static string loadResources(string fileName)
        {
            string s = "";
            StreamReader reader = new StreamReader(fileName);
            try
            {
                do
                {
                    s += reader.ReadLine() + "\n";
                }
                while (reader.Peek() != -1);
            }

            catch
            {
                s = "Error!";
            }

            finally
            {
                reader.Close();
            }
            return s;
        }

    }

    public enum ProgramAction
    {
        QUIT, INTRO, PLAY, SETTINGS, HELP, MAIN, GAME_PAUSE
    }

    abstract class ProgramState
    {
        public abstract ProgramAction getAction();
    }

    class IntroState : ProgramState
    {
        private string[] frames;
        public IntroState()
        {
            frames = new string[10];
            StreamReader reader = new StreamReader("Res\\intro_Animated.ascii");
            int c = 0, sc = 1;
            string s = "";
            try
            {
                do
                {
                    s += reader.ReadLine() + "\n";
                    sc++;
                    if (sc == 25)
                    {
                        sc = 0;
                        frames[c] = s;
                        s = "";
                        c++;
                    }
                }
                while (reader.Peek() != -1);
            }

            catch
            {
                s = "Error!";
            }

            finally
            {
                reader.Close();
            }
        }

        override public ProgramAction getAction()
        {
            for (int f = 0; f < frames.Length && !Console.KeyAvailable; f++)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.Write(frames[f]);
                Console.SetCursorPosition(0, 0);
                System.Threading.Thread.Sleep(400 - f*20);
            }
            Console.ReadKey();
            Console.Clear();
            return ProgramAction.MAIN;
        }
    }

    class GameState : ProgramState
    {
        private Settings currentSettings;
        public GameState(Settings newSettings)
        {
            setSettings(newSettings);
        }

        public void setSettings(Settings newSettings)
        {
            currentSettings = newSettings;
        }

        override public ProgramAction getAction()
        {
            Game game = new Game(currentSettings);
            game.run(false);
            int stTotal = game.list.total;
            int stCount = game.moveCount;
            
            while (game.moveCount > 0 && game.list.total > 0)
            {
                Console.SetCursorPosition(currentSettings.scoreBoard.X, currentSettings.scoreBoard.Y);
                Console.Write("{0:000}", game.moveCount);
                Console.SetCursorPosition(currentSettings.scoreBoard.X + 4, currentSettings.scoreBoard.Y + 1);
                Console.Write("{0:000}", stCount);
                Console.SetCursorPosition(currentSettings.scoreBoard.X, currentSettings.scoreBoard.Y + 3);
                Console.Write("{0:000}", stTotal - game.list.total);
                Console.SetCursorPosition(currentSettings.scoreBoard.X + 4, currentSettings.scoreBoard.Y + 4);
                Console.Write("{0:000}", stTotal);

                if (!game.step())
                {
                    return ProgramAction.MAIN;
                }
            }

            game.finish();

            return ProgramAction.MAIN;
        }
    }

    class MainMenuState : ProgramState
    {
        private static Menu mainMenu;
        private static string menuBg;
        private ProgramAction action;

        static MainMenuState()
        {
            menuBg = Program.loadResources("Res\\menu.ascii");
        }

        override public ProgramAction getAction()
        {
            if (mainMenu == null)
            {
                Console.Write(menuBg);
                Console.CursorTop = 0;
                Button[] buttons = new Button[4];
                buttons[0] = new Button("     PLAY     ", new Point(9, 8), new MenuAction((int)ProgramAction.PLAY));
                buttons[1] = new Button("   SETTINGS   ", new Point(9, 10), new MenuAction((int)ProgramAction.SETTINGS));
                buttons[2] = new Button("     HELP     ", new Point(9, 12), new MenuAction((int)ProgramAction.HELP));
                buttons[3] = new Button("     QUIT     ", new Point(9, 14), new MenuAction((int)ProgramAction.QUIT));
                mainMenu = new Menu(buttons, menu_handler); //This menu now works.
            }
            else
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.Write(menuBg);
                Console.SetCursorPosition(0, 0);
                mainMenu.reset();
            }
            return action;
        }

        void menu_handler(Menu menu, MenuActionEvent e)
        {
            action = (ProgramAction)e.Action.value;
        }
    }



}
