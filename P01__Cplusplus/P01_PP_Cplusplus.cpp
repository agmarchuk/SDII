// P01_PP_Cplusplus.cpp: определ€ет точку входа дл€ консольного приложени€.
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
//#define NMAX 4000000
//using namespace std;
//int arr[NMAX];
//vector <int> vec(NMAX);
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
	// —охранение теста в CSV-строке
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

int main()
{

	printf("Start P01_PlatformPreperties\n");


	char *sol;
	int siz;
	//int nte;

	siz = 50000000; //00;
	//nte = 10000000;

		long long *arr;
	arr = new long long[siz];
	auto netarray = gcnew array<long long>(siz);
	for (int i = 0; i < siz; i++) arr[i] = netarray[i]= siz - i;
	printf("Sort\n");
	auto stopWatch = Stopwatch::StartNew();
	std::sort(arr, &arr[siz - 1]);
	stopWatch->Stop();
	printf("std duration = %d\n", stopWatch->ElapsedMilliseconds);

	stopWatch = Stopwatch::StartNew();	
	//qsort(arr, siz, sizeof(long long), compare);
	//сортирую весь массив
	Array::Sort(netarray);
	stopWatch->Stop();
	printf(".net duration = %d\n", stopWatch->ElapsedMilliseconds);
	
	return 0;
}



