using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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
}

struct Human
{
    public Point Position;
    public int Id;
    public bool Dead;
}

struct Zombie
{
    public const int Speed = 400;
    public const int Speed2 = Speed * Speed;

    public Point Position;
    public int Id;
    public Point NextPosition;
    public bool Dead;
    public int HumanIndex; // Where it is running to
}

class GameState : IDisposable
{
    private static int[] FibonacciSequence;
    private static Stack<GameState> gameStatesStack = new Stack<GameState>();
    private static Stack<Human[]>[] humansDictionaryStack = new Stack<Human[]>[100];
    private static Stack<Zombie[]>[] zombiesDictionaryStack = new Stack<Zombie[]>[100];
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
            humansDictionaryStack[i] = new Stack<Human[]>();
        for (int i = 0; i < zombiesDictionaryStack.Length; i++)
            zombiesDictionaryStack[i] = new Stack<Zombie[]>();
    }

    public void Dispose()
    {
        humansDictionaryStack[Humans.Length].Push(Humans);
        zombiesDictionaryStack[Zombies.Length].Push(Zombies);
        gameStatesStack.Push(this);
    }

    public GameState Clone()
    {
        GameState gameState;

        if (gameStatesStack.Count > 0)
            gameState = gameStatesStack.Pop();
        else
            gameState = new GameState();

        Stack<Human[]> humansStack = humansDictionaryStack[Humans.Length];

        if (humansStack.Count > 0)
            gameState.Humans = humansStack.Pop();
        else
            gameState.Humans = new Human[Humans.Length];

        Stack<Zombie[]> zombiesStack = zombiesDictionaryStack[Zombies.Length];

        if (zombiesStack.Count > 0)
            gameState.Zombies = zombiesStack.Pop();
        else
            gameState.Zombies = new Zombie[Zombies.Length];

        gameState.Me = Me;
        Array.Copy(Humans, gameState.Humans, Humans.Length);
        //for (int i = 0; i < Humans.Length; i++)
        //    gameState.Humans[i] = Humans[i];
        Array.Copy(Zombies, gameState.Zombies, Zombies.Length);
        //for (int i = 0; i < Zombies.Length; i++)
        //    gameState.Zombies[i] = Zombies[i];
        gameState.Score = Score;
        gameState.HumansLeft = HumansLeft;
        gameState.ZombiesLeft = ZombiesLeft;
        return gameState;
    }

    public void Init()
    {
        HumansLeft = 0;
        foreach (Human human in Humans)
            if (!human.Dead)
                HumansLeft++;
        ZombiesLeft = 0;
        foreach (Zombie zombie in Zombies)
            if (!zombie.Dead)
                ZombiesLeft++;
    }

    public void Simulate(Point myDestination)
    {
        if (ZombiesLeft < 0 || HumansLeft < 0)
            Init();

        // 1. Move zombies
        for (int i = 0; i < Zombies.Length; i++)
        {
            if (Zombies[i].Dead)
                continue;

            // Find who is zombie chasing
            Point zombiePosition = Zombies[i].Position;
            double minDistance2 = zombiePosition.Distance2(Me);
            Point destination = Me;

            Zombies[i].HumanIndex = -1;
            for (int j = 0; j < Humans.Length; j++)
                if (!Humans[j].Dead)
                {
                    double distance2 = zombiePosition.Distance2(Humans[j].Position);

                    if (distance2 < minDistance2)
                    {
                        minDistance2 = distance2;
                        destination = Humans[j].Position;
                        Zombies[i].HumanIndex = j;
                    }
                }

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

        for (int i = 0; i < Zombies.Length; i++)
            if (!Zombies[i].Dead && Zombies[i].Position.Distance2(Me) <= Game.MyRange2)
            {
                kills++;
                Zombies[i].Dead = true;
                ZombiesLeft--;

                // Update score
                Score += HumansLeft * HumansLeft * 10 * FibonacciSequence[kills + 1];
            }

        // 4. Kill humans
        for (int i = 0; i < Zombies.Length; i++)
            if (!Zombies[i].Dead)
            {
                int j = Zombies[i].HumanIndex;

                if (j >= 0 && !Humans[j].Dead && Humans[j].Position.X == Zombies[i].Position.X && Humans[j].Position.Y == Zombies[i].Position.Y)
                {
                    Humans[j].Dead = true;
                    HumansLeft--;
                }
            }
        //for (int i = 0; i < Humans.Length; i++)
        //    if (!Humans[i].Dead)
        //        foreach (Zombie zombie in Zombies)
        //            if (!zombie.Dead && zombie.Position.X == Humans[i].Position.X && zombie.Position.Y == Humans[i].Position.Y)
        //            {
        //                Humans[i].Dead = true;
        //                HumansLeft--;
        //                break;
        //            }
    }

    public bool IsHumanAlive(int humanId)
    {
        foreach (Human human in Humans)
            if (human.Id == humanId)
                return !human.Dead;
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

    private bool CanHumanBeSaved(Human human, GameState gameState)
    {
        for (int turn = 1; (gameState.Me.X != human.Position.X || gameState.Me.Y != human.Position.Y); turn++)
        {
            gameState.Simulate(human.Position);
        }

        bool result = gameState.IsHumanAlive(human.Id);

        for (int turn = 1; gameState.HumansLeft > 0 && gameState.ZombiesLeft > 0; turn++)
        {
            gameState.Simulate(human.Position);
        }

        return result;
    }

    public Point PlayTurn(GameState gameState)
    {
        Stopwatch sw = Stopwatch.StartNew();

        gameState.Init();

        int bestScore;
        Point bestDestination = SimpleStrategy(gameState, out bestScore);

        int complexScore;
        Point complexDestination;

        ComplexStrategy(sw, gameState, out complexScore, out complexDestination);
        DumpLine("Complex: {0}", complexScore);
        if (complexScore >= bestScore)
            return complexDestination;
        return bestDestination;
    }

    private bool FindInterseptionPoint(GameState gameState, int zombieIndex, out Point interseption)
    {
        using (GameState gs = gameState.Clone())
        {
            int turn = 0;

            for (; gs.HumansLeft > 0 && gs.ZombiesLeft > 0; turn++)
            {
                if (turn * MySpeed + MyRange >= gs.Zombies[zombieIndex].Position.Distance(gameState.Me))
                {
                    interseption = gs.Zombies[zombieIndex].Position;
                    return true;
                }

                gs.Simulate(gs.Me);
            }

            if (gs.HumansLeft > 0 && turn * MySpeed + MyRange >= gs.Zombies[zombieIndex].Position.Distance(gameState.Me))
            {
                interseption = gs.Zombies[zombieIndex].Position;
                return true;
            }

            interseption = new Point();
            return false;
        }
    }

    private bool ComplexStrategy(Stopwatch sw, GameState gameState, out int bestScore, out Point bestPoint)
    {
        bool timeToStop = false;

        bestPoint = new Point();
        bestScore = 0;
        for (int zombieIndex = 0; zombieIndex < gameState.Zombies.Length && !timeToStop; zombieIndex++, timeToStop = IsTimeToStop(sw))
            if (!gameState.Zombies[zombieIndex].Dead)
            {
                using (GameState gs = gameState.Clone())
                {
                    Point interseption;

                    if (FindInterseptionPoint(gs, zombieIndex, out interseption))
                    {
                        while (!gs.Zombies[zombieIndex].Dead)
                            gs.Simulate(interseption);

                        int score;
                        Point point;

                        if (gs.ZombiesLeft == 0)
                            score = gs.Score;
                        else
                            timeToStop = ComplexStrategy(sw, gs, out score, out point);
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
        foreach (Human human in gameState.Humans)
            if (!human.Dead)
            {
                using (GameState gs = gameState.Clone())
                {
                    bool saved = CanHumanBeSaved(human, gs);

                    DumpLine("{0}. ({1}) {2}", human.Id, saved, gs.Score);
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
                    Id = humanId,
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
                    Id = zombieId,
                    NextPosition = new Point { X = zombieXNext, Y = zombieYNext },
                };
            }

            GameState gameState = new GameState
            {
                Me = me,
                Humans = humans,
                Zombies = zombies,
            };
            Point whereTo = game.PlayTurn(gameState);
            Console.WriteLine("{0} {1}", whereTo.X, whereTo.Y);
        }
    }
}
#endif
