using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

class ArrayStack<T> : IEnumerable<T>
{
    private T[] elements;
    private int used;

    public ArrayStack(int count)
    {
        elements = new T[count];
        used = 0;
    }

    public int Count
    {
        get
        {
            return used;
        }
    }

    public void Push(T element)
    {
        if (used + 1 < elements.Length)
        {
            elements[used] = element;
            used++;
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public T Pop()
    {
        used--;
        return elements[used];
    }

    public void Clear()
    {
        used = 0;
    }

    public void Sort(IComparer<T> comparer)
    {
        Array.Sort(elements, 0, used, comparer);
    }

    public T Bottom()
    {
        return elements[0];
    }

    public T Top()
    {
        return elements[used - 1];
    }

    public IEnumerator<T> GetEnumerator()
    {
        return elements.Take(used).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return elements.Take(used).GetEnumerator();
    }
}

struct Point
{
    public short X;
    public short Y;

    public int Distance2(Point p)
    {
        return (X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y);
    }

    public double Distance(Point p)
    {
        return Math.Sqrt(Distance2(p));
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", X, Y);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Point))
        {
            return false;
        }

        var point = (Point)obj;

        return X == point.X && Y == point.Y;
    }

    public override int GetHashCode()
    {
        var hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        return hashCode;
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return p1.X != p2.X || p1.Y != p2.Y;
    }

    public static bool operator ==(Point p1, Point p2)
    {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public static Point operator -(Point p1, Point p2)
    {
        return new Point()
        {
            X = (short)(p1.X - p2.X),
            Y = (short)(p1.Y - p2.Y),
        };
    }
}

struct Human
{
    public Point Position;
#if EXTENDED_INFO
    public int Id;
#endif

    public override string ToString()
    {
#if EXTENDED_INFO
        return $"{Id}: Position";
#else
        return Position.ToString();
#endif
    }
}

struct Zombie
{
    public const int Speed = 400;
    public const int Speed2 = Speed * Speed;

    public Point Position;
#if EXTENDED_INFO
    public int Id;
#if !LOCAL
    public Point NextPosition;
#endif
#endif

    public override string ToString()
    {
#if EXTENDED_INFO
        return $"{Id}: Position";
#else
        return Position.ToString();
#endif
    }
}

class GameState : IDisposable
{
    public const int MaxHumans = 100;
    public const int MaxZombies = 100;
    public const int MaxArrayStackElements = 1000000;
    private static long[] FibonacciSequence;
    public static ArrayStack<GameState> gameStatesStack = new ArrayStack<GameState>(MaxArrayStackElements);
    public static ArrayStack<Human[]>[] humansDictionaryStack = new ArrayStack<Human[]>[MaxHumans];
    public static ArrayStack<Zombie[]>[] zombiesDictionaryStack = new ArrayStack<Zombie[]>[MaxZombies];
    public Point Me;
    public Human[] Humans;
    public Zombie[] Zombies;
    public long Score;
    public int HumansLeft = -1;
    public int ZombiesLeft = -1;
    public int Turn = 1;

    static GameState()
    {
        FibonacciSequence = new long[105];
        FibonacciSequence[0] = 0;
        FibonacciSequence[1] = 1;
        for (int i = 2; i < FibonacciSequence.Length; i++)
            FibonacciSequence[i] = FibonacciSequence[i - 1] + FibonacciSequence[i - 2];
        for (int i = 0; i < humansDictionaryStack.Length; i++)
            humansDictionaryStack[i] = new ArrayStack<Human[]>(MaxArrayStackElements);
        for (int i = 0; i < zombiesDictionaryStack.Length; i++)
            zombiesDictionaryStack[i] = new ArrayStack<Zombie[]>(MaxArrayStackElements);
    }

    public long MaxScore
    {
        get
        {
            long result = Score;

            for (int i = 1; i <= ZombiesLeft; i++)
                result += HumansLeft * HumansLeft * 10 * FibonacciSequence[i + 1];
            return result;
        }
    }

    public unsafe long ApproximatedScore
    {
        get
        {
            //int* moves = stackalloc int[100];

            //for (int i = 0; i < ZombiesLeft; i++)
            //{
            //    int minDistance = Me.Distance2(Zombies[i].Position);
            //    int index = -1;

            //    for (int j = 0; j < HumansLeft; j++)
            //    {
            //        int distance2 = Zombies[i].Position.Distance2(Humans[j].Position);

            //        if (distance2 < minDistance)
            //        {
            //            minDistance = distance2;
            //            index = j;
            //        }

            //        if (index >= 0)
            //        {
            //            int m = (int)Math.Ceiling(Zombies[i].Position.Distance(Humans[index].Position) / Zombie.Speed);

            //            if (moves[index] == 0 || m < moves[index])
            //                moves[index] = m;
            //        }
            //    }
            //}
            //int hd = 0;
            //for (int i = 0; i < HumansLeft; i++)
            //{
            //    int m = (int)Math.Ceiling((Me.Distance(Humans[i].Position) - Game.MyRange) / Game.MySpeed);

            //    if (moves[i] > m)
            //        hd++;
            //}

            //int hl = HumansLeft - hd;
            //long result = Score;

            //for (int i = 1; i <= ZombiesLeft; i++)
            //    result += hl * hl * 10 * FibonacciSequence[i + 1];
            //return result;
            return MaxScore;
        }
    }

    public long MinScore
    {
        get
        {
            return Score + ZombiesLeft * 10;
        }
    }

    public void Dispose()
    {
        humansDictionaryStack[Humans.Length].Push(Humans);
        zombiesDictionaryStack[Zombies.Length].Push(Zombies);
        gameStatesStack.Push(this);
    }

    public GameState Clone(int zombieIndex = -1)
    {
        int zombiesCount = zombieIndex == -1 ? ZombiesLeft : 1;
        GameState gameState;

        if (gameStatesStack.Count > 0)
            gameState = gameStatesStack.Pop();
        else
            gameState = new GameState();

        ArrayStack<Human[]> humansStack = humansDictionaryStack[HumansLeft];

        if (humansStack.Count > 0)
            gameState.Humans = humansStack.Pop();
        else
            gameState.Humans = new Human[HumansLeft];

        ArrayStack<Zombie[]> zombiesStack = zombiesDictionaryStack[zombiesCount];

        if (zombiesStack.Count > 0)
            gameState.Zombies = zombiesStack.Pop();
        else
            gameState.Zombies = new Zombie[zombiesCount];

        gameState.Me = Me;
        for (int i = 0; i < HumansLeft; i++)
            gameState.Humans[i] = Humans[i];
        if (zombieIndex != -1)
            gameState.Zombies[0] = Zombies[zombieIndex];
        else
            for (int i = 0; i < ZombiesLeft; i++)
                gameState.Zombies[i] = Zombies[i];
        gameState.Score = Score;
        gameState.HumansLeft = HumansLeft;
        gameState.ZombiesLeft = zombiesCount;
        gameState.Turn = Turn;
        return gameState;
    }

    public unsafe void Simulate(Point myDestination)
    {
        Turn++;

        bool* humanCaught = stackalloc bool[HumansLeft];

        // 1. Move zombies
        for (int i = 0; i < ZombiesLeft; i++)
        {
            // Find which human zombie is chasing
            Point zombiePosition = Zombies[i].Position;
            double minDistance2 = zombiePosition.Distance2(Me);
            Point destination = Me;
            int humanIndex = -1;

            for (int j = 0; j < HumansLeft; j++)
            {
                double distance2 = zombiePosition.Distance2(Humans[j].Position);

                if (distance2 < minDistance2)
                {
                    minDistance2 = distance2;
                    humanIndex = j;
                }
            }
            if (humanIndex != -1)
                destination = Humans[humanIndex].Position;

            // Find exact position where zombie will end up after this turn
            if (minDistance2 > Zombie.Speed2)
            {
                double scale2 = minDistance2 / (double)Zombie.Speed2;
                double scale = Math.Sqrt(scale2);
                double x = (destination.X - zombiePosition.X) / scale;
                double y = (destination.Y - zombiePosition.Y) / scale;

                destination.X = (short)(zombiePosition.X + x);
                destination.Y = (short)(zombiePosition.Y + y);
            }
            else if (humanIndex != -1)
            {
                humanCaught[humanIndex] = true;
            }

            // Move zombie
            Zombies[i].Position = destination;
        }

        // 2. Move me
        int myDistance2 = Me.Distance2(myDestination);

        if (myDistance2 > Game.MySpeed2)
        {
            double scale2 = myDistance2 / (double)Game.MySpeed2;
            double scale = Math.Sqrt(scale2);
            double x = (myDestination.X - Me.X) / scale;
            double y = (myDestination.Y - Me.Y) / scale;

            myDestination.X = (short)(Me.X + x);
            myDestination.Y = (short)(Me.Y + y);
        }
        Me = myDestination;

        // 3. Kill zombies
        int kills = 0;

        for (int i = ZombiesLeft - 1; i >= 0; i--)
            if (Zombies[i].Position.Distance2(Me) <= Game.MyRange2)
            {
                kills++;
                ZombiesLeft--;
                if (i != ZombiesLeft)
                    Zombies[i] = Zombies[ZombiesLeft];

                // Update score
                Score += HumansLeft * HumansLeft * 10 * FibonacciSequence[kills + 1];
            }

        // 4. Kill humans
        for (int i = HumansLeft - 1; i >= 0; i--)
            if (humanCaught[i] && Humans[i].Position.Distance2(Me) > Game.MyRange2)
            {
                HumansLeft--;
                if (HumansLeft != i)
                    Humans[i] = Humans[HumansLeft];
            }
    }

    public bool IsHumanAlive(Human human)
    {
        foreach (Human h in Humans)
            if (human.Position == h.Position)
                return true;
        return false;
    }
}

class Game
{
#if LOCAL
    public const int MaxTurnTimeMs = 50;
#else
    public const int MaxTurnTimeMs = 95;
#endif
    public const int MySpeed = 1000;
    public const int MySpeed2 = MySpeed * MySpeed;
    public const int MyRange = 2000;
    public const int MyRange2 = MyRange * MyRange;
    public const int BoardWidth = 16000;
    public const int BoardHeight = 9000;
    public const int MaxTurns = 100;
    public static bool DumpEnabled = true;
    private long previousScore = 0;
    private long previousDestinationScore = -1;
    private Point previousDestination;

    private bool CanHumanBeSaved(Human human, GameState gameState)
    {
        for (int turn = 1; (gameState.Me.X != human.Position.X || gameState.Me.Y != human.Position.Y); turn++)
        {
            gameState.Simulate(human.Position);
        }

        bool result = gameState.IsHumanAlive(human);

        for (int turn = 1; gameState.HumansLeft > 0 && gameState.ZombiesLeft > 0; turn++)
        {
            gameState.Simulate(human.Position);
        }

        return result;
    }

    public Point PlayTurn(GameState gameState)
    {
        gameState.Score = previousScore;
        gameState.Turn = 1;

        if (previousDestinationScore > 0 && gameState.Me == previousDestination)
            previousDestinationScore = -1;

        if (previousDestinationScore > 0)
            DumpLine($"Previous: {previousDestinationScore}");

        Stopwatch sw = Stopwatch.StartNew();

        long bestScore;
        Point bestDestination = SimpleStrategy(gameState, out bestScore);

        long complexScore = Math.Max(bestScore, previousDestinationScore);
        Point complexDestination;

        FindRaySolution(sw, gameState, ref complexScore, out complexDestination);
        //BruteForceStrategy(sw, gameState, out complexScore, out complexDestination);
        DumpLine("Complex: {0}", complexScore);

        long score = bestScore;
        Point destination = bestDestination;

        if (complexScore > score)
        {
            score = complexScore;
            destination = complexDestination;
        }

        if (previousDestinationScore > score)
        {
            score = previousDestinationScore;
            destination = previousDestination;
        }

        previousDestinationScore = score;
        previousDestination = destination;
        gameState.Simulate(destination);
        previousScore = gameState.Score;
        return destination;
    }

    private bool FindInterseptionPoint(GameState gameState, int zombieIndex, out Point interseption, out Point preInterseption)
    {
        using (GameState gs = gameState.Clone(zombieIndex))
        {
            int turn = 0;

            for (; gs.HumansLeft > 0 && gs.ZombiesLeft > 0; turn++)
            {
                if (turn * MySpeed + MyRange >= gs.Zombies[0].Position.Distance(gameState.Me))
                    break;
                gs.Simulate(gs.Me);
            }

            if (gs.HumansLeft > 0 && turn * MySpeed + MyRange >= gs.Zombies[0].Position.Distance(gameState.Me))
            {
                interseption = gs.Zombies[0].Position;

                // Improve interseption point
                using (GameState gs2 = gameState.Clone(zombieIndex))
                {
                    Point lastMePosition = gs2.Me;

                    while (gs2.HumansLeft > 0 && gs2.ZombiesLeft > 0)
                    {
                        lastMePosition = gs2.Me;
                        gs2.Simulate(interseption);
                    }
                    interseption = gs2.Me;
                    Point lastZombiePosition = gs2.Zombies[0].Position;
                    Point vector = lastZombiePosition - lastMePosition;
                    double distance = lastZombiePosition.Distance(lastMePosition);
                    double x = vector.X * (MyRange + 2) / distance;
                    double y = vector.Y * (MyRange + 2) / distance;
                    vector.X = (short)x;
                    vector.Y = (short)y;
                    preInterseption = lastZombiePosition - vector;
                }
                return true;
            }

            interseption = new Point();
            preInterseption = new Point();
            return false;
        }
    }

    struct PointDistanceComparer : IComparer<Point>
    {
        public Point Point;

        public int Compare(Point z1, Point z2)
        {
            return z2.Distance2(Point) - z1.Distance2(Point);
        }
    }

    private bool NoZombieKills(GameState gameState, Point movePoint)
    {
        using (GameState gs = gameState.Clone())
        {
            while (gs.Me != movePoint && gs.ZombiesLeft == gameState.ZombiesLeft)
                gs.Simulate(movePoint);
            return gs.Me == movePoint;
        }
    }

    private bool ClosestToZombies(GameState gameState, Point movePoint)
    {
        using (GameState gs = gameState.Clone())
        {
            while (gs.Me != movePoint)
                gs.Simulate(movePoint);
            for (int i = 0; i < gs.ZombiesLeft; i++)
            {
                int minDistance2 = gs.Me.Distance2(gs.Zombies[i].Position);

                for (int j = 0; j < gs.HumansLeft; j++)
                {
                    int distance2 = gs.Zombies[i].Position.Distance2(gs.Humans[j].Position);

                    if (distance2 < minDistance2)
                        return false;
                }
            }
            return true;
        }
    }

    private unsafe void GeneratePoints(Stopwatch sw, GameState gameState, Point* points, int maxPoints, out int generatedPoints, bool doChecks = true)
    {
        generatedPoints = 0;
        if (gameState.ZombiesLeft > 1)
        {
            if (generatedPoints < maxPoints)
                points[generatedPoints++] = new Point() { X = BoardWidth - 1, Y = BoardHeight - 1 };
            if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                generatedPoints--;
            if (generatedPoints < maxPoints)
                points[generatedPoints++] = new Point() { X = BoardWidth - 1, Y = 0 };
            if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                generatedPoints--;
            if (generatedPoints < maxPoints)
                points[generatedPoints++] = new Point() { X = 0, Y = BoardHeight - 1 };
            if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                generatedPoints--;
            if (generatedPoints < maxPoints)
                points[generatedPoints++] = new Point() { X = 0, Y = 0 };
            if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                generatedPoints--;

            // Middle of the zombies
            Point zombieCenter = new Point();
            int zcX = 0;
            int zcY = 0;
            for (int i = 0; i < gameState.ZombiesLeft; i++)
            {
                zcX += gameState.Zombies[i].Position.X;
                zcY += gameState.Zombies[i].Position.Y;
            }
            zombieCenter.X = (short)(zcX / gameState.ZombiesLeft);
            zombieCenter.Y = (short)(zcY / gameState.ZombiesLeft);
            //if (gameState.Me != zombieCenter)
            //{
            //    points[generatedPoints] = zombieCenter;
            //    generatedPoints++;

            //    // future center
            //    using (GameState gs = gameState.Clone())
            //    {
            //        while (gs.Me != zombieCenter)
            //            gs.Simulate(zombieCenter);

            //        if (gs.ZombiesLeft > 0)
            //        {
            //            Point futureZombieCenter = new Point();
            //            for (int i = 0; i < gs.ZombiesLeft; i++)
            //            {
            //                futureZombieCenter.X += gs.Zombies[i].Position.X;
            //                futureZombieCenter.Y += gs.Zombies[i].Position.Y;
            //            }
            //            futureZombieCenter.X /= gs.ZombiesLeft;
            //            futureZombieCenter.Y /= gs.ZombiesLeft;
            //            if (gameState.Me != futureZombieCenter)
            //            {
            //                points[generatedPoints] = futureZombieCenter;
            //                generatedPoints++;
            //            }
            //        }
            //    }
            //}
            if (generatedPoints < maxPoints)
                points[generatedPoints++] = new Point() { X = 0, Y = zombieCenter.Y };
            if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                generatedPoints--;
            if (generatedPoints < maxPoints)
                points[generatedPoints++] = new Point() { X = BoardWidth - 1, Y = zombieCenter.Y };
            if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                generatedPoints--;
            if (generatedPoints < maxPoints)
                points[generatedPoints++] = new Point() { X = zombieCenter.X, Y = 0 };
            if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                generatedPoints--;
            if (generatedPoints < maxPoints)
                points[generatedPoints++] = new Point() { X = zombieCenter.X, Y = BoardHeight - 1 };
            if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                generatedPoints--;

            // Find closest zombie
            {
                int closest2 = int.MaxValue;
                Point closestPosition = new Point();

                for (int i = 0; i < gameState.ZombiesLeft; i++)
                {
                    int distance2 = gameState.Me.Distance2(gameState.Zombies[i].Position);

                    if (distance2 < closest2)
                    {
                        closest2 = distance2;
                        closestPosition = gameState.Zombies[i].Position;
                    }
                }
                if (generatedPoints < maxPoints)
                    points[generatedPoints++] = new Point() { X = BoardWidth - 1, Y = closestPosition.Y };
                if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                    generatedPoints--;
                if (generatedPoints < maxPoints)
                    points[generatedPoints++] = new Point() { X = 0, Y = closestPosition.Y };
                if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                    generatedPoints--;
                if (generatedPoints < maxPoints)
                    points[generatedPoints++] = new Point() { X = closestPosition.X, Y = BoardHeight - 1 };
                if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                    generatedPoints--;
                if (generatedPoints < maxPoints)
                    points[generatedPoints++] = new Point() { X = closestPosition.X, Y = 0 };
                if (doChecks && (!NoZombieKills(gameState, points[generatedPoints - 1]) || !ClosestToZombies(gameState, points[generatedPoints - 1])))
                    generatedPoints--;
            }
        }

        int sortStart = generatedPoints;
        bool timeToStop = false;

        for (int i = 0; i < gameState.ZombiesLeft && !timeToStop && generatedPoints + 1 < maxPoints; i++, timeToStop = IsTimeToStop(sw))
            if (FindInterseptionPoint(gameState, i, out points[generatedPoints], out points[generatedPoints + 1]))
            {
                //using (GameState gs = gameState.Clone())
                //{
                //    while (gs.Me != points[generatedPoints] && gs.HumansLeft > 0 && gameState.ZombiesLeft == gs.ZombiesLeft)
                //        gs.Simulate(points[generatedPoints]);
                //    if (gs.Me != points[generatedPoints])
                //        continue;
                //}
                if (points[generatedPoints].X >= 0 && points[generatedPoints].X < Game.BoardWidth && points[generatedPoints].Y >= 0 && points[generatedPoints].Y < Game.BoardHeight)
                {
                    generatedPoints++;
                    //if (points[generatedPoints].X >= 0 && points[generatedPoints].X < Game.BoardWidth && points[generatedPoints].Y >= 0 && points[generatedPoints].Y < Game.BoardHeight)
                    //    generatedPoints++;
                }
            }

        //for (int i = 0; i < gameState.HumansLeft && generatedPoints < maxPoints; i++)
        //    if (gameState.Me != gameState.Humans[i].Position)
        //    {
        //        points[generatedPoints] = gameState.Humans[i].Position;
        //        generatedPoints++;
        //    }

        //for (int i = generatedPoints - 1; i >= 0 && generatedPoints < maxPoints; i--)
        //{
        //    Point myDestination = points[i];
        //    int myDistance2 = gameState.Me.Distance2(myDestination);

        //    if (myDistance2 < Game.MySpeed2)
        //        continue;

        //    double scale2 = myDistance2 / (double)Game.MySpeed2;
        //    double scale = Math.Sqrt(scale2);
        //    double x = (myDestination.X - gameState.Me.X) / scale;
        //    double y = (myDestination.Y - gameState.Me.Y) / scale;

        //    myDestination.X = (short)(gameState.Me.X + x);
        //    myDestination.Y = (short)(gameState.Me.Y + y);

        //    points[generatedPoints++] = myDestination;
        //}
        //if (generatedPoints < maxPoints)
        //    points[generatedPoints++] = gameState.Me;
    }

    struct RaySolutionItem
    {
        public long Score;
        public Point FirstDestination;
        public GameState GameState;
        public int Id;
        public int ParentId;
    }

    struct RaySolutionComparer : IComparer<RaySolutionItem>
    {
        public int Compare(RaySolutionItem z1, RaySolutionItem z2)
        {
            return (int)(z2.Score - z1.Score);
        }
    }

    private unsafe void FindRaySolution(Stopwatch sw, GameState gameState, ref long bestScore, out Point bestPoint)
    {
        const int MaxNextMoves = 30000;
        const int TopNextMoves = 75;
        int maxPoints = 240, generatedPoints;
        Point* points = stackalloc Point[maxPoints];
        var moves = new ArrayStack<RaySolutionItem>(MaxNextMoves);
        var nextMoves = new ArrayStack<RaySolutionItem>(MaxNextMoves);
        int aid = 0;

        bestScore = 0;
        bestPoint = new Point();

        // Generate starting moves
        GeneratePoints(sw, gameState, points, maxPoints, out generatedPoints, false);
        for (int i = 0; i < generatedPoints; i++)
        {
            GameState gs = gameState.Clone();

            if (gs.Me != points[i])
                while (gs.Me != points[i] && gs.HumansLeft > 0)
                    gs.Simulate(points[i]);
            else
                gs.Simulate(points[i]);
            moves.Push(new RaySolutionItem()
            {
                Score = gs.ApproximatedScore,
                FirstDestination = points[i],
                GameState = gs,
                Id = aid++,
                ParentId = -1,
            });
        }

        // Leave only top n moves
        var comparer = new RaySolutionComparer();
        int generation = 1;
        moves.Sort(comparer);
        while (moves.Count > TopNextMoves)
        {
            var move = moves.Pop();

            move.GameState.Dispose();
        }

        int previousAid = aid;

        while (moves.Count > 0 && !IsTimeToStop(sw))
        {
            DumpLine($"{generation++} [{moves.Count}]: {moves.Bottom().Score} ({aid - previousAid})");
            previousAid = aid;
            while (moves.Count > 0 && !IsTimeToStop(sw))
            {
                var move = moves.Pop();

                // Generate next moves
                long parentMaxScore = move.GameState.MaxScore;

                GeneratePoints(sw, move.GameState, points, maxPoints, out generatedPoints, false);
                for (int i = 0; i < generatedPoints && parentMaxScore > bestScore; i++)
                {
                    GameState gs = move.GameState.Clone();

                    if (gs.Me != points[i])
                        while (gs.Me != points[i] && gs.HumansLeft > 0)
                            gs.Simulate(points[i]);
                    else
                        gs.Simulate(points[i]);
                    if (gs.HumansLeft == 0 || gs.MaxScore <= bestScore)
                    {
                        gs.Dispose();
                        continue;
                    }
                    if (gs.ZombiesLeft == 0 || gs.Turn > MaxTurns)
                    {
                        if (gs.Score > bestScore)
                        {
                            bestScore = gs.Score;
                            bestPoint = move.FirstDestination;
                        }
                        gs.Dispose();
                        continue;
                    }
                    nextMoves.Push(new RaySolutionItem()
                    {
                        Score = gs.ApproximatedScore,
                        FirstDestination = move.FirstDestination,
                        GameState = gs,
                        Id = aid++,
                        ParentId = move.Id,
                    });
                }
            }

            {
                var temp = moves;
                moves = nextMoves;
                nextMoves = temp;
            }

            // Dispose previous moves
            while (nextMoves.Count > 0)
            {
                var move = nextMoves.Pop();

                move.GameState.Dispose();
            }

            // Leave only top n moves
            moves.Sort(comparer);
            while (moves.Count > TopNextMoves)
            {
                var move = moves.Pop();

                move.GameState.Dispose();
            }

            //File.AppendAllLines("test.txt", moves.Select(s => $"{s.Id},{s.ParentId},{s.Score}"));
        }

        while (moves.Count > 0)
        {
            var move = moves.Pop();

            if (move.GameState.MinScore > bestScore)
            {
                bestScore = move.Score;
                bestPoint = move.FirstDestination;
            }
            move.GameState.Dispose();
        }
        DumpLine($" AID: {aid}; {sw.Elapsed.TotalMilliseconds:0.00}ms");
    }

    private unsafe bool BruteForceStrategy(Stopwatch sw, GameState gameState, out long bestScore, out Point bestPoint)
    {
        bestPoint = new Point();
        bestScore = 0;
        if (IsTimeToStop(sw))
            return false;

        bool timeToStop = false;
        int maxPoints = 240;
        //Point* points = stackalloc Point[maxPoints];
        Point[] points = new Point[maxPoints];
        int generatedPoints = 0;

        //if (false)
        //{
        //    int[,] board = new int[BoardWidth, BoardHeight];
        //    int[] minDistances2 = new int[gameState.ZombiesLeft];

        //    for (int i = 0; i < gameState.ZombiesLeft; i++)
        //    {
        //        minDistances2[i] = int.MaxValue;
        //        for (int j = 0; j < gameState.HumansLeft; j++)
        //        {
        //            int distance2 = gameState.Humans[j].Position.Distance2(gameState.Zombies[i].Position);

        //            if (distance2 < minDistances2[i])
        //                minDistances2[i] = distance2;
        //        }
        //    }

        //    int[] sortedDistances = new int[minDistances2.Length];
        //    for (int i = 0; i < sortedDistances.Length; i++)
        //        sortedDistances[i] = i;
        //    Array.Sort(sortedDistances, (i1, i2) => { return minDistances2[i1] - minDistances2[i2]; });

        //    int maxValue = 0;
        //    int maxxstart = 0;
        //    int maxxend = BoardWidth - 1;
        //    int maxystart = 0;
        //    int maxyend = BoardHeight - 1;
        //    int additionalValue = 0;

        //    for (int l = 0; l < gameState.ZombiesLeft; l++)
        //    {
        //        int i = sortedDistances[l];
        //        int d = (int)Math.Ceiling(Math.Sqrt(minDistances2[i]));
        //        int xstart = Math.Max(0, gameState.Zombies[i].Position.X - d);
        //        int xend = Math.Min(BoardWidth, gameState.Zombies[i].Position.X + d + 1);
        //        int ystart = Math.Max(0, gameState.Zombies[i].Position.Y - d);
        //        int yend = Math.Min(BoardHeight, gameState.Zombies[i].Position.Y + d + 1);

        //        Point topLeft = new Point() { X = maxxstart, Y = maxystart };
        //        Point topRight = new Point() { X = maxxstart, Y = maxyend - 1 };
        //        Point bottomLeft = new Point() { X = maxxend - 1, Y = maxystart };
        //        Point bottomRight = new Point() { X = maxxend - 1, Y = maxyend - 1 };

        //        if (topLeft.Distance2(gameState.Zombies[i].Position) < minDistances2[i]
        //            && topRight.Distance2(gameState.Zombies[i].Position) < minDistances2[i]
        //            && bottomLeft.Distance2(gameState.Zombies[i].Position) < minDistances2[i]
        //            && bottomRight.Distance2(gameState.Zombies[i].Position) < minDistances2[i])
        //        {
        //            additionalValue++;
        //            continue;
        //        }

        //        int newmaxxstart = -1;
        //        int newmaxxend = -1;
        //        int newmaxystart = -1;
        //        int newmaxyend = -1;

        //        if (maxxstart > xstart)
        //            xstart = maxxstart;
        //        if (maxxend < xend)
        //            xend = maxxend;
        //        if (maxystart > ystart)
        //            ystart = maxystart;
        //        if (maxyend < yend)
        //            yend = maxyend;

        //        for (int x = xstart; x < xend; x++)
        //            for (int y = ystart; y < yend; y++)
        //                if (gameState.Zombies[i].Position.Distance2(new Point() { X = x, Y = y }) < minDistances2[i])
        //                {
        //                    board[x, y]++;
        //                    if (board[x, y] > maxValue)
        //                    {
        //                        if (newmaxxstart == -1)
        //                        {
        //                            newmaxxstart = x;
        //                            newmaxxend = x + 1;
        //                            newmaxystart = y;
        //                            newmaxyend = y + 1;
        //                        }
        //                        else
        //                        {
        //                            if (x < newmaxxstart)
        //                                newmaxxstart = x;
        //                            if (x + 1 > newmaxxend)
        //                                newmaxxend = x + 1;
        //                            if (y < newmaxystart)
        //                                newmaxystart = y;
        //                            if (y + 1 > newmaxyend)
        //                                newmaxyend = y + 1;
        //                        }
        //                    }
        //                }
        //        if (newmaxxstart != -1)
        //        {
        //            maxValue++;
        //            maxxstart = newmaxxstart;
        //            maxxend = newmaxxend;
        //            maxystart = newmaxystart;
        //            maxyend = newmaxyend;
        //        }
        //    }
        //    int max = 0;
        //    for (int x = 0; x < BoardWidth; x++)
        //        for (int y = 0; y < BoardHeight; y++)
        //            if (board[x, y] > max)
        //                max = board[x, y];
        //}

        fixed (Point* pp = points)
        {
            GeneratePoints(sw, gameState, pp, maxPoints, out generatedPoints);
        }
        //Array.Sort(points, 0, generatedPoints, new PointDistanceComparer() { Point = gameState.Me });

        for (int i = 0; i < generatedPoints; i++)
        {
            Point interseption = points[i];
            {
                using (GameState gs = gameState.Clone())
                {
                    if (gs.Me != interseption)
                        while (gs.Me != interseption && gs.HumansLeft > 0)
                            gs.Simulate(interseption);
                    else
                        //gs.Simulate(interseption);
                        continue;

                    if (gs.HumansLeft == 0 || gs.Turn > MaxTurns)
                        continue;

                    long score;
                    Point point;

                    if (gs.ZombiesLeft == 0)
                        score = gs.Score;
                    else
                        timeToStop = BruteForceStrategy(sw, gs, out score, out point);
                    if (score > bestScore)
                    {
                        bestPoint = interseption;
                        bestScore = score;
#if LOCAL
                        if (gameState.Turn == 1)
                            Console.Write($"{i}/{generatedPoints}: {score}                    \r");
#endif
                    }

                    if (timeToStop)
                        break;
                }
            }
        }

        return timeToStop;
    }

    private static int TimeQueries = 0;

    private static bool IsTimeToStop(Stopwatch sw)
    {
        //TimeQueries++;
        //if (TimeQueries == 100)
        //{
        //    TimeQueries = 0;
            return sw.ElapsedMilliseconds >= MaxTurnTimeMs;
        //}

        //return false;
    }

    private Point SimpleStrategy(GameState gameState, out long bestScore)
    {
        // Find one human that can survive and guard that human
        Point destination = new Point();

        bestScore = 0;
        for (int i = 0; i < gameState.HumansLeft; i++)
        {
            using (GameState gs = gameState.Clone())
            {
                Human human = gameState.Humans[i];
                bool saved = CanHumanBeSaved(human, gs);

                DumpLine("{0}. ({1}) {2}", human.Position, saved, gs.Score);
                if (saved && gs.Score > bestScore)
                {
                    bestScore = gs.Score;
                    destination = human.Position;
                }
            }
        }

        return destination;
    }

    private static void DumpLine(string format, params object[] objects)
    {
        if (DumpEnabled)
            Console.Error.WriteLine(format, objects);
    }
}

#if !LOCAL
class Program
{
    static void Main(string[] args)
    {
        string[] inputs;
        Game game = new Game();

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
            Point me = new Point { X = x, Y = y };
            int humanCount = int.Parse(Console.ReadLine());
            Human[] humans = new Human[humanCount];
            for (int i = 0; i < humanCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int humanId = int.Parse(inputs[0]);
                int humanX = int.Parse(inputs[1]);
                int humanY = int.Parse(inputs[2]);
                humans[i] = new Human
                {
                    Position = new Point { X = humanX, Y = humanY },
#if EXTENDED_INFO
                    Id = humanId,
#endif
                };
            }
            int zombieCount = int.Parse(Console.ReadLine());
            Zombie[] zombies = new Zombie[zombieCount];
            for (int i = 0; i < zombieCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int zombieId = int.Parse(inputs[0]);
                int zombieX = int.Parse(inputs[1]);
                int zombieY = int.Parse(inputs[2]);
                int zombieXNext = int.Parse(inputs[3]);
                int zombieYNext = int.Parse(inputs[4]);

                zombies[i] = new Zombie
                {
                    Position = new Point { X = zombieX, Y = zombieY },
#if EXTENDED_INFO
                    Id = zombieId,
                    NextPosition = new Point { X = zombieXNext, Y = zombieYNext },
#endif
                };
            }

            GameState gameState = new GameState
            {
                Me = me,
                Humans = humans,
                HumansLeft = humans.Length,
                Zombies = zombies,
                ZombiesLeft = zombies.Length,
            };
            Point whereTo = game.PlayTurn(gameState);
            Console.WriteLine("{0} {1}", whereTo.X, whereTo.Y);
        }
    }
}
#endif
