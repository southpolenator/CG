using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseEngineering_Maze
{
    class Program
    {

        static MapStates[,] ParseMap(string mapInput, out Point me, out Bot[] bots)
        {
            string[] lines = mapInput.Split("\n".ToCharArray());
            int height = lines.Length, width = (lines[0].Length + 1) / 2;
            MapStates[,] map = new MapStates[width, height];
            Point[] botPoints = new Point[4];

            bots = new Bot[4];
            for (int i = 0; i < bots.Length; i++)
                bots[i] = new Bot();
            me = new Point();
            for (int y = 0; y < lines.Length; y++)
                for (int i = 0, x = 0; i < lines[y].Length; i += 2, x++)
                {
                    char c = lines[y][i];

                    switch (c)
                    {
                        case 'X':
                            me.X = x;
                            me.Y = y;
                            map[x, y] = MapStates.Visited;
                            break;
                        case '1':
                            map[x, y] = MapStates.Visited;
                            break;
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                            map[x, y] = c - '3' + MapStates.Visited2;
                            break;
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                            // TODO: map[x, y] = c - 'A' + MapStates.Visited2;
                            map[x, y] = MapStates.Visited;
                            botPoints[c - 'A'] = new Point { X = x, Y = y };
                            break;
                        case ' ':
                            map[x, y] = MapStates.Unknown;
                            break;
                        case '#':
                            map[x, y] = MapStates.Wall;
                            break;
                    }
                }

            for (int i = 0; i < bots.Length; i++)
                bots[i].AddPoint(map, botPoints[i].X, botPoints[i].Y);
            return map;
        }

        static void PrintMap(MapStates[,] map, int meX, int meY, Bot[] bots)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == meX && y == meY)
                        Console.Error.Write("X ");
                    else
                    {
                        int foundIndex = -1;

                        for (int i = 0; i < bots.Length; i++)
                            if (bots[i].Position.X == x && bots[i].Position.Y == y)
                            {
                                foundIndex = i;
                                break;
                            }
                        if (foundIndex != -1)
                            Console.Error.Write("{0} ", (char)('A' + foundIndex));
                        else if (map[x, y] == MapStates.Wall)
                            Console.Error.Write("# ");
                        else if (map[x, y] == MapStates.Unknown)
                            Console.Error.Write("  ");
                        else
                            Console.Error.Write("{0} ", (int)map[x, y]);
                    }
                }
                Console.Error.WriteLine();
            }
        }

        static void Test1()
        {
            const string mapInput = @"                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                        B A C D                         
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                          #                             
                        X 1                             
                        # #                             
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        
                                                        ";
            Bot[] bots;
            Point me;
            MapStates[,] map = ParseMap(mapInput, out me, out bots);
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            PrintMap(map, me.X, me.Y, bots);

            for (int i = 0; i < bots.Length; i++)
                if (bots[i].Nostradamus is PesimisticNostradamus)
                {
                    Console.WriteLine((char)(i + 'A'));
                    PesimisticNostradamus pn = bots[i].Nostradamus as PesimisticNostradamus;
                    int[,] distances = pn.Distances;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                            if (distances[x, y] == -1)
                                Console.Write(" # ");
                            else
                                Console.Write("{0,2} ", distances[x, y]);
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }

            MovementScore score = Game.FindMovementScore(map, me.X + 1, me.Y, bots);
        }

        static void ExploreTest(Level level, Level levelExplored)
        {
            int width = level.map.GetLength(0);
            int height = level.map.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    Console.Write("{0}", level.map[x, y] == MapStates.Wall ? '#' : level.map[x, y] == MapStates.Visited ? '.' : ' ');
                Console.WriteLine();
            }
            Console.WriteLine();

            width = levelExplored.map.GetLength(0);
            height = levelExplored.map.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    Console.Write("{0}", levelExplored.map[x, y] == MapStates.Wall ? '#' : levelExplored.map[x, y] == MapStates.Visited ? '.' : ' ');
                Console.WriteLine();
            }
            Console.WriteLine();

            MapStates[,] map = new MapStates[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (level.map[x, y] == MapStates.Wall || levelExplored.map[x, y] == MapStates.Wall)
                        map[x, y] = MapStates.Wall;
                    else if (level.map[x, y] == MapStates.Visited || levelExplored.map[x, y] == MapStates.Visited)
                        map[x, y] = MapStates.Visited;
                    else
                        map[x, y] = MapStates.Unknown;
                }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    Console.Write("{0}", map[x, y] == MapStates.Wall ? '#' : map[x, y] == MapStates.Visited ? '.' : ' ');
                Console.WriteLine("|");
            }
            Console.WriteLine();

            int nextx = -1, nexty = -1;
            int[] directionX = new int[] { -1, 0, 1, 0 };
            int[] directionY = new int[] { 0, -1, 0, 1 };

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    if (map[x, y] == MapStates.Visited)
                    {
                        bool wallFound = false, allVisited = true;

                        for (int i = 0; i < directionX.Length && !wallFound; i++)
                        {
                            int newx = x + directionX[i];
                            int newy = y + directionY[i];

                            if (newx >= 0 && newy >= 0 && newx < width && newy < height)
                            {
                                if (map[newx, newy] == MapStates.Wall)
                                    wallFound = true;
                                else if (map[newx, newy] != MapStates.Visited)
                                    allVisited = false;
                            }
                        }

                        if (!wallFound && !allVisited)
                            map[x, y] = MapStates.Visited2;
                    }

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    if (map[x, y] == MapStates.Visited)
                    {
                        for (int i = 0; i < directionX.Length; i++)
                        {
                            int newx = x + directionX[i];
                            int newy = y + directionY[i];

                            if (newx >= 0 && newy >= 0 && newx < width && newy < height && map[newx, newy] != MapStates.Wall && map[newx, newy] != MapStates.Visited)
                            {
                                nextx = newx;
                                nexty = newy;
                                Console.WriteLine("Next: ({0}, {1})", nextx, nexty);
                            }
                        }
                    }

            PrintMap(map, nextx, nexty, new Bot[0]);
        }

        static void Main(string[] args)
        {
            ExploreTest(M2L1.ToLevel(), M2L1_Explore.ToLevel());
        }


        struct Level
        {
            public MapStates[,] map;
            public Point[,] paths;
            public Point playerStart;
        }

        struct LevelDefinition
        {
            public string map;
            public string paths;
            public int startx;
            public int starty;

            public Level ToLevel()
            {
                string[] mapLines = this.map.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int height = mapLines.Length, width = mapLines[0].Length;
                MapStates[,] map = new MapStates[width, height];

                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        switch (mapLines[y][x])
                        {
                            case '.':
                                map[x, y] = MapStates.Visited;
                                break;
                            default:
                            case ' ':
                                map[x, y] = MapStates.Unknown;
                                break;
                            case '#':
                                map[x, y] = MapStates.Wall;
                                break;
                        }
                string[] pathLines = paths.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int pheight = pathLines.Length, pwidth = pathLines[0].Length;
                Point[,] points = new Point[pwidth, pheight];

                for (int y = 0; y < pheight; y++)
                    for (int i = 0, x = 0; i < pwidth; i += 2, x++)
                    {
                        points[x, y] = new Point
                        {
                            X = (int)(pathLines[y][i] - 'A'),
                            Y = (int)(pathLines[y][i + 1] - 'A'),
                        };
                    }
                return new Level
                {
                    map = map,
                    paths = points,
                    playerStart = new Point
                    {
                        X = startx,
                        Y = starty,
                    }
                };
            }
        }

        static LevelDefinition M2L1 = new LevelDefinition
        {
            map = @"                            
                            
 ############  ############ 
#............##............#
#.####.#####.##.#####.####.#
#.#  #.#   #.##.#   #.#  #.#
#.####.#####.##.#####.####.#
#..........................#
#.####.##.########.##.####.#
#.####.##.###  ###.##.####.#
#......##....##....##......#
 #####.# ###.##.### #.##### 
     #.# ###.##.### #.#     
     #.##..........##.#     
     #.##.########.##.#     
######.##.#      #.##.######
..........#      #..........
######.##.#      #.##.######
     #.##.########.##.#     
     #.##..........##.#     
     #.##.########.##.#     
 #####.##.###  ###.##.##### 
#............##............#
#.####.#####.##.#####.####.#
#.## #.#####.##.#####.# ##.#
#...##................##...#
 ##.##.##.########.##.##.## 
 ##.##.##.###  ###.##.##.## 
#......##....##....##......#
#.#####  ###.##.###  #####.#
#.##########.##.##########.#
#..........................#
 ########################## 
                            
                            ",
            paths = @"MNNNONPNPMPLPKQKRKSKSJSISHTHUHVHVGVFVEVDWDXDYDZD[D[E[F[G[HZHYHXHWHVHVGVFVEVDWDXDYDZD[D[E[F[G[HZHYHXHWHVHVIVJVKVLVMVNVOVPVQUQTQSQSRSSSTRTQTPTOTNTMTLTKTJTJSJRJQJPJOJNKNLNMNNNONPNQNRNSNSOSPSQSRSSSTRTQT
NNMNMMMLMKLKKKJKJJJIJHIHHHGHGGGFGEGDFDEDDDCDBDBEBFBGBHBIBJBKCKDKEKFKGKGJGIGHGGGFGEGDFDEDDDCDBDBEBFBGBHCHDHEHFHGHGIGJGKGLGMGNGOGPGQFQEQDQCQBQAQ\Q[QZQYQXQWQVQUQTQSQSPSOSNRNQNPNONNNMNLNKNJNJOJPJQJRJSJT
ONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONNNMNLNKNJNJOJPJQJRJSJTKTLTMTNTOTPTQTRTSTSUSVSWRWQWPWPXPYPZOZNZMZLZKZJZIZHZGZGYGXGWHWIWJWKWLWMWMXMYMZNZOZPZPYPXPWQWRWSWSVSUSTRTQTPTOTNTMT
PNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPN",
            startx = 13,
            starty = 25,
        };

        static LevelDefinition M2L1_Explore = new LevelDefinition
        {
            map = @"                            
                            
 #####         ############ 
#............ #............#
#.####.     . #.#####.####.#
#.#   .     . #.#    .   #.#
#.#   .#####.##.#    . ###.#
#..........................#
#.#  #.##.######  .  . ###.#
#.####.# .        .  .####.#
#......# ....  .... #......#
 #####      .  .    #.##### 
            .  .### #.#     
         ..........##.#     
         .      ##.##.#     
         .       #.##.#     
         .       #....      
         .       #.##       
         .       #.#        
         ..........#        
                 #.#        
               ###.#        
              #....         
              #.###         
             ##.#           
             ...            
             ###            
                            
                            
                            
                            
                            
                            
                            
                            ",
            paths = @"MNNNONPNPMPLPKQKRKSKSJSISHTHUHVHVGVFVEVDWDXDYDZD[D[E[F[G[H[I[J[KZKYKXKWKVKVJVIVHVGVFVEVDWDXDYDZD[D[E[F[G[H[I[J[KZKYKXKWKVKVLVMVNVOVPVQUQTQSQSRSSSTRTQTPTOTNTMTLTKTJTJUJVJWIWHWGWGXGYGZHZIZJZKZLZMZNZOZPZ
NNMNMMMLMKLKKKJKJJJIJHIHHHGHGGGFGEGDFDEDDDCDBDBEBFBGBHCHDHEHFHGHGGGFGEGDFDEDDDCDBDBEBFBGBHCHDHEHFHGHGGGFGEGDFDEDDDCDBDBEBFBGBHBIBJBKCKDKEKFKGKGLGMGNGOGPGQFQEQDQCQBQAQ\Q[QZQYQXQWQVQUQTQSQSRSSSTSUSVSWRW
ONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONONPNQNRNSNSOSPSQSRSSSTRTQTPTOTNTMTLTKTJTJUJVJWIWHWGWFWEWDWCWBWBXBYBZCZDZD[D\D]E]F]G]G\G[GZHZIZJZKZLZMZNZOZPZPYPX
PNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPNPN",
            startx = 13,
            starty = 25,
        };
    }
}
