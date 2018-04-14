using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

struct TargetWeight
{
    public int Min;
    public int Max;

    public int Distance(int value)
    {
        if (value < Min)
            return Min - value;
        return value - Max;
    }
}

class Box
{
    public const int FixedPoint = 100000;
    public int Weight;
    public int Volume;
    public Truck Truck;
}

class ComplexMove
{
    public int Weight;
    public int Volume;
    public Box[] Boxes;
}

class Truck
{
    public const int MaxVolume = 100 * Box.FixedPoint;
    public int Id;
    public int Weight;
    public int Volume;
    public List<Box> Boxes = new List<Box>();
    public ComplexMove[] Moves;

    public void AddBox(Box b)
    {
        Weight += b.Weight;
        Volume += b.Volume;
        Boxes.Add(b);
        b.Truck = this;
    }

    public void RemoveBox(Box b)
    {
        Weight -= b.Weight;
        Volume -= b.Volume;
        Boxes.Remove(b);
        b.Truck = null;
    }

    public void UpdateMoves(int maxMoves)
    {
        int overbookedMoves = maxMoves * 3;
        List<ComplexMove> moves = new List<ComplexMove>();

        foreach (Box b1 in Boxes)
            moves.Add(new ComplexMove()
            {
                Weight = b1.Weight,
                Volume = b1.Volume,
                Boxes = new Box[] { b1 },
            });
        for (int a1 = 0; a1 < Boxes.Count && moves.Count < overbookedMoves; a1++)
        {
            Box b1 = Boxes[a1];
            for (int a2 = a1 + 1; a2 < Boxes.Count && moves.Count < overbookedMoves; a2++)
            {
                Box b2 = Boxes[a2];
                moves.Add(new ComplexMove()
                {
                    Weight = b1.Weight + b2.Weight,
                    Volume = b1.Volume + b2.Volume,
                    Boxes = new Box[] { b1, b2 },
                });
            }
        }
        for (int a1 = 0; a1 < Boxes.Count && moves.Count < overbookedMoves; a1++)
        {
            Box b1 = Boxes[a1];
            for (int a2 = a1 + 1; a2 < Boxes.Count && moves.Count < overbookedMoves; a2++)
            {
                Box b2 = Boxes[a2];
                for (int a3 = a2 + 1; a3 < Boxes.Count && moves.Count < overbookedMoves; a3++)
                {
                    Box b3 = Boxes[a3];
                    moves.Add(new ComplexMove()
                    {
                        Weight = b1.Weight + b2.Weight + b3.Weight,
                        Volume = b1.Volume + b2.Volume + b3.Volume,
                        Boxes = new Box[] { b1, b2, b3 },
                    });
                }
            }
        }
        for (int a1 = 0; a1 < Boxes.Count && moves.Count < overbookedMoves; a1++)
        {
            Box b1 = Boxes[a1];
            for (int a2 = a1 + 1; a2 < Boxes.Count && moves.Count < overbookedMoves; a2++)
            {
                Box b2 = Boxes[a2];
                for (int a3 = a2 + 1; a3 < Boxes.Count && moves.Count < overbookedMoves; a3++)
                {
                    Box b3 = Boxes[a3];
                    for (int a4 = a3 + 1; a4 < Boxes.Count && moves.Count < overbookedMoves; a4++)
                    {
                        Box b4 = Boxes[a4];
                        moves.Add(new ComplexMove()
                        {
                            Weight = b1.Weight + b2.Weight + b3.Weight + b4.Weight,
                            Volume = b1.Volume + b2.Volume + b3.Volume + b4.Volume,
                            Boxes = new Box[] { b1, b2, b3, b4 },
                        });
                    }
                }
            }
        }
        for (int a1 = 0; a1 < Boxes.Count && moves.Count < overbookedMoves; a1++)
        {
            Box b1 = Boxes[a1];
            for (int a2 = a1 + 1; a2 < Boxes.Count && moves.Count < overbookedMoves; a2++)
            {
                Box b2 = Boxes[a2];
                for (int a3 = a2 + 1; a3 < Boxes.Count && moves.Count < overbookedMoves; a3++)
                {
                    Box b3 = Boxes[a3];
                    for (int a4 = a3 + 1; a4 < Boxes.Count && moves.Count < overbookedMoves; a4++)
                    {
                        Box b4 = Boxes[a4];
                        for (int a5 = a4 + 1; a5 < Boxes.Count && moves.Count < overbookedMoves; a5++)
                        {
                            Box b5 = Boxes[a5];
                            moves.Add(new ComplexMove()
                            {
                                Weight = b1.Weight + b2.Weight + b3.Weight + b4.Weight + b5.Weight,
                                Volume = b1.Volume + b2.Volume + b3.Volume + b4.Volume + b5.Volume,
                                Boxes = new Box[] { b1, b2, b3, b4, b5 },
                            });
                        }
                    }
                }
            }
        }
        for (int a1 = 0; a1 < Boxes.Count && moves.Count < overbookedMoves; a1++)
        {
            Box b1 = Boxes[a1];
            for (int a2 = a1 + 1; a2 < Boxes.Count && moves.Count < overbookedMoves; a2++)
            {
                Box b2 = Boxes[a2];
                for (int a3 = a2 + 1; a3 < Boxes.Count && moves.Count < overbookedMoves; a3++)
                {
                    Box b3 = Boxes[a3];
                    for (int a4 = a3 + 1; a4 < Boxes.Count && moves.Count < overbookedMoves; a4++)
                    {
                        Box b4 = Boxes[a4];
                        for (int a5 = a4 + 1; a5 < Boxes.Count && moves.Count < overbookedMoves; a5++)
                        {
                            Box b5 = Boxes[a5];
                            for (int a6 = a5 + 1; a6 < Boxes.Count && moves.Count < overbookedMoves; a6++)
                            {
                                Box b6 = Boxes[a6];
                                moves.Add(new ComplexMove()
                                {
                                    Weight = b1.Weight + b2.Weight + b3.Weight + b4.Weight + b5.Weight + b6.Weight,
                                    Volume = b1.Volume + b2.Volume + b3.Volume + b4.Volume + b5.Volume + b6.Volume,
                                    Boxes = new Box[] { b1, b2, b3, b4, b5, b6 },
                                });
                            }
                        }
                    }
                }
            }
        }
        for (int a1 = 0; a1 < Boxes.Count && moves.Count < overbookedMoves; a1++)
        {
            Box b1 = Boxes[a1];
            for (int a2 = a1 + 1; a2 < Boxes.Count && moves.Count < overbookedMoves; a2++)
            {
                Box b2 = Boxes[a2];
                for (int a3 = a2 + 1; a3 < Boxes.Count && moves.Count < overbookedMoves; a3++)
                {
                    Box b3 = Boxes[a3];
                    for (int a4 = a3 + 1; a4 < Boxes.Count && moves.Count < overbookedMoves; a4++)
                    {
                        Box b4 = Boxes[a4];
                        for (int a5 = a4 + 1; a5 < Boxes.Count && moves.Count < overbookedMoves; a5++)
                        {
                            Box b5 = Boxes[a5];
                            for (int a6 = a5 + 1; a6 < Boxes.Count && moves.Count < overbookedMoves; a6++)
                            {
                                Box b6 = Boxes[a6];
                                for (int a7 = a6 + 1; a7 < Boxes.Count && moves.Count < overbookedMoves; a7++)
                                {
                                    Box b7 = Boxes[a7];
                                    moves.Add(new ComplexMove()
                                    {
                                        Weight = b1.Weight + b2.Weight + b3.Weight + b4.Weight + b5.Weight + b6.Weight + b7.Weight,
                                        Volume = b1.Volume + b2.Volume + b3.Volume + b4.Volume + b5.Volume + b6.Volume + b7.Volume,
                                        Boxes = new Box[] { b1, b2, b3, b4, b5, b6, b7 },
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
        for (int a1 = 0; a1 < Boxes.Count && moves.Count < overbookedMoves; a1++)
        {
            Box b1 = Boxes[a1];
            for (int a2 = a1 + 1; a2 < Boxes.Count && moves.Count < overbookedMoves; a2++)
            {
                Box b2 = Boxes[a2];
                for (int a3 = a2 + 1; a3 < Boxes.Count && moves.Count < overbookedMoves; a3++)
                {
                    Box b3 = Boxes[a3];
                    for (int a4 = a3 + 1; a4 < Boxes.Count && moves.Count < overbookedMoves; a4++)
                    {
                        Box b4 = Boxes[a4];
                        for (int a5 = a4 + 1; a5 < Boxes.Count && moves.Count < overbookedMoves; a5++)
                        {
                            Box b5 = Boxes[a5];
                            for (int a6 = a5 + 1; a6 < Boxes.Count && moves.Count < overbookedMoves; a6++)
                            {
                                Box b6 = Boxes[a6];
                                for (int a7 = a6 + 1; a7 < Boxes.Count && moves.Count < overbookedMoves; a7++)
                                {
                                    Box b7 = Boxes[a7];
                                    for (int a8 = a7 + 1; a8 < Boxes.Count && moves.Count < overbookedMoves; a8++)
                                    {
                                        Box b8 = Boxes[a8];
                                        moves.Add(new ComplexMove()
                                        {
                                            Weight = b1.Weight + b2.Weight + b3.Weight + b4.Weight + b5.Weight + b6.Weight + b7.Weight + b8.Weight,
                                            Volume = b1.Volume + b2.Volume + b3.Volume + b4.Volume + b5.Volume + b6.Volume + b7.Volume + b8.Volume,
                                            Boxes = new Box[] { b1, b2, b3, b4, b5, b6, b7, b8 },
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (moves.Count > maxMoves)
        {
            moves = moves.OrderBy(m => m.Weight).Take(maxMoves).ToList();
        }
        Moves = moves.OrderBy(m => m.Volume).ToArray();
    }
}

static class Solver
{
    public static void Solve(Box[] boxes)
    {
        Stopwatch sw = Stopwatch.StartNew();

        Truck[] trucks = new Truck[100];
        for (int i = 0; i < trucks.Length; i++)
            trucks[i] = new Truck()
            {
                Id = i,
            };

        int minTargetWeight = (int)Math.Floor(boxes.Sum(b => (double)b.Weight) / trucks.Length);
        TargetWeight targetWeight = new TargetWeight()
        {
            Min = minTargetWeight,
            Max = minTargetWeight + 1,
        };

        foreach (Box b in boxes.OrderBy(b => -b.Weight))
        {
            Truck truck = null;
            int truckBalance = int.MaxValue;

            foreach (Truck t in trucks)
            {
                int balance = t.Weight;

                if (balance < truckBalance)
                {
                    truck = t;
                    truckBalance = balance;
                }
            }
            truck.AddBox(b);
        }

        int bestScore = int.MaxValue;
        Truck[] solution = null;

        while (sw.Elapsed.TotalSeconds < 45)
        {
            // Fix volume problems
            foreach (Truck truck1 in trucks.OrderBy(t => -t.Volume).ToArray())
                while (truck1.Volume > Truck.MaxVolume)
                {
                    Box box1 = null;
                    Box box2 = null;
                    Truck truck2 = null;
                    int bestBalance = int.MaxValue;

                    foreach (Truck t2 in trucks)
                        if (truck1 != t2 && t2.Volume < Truck.MaxVolume)
                        {
                            foreach (Box b1 in truck1.Boxes)
                                foreach (Box b2 in t2.Boxes)
                                    if (b1.Volume > b2.Volume && t2.Volume + b1.Volume - b2.Volume < Truck.MaxVolume)
                                    {
                                        int t1weight = truck1.Weight - b1.Weight + b2.Weight;
                                        int t2weight = t2.Weight - b2.Weight + b1.Weight;
                                        int balance = (targetWeight.Distance(t1weight) + targetWeight.Distance(t2weight)) - (targetWeight.Distance(truck1.Weight) + targetWeight.Distance(t2.Weight));

                                        if (balance < bestBalance)
                                        {
                                            box1 = b1;
                                            box2 = b2;
                                            truck2 = t2;
                                            bestBalance = balance;
                                        }
                                    }
                        }
                    if (box1 == null)
                        break;
                    truck1.RemoveBox(box1);
                    truck2.RemoveBox(box2);
                    truck1.AddBox(box2);
                    truck2.AddBox(box1);
                }

            // Complex optimization
            const int ComplexOptimizationMaxMoves = 1000;

            foreach (Truck t in trucks)
                t.UpdateMoves(ComplexOptimizationMaxMoves);
            for (int skip = 0; skip < trucks.Length && sw.Elapsed.TotalSeconds < 45;)
            {
                int score = trucks.Max(t => t.Weight) - trucks.Min(t => t.Weight);

                if (score < bestScore && trucks.Max(t => -t.Volume) < Truck.MaxVolume)
                {
                    bestScore = score;
                    solution = boxes.Select(b => b.Truck).ToArray();
                }
#if LOCAL
                Console.Write($"{bestScore} {sw.Elapsed.TotalSeconds}s {score}                               \r");
#endif
                Truck truck1 = trucks.OrderBy(t => -targetWeight.Distance(t.Weight)).Skip(skip).First();

                if (targetWeight.Distance(truck1.Weight) == 0)
                    break;

                int t1balance = targetWeight.Distance(truck1.Weight);
                ComplexMove move1 = null;
                ComplexMove move2 = null;
                Truck truck2 = null;
                int newWeightBalance = t1balance;

                foreach (Truck t2 in trucks)
                    if (truck1 != t2)
                    {
                        int t2balance = targetWeight.Distance(t2.Weight);
                        int previousWeightBalance = t1balance + t2balance;

                        foreach (ComplexMove m1 in truck1.Moves)
                        {
                            int minM2Volume = (t2.Volume + m1.Volume) - Truck.MaxVolume;
                            int maxM2Volume = Truck.MaxVolume - (truck1.Volume - m1.Volume);
                            int start = minM2Volume < t2.Moves[0].Volume ? 0 : BinarySearch(t2.Moves, minM2Volume);
                            int end = maxM2Volume > t2.Moves[t2.Moves.Length - 1].Volume ? t2.Moves.Length : BinarySearch(t2.Moves, maxM2Volume);
                            int t1weight = truck1.Weight - m1.Weight;
                            int t2weight = t2.Weight + m1.Weight;
                            int minM2Weight = (-previousWeightBalance - t1weight + t2weight) / 2;
                            int maxM2Weight = (previousWeightBalance - t1weight + t2weight) / 2;

                            for (int i = start; i < end; i++)
                            {
                                ComplexMove m2 = t2.Moves[i];

                                if (m2.Weight < minM2Weight || m2.Weight > maxM2Weight)
                                    continue;

                                int t1w = t1weight + m2.Weight;
                                int t2w = t2weight - m2.Weight;
                                int balance = targetWeight.Distance(t1w) + targetWeight.Distance(t2w) - previousWeightBalance;

                                if (balance >= 0)
                                    continue;

                                int weight = truck1.Weight + m2.Weight - m1.Weight;
                                int weightBalance = targetWeight.Distance(weight);

                                if (weightBalance < newWeightBalance)
                                {
                                    move1 = m1;
                                    move2 = m2;
                                    truck2 = t2;
                                    newWeightBalance = weightBalance;
                                }
                            }
                        }
                    }
                if (truck2 == null)
                {
                    skip++;
                    continue;
                }

                skip = 0;
                foreach (Box b in move1.Boxes)
                    truck1.RemoveBox(b);
                foreach (Box b in move2.Boxes)
                    truck2.RemoveBox(b);
                foreach (Box b in move2.Boxes)
                    truck1.AddBox(b);
                foreach (Box b in move1.Boxes)
                    truck2.AddBox(b);
                truck1.UpdateMoves(ComplexOptimizationMaxMoves);
                truck2.UpdateMoves(ComplexOptimizationMaxMoves);
            }

            // Do few iterations of unconstrained optimization
            int count = 1;

            for (int c = 0; c < count; c++)
            {
                Truck truck1 = trucks.OrderBy(t => -targetWeight.Distance(t.Weight)).First();

                if (targetWeight.Distance(truck1.Weight) == 0)
                    break;

                int t1balance = targetWeight.Distance(truck1.Weight);
                ComplexMove move1 = null;
                ComplexMove move2 = null;
                Truck truck2 = null;
                int newWeightBalance = t1balance;

                foreach (Truck t2 in trucks)
                    if (truck1 != t2)
                    {
                        int t2balance = targetWeight.Distance(t2.Weight);
                        int previousWeightBalance = t1balance + t2balance;

                        foreach (ComplexMove m1 in truck1.Moves)
                        {
                            int t1weight = truck1.Weight - m1.Weight;
                            int t2weight = t2.Weight + m1.Weight;
                            int minM2Weight = (-previousWeightBalance - t1weight + t2weight) / 2;
                            int maxM2Weight = (previousWeightBalance - t1weight + t2weight) / 2;

                            foreach (ComplexMove m2 in t2.Moves)
                            {
                                if (m2.Weight < minM2Weight || m2.Weight > maxM2Weight)
                                    continue;

                                int t1w = t1weight + m2.Weight;
                                int t2w = t2weight - m2.Weight;
                                int balance = targetWeight.Distance(t1w) + targetWeight.Distance(t2w) - previousWeightBalance;

                                if (balance > 0 || targetWeight.Distance(t1w) == t2balance)
                                    continue;

                                int weight = truck1.Weight + m2.Weight - m1.Weight;
                                int weightBalance = targetWeight.Distance(weight);

                                if (weightBalance < newWeightBalance)
                                {
                                    move1 = m1;
                                    move2 = m2;
                                    truck2 = t2;
                                    newWeightBalance = weightBalance;
                                }
                            }
                        }
                    }
                if (truck2 == null)
                    break;

                foreach (Box b in move1.Boxes)
                    truck1.RemoveBox(b);
                foreach (Box b in move2.Boxes)
                    truck2.RemoveBox(b);
                foreach (Box b in move2.Boxes)
                    truck1.AddBox(b);
                foreach (Box b in move1.Boxes)
                    truck2.AddBox(b);
                truck1.UpdateMoves(ComplexOptimizationMaxMoves);
                truck2.UpdateMoves(ComplexOptimizationMaxMoves);
            }
        }

        for (int i = 0; i < solution.Length; i++)
            boxes[i].Truck = solution[i];

#if LOCAL
        //foreach (Truck t in trucks.OrderBy(t => Math.Abs(t.Weight - targetWeight)).ToArray())
        //    Console.WriteLine($"{t.Id}: {Math.Abs(t.Weight - targetWeight)}   {t.Weight / (double)Box.FixedPoint}  {t.Volume / (double)Box.FixedPoint}");
#endif
    }

    private static int BinarySearch(ComplexMove[] moves, int minVolume)
    {
        int first = 0;
        int last = moves.Length - 1;

        while (first + 1 < last)
        {
            int middle = (first + last) / 2;

            if (moves[middle].Volume < minVolume)
                first = middle;
            else
                last = middle;
        }
        return first + 1;
    }
}

#if !LOCAL
class Program
{
    static void Main(string[] args)
    {
        int boxCount = int.Parse(Console.ReadLine());
        Box[] boxes = new Box[boxCount];
        for (int i = 0; i < boxCount; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int weight = (int)Math.Round(double.Parse(inputs[0]) * Box.FixedPoint);
            int volume = (int)Math.Round(double.Parse(inputs[1]) * Box.FixedPoint);

            boxes[i] = new Box()
            {
                Weight = weight,
                Volume = volume,
            };
        }

        Solver.Solve(boxes);

        for (int i = 0; i < boxes.Length; i++)
            Console.Write($"{boxes[i].Truck.Id} ");
        Console.WriteLine();
    }
}
#endif
