// P01_PP_Cplusplus.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include <stdio.h>
#include <time.h>
#include <sys/timeb.h>
#include <algorithm>
#include <vector>
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
	clock_t start, finish;
	printf("Start P01_PlatformPreperties\n");

	ProbeFrame probe;



	struct _timeb timebuffer;
	char timeline[26];
	errno_t err;
	time_t time1;

	_ftime_s(&timebuffer);

	time1 = timebuffer.time;
	printf("Seconds since midnight, January 1, 1970 (UTC): %I64d\n", time1);

	err = ctime_s(timeline, 26, &(timebuffer.time));
	if (err)
	{
		printf("Invalid argument to ctime_s. ");
	}
	//printf("The time is %.19s.%hu %s", timeline, timebuffer.millitm,
	//	&timeline[20]);
	printf("The time is %s", &timeline[0]);

	start = clock();

	char *sol;
	int siz;
	int nte;

	siz = 50000000; //00;
	nte = 10000000;
	long long *arr;
	arr = new long long[siz];
	for (int i = 0; i < siz; i++) arr[i] = siz - i;
	finish = clock();
	printf("duration = %d\n", (finish - start));

	start = clock();
	std::sort(arr, &arr[siz - 1]);
	//qsort(arr, siz, sizeof(long long), compare);
	//сортирую весь массив
	finish = clock();
	printf("duration = %d\n", (finish - start));

	//start = clock();
	//sort(arr, arr + NMAX, comp);
	////сортирую массив по компаратору
	//finish = clock();
	//printf("duration = %d\n", (finish - start));

	return 0;
}



