// TheGreatDispatchCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Online.h"
#include <string>
#include <iostream>
#include <sstream>
#include <fstream>

int RunTest(std::vector<Box>& boxes)
{
    std::vector<Truck> trucksBuffer;
    std::chrono::nanoseconds clockStart = now();
    Solver::Solve(boxes, trucksBuffer);
    printf("Time: %.3lfs\n", std::chrono::duration_cast<std::chrono::milliseconds>(now() - clockStart).count() / 1000.0);

    std::vector<std::vector<Box*>> trucks(100);

    for (Box& b : boxes)
        trucks[b.T->Id].push_back(&b);
    for (size_t i = 0; i < trucks.size(); i++)
    {
        int volume = 0;
        for (Box* b : trucks[i])
            volume += b->Volume;
        if (volume > MaxTruckVolume)
        {
            printf("Truck %d is overloaded\n", (int)i);
            exit(1);
        }
    }

    int minWeight = std::numeric_limits<int>::max(), maxWeight = 0;
    for (size_t i = 0; i < trucks.size(); i++)
    {
        int weight = 0;
        for (Box* b : trucks[i])
            weight += b->Weight;
        if (weight > maxWeight)
            maxWeight = weight;
        if (weight < minWeight)
            minWeight = weight;
    }

    int cost = maxWeight - minWeight;
    printf("Cost: %d\n", cost);
    return cost;
}

int RunTest(const std::string& fileName)
{
    printf("* %s\n", fileName.c_str());
    std::ifstream in(fileName);
    size_t boxCount;
    in >> boxCount;
    std::vector<Box> boxes(boxCount);
    for (size_t i = 0; i < boxCount; i++)
    {
        double weight, volume;

        in >> weight >> volume;
        boxes[i].Weight = (int)std::round(weight * FixedPoint);
        boxes[i].Volume = (int)std::round(volume * FixedPoint);
    }
    return RunTest(boxes);
}

int main()
{
    RunTest("test.01");
    //int cost = 0;

    //for (int i = 1; i <= 6; i++)
    //{
    //    std::stringstream ss;
    //    ss << "test.0" << i;
    //    cost += RunTest(ss.str());
    //}
    //printf("Total cost: %d\n", cost);
    //return 0;
}
