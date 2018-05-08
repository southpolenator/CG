using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Code_vs_Zombies
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunTest(Level08);
            RunAllLevel();
        }

        static void RunAllLevel()
        {
            Game.DumpEnabled = false;
            RunTests(AllLevels);
        }

        static void RunTests(string[] tests)
        {
            int total = 0;

            for (int i = 0; i < tests.Length; i++)
            {
                if (!string.IsNullOrEmpty(tests[i]))
                {
                    Console.Write("{0}. ", i + 1);
                    total += RunTest(tests[i]);
                }
            }
            Console.WriteLine("Total score: {0}", total);
        }

        static int RunTest(string test)
        {
            GameState gameState = Parse(test);
            Game game = new Game();
            int turns = 1;

            for (int turn = 1; (gameState.ZombiesLeft > 0 && gameState.HumansLeft > 0); turn++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                GameState gs = gameState.Clone();
                gs.Score = 0;
                Point destination = game.PlayTurn(gs);
                if (sw.ElapsedMilliseconds > 50)
                    Console.WriteLine("Long turn: {0}", sw.ElapsedMilliseconds);

                gameState.Simulate(destination);
                turns++;
            }

            if (gameState.HumansLeft <= 0)
                gameState.Score = 0;

            // Calculate game score
            Console.WriteLine("Score: {0}, Turns: {1}", gameState.Score, turns);
            return gameState.Score;
        }

        static GameState Parse(string s)
        {
            GameState gameState = new GameState();
            string[] lines = s.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            gameState.Me = ParsePoints(lines[0]).First();
            Point[] humans = ParsePoints(lines[1]).ToArray();
            Point[] zombies = ParsePoints(lines[2]).ToArray();

            gameState.Zombies = new Zombie[zombies.Length];
            for (int i = 0; i < zombies.Length; i++)
                gameState.Zombies[i] = new Zombie
                {
#if EXTENDED_INFO
                    Id = i,
#endif
                    Position = zombies[i],
                };
            gameState.ZombiesLeft = zombies.Length;
            gameState.Humans = new Human[humans.Length];
            for (int i = 0; i < humans.Length; i++)
                gameState.Humans[i] = new Human
                {
#if EXTENDED_INFO
                    Id = i,
#endif
                    Position = humans[i],
                };
            gameState.HumansLeft = humans.Length;
            return gameState;
        }

        static IEnumerable<Point> ParsePoints(string s)
        {
            if (s.Length % 4 != 0)
                throw new Exception("Wrong string length: " + s.Length);

            for (int i = 0; i < s.Length; i += 8)
            {
                int x = ParseInt(s.Substring(i, 4));
                int y = ParseInt(s.Substring(i + 4, 4));

                yield return new Point { X = x, Y = y };
            }
        }

        static int ParseInt(string s)
        {
            return int.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }

        // Levels
        // 01. Simple
        static readonly string Level01 = @"   0   0
203A1194
203A2327";

        // 02. 2 zombies
        static readonly string Level02 = @"1388   0
 3B617701F4017D4
 C1C1B582CEC1BBC";

        // 03. 2 zombies redux
        static readonly string Level03 = @"2AF7   0
1F40157C FA0157C
 4E2157C3E7F157C";

        // 04. Scared human
        static readonly string Level04 = @"1F40 7D0
1F401194
 7D0196436B01964";

        // 05. 3 vs 3
        static readonly string Level05 = @"1D4C 7D0
2328 4B0 1901770
 7D0 5DC364C19641B581D4C";

        // 06. Combo opportunity
        static readonly string Level06 = @" 1F41194
  64 FA0  821388   A1194 1F4 DAC   A157C  64 BB8
1F40119423281194271011942AF811942EE0119432C8119436B011943A98 DAC38A4 9C43E1C 1F4";

        // 07. Rows to defend
        static readonly string Level07 = @"   0 FA0
   0 3E8   01F40
1388 3E813881F401B58 3E81B581F402328 3E823281F402AF8 3E82AF81F4032C8 3E832C81F4036B0 3E836B01F4038A4 3E838A41F403A98 3E83A981F40";

        // 08. Rows to defend redux
        static readonly string Level08 = @"   0 FA0
   0 3E8   01F40
 BB8 3E8 BB81F40 FA0 3E8 FA01F401388 3E813881F401B58 3E81B581F402328 3E823281F402AF8 3E82AF81F4032C8 3E832C81F4036B0 3E836B01F4038A4 3E838A41F403A98 3E83A981F40";

        // 09. Rectangle
        static readonly string Level09 = @"1F401194
 FA0 8CA FA01A5E2EE0 8CA2EE01A5E
 FA0 D2F2EE0 D2F FA011942EE01194 FA015F92EE015F91770 8CA1F40 8CA2710 8CA17701A5E1F401A5E27101A5E";

        // 10. Cross
        static readonly string Level10 = @"1F40   0
   011943E7F11941F401F3F
 7D0 4B0 BB8 708 FA0 9601388 BB81770 E1023281518271017702AF819C82EE01C2032C81E7836B020D036B0 25832C8 4B02EE0 7082AF8 9602710 BB82328 E101770151813881770 FA019C8 BB81C20 7D01E78 3E820D0";

        // 11. Unavoidable deaths
        static readonly string Level11 = @"2328 2AC
3E7F11941F401F3F   01194
   0 BD9 5DC186B BB8 9C61194199C1770 F411D4C15602904 8902EE019A834BC1D18";

        // 12. Columns of death
        static readonly string Level12 = @"1F40 FA0
   0 FA03A98 FA0
10ED 70810ED E1010ED151810ED1C2029AA 70829AA E1029AA151829AA1C20   01C20";

        // 13. Rescue mission
        static readonly string Level13 = @"13381A9A
  3212CA39E4 F1E2A75203A25DF1C3427B015E032BC16BC3A2C143C 371 4BA1C5A 85232E51B4E
2B28 2D0 86B 6722592 B042F7D EBA 8CA143C21A9131A1B74 3C0 5EE 1181F3C FF032E5  96 C2F11F8 D0B1036 37E1CAC1D7E1D7E";

        // 14. Triangle
        static readonly string Level14 = @"1F54 DAC
2AF8 3E82AF81770 FA0 DAC
3A98 3E83A981770  78 DAC   0 FA0  78 BB8";

        // 15. Grave danger
        static readonly string Level15 = @" F3C1388
 BB8 BB8 BB81388 BB81B582EE0 DAC
2710 3E8271017703C8C 7D03C8C E103C8C1388   0 4B0";

        // 16. Grid
        static readonly string Level16 = @" F95 CBB
 12E17DD E57 3D51ACF 329
  D0  9C2791 2C733AD 19D  CB E2B1C8E F482656 C9734F4 E54 F53186B1A4019AE289317F83325186D";

        // 17. Hoard
        static readonly string Level17 = @" F95 CBB
 E3F 180  3C CBE 957 641 93B D5E
1955 1F31E8E 1BE23F2 33A2B34  FD3118 3283744 28A19AB 7652124 7DD25C5 7B01D92 D0A2634 E1B20A8129F264C103A2AB7137130161206364310CE 3911604 96A1718 F7017FF1207176B19A817C5200C15CB2359155E  1E1A8E 7061A1A CAF1DF0138D1C97190F1BB61FDF1D17254E1ABF";

        // 18. Flanked!
        static readonly string Level18 = @" F95 CBB
 287 180  3C 4EE 56F 641 553 1A63C6E 1803AD4 4EE2C7F 6412C63 1A6
1EDC 62B2134 9A61D4C ED61964124A232816201D4C18AF21341BB61E7820FF1FA4228F   01B58 3E81EDC BB8213413881D4C1B58196423281B582AF81D4C32C821343A981E78";

        // 19. Split-second reflex
        static readonly string Level19 = @"1F401194
 BB8119436B01194
 9C411943C8C1964";

        // 20. Swervy pattern
        static readonly string Level20 = @"   01194
1B58 DAC   0 1F41B58157C DAC 3E824221F4032C81194
 E10 DAC E741194 D4819642328 DAC231E11942328157C2AF8 FA0238C   A";

        // 21. Devastation
        static readonly string Level21 = @"1F382070
 2F5 DD9 1FE1FEA 45F 2DD 5881CF1 4562128 846 7BF C5F 1E019B0 2982200 4FC341C161F35F0127B3BFB DC83C8713AB3B4E18283CCC1DD8
 F9C1038 F9C12EC F9C1DBC14D0 5681F38 D842C3A15A02C3A2070";

        // All levels
        static readonly string[] AllLevels = new string[]
        {
            Level01, Level02, Level03, Level04, Level05, Level06, Level07, Level08, Level09, Level10,
            Level11, Level12, Level13, Level14, Level15, Level16, Level17, Level18, Level19, Level20,
            Level21,
        };
    }
}
