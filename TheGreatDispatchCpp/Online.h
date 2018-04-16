#include <algorithm>
#include <chrono>
#include <cmath>
#include <vector>

const int FixedPoint = 100000;
const int MaxTruckVolume = 100 * FixedPoint;
#if LOCAL
  const std::chrono::seconds MaxExecutionTime(30);
#else
  const std::chrono::seconds MaxExecutionTime(45);
#endif

struct TargetWeight
{
    int Min;
    int Max;

    int Distance(int value)
    {
        if (value <= Min)
            return Min - value;
        return value - Max;
    }
};

struct Truck;

struct Box
{
    int Weight;
    int Volume;
    Truck* T;
};

struct ComplexMove
{
    int Weight;
    int Volume;
    std::vector<Box*> Boxes;

    ComplexMove()
        : Weight(0)
        , Volume(0)
    {
    }
    ComplexMove(int weight, int volume, std::vector<Box*> boxes)
        : Weight(weight)
        , Volume(volume)
        , Boxes(std::move(boxes))
    {
    }
};

struct Truck
{
    int Id;
    int Weight = 0;
    int Volume = 0;
    std::vector<Box*> Boxes;
    std::vector<ComplexMove> Moves;

    void AddBox(Box* b)
    {
        Weight += b->Weight;
        Volume += b->Volume;
        Boxes.push_back(b);
        b->T = this;
    }

    void RemoveBox(Box* b)
    {
        Weight -= b->Weight;
        Volume -= b->Volume;
        for (size_t i = 0; i < Boxes.size(); i++)
            if (Boxes[i] == b)
            {
                Boxes.erase(Boxes.begin() + i);
                break;
            }
        b->T = nullptr;
    }

    void UpdateMoves(size_t maxMoves)
    {
        size_t overbookedMoves = maxMoves * 3;

        Moves.clear();
        for (Box* b1 : Boxes)
        {
            Moves.emplace_back(
                b1->Weight,
                b1->Volume,
                std::vector<Box*>({ b1 }));
        }
        for (size_t a1 = 0; a1 < Boxes.size() && Moves.size() < overbookedMoves; a1++)
        {
            Box* b1 = Boxes[a1];
            for (size_t a2 = a1 + 1; a2 < Boxes.size() && Moves.size() < overbookedMoves; a2++)
            {
                Box* b2 = Boxes[a2];
                Moves.emplace_back(
                    b1->Weight + b2->Weight,
                    b1->Volume + b2->Volume,
                    std::vector<Box*>({ b1, b2 }));
            }
        }
        for (size_t a1 = 0; a1 < Boxes.size() && Moves.size() < overbookedMoves; a1++)
        {
            Box* b1 = Boxes[a1];
            for (size_t a2 = a1 + 1; a2 < Boxes.size() && Moves.size() < overbookedMoves; a2++)
            {
                Box* b2 = Boxes[a2];
                for (size_t a3 = a2 + 1; a3 < Boxes.size() && Moves.size() < overbookedMoves; a3++)
                {
                    Box* b3 = Boxes[a3];
                    Moves.emplace_back(
                        b1->Weight + b2->Weight + b3->Weight,
                        b1->Volume + b2->Volume + b3->Volume,
                        std::vector<Box*>({ b1, b2, b3 }));
                }
            }
        }
        for (size_t a1 = 0; a1 < Boxes.size() && Moves.size() < overbookedMoves; a1++)
        {
            Box* b1 = Boxes[a1];
            for (size_t a2 = a1 + 1; a2 < Boxes.size() && Moves.size() < overbookedMoves; a2++)
            {
                Box* b2 = Boxes[a2];
                for (size_t a3 = a2 + 1; a3 < Boxes.size() && Moves.size() < overbookedMoves; a3++)
                {
                    Box* b3 = Boxes[a3];
                    for (size_t a4 = a3 + 1; a4 < Boxes.size() && Moves.size() < overbookedMoves; a4++)
                    {
                        Box* b4 = Boxes[a4];
                        Moves.emplace_back(
                            b1->Weight + b2->Weight + b3->Weight + b4->Weight,
                            b1->Volume + b2->Volume + b3->Volume + b4->Volume,
                            std::vector<Box*>({ b1, b2, b3, b4 }));
                    }
                }
            }
        }
        for (size_t a1 = 0; a1 < Boxes.size() && Moves.size() < overbookedMoves; a1++)
        {
            Box* b1 = Boxes[a1];
            for (size_t a2 = a1 + 1; a2 < Boxes.size() && Moves.size() < overbookedMoves; a2++)
            {
                Box* b2 = Boxes[a2];
                for (size_t a3 = a2 + 1; a3 < Boxes.size() && Moves.size() < overbookedMoves; a3++)
                {
                    Box* b3 = Boxes[a3];
                    for (size_t a4 = a3 + 1; a4 < Boxes.size() && Moves.size() < overbookedMoves; a4++)
                    {
                        Box* b4 = Boxes[a4];
                        for (size_t a5 = a4 + 1; a5 < Boxes.size() && Moves.size() < overbookedMoves; a5++)
                        {
                            Box* b5 = Boxes[a5];
                            Moves.emplace_back(
                                b1->Weight + b2->Weight + b3->Weight + b4->Weight + b5->Weight,
                                b1->Volume + b2->Volume + b3->Volume + b4->Volume + b5->Volume,
                                std::vector<Box*>({ b1, b2, b3, b4, b5 }));
                        }
                    }
                }
            }
        }
        for (size_t a1 = 0; a1 < Boxes.size() && Moves.size() < overbookedMoves; a1++)
        {
            Box* b1 = Boxes[a1];
            for (size_t a2 = a1 + 1; a2 < Boxes.size() && Moves.size() < overbookedMoves; a2++)
            {
                Box* b2 = Boxes[a2];
                for (size_t a3 = a2 + 1; a3 < Boxes.size() && Moves.size() < overbookedMoves; a3++)
                {
                    Box* b3 = Boxes[a3];
                    for (size_t a4 = a3 + 1; a4 < Boxes.size() && Moves.size() < overbookedMoves; a4++)
                    {
                        Box* b4 = Boxes[a4];
                        for (size_t a5 = a4 + 1; a5 < Boxes.size() && Moves.size() < overbookedMoves; a5++)
                        {
                            Box* b5 = Boxes[a5];
                            for (size_t a6 = a5 + 1; a6 < Boxes.size() && Moves.size() < overbookedMoves; a6++)
                            {
                                Box* b6 = Boxes[a6];
                                Moves.emplace_back(
                                    b1->Weight + b2->Weight + b3->Weight + b4->Weight + b5->Weight + b6->Weight,
                                    b1->Volume + b2->Volume + b3->Volume + b4->Volume + b5->Volume + b6->Volume,
                                    std::vector<Box*>({ b1, b2, b3, b4, b5, b6 }));
                            }
                        }
                    }
                }
            }
        }
        for (size_t a1 = 0; a1 < Boxes.size() && Moves.size() < overbookedMoves; a1++)
        {
            Box* b1 = Boxes[a1];
            for (size_t a2 = a1 + 1; a2 < Boxes.size() && Moves.size() < overbookedMoves; a2++)
            {
                Box* b2 = Boxes[a2];
                for (size_t a3 = a2 + 1; a3 < Boxes.size() && Moves.size() < overbookedMoves; a3++)
                {
                    Box* b3 = Boxes[a3];
                    for (size_t a4 = a3 + 1; a4 < Boxes.size() && Moves.size() < overbookedMoves; a4++)
                    {
                        Box* b4 = Boxes[a4];
                        for (size_t a5 = a4 + 1; a5 < Boxes.size() && Moves.size() < overbookedMoves; a5++)
                        {
                            Box* b5 = Boxes[a5];
                            for (size_t a6 = a5 + 1; a6 < Boxes.size() && Moves.size() < overbookedMoves; a6++)
                            {
                                Box* b6 = Boxes[a6];
                                for (size_t a7 = a6 + 1; a7 < Boxes.size() && Moves.size() < overbookedMoves; a7++)
                                {
                                    Box* b7 = Boxes[a7];
                                    Moves.emplace_back(
                                        b1->Weight + b2->Weight + b3->Weight + b4->Weight + b5->Weight + b6->Weight + b7->Weight,
                                        b1->Volume + b2->Volume + b3->Volume + b4->Volume + b5->Volume + b6->Volume + b7->Volume,
                                        std::vector<Box*>({ b1, b2, b3, b4, b5, b6, b7 }));
                                }
                            }
                        }
                    }
                }
            }
        }
        for (size_t a1 = 0; a1 < Boxes.size() && Moves.size() < overbookedMoves; a1++)
        {
            Box* b1 = Boxes[a1];
            for (size_t a2 = a1 + 1; a2 < Boxes.size() && Moves.size() < overbookedMoves; a2++)
            {
                Box* b2 = Boxes[a2];
                for (size_t a3 = a2 + 1; a3 < Boxes.size() && Moves.size() < overbookedMoves; a3++)
                {
                    Box* b3 = Boxes[a3];
                    for (size_t a4 = a3 + 1; a4 < Boxes.size() && Moves.size() < overbookedMoves; a4++)
                    {
                        Box* b4 = Boxes[a4];
                        for (size_t a5 = a4 + 1; a5 < Boxes.size() && Moves.size() < overbookedMoves; a5++)
                        {
                            Box* b5 = Boxes[a5];
                            for (size_t a6 = a5 + 1; a6 < Boxes.size() && Moves.size() < overbookedMoves; a6++)
                            {
                                Box* b6 = Boxes[a6];
                                for (size_t a7 = a6 + 1; a7 < Boxes.size() && Moves.size() < overbookedMoves; a7++)
                                {
                                    Box* b7 = Boxes[a7];
                                    for (size_t a8 = a7 + 1; a8 < Boxes.size() && Moves.size() < overbookedMoves; a8++)
                                    {
                                        Box* b8 = Boxes[a8];
                                        Moves.emplace_back(
                                            b1->Weight + b2->Weight + b3->Weight + b4->Weight + b5->Weight + b6->Weight + b7->Weight + b8->Weight,
                                            b1->Volume + b2->Volume + b3->Volume + b4->Volume + b5->Volume + b6->Volume + b7->Volume + b8->Volume,
                                            std::vector<Box*>({ b1, b2, b3, b4, b5, b6, b7, b8 }));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (Moves.size() > maxMoves)
        {
            std::partial_sort(Moves.begin(), Moves.begin() + maxMoves, Moves.end(), [](const ComplexMove& m1, const ComplexMove& m2) { return m1.Weight < m2.Weight; });
            Moves.resize(maxMoves);
        }
        std::sort(Moves.begin(), Moves.end(), [](const ComplexMove& m1, const ComplexMove& m2) { return m1.Volume < m2.Volume; });
    }
};

std::chrono::nanoseconds now()
{
    return std::chrono::steady_clock::now().time_since_epoch();
}

struct Solver
{
    static void Solve(std::vector<Box>& boxes, std::vector<Truck>& trucks)
    {
        std::chrono::nanoseconds clockStart = now();
        trucks.resize(100);

        for (size_t i = 0; i < trucks.size(); i++)
            trucks[i].Id = (int)i;

        double minTargetWeight = 0;
        for (Box& b : boxes)
            minTargetWeight += b.Weight;
        minTargetWeight /= trucks.size();
        TargetWeight targetWeight;
        targetWeight.Min = (int)std::floor(minTargetWeight);
        targetWeight.Max = (int)std::ceil(minTargetWeight);

        std::vector<Box*> boxesByWeight(boxes.size());
        for (size_t i = 0; i < boxes.size(); i++)
            boxesByWeight[i] = &boxes[i];
        std::sort(boxesByWeight.begin(), boxesByWeight.end(), [](Box* b1, Box* b2) { return b2->Weight < b1->Weight; });
        for (Box* b : boxesByWeight)
        {
            Truck* truck = nullptr;
            int truckBalance = std::numeric_limits<int>::max();

            for (Truck& t : trucks)
            {
                int balance = t.Weight;

                if (balance < truckBalance)
                {
                    truck = &t;
                    truckBalance = balance;
                }
            }
            truck->AddBox(b);
        }

        int bestScore = std::numeric_limits<int>::max();
        std::vector<Truck*> solution;
        int ComplexOptimizationMaxMoves = boxes.size() < 1000 ? 1500 : 250;

        while (now() - clockStart < MaxExecutionTime && bestScore > targetWeight.Max - targetWeight.Min)
        {
            // Fix volume problems
            std::vector<Truck*> trucksByVolume;
            trucksByVolume.reserve(trucks.size());
            for (Truck& t : trucks)
                trucksByVolume.push_back(&t);
            std::sort(trucksByVolume.begin(), trucksByVolume.end(), [](Truck* t1, Truck* t2) { return t2->Volume < t1->Volume; });
            for (Truck* truck1 : trucksByVolume)
                while (truck1->Volume > MaxTruckVolume)
                {
                    Box* box1 = nullptr;
                    Box* box2 = nullptr;
                    Truck* truck2 = nullptr;
                    int bestBalance = std::numeric_limits<int>::max();

                    for (Truck& t2 : trucks)
                        if (truck1->Id != t2.Id && t2.Volume < MaxTruckVolume)
                        {
                            for (Box* b1 : truck1->Boxes)
                                for (Box* b2 : t2.Boxes)
                                    if (b1->Volume > b2->Volume && t2.Volume + b1->Volume - b2->Volume < MaxTruckVolume)
                                    {
                                        int t1weight = truck1->Weight - b1->Weight + b2->Weight;
                                        int t2weight = t2.Weight - b2->Weight + b1->Weight;
                                        int balance = (targetWeight.Distance(t1weight) + targetWeight.Distance(t2weight)) - (targetWeight.Distance(truck1->Weight) + targetWeight.Distance(t2.Weight));

                                        if (balance < bestBalance)
                                        {
                                            box1 = b1;
                                            box2 = b2;
                                            truck2 = &t2;
                                            bestBalance = balance;
                                        }
                                    }
                        }
                    if (box1 == nullptr)
                        break;
                    truck1->RemoveBox(box1);
                    truck2->RemoveBox(box2);
                    truck1->AddBox(box2);
                    truck2->AddBox(box1);
                }

            // Complex optimization
            ComplexOptimizationMaxMoves *= 2;
            for (Truck& t : trucks)
                t.UpdateMoves(ComplexOptimizationMaxMoves);
            for (size_t skip = 0; skip < trucks.size() && now() - clockStart < MaxExecutionTime;)
            {
                int minWeight = std::numeric_limits<int>::max(), maxWeight = 0, maxVolume = 0;
                for (Truck& t : trucks)
                {
                    if (t.Weight < minWeight)
                        minWeight = t.Weight;
                    if (t.Weight > maxWeight)
                        maxWeight = t.Weight;
                    if (t.Volume > maxVolume)
                        maxVolume = t.Volume;
                }
                int score = maxWeight - minWeight;

                if (score < bestScore && maxVolume < MaxTruckVolume)
                {
                    bestScore = score;
                    solution.resize(boxes.size());
                    for (size_t i = 0; i < boxes.size(); i++)
                        solution[i] = boxes[i].T;
                }
#if LOCAL
                static std::chrono::nanoseconds lastPrint = clockStart;

                if (now() - lastPrint > std::chrono::milliseconds(20))
                {
                    printf("%d %.3lfs %d        \r", bestScore, std::chrono::duration_cast<std::chrono::milliseconds>(now() - clockStart).count() / 1000.0, score);
                    lastPrint = now();
                }
#endif
                std::vector<Truck*> fats(trucks.size());
                for (size_t i = 0; i < fats.size(); i++)
                    fats[i] = &trucks[i];
                std::sort(fats.begin(), fats.end(), [&](Truck* t1, Truck* t2)
                {
                    if (targetWeight.Distance(t2->Weight) == targetWeight.Distance(t1->Weight))
                        return t1->Id < t2->Id;
                    return targetWeight.Distance(t2->Weight) < targetWeight.Distance(t1->Weight);
                });
                for (size_t i = 0; i < skip; i++)
                    fats.erase(fats.begin());
                Truck* truck1 = fats[0];

                if (targetWeight.Distance(truck1->Weight) == 0)
                    break;

                int t1balance = targetWeight.Distance(truck1->Weight);
                ComplexMove* move1 = nullptr;
                ComplexMove* move2 = nullptr;
                Truck* truck2 = nullptr;
                int newWeightBalance = t1balance;

                for (Truck* t2 : fats)
                    if (truck1 != t2)
                    {
                        int t2balance = targetWeight.Distance(t2->Weight);
                        int previousWeightBalance = t1balance + t2balance;

                        for (ComplexMove& m1 : truck1->Moves)
                        {
                            int minM2Volume = (t2->Volume + m1.Volume) - MaxTruckVolume;
                            int maxM2Volume = MaxTruckVolume - (truck1->Volume - m1.Volume);
                            size_t start = minM2Volume < t2->Moves[0].Volume ? 0 : LowerBound(t2->Moves, minM2Volume);
                            size_t end = maxM2Volume > t2->Moves[t2->Moves.size() - 1].Volume ? t2->Moves.size() : UpperBound(t2->Moves, maxM2Volume);
                            int t1weight = truck1->Weight - m1.Weight;
                            int t2weight = t2->Weight + m1.Weight;

                            for (size_t i = start; i < end; i++)
                            {
                                ComplexMove& m2 = t2->Moves[i];

                                int t1w = t1weight + m2.Weight;
                                int weightBalance = targetWeight.Distance(t1w);

                                if (weightBalance >= newWeightBalance)
                                    continue;

                                int t2w = t2weight - m2.Weight;
                                int balance = targetWeight.Distance(t2w) - t1balance;

                                if (balance >= 0)
                                    continue;

                                {
                                    move1 = &m1;
                                    move2 = &m2;
                                    truck2 = t2;
                                    newWeightBalance = weightBalance;
                                }
                            }
                        }
                    }
                if (truck2 == nullptr)
                {
                    skip++;
                    continue;
                }

                skip = 0;
                for (Box* b : move1->Boxes)
                    truck1->RemoveBox(b);
                for (Box* b : move2->Boxes)
                    truck2->RemoveBox(b);
                for (Box* b : move2->Boxes)
                    truck1->AddBox(b);
                for (Box* b : move1->Boxes)
                    truck2->AddBox(b);
                truck1->UpdateMoves(ComplexOptimizationMaxMoves);
                truck2->UpdateMoves(ComplexOptimizationMaxMoves);
            }

#if LOCAL
            //PrintTrucks(trucks, targetWeight);
#endif

            // Randomize a bit
            std::vector<Truck*> ttt;
            ttt.reserve(trucks.size());
            for (Truck& t : trucks)
                if (targetWeight.Distance(t.Weight) > 0)
                    ttt.push_back(&t);
            std::sort(ttt.begin(), ttt.end(), [&](Truck* t1, Truck* t2) { return targetWeight.Distance(t2->Weight) < targetWeight.Distance(t1->Weight); });
            std::vector<bool> touched(ttt.size());

            for (size_t i = 0; i < ttt.size() / 2 && now() - clockStart < MaxExecutionTime; i++)
                if (!touched[i])
                {
                    Truck* t1 = ttt[i];
                    Truck* truck1 = t1;
                    Truck* truck2 = nullptr;
                    size_t bestj = (size_t)-1;
                    ComplexMove* move1 = nullptr;
                    ComplexMove* move2 = nullptr;
                    int bestDistance = std::numeric_limits<int>::max();
                    for (size_t j = 0; j < ttt.size(); j++)
                        if (i != j && !touched[j])
                        {
                            Truck* t2 = ttt[j];

                            for (ComplexMove& m1 : t1->Moves)
                                if (m1.Boxes.size() != t1->Boxes.size())
                                {
                                    int minM2Volume = (t2->Volume + m1.Volume) - MaxTruckVolume;
                                    int maxM2Volume = MaxTruckVolume - (truck1->Volume - m1.Volume);
                                    size_t start = minM2Volume < t2->Moves[0].Volume ? 0 : LowerBound(t2->Moves, minM2Volume);
                                    size_t end = maxM2Volume > t2->Moves[t2->Moves.size() - 1].Volume ? t2->Moves.size() : UpperBound(t2->Moves, maxM2Volume);
                                    for (size_t i = start; i < end; i++)
                                    {
                                        ComplexMove& m2 = t2->Moves[i];

                                        int distance = std::abs(m1.Weight - m2.Weight);

                                        if (distance < bestDistance ||
                                            (distance == bestDistance && m1.Boxes.size() + m2.Boxes.size() > move1->Boxes.size() + move2->Boxes.size()))
                                        {
                                            move1 = &m1;
                                            move2 = &m2;
                                            truck2 = t2;
                                            bestDistance = distance;
                                            bestj = j;
                                        }
                                    }
                                }
                        }
                    if (bestj == (size_t)-1)
                        continue;
                    touched[i] = true;
                    touched[bestj] = true;
                    for (Box* b : move1->Boxes)
                        truck1->RemoveBox(b);
                    for (Box* b : move2->Boxes)
                        truck2->RemoveBox(b);
                    for (Box* b : move2->Boxes)
                        truck1->AddBox(b);
                    for (Box* b : move1->Boxes)
                        truck2->AddBox(b);
                    truck1->UpdateMoves(ComplexOptimizationMaxMoves);
                    truck2->UpdateMoves(ComplexOptimizationMaxMoves);
                }
        }

        for (size_t i = 0; i < solution.size(); i++)
            boxes[i].T = solution[i];
    }

private:
#if LOCAL
    static void PrintTrucks(std::vector<Truck*>& trucks, TargetWeight targetWeight)
    {
        for (Truck* t : trucks)
            printf("%2d: %3d[%2d]   %.5lf  %.5lf\n", t->Id, targetWeight.Distance(t->Weight), (int)t->Boxes.size(), t->Weight / (double)FixedPoint, t->Volume / (double)FixedPoint);
    }

    static void PrintTrucks(std::vector<Truck>& trucks, TargetWeight targetWeight)
    {
        std::vector<Truck*> trucksByScore(trucks.size());
        for (size_t i = 0; i < trucks.size(); i++)
            trucksByScore[i] = &trucks[i];
        std::sort(trucksByScore.begin(), trucksByScore.end(), [&](const Truck* t1, const Truck* t2) { return targetWeight.Distance(t1->Weight) < targetWeight.Distance(t2->Weight); });
        PrintTrucks(trucksByScore, targetWeight);
    }
#endif

    static size_t LowerBound(std::vector<ComplexMove>& moves, int minVolume)
    {
        size_t first = 0;
        size_t count = moves.size();

        while (0 < count)
        {
            size_t count2 = count >> 1;
            size_t mid = first + count2;
            if (moves[mid].Volume < minVolume)
            {
                first = mid + 1;
                count -= count2 + 1;
            }
            else
            {
                count = count2;
            }
        }
        return first;
    }

    static size_t UpperBound(std::vector<ComplexMove>& moves, int minVolume)
    {
        size_t first = 0;
        size_t count = moves.size();

        while (0 < count)
        {
            size_t count2 = count >> 1;
            size_t mid = first + count2;
            if (minVolume < moves[mid].Volume)
            {
                count = count2;
            }
            else
            {
                first = mid + 1;
                count -= count2 + 1;
            }
        }
        return first;
    }
};

#if !LOCAL
#include <iostream>

int main()
{
    size_t boxCount;
    std::cin >> boxCount; std::cin.ignore();
    std::vector<Box> boxes(boxCount);
    for (size_t i = 0; i < boxCount; i++)
    {
        double weight, volume;

        std::cin >> weight >> volume; std::cin.ignore();
        boxes[i].Weight = (int)std::round(weight * FixedPoint);
        boxes[i].Volume = (int)std::round(volume * FixedPoint);
    }

    std::vector<Truck> trucks;
    Solver::Solve(boxes, trucks);

    for (Box& b : boxes)
        std::cout << b.T->Id << " ";
    std::cout << std::endl;
}
#endif
