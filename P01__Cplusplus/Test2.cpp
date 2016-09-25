#include "stdafx.h"
#include <array>
#include <algorithm>
using namespace System;
using namespace System::Diagnostics;

const int ARRAY_SIZE = 50000000;
const int NUM_LOOPS = 4;

// Testing .Net Array.Sort() vs C++'s std::sort on both languages standard arrays.
//int main2(array<System::String ^> ^args)
int main2()
{
	auto cppArray = new int[ARRAY_SIZE];
	auto netArray = gcnew array<long long>(ARRAY_SIZE);
	double totalTimeCpp = 0.0;
	double totalTimeNet = 0.0;

	auto randGen = gcnew Random();
	auto stopWatch = Stopwatch::StartNew();

	for (int i = 0; i < ARRAY_SIZE; ++i) {
		int randNum = randGen->Next();
		cppArray[i] = randNum;
		netArray[i] = randNum;
	}
	for (int i = 0; i < NUM_LOOPS; ++i) {
		//for (int i = 0; i < ARRAY_SIZE; ++i) {
		//	int randNum = randGen->Next();
		//	cppArray[i] = randNum;
		//	netArray[i] = randNum;
		//}

		stopWatch = Stopwatch::StartNew();
		Array::Sort(netArray);
		stopWatch->Stop();
		totalTimeNet += (double)stopWatch->ElapsedMilliseconds;
		Console::WriteLine(L"C#: System.Sort {0} milliseconds.", stopWatch->ElapsedMilliseconds);

		stopWatch = Stopwatch::StartNew();
		std::sort(cppArray, cppArray + ARRAY_SIZE);
		stopWatch->Stop();
		totalTimeCpp += (double)stopWatch->ElapsedMilliseconds;
		Console::WriteLine(L"C++: std::sort {0} milliseconds.", stopWatch->ElapsedMilliseconds);

	}

	Console::WriteLine(L"Average time C++: {0} milliseconds.", totalTimeCpp / (double)NUM_LOOPS);
	Console::WriteLine(L"Average time .NET: {0} milliseconds.", totalTimeNet / (double)NUM_LOOPS);
	Console::WriteLine(L"Array.Sort time / std::sort time: {0}.", totalTimeNet / totalTimeCpp);
	Console::ReadKey(false);
	return 0;
}