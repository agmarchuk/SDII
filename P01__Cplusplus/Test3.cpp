// Testing .Net Array.Sort() vs C++'s std::sort on both languages standard arrays.
#include "stdafx.h"
#include <array>
#include <algorithm>
using namespace System;
using namespace System::Diagnostics;

const int ARRAY_SIZE = 10000000;
const int NUM_LOOPS = 1;

void generateArrays(Byte* cppArray, array<Byte>^ netArray) {
	auto randGen = gcnew Random();
	randGen->NextBytes(netArray);
	for (int i = 0; i < ARRAY_SIZE; ++i) {
		cppArray[i] = netArray[i];
	}
}

void generateArrays(Single* cppArray, array<Single>^ netArray) {
	auto randGen = gcnew Random();
	for (int i = 0; i < ARRAY_SIZE; ++i) {
		auto randNum = (Single)randGen->NextDouble();
		cppArray[i] = netArray[i] = randNum;
	}
}

void generateArrays(Double* cppArray, array<Double>^ netArray) {
	auto randGen = gcnew Random();
	for (int i = 0; i < ARRAY_SIZE; ++i) {
		auto randNum = randGen->NextDouble();
		cppArray[i] = netArray[i] = randNum;
	}
}

void generateArrays(Int32* cppArray, array<Int32>^ netArray) {
	auto randGen = gcnew Random();
	for (int i = 0; i < ARRAY_SIZE; ++i) {
		auto randNum = randGen->Next();
		cppArray[i] = netArray[i] = randNum;
	}
}

void generateArrays(Int64* cppArray, array<Int64>^ netArray) {
	auto randGen = gcnew Random();
	for (int i = 0; i < ARRAY_SIZE; ++i) {
		auto randNum = (Int64)randGen->Next();
		cppArray[i] = netArray[i] = randNum;
	}
}

template<typename T>
void benchmark() {
	auto cppArray = new T[ARRAY_SIZE];
	auto netArray = gcnew array<T>(ARRAY_SIZE);
	Double totalTimeCpp = 0.0;
	Double totalTimeNet = 0.0;

	Console::Write("Testing {0}", T::typeid);

	for (int i = 0; i < NUM_LOOPS; ++i) {
		generateArrays(cppArray, netArray);

		auto stopWatch = Stopwatch::StartNew();
		std::sort(cppArray, cppArray + ARRAY_SIZE);
		stopWatch->Stop();
		totalTimeCpp += (Double)stopWatch->ElapsedMilliseconds;

		stopWatch = Stopwatch::StartNew();
		Array::Sort(netArray);
		stopWatch->Stop();
		totalTimeNet += (Double)stopWatch->ElapsedMilliseconds;

		// progress indicator and sanity check
		Console::Write(cppArray[0] < netArray[ARRAY_SIZE - 1] ?
			"." :
			"wuuuuuuut?!");
	}

	Console::WriteLine(L"\nAverage time C++: {0} milliseconds.", totalTimeCpp / (double)NUM_LOOPS);
	Console::WriteLine(L"Average time .NET: {0} milliseconds.\n", totalTimeNet / (double)NUM_LOOPS);
};

int main3() {
	benchmark<Byte>();
	benchmark<Int32>();
	benchmark<Int64>();
	benchmark<Single>();
	benchmark<Double>();

	Console::WriteLine("All done.");
	Console::ReadKey(false);
	return 0;
}