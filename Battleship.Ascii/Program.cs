
namespace Battleship.Ascii
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Battleship.Ascii.TelemetryClient;
    using Battleship.GameController;
    using Battleship.GameController.Contracts;

    public class Program
    {
        private static List<Ship> myFleet;

        private static List<Ship> enemyFleet;

        private static ITelemetryClient telemetryClient;

        static void Main()
        {
            telemetryClient = new ApplicationInsightsTelemetryClient();
            telemetryClient.TrackEvent("ApplicationStarted", new Dictionary<string, string> { { "Technology", ".NET"} });

            try
            {
                Console.Title = "Battleship";
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                Console.WriteLine("                                     |__");
                Console.WriteLine(@"                                     |\/");
                Console.WriteLine("                                     ---");
                Console.WriteLine("                                     / | [");
                Console.WriteLine("                              !      | |||");
                Console.WriteLine("                            _/|     _/|-++'");
                Console.WriteLine("                        +  +--|    |--|--|_ |-");
                Console.WriteLine(@"                     { /|__|  |/\__|  |--- |||__/");
                Console.WriteLine(@"                    +---------------___[}-_===_.'____                 /\");
                Console.WriteLine(@"                ____`-' ||___-{]_| _[}-  |     |_[___\==--            \/   _");
                Console.WriteLine(@" __..._____--==/___]_|__|_____________________________[___\==--____,------' .7");
                Console.WriteLine(@"|                        Welcome to Battleship                         BB-61/");
                Console.WriteLine(@" \_________________________________________________________________________|");
                Console.WriteLine();

                InitializeGame();

                StartGame();
            }
            catch (Exception e)
            {
                Console.WriteLine("A serious problem occured. The application cannot continue and will be closed.");
                telemetryClient.TrackException(e);
                Console.WriteLine("");
                Console.WriteLine("Error details:");      
                throw new Exception("Fatal error", e);
            }

        }

        private static void StartGame()
        {
            Console.Clear();
            Console.WriteLine("                  __");
            Console.WriteLine(@"                 /  \");
            Console.WriteLine("           .-.  |    |");
            Console.WriteLine(@"   *    _.-'  \  \__/");
            Console.WriteLine(@"    \.-'       \");
            Console.WriteLine("   /          _/");
            Console.WriteLine(@"  |      _  /""");
            Console.WriteLine(@"  |     /_\'");
            Console.WriteLine(@"   \    \_/");
            Console.WriteLine(@"    """"""""");

            do
            {
                Console.WriteLine();
                Console.WriteLine("Player, it's your turn");
                Console.WriteLine("Enter coordinates for your shot :");
                var position = ParsePosition(Console.ReadLine());                
                var isHit = GameController.CheckIsHit(enemyFleet, position);
                telemetryClient.TrackEvent("Player_ShootPosition", new Dictionary<string, string>() { { "Position", position.ToString() }, { "IsHit", isHit.ToString() } });
                if (isHit)
                {
                    Console.Beep();

                    Console.WriteLine(@"                \         .  ./");
                    Console.WriteLine(@"              \      .:"";'.:..""   /");
                    Console.WriteLine(@"                  (M^^.^~~:.'"").");
                    Console.WriteLine(@"            -   (/  .    . . \ \)  -");
                    Console.WriteLine(@"               ((| :. ~ ^  :. .|))");
                    Console.WriteLine(@"            -   (\- |  \ /  |  /)  -");
                    Console.WriteLine(@"                 -\  \     /  /-");
                    Console.WriteLine(@"                   \  \   /  /");
                }

                if (isHit == true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.WriteLine(isHit ? "Yeah ! Nice hit !" : "Hit Missed");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("-------------------------------------------------------");
                position = GetRandomPosition();
                isHit = GameController.CheckIsHit(myFleet, position);
                telemetryClient.TrackEvent("Computer_ShootPosition", new Dictionary<string, string>() { { "Position", position.ToString() }, { "IsHit", isHit.ToString() } });
                Console.WriteLine();
                Console.WriteLine("Computer shot in {0}{1}", position.Column, position.Row);
                if (isHit)
                {
                    Console.Beep();

                    Console.WriteLine(@"                \         .  ./");
                    Console.WriteLine(@"              \      .:"";'.:..""   /");
                    Console.WriteLine(@"                  (M^^.^~~:.'"").");
                    Console.WriteLine(@"            -   (/  .    . . \ \)  -");
                    Console.WriteLine(@"               ((| :. ~ ^  :. .|))");
                    Console.WriteLine(@"            -   (\- |  \ /  |  /)  -");
                    Console.WriteLine(@"                 -\  \     /  /-");
                    Console.WriteLine(@"                   \  \   /  /");

                }
                if (isHit == true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.WriteLine(isHit ? "Computer, player has hit your ship !" : "Hit Missed");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("-------------------------------------------------------");
            }
            while (!gameOver());
        }

        public static Position ParsePosition(string input)
        {
            var letter = (Letters)Enum.Parse(typeof(Letters), input.ToUpper().Substring(0, 1));
            var number = int.Parse(input.Substring(1, 1));
            return new Position(letter, number);
        }

        private static Position GetRandomPosition()
        {
            int rows = 8;
            int lines = 8;
            var random = new Random();
            var letter = (Letters)random.Next(lines);
            var number = random.Next(rows);
            var position = new Position(letter, number);
            return position;
        }

        private static void InitializeGame()
        {
            InitializeDefaultFleet();

            InitializeEnemyFleet();
        }

        private static void InitializeMyFleet()
        {
            myFleet = GameController.InitializeShips().ToList();

            Console.WriteLine("Please position your fleet (Game board size is from A to H and 1 to 8) :");

            foreach (var ship in myFleet)
            {
                Console.WriteLine();
                Console.WriteLine("Please enter the positions for the {0} (size: {1})", ship.Name, ship.Size);
                for (var i = 1; i <= ship.Size; i++)
                {
                    Console.WriteLine("Enter position {0} of {1} (i.e A3):", i, ship.Size);
                    var position = Console.ReadLine();
                    ship.AddPosition(position);
                    telemetryClient.TrackEvent("Player_PlaceShipPosition", new Dictionary<string, string>() { { "Position", position }, { "Ship", ship.Name }, { "PositionInShip", i.ToString() } });
                    Console.WriteLine("-------------------------------------------------------");
                }
            }
        }

        private static void InitializeDefaultFleet()
        {
            myFleet = GameController.InitializeShips().ToList();

            Console.WriteLine("Do you want default postions?" + " [y/n] : ");
            ConsoleKeyInfo cki =    Console.ReadKey();
            Console.WriteLine("");
            
            if (cki.Key.ToString().ToLower() == "y")
            {
                 CreateDefaultFleet();
            }else {
                InitializeMyFleet();
            }
        }

        private static void CreateDefaultFleet()
        {
            myFleet = GameController.InitializeShips().ToList();

            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 4 });
            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 5 });
            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 6 });
            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 7 });
            myFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 8 });

            myFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 6 });
            myFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 7 });
            myFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 8 });
            myFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 9 });

            myFleet[2].Positions.Add(new Position { Column = Letters.A, Row = 3 });
            myFleet[2].Positions.Add(new Position { Column = Letters.B, Row = 3 });
            myFleet[2].Positions.Add(new Position { Column = Letters.C, Row = 3 });

            myFleet[3].Positions.Add(new Position { Column = Letters.F, Row = 8 });
            myFleet[3].Positions.Add(new Position { Column = Letters.G, Row = 8 });
            myFleet[3].Positions.Add(new Position { Column = Letters.H, Row = 8 });

            myFleet[4].Positions.Add(new Position { Column = Letters.C, Row = 5 });
            myFleet[4].Positions.Add(new Position { Column = Letters.C, Row = 6 });  
        }

        private static void InitializeEnemyFleet()
        {
            enemyFleet = GameController.InitializeShips().ToList();

            enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 4 });
            enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 5 });
            enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 6 });
            enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 7 });
            enemyFleet[0].Positions.Add(new Position { Column = Letters.B, Row = 8 });

            enemyFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 6 });
            enemyFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 7 });
            enemyFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 8 });
            enemyFleet[1].Positions.Add(new Position { Column = Letters.E, Row = 9 });

            enemyFleet[2].Positions.Add(new Position { Column = Letters.A, Row = 3 });
            enemyFleet[2].Positions.Add(new Position { Column = Letters.B, Row = 3 });
            enemyFleet[2].Positions.Add(new Position { Column = Letters.C, Row = 3 });

            enemyFleet[3].Positions.Add(new Position { Column = Letters.F, Row = 8 });
            enemyFleet[3].Positions.Add(new Position { Column = Letters.G, Row = 8 });
            enemyFleet[3].Positions.Add(new Position { Column = Letters.H, Row = 8 });

            enemyFleet[4].Positions.Add(new Position { Column = Letters.C, Row = 5 });
            enemyFleet[4].Positions.Add(new Position { Column = Letters.C, Row = 6 });
        }

        private static Boolean gameOver()
        {
            if (GameController.AllEnemyShipsSunk(enemyFleet))
            {
                foreach (var ship in enemyFleet)
                {
                    Console.WriteLine("Enemy ship {0} is destroyed!", ship.Name);
                }
                Console.WriteLine("You are the winner!");
                return true;
            };

            if (GameController.AllMyShipsSunk(myFleet))
            {
                foreach (var ship in myFleet)
                {
                    Console.WriteLine("Your ship {0} is destroyed!", ship.Name);
                }
                Console.WriteLine("You");
                return true;
            }

            return false;
        }
    }
}
