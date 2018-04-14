using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGreatDispatch
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest("test.02");
            //int cost = 0;

            //for (int i = 1; i <= 6; i++)
            //    cost += RunTest($"test.0{i}");
            //Console.WriteLine($"Total cost: {cost / (double)Box.FixedPoint}");
        }

        static int RunTest(string fileName)
        {
            Console.WriteLine($"* {fileName}");
            using (StreamReader reader = new StreamReader(fileName))
            {
                int boxCount = int.Parse(reader.ReadLine());
                Box[] boxes = new Box[boxCount];
                for (int i = 0; i < boxCount; i++)
                {
                    string[] inputs = reader.ReadLine().Split(' ');
                    int weight = (int)Math.Round(double.Parse(inputs[0]) * Box.FixedPoint);
                    int volume = (int)Math.Round(double.Parse(inputs[1]) * Box.FixedPoint);

                    boxes[i] = new Box()
                    {
                        Weight = weight,
                        Volume = volume,
                    };
                }
                return RunTest(boxes);
            }
        }

        static int RunTest(Box[] boxes)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Solver.Solve(boxes);
            Console.WriteLine($"Time: {sw.Elapsed.TotalSeconds}s");

            Box[][] trucks = new Box[100][];

            for (int i = 0; i < trucks.Length; i++)
                trucks[i] = boxes.Where(b => b.Truck.Id == i).ToArray();
            for (int i = 0; i < trucks.Length; i++)
                if (trucks[i].Sum(b => b.Volume) > Truck.MaxVolume)
                {
                    throw new Exception($"Truck {i} is overloaded");
                }
            int averageSum = (int)Math.Round(boxes.Sum(b => (double)b.Weight) / trucks.Length);
            int[] sums = trucks.Select(t => t.Sum(b => b.Weight)).OrderBy(a => a).ToArray();
            int cost = sums.Max() - sums.Min();
            Console.WriteLine($"Cost: {cost / (double)Box.FixedPoint}");
            return cost;
        }
    }
}
