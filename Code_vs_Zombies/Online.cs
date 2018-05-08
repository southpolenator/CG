using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

class ArrayStack<T>
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
    }

    public T Pop()
    {
        used--;
        return elements[used];
    }
}

struct Point
{
    public int X;
    public int Y;

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
    public const int MaxArrayStackElements = 100;
    private static int[] FibonacciSequence;
    public static ArrayStack<GameState> gameStatesStack = new ArrayStack<GameState>(MaxArrayStackElements);
    public static ArrayStack<Human[]>[] humansDictionaryStack = new ArrayStack<Human[]>[MaxHumans];
    public static ArrayStack<Zombie[]>[] zombiesDictionaryStack = new ArrayStack<Zombie[]>[MaxZombies];
    public Point Me;
    public Human[] Humans;
    public Zombie[] Zombies;
    public int Score;
    public int HumansLeft = -1;
    public int ZombiesLeft = -1;

    static GameState()
    {
        FibonacciSequence = new int[105];
        FibonacciSequence[0] = 0;
        FibonacciSequence[1] = 1;
        for (int i = 2; i < FibonacciSequence.Length; i++)
            FibonacciSequence[i] = FibonacciSequence[i - 1] + FibonacciSequence[i - 2];
        for (int i = 0; i < humansDictionaryStack.Length; i++)
            humansDictionaryStack[i] = new ArrayStack<Human[]>(MaxArrayStackElements);
        for (int i = 0; i < zombiesDictionaryStack.Length; i++)
            zombiesDictionaryStack[i] = new ArrayStack<Zombie[]>(MaxArrayStackElements);
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
        return gameState;
    }

    public unsafe void Simulate(Point myDestination)
    {
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

                destination.X = (int)(zombiePosition.X + x);
                destination.Y = (int)(zombiePosition.Y + y);
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

            myDestination.X = (int)(Me.X + x);
            myDestination.Y = (int)(Me.Y + y);
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
    public static bool DumpEnabled = true;
    private int previousScore = 0;
    private int previousDestinationScore = -1;
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

        if (previousDestinationScore > 0 && gameState.Me == previousDestination)
            previousDestinationScore = -1;

        if (previousDestinationScore > 0)
            DumpLine($"Previous: {previousDestinationScore}");

        Stopwatch sw = Stopwatch.StartNew();

        int bestScore;
        Point bestDestination = SimpleStrategy(gameState, out bestScore);

        int complexScore;
        Point complexDestination;

        BruteForceStrategy(sw, gameState, out complexScore, out complexDestination);
        DumpLine("Complex: {0}", complexScore);

        int score = bestScore;
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

    private bool FindInterseptionPoint(GameState gameState, int zombieIndex, out Point interseption)
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
                    while (gs2.HumansLeft > 0 && gs2.ZombiesLeft > 0)
                        gs2.Simulate(interseption);
                    interseption = gs2.Me;
                }
                return true;
            }

            interseption = new Point();
            return false;
        }
    }

    struct ZombieDistanceComparer : IComparer<Zombie>
    {
        public Point Point;

        public int Compare(Zombie z1, Zombie z2)
        {
            return z2.Position.Distance2(Point) - z1.Position.Distance2(Point);
        }
    }

    private bool BruteForceStrategy(Stopwatch sw, GameState gameState, out int bestScore, out Point bestPoint)
    {
        bool timeToStop = false;

        bestPoint = new Point();
        bestScore = 0;
        Array.Sort(gameState.Zombies, 0, gameState.ZombiesLeft, new ZombieDistanceComparer() { Point = gameState.Me });
        for (int zombieIndex = 0; zombieIndex < gameState.ZombiesLeft && !timeToStop; zombieIndex++, timeToStop = IsTimeToStop(sw))
        {
            using (GameState gs = gameState.Clone())
            {
                Point interseption;

                if (FindInterseptionPoint(gs, zombieIndex, out interseption))
                {
                    while (gs.Me != interseption && gs.HumansLeft > 0)
                        gs.Simulate(interseption);

                    if (gs.HumansLeft == 0)
                        continue;

                    int score;
                    Point point;

                    if (gs.ZombiesLeft == 0)
                        score = gs.Score;
                    else
                        timeToStop = BruteForceStrategy(sw, gs, out score, out point);
                    if (score > bestScore)
                    {
                        bestPoint = interseption;
                        bestScore = score;
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
        TimeQueries++;
        if (TimeQueries == 100)
        {
            TimeQueries = 0;
            return sw.ElapsedMilliseconds >= MaxTurnTimeMs;
        }

        return false;
    }

    private Point SimpleStrategy(GameState gameState, out int bestScore)
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
