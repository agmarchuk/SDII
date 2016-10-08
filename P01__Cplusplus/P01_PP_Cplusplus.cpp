// P01_PP_Cplusplus.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include <stdio.h>
#include <time.h>
#include <sys/timeb.h>
#include <algorithm>
#include <vector>
#using <System.dll>

using namespace System;
using namespace System::Diagnostics;

int comp(int a, int b)
{
	return a - b;
}
int compare(const void * a, const void * b)
{
	return (*(long long*)a - *(long long*)b);
}
struct ProbeFrame
{
	char *tsk = "", *sol = "", *reg = "", *cnf = "", *com = "";
	long long siz = 0L;
	int nte = 0;
	long long lod = -1, ndx = -1, scn = -1, tim = -1, vol = -1, ram = -1, sum = 0;
	clock_t *dte;
public:
	// Сохранение теста в CSV-строке
	char* ToCSV()
	{
		return "";
			//tsk + "," +
			//sol + "," +
			//reg + "," +
			//cnf + "," +
			//siz + "," +
			//nte + "," +
			//lod + "," +
			//ndx + "," +
			//scn + "," +
			//tim + "," +
			//vol + "," +
			//ram + "," +
			//sum + "," +
			//dte.ToString("s") + "," +
			//"\"" + com + "\"";
	}

};

int main0()
{

	printf("Start P01_PlatformPreperties\n");

	// Два варианта массива
	auto netarray = gcnew array<long long>(10); // Это инициализация и вычисление типа
	long long *arr = NULL;

	char *sol;
	int siz;
	//int nte;

	siz = 50000000; //00;
	//nte = 10000000;
	auto randGen = gcnew Random();

	auto stopWatch = Stopwatch::StartNew();
	bool dotnet = true;
	if (dotnet)
	{
		netarray = gcnew array<long long>(siz);
		for (int i = 0; i < siz; i++)
		{
			int randNum = randGen->Next();
			netarray[i] = randNum; //siz - i;
		}
		stopWatch = Stopwatch::StartNew();
		Array::Sort(netarray);
		printf("System.Sort "); // 6.5 сек. на 50 млн.
	}
	else 
	{
		arr = new long long[siz];
		for (int i = 0; i < siz; i++)
		{
			int randNum = randGen->Next();
			arr[i] = randNum; // siz - i;
		}
		stopWatch = Stopwatch::StartNew();
		std::sort(arr, &arr[siz - 1]);
		printf("std.sort "); // 32-34 сек на 50 млн.
	}
	stopWatch->Stop();
	printf("siz=%d duration = %d\n", siz, stopWatch->ElapsedMilliseconds);

	if (dotnet)
	{
		long long ssum = 0;
		System::Int64 dsum = 0;
		stopWatch = Stopwatch::StartNew();
		for (long long ii = 0; ii < siz; ii++)
		{
			//ssum += netarray[ii];
			dsum += netarray[ii];
		}
		stopWatch->Stop();
		//printf("Scan %lld ms, sum = %lld\n", stopWatch->ElapsedMilliseconds, ssum); // 127 ms для 50 млн
		System::Console::WriteLine("Scan duration {0}, sum={1}", stopWatch->ElapsedMilliseconds, dsum);
	}
	else 
	{
		long long ssum = 0;
		stopWatch = Stopwatch::StartNew();
		long long *p = arr;
		//for (int ii = 0; ii < siz; ii++) { ssum += *(p++); }
		long long *p1 = p + siz;
		while (p != p1) ssum += *(p++);
		stopWatch->Stop();
		printf("Scan %d ms, sum = %lld\n", stopWatch->ElapsedMilliseconds, ssum); // 108 ms на 50 млн.
	}

	return 0;
}



