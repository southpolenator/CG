using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

enum MapStates
{
    Unknown = 0,
    Visited = 1,
    Wall = 2,
    Visited2 = 3,
}

enum Movement
{
    Left,
    Right,
    Up,
    Down,
    Stand,
}

struct MovementScore
{
    public int certainDeath;
    public int closestPoints;
}

struct Point
{
    public int X;
    public int Y;

    public int Distance(int x, int y)
    {
        return Math.Abs(x - X) + Math.Abs(y - Y);
    }
}

interface INostradamus
{
    bool IsThere(int x, int y, int step);
}

class PesimisticNostradamus : INostradamus
{
    public PesimisticNostradamus(int[,] distances)
    {
        Distances = distances;
    }

    public int[,] Distances { get; private set; }

    public bool IsThere(int x, int y, int step)
    {
        return Distances[x, y] >= 0 && Distances[x, y] <= step;
    }
}

class PathFollowingNostradamus : INostradamus
{
    private List<Point> points;
    private int repeatStart;
    private int position;

    public PathFollowingNostradamus(List<Point> points, int repeatStart, int position)
    {
        this.points = points;
        this.repeatStart = repeatStart;
        this.position = position;
    }

    public bool IsThere(int x, int y, int step)
    {
        int future = position - 1 + step;
        int future1 = future - 1;

        while (future >= points.Count)
            future -= points.Count - repeatStart;
        if (future < repeatStart)
            future += points.Count - repeatStart;
        while (future1 >= points.Count)
            future1 -= points.Count - repeatStart;
        if (future1 < repeatStart)
            future1 += points.Count - repeatStart;
        return (points[future].X == x && points[future].Y == y) || (points[future1].X == x && points[future1].Y == y);
    }
}

class Bot
{
    private List<Point> allPoints = new List<Point>();
    private List<Point> points = new List<Point>();
    private int position = 0, repeatStart = -1, repeatingSteps = 0;

    public bool IsRepeating
    {
        get
        {
            return repeatingSteps > 5 && points.Count < 10;
            //return repeatingSteps > 10 && points.Count < 30;
            //return false;//repeatingSteps > 5;
        }
    }

    public Point Position { get; private set; }

    public INostradamus Nostradamus { get; private set; }

    public IReadOnlyList<Point> Path
    {
        get
        {
            return allPoints;
        }
    }

    public int RepeatStart
    {
        get
        {
            return repeatStart;
        }
    }

    public int PointPosition
    {
        get
        {
            return position;
        }
    }

    public IReadOnlyList<Point> Points
    {
        get
        {
            return points;
        }
    }

    public void AddPoint(MapStates[,] map, int x, int y)
    {
        int directionx = x - Position.X, directiony = y - Position.Y;
        Position = new Point { X = x, Y = y };

        allPoints.Add(Position);
        if (position == points.Count)
        {
            if (repeatStart >= 0 && points[repeatStart].X == x && points[repeatStart].Y == y)
            {
                position = repeatStart + 1;
                repeatingSteps++;
            }
            else
            {
                bool repeatFound = false;

                for (int i = points.Count - 1; i >= 0; i--)
                    if (points[i].X == x && points[i].Y == y)
                    {
                        position = i + 1;
                        repeatStart = i;
                        repeatingSteps = 1;
                        repeatFound = true;
                        break;
                    }

                if (!repeatFound)
                {
                    points.Add(new Point { X = x, Y = y });
                    position = points.Count;
                    repeatStart = -1;
                    repeatingSteps = 0;
                }
            }
        }
        else if (points[position].X != x || points[position].Y != y)
        {
            for (int i = repeatStart; i < position; i++)
                points.Add(points[i]);
            points.Add(new Point { X = x, Y = y });
            position = points.Count;
            repeatStart = -1;
            repeatingSteps = 0;
            Point cp = points[position - 1];
            for (int i = position - 2; i > 0; i--)
                if (points[i].X == cp.X && points[i].Y == cp.Y)
                {
                    if (i < position - 1 - i)
                        break;

                    // Check if this is repeating sequence
                    bool repeating = true;

                    for (int j = 1; j < position - 1 - i && repeating; j++)
                        if (points[i - j].X != points[position - 1 - j].X || points[i - j].Y != points[position - 1 - j].Y)
                            repeating = false;

                    if (repeating)
                    {
                        repeatStart = i + 1;
                        repeatingSteps = position - i;
                        position = repeatStart;
                        break;
                    }
                }
        }
        else
        {
            position++;
            repeatingSteps++;
        }

        Nostradamus = FindNostradamus(map, directionx, directiony);
    }

    private INostradamus FindNostradamus(MapStates[,] map, int directionx, int directiony)
    {
        if (IsRepeating)
            return new PathFollowingNostradamus(points, repeatStart, position);
        return new PesimisticNostradamus(Game.FindDistancesMap(map, Position.X, Position.Y, directionx, directiony));
    }
}

class Game
{
    public static int[,] FindDistancesMap(MapStates[,] map, int startx, int starty, int directionx = 0, int directiony = 0)
    {
        int[] directionX = new int[] { -1, 0, 1, 0 };
        int[] directionY = new int[] { 0, -1, 0, 1 };
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int[,] distances = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                distances[x, y] = -1;

        Queue<Point> queue = new Queue<Point>();
        Queue<Point> newQueue = new Queue<Point>();

        queue.Enqueue(new Point { X = startx, Y = starty });
        for (int step = 0; queue.Count > 0; step++)
        {
            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();

                if (distances[p.X, p.Y] < 0)
                {
                    distances[p.X, p.Y] = step;
                    for (int i = 0; i < directionX.Length; i++)
                    {
                        int newx = p.X + directionX[i];
                        int newy = p.Y + directionY[i];

                        //if (step == 0 && directionX[i] + directionx == 0 && directionY[i] + directiony == 0)
                        //    continue;

                        if (newx < 0)
                            newx += width;
                        if (newx >= width)
                            newx -= width;
                        if (newy < 0)
                            newy += height;
                        if (newy >= height)
                            newy -= height;
                        if (distances[newx, newy] == -1 && map[newx, newy] != MapStates.Wall)
                        {
                            newQueue.Enqueue(new Point { X = newx, Y = newy });
                            distances[newx, newy] = -2;
                        }
                    }
                }
            }

            var q = queue;
            queue = newQueue;
            newQueue = q;
        }

        return distances;
    }

    static bool IsOnlyUnknownConnected(MapStates[,] map, int startx, int starty)
    {
        int[] directionX = new int[] { 0, -1, 0, 1, 0 };
        int[] directionY = new int[] { 0, 0, -1, 0, 1 };
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int[,] distances = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                distances[x, y] = -1;

        Queue<Point> queue = new Queue<Point>();
        Queue<Point> newQueue = new Queue<Point>();

        if (map[startx, starty] != MapStates.Wall)
            queue.Enqueue(new Point { X = startx, Y = starty });
        for (int step = 1; queue.Count > 0; step++)
        {
            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();

                if (distances[p.X, p.Y] < 0)
                {
                    distances[p.X, p.Y] = step;
                    for (int i = 0; i < directionX.Length; i++)
                    {
                        int newx = p.X + directionX[i];
                        int newy = p.Y + directionY[i];

                        if (newx < 0)
                            newx += width;
                        if (newx >= width)
                            newx -= width;
                        if (newy < 0)
                            newy += height;
                        if (newy >= height)
                            newy -= height;
                        if (distances[newx, newy] == -1 && map[newx, newy] == MapStates.Visited)
                        {
                            newQueue.Enqueue(new Point { X = newx, Y = newy });
                            distances[newx, newy] = -2;
                            continue;
                        }
                        if (map[newx, newy] != MapStates.Visited && map[newx, newy] != MapStates.Wall && map[newx, newy] != MapStates.Unknown)
                            return false;
                    }
                }
            }

            var q = queue;
            queue = newQueue;
            newQueue = q;
        }

        return true;
    }

    public static MovementScore FindMovementScore(MapStates[,] map, int startx, int starty, Bot[] bots)
    {
        int[] directionX = new int[] { 0, -1, 0, 1, 0 };
        int[] directionY = new int[] { 0, 0, -1, 0, 1 };
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int[,] distances = new int[width, height];
        MovementScore score = new MovementScore();

        score.certainDeath = 0;
        score.closestPoints = int.MaxValue;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                distances[x, y] = -1;

        Queue<Point> queue = new Queue<Point>();
        Queue<Point> newQueue = new Queue<Point>();

        if (map[startx, starty] != MapStates.Wall)
            queue.Enqueue(new Point { X = startx, Y = starty });
        for (int step = 1; queue.Count > 0; step++)
        {
            score.certainDeath = step;
            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();

                if (distances[p.X, p.Y] < 0)
                {
                    bool death = false;

                    foreach (Bot bot in bots)
                        if (bot.Nostradamus.IsThere(p.X, p.Y, step))
                        {
                            death = true;
                            break;
                        }
                    if (!death)
                    {
                        if (map[p.X, p.Y] != MapStates.Visited && score.closestPoints > step)
                            score.closestPoints = step;
                        distances[p.X, p.Y] = step;
                        for (int i = 0; i < directionX.Length; i++)
                        {
                            int newx = p.X + directionX[i];
                            int newy = p.Y + directionY[i];

                            if (newx < 0)
                                newx += width;
                            if (newx >= width)
                                newx -= width;
                            if (newy < 0)
                                newy += height;
                            if (newy >= height)
                                newy -= height;
                            if (distances[newx, newy] == -1 && map[newx, newy] != MapStates.Wall && (map[p.X, p.Y] == MapStates.Visited || map[newx, newy] != MapStates.Unknown))
                            {
                                newQueue.Enqueue(new Point { X = newx, Y = newy });
                                distances[newx, newy] = -2;
                            }
                        }
                    }
                }
            }

            var q = queue;
            queue = newQueue;
            newQueue = q;
        }

        return score;
    }

    static MovementScore FindMovementScoreExplore(MapStates[,] map, int startx, int starty, Bot[] bots, int endx, int endy)
    {
        int[] directionX = new int[] { 0, -1, 0, 1, 0 };
        int[] directionY = new int[] { 0, 0, -1, 0, 1 };
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int[,] distances = new int[width, height];
        MovementScore score = new MovementScore();

        score.certainDeath = 0;
        score.closestPoints = int.MaxValue;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                distances[x, y] = -1;

        Queue<Point> queue = new Queue<Point>();
        Queue<Point> newQueue = new Queue<Point>();

        if (map[startx, starty] != MapStates.Wall)
            queue.Enqueue(new Point { X = startx, Y = starty });
        for (int step = 1; queue.Count > 0; step++)
        {
            score.certainDeath = step;
            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();

                if (distances[p.X, p.Y] < 0)
                {
                    bool death = false;

                    foreach (Bot bot in bots)
                        if (bot.Nostradamus.IsThere(p.X, p.Y, step))
                        {
                            death = true;
                            break;
                        }
                    if (!death)
                    {
                        if (map[p.X, p.Y] != MapStates.Visited && score.closestPoints > step)
                            score.closestPoints = step;
                        distances[p.X, p.Y] = step;
                        for (int i = 0; i < directionX.Length; i++)
                        {
                            int newx = p.X + directionX[i];
                            int newy = p.Y + directionY[i];

                            if (newx < 0)
                                newx += width;
                            if (newx >= width)
                                newx -= width;
                            if (newy < 0)
                                newy += height;
                            if (newy >= height)
                                newy -= height;
                            if (distances[newx, newy] == -1 && map[newx, newy] != MapStates.Wall)
                            {
                                newQueue.Enqueue(new Point { X = newx, Y = newy });
                                distances[newx, newy] = -2;
                            }
                        }
                    }
                }
            }

            var q = queue;
            queue = newQueue;
            newQueue = q;
        }

        if (score.certainDeath > 0)
        {
            int[,] d = FindDistancesMap(map, startx, starty);
            score.closestPoints = d[endx, endy];
        }

        return score;
    }

    private const bool explore = false;
    private const bool fullDumpEnabled = false;
    private const int fullDumpFrame = 100;
    private const int explorex = 1;
    private const int explorey = 1;

    private bool dumpEnabled = !fullDumpEnabled;
    private int frame = 1;
    private MapStates[,] map;
    private Bot[] bots;

    public void Initialize(int width, int height, int playersCount)
    {
        DumpLine("Height: {0}", height);
        DumpLine("Width: {0}", width);
        //DumpLine("Players: {0}", playersCount);

        map = new MapStates[width, height];
        bots = new Bot[playersCount - 1];
        for (int i = 0; i < bots.Length; i++)
            bots[i] = new Bot();
    }

    public Movement Step(Point[] playerPositions, bool wallLeft, bool wallUp, bool wallRight, bool wallDown)
    {
        Point me = playerPositions[playerPositions.Length - 1];

        for (int i = 0; i < playerPositions.Length; i++)
            if (i < bots.Length)
            {
                int x = playerPositions[i].X, y = playerPositions[i].Y;

                bots[i].AddPoint(map, x, y);
                if (map[x, y] == MapStates.Unknown)
                    map[x, y] = i + MapStates.Visited2;
            }
        map[me.X, me.Y] = MapStates.Visited;
        frame++;

        if (wallUp)
            map[me.X, me.Y - 1] = MapStates.Wall;
        if (wallDown)
            map[me.X, me.Y + 1] = MapStates.Wall;
        if (wallLeft)
            map[me.X - 1, me.Y] = MapStates.Wall;
        if (wallRight)
            map[me.X + 1, me.Y] = MapStates.Wall;

        // Dump some stats
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int visited = 0;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (map[x, y] == MapStates.Visited)
                    visited++;
        DumpLine("Position: ({0}, {1})", me.X, me.Y);
        DumpLine("Visited: {0}", visited);

        // Decide where to go
        int[] directionX = new int[] { 0, -1, 0, 1, 0 };
        int[] directionY = new int[] { 0, 0, -1, 0, 1 };
        Movement[] direction = new Movement[] { Movement.Stand, Movement.Left, Movement.Up, Movement.Right, Movement.Down };
        Movement movement = Movement.Stand;
        MovementScore bestMoveScore = new MovementScore();
        bool testCloseDeath = !IsOnlyUnknownConnected(map, me.X, me.Y);

        DumpLine("TCD: {0}", testCloseDeath);
        bestMoveScore.certainDeath = 0;
        bestMoveScore.closestPoints = int.MaxValue;
        for (int i = 0; i < directionX.Length; i++)
        {
            int newX = directionX[i] + me.X;
            int newY = directionY[i] + me.Y;

            if (newX < 0)
                newX += width;
            if (newX >= width)
                newX -= width;
            if (newY < 0)
                newY += height;
            if (newY >= height)
                newY -= height;

            MovementScore score;

            if (!explore)
                score = FindMovementScore(map, newX, newY, bots);
            else
            {
                score = FindMovementScoreExplore(map, newX, newY, bots, explorex, explorey);
                if (direction[i] == Movement.Stand)
                    continue;
            }

            Dump("{0}: {1} [{2}, {3}]", direction[i], map[newX, newY], score.certainDeath, score.closestPoints);

            bool s1e = score.certainDeath < 13 && testCloseDeath;
            bool s2e = bestMoveScore.certainDeath < 13 && testCloseDeath;

            if (s1e && !s2e)
            {
                DumpLine();
                continue;
            }

            bool betterScore = !s1e && s2e;

            if (!betterScore && s1e && s2e)
                betterScore = score.certainDeath > bestMoveScore.certainDeath;

            if (betterScore || (score.closestPoints < bestMoveScore.closestPoints
                || (score.closestPoints == bestMoveScore.closestPoints && score.certainDeath > bestMoveScore.certainDeath)))
            {
                movement = direction[i];
                bestMoveScore = score;
                Dump(" *");
            }
            DumpLine();
        }

        foreach (Bot bot in bots)
            DumpLine("{0} {1} {2} {3}", bot.IsRepeating, bot.RepeatStart, bot.PointPosition, bot.Points.Count);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == me.X && y == me.Y)
                    Dump("X ");
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
                        Dump("{0} ", (char)('A' + foundIndex));
                    else if (map[x, y] == MapStates.Wall)
                        Dump("# ");
                    else if (map[x, y] == MapStates.Unknown)
                        Dump("  ");
                    else
                        Dump("{0} ", (int)map[x, y]);
                }
            }
            DumpLine();
        }

        if (fullDumpEnabled && frame == fullDumpFrame)
        {
            Console.Error.WriteLine("width: {0}, height = {1}", width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[x, y] == MapStates.Wall)
                        Console.Error.Write("#");
                    else if (map[x, y] == MapStates.Unknown)
                        Console.Error.Write(" ");
                    else
                        Console.Error.Write(".");
                }
                Console.Error.WriteLine();
            }
            foreach (Bot bot in bots)
            {
                foreach (Point p in bot.Path)
                    Console.Error.Write("{0}{1}", (char)('A' + p.X), (char)('A' + p.Y));
                Console.Error.WriteLine();
            }
        }

        return movement;
    }

    private void DumpLine(string format, params object[] objects)
    {
        if (dumpEnabled)
            Console.Error.WriteLine(format, objects);
    }

    private void DumpLine()
    {
        if (dumpEnabled)
            Console.Error.WriteLine();
    }

    private void Dump(string format, params object[] objects)
    {
        if (dumpEnabled)
            Console.Error.Write(format, objects);
    }
}

#if !LOCAL
class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        int height = int.Parse(Console.ReadLine());
        int width = int.Parse(Console.ReadLine());
        int playersCount = int.Parse(Console.ReadLine());

        game.Initialize(width, height, playersCount);

        while (true)
        {
            string hintUp = Console.ReadLine();
            string hintRight = Console.ReadLine();
            string hintDown = Console.ReadLine();
            string hintLeft = Console.ReadLine();

            bool wallUp = hintUp == "#";
            bool wallDown = hintDown == "#";
            bool wallLeft = hintLeft == "#";
            bool wallRight = hintRight == "#";
            Point[] playerPositions = new Point[playersCount];

            for (int i = 0; i < playersCount; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int x = int.Parse(inputs[0]);
                int y = int.Parse(inputs[1]);

                playerPositions[i] = new Point
                {
                    X = x,
                    Y = y,
                };
            }

            Movement movement = game.Step(playerPositions, wallLeft, wallUp, wallRight, wallDown);

            switch (movement)
            {
                case Movement.Left:
                    Console.WriteLine("E");
                    break;
                case Movement.Up:
                    Console.WriteLine("C");
                    break;
                case Movement.Right:
                    Console.WriteLine("A");
                    break;
                case Movement.Down:
                    Console.WriteLine("D");
                    break;
                default:
                case Movement.Stand:
                    Console.WriteLine("B");
                    break;
            }
        }
    }
}
#endif
