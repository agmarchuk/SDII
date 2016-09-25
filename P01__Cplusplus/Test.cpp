#include "stdafx.h"
#include <iostream>
#include <vector>
#include <algorithm>
#include <time.h>
#define NMAX 50000000

using namespace std;
_int64 arr2[NMAX];
vector <int> vec2(NMAX);
int comp2(_int64 a, _int64 b)
{
	return a - b;
}
int main1()
{
	clock_t start, finish;

	for (int i = 0; i < NMAX; i++) arr2[i] = NMAX - i;

	start = clock();
	sort(arr2, &arr2[NMAX-1]);
	finish = clock();
	//сортирую весь массив
	printf("sorted arr\n");
	printf("duration = %d\n", (finish - start));

	start = clock();
	//sort(vec2.begin(), vec2.end());
	std::sort(vec2.begin(), vec2.end(), [](int x, int y) { return x>y; });
	finish = clock();
	//сортирую весь вектор
	printf("sorted vec\n");
	printf("duration = %d\n", (finish - start));

	start = clock();
	//sort(arr2, &arr2[NMAX-1], comp2);
	sort(arr2, &arr2[NMAX - 1]);
	finish = clock();
	//сортирую массив по компаратору
	printf("sorted arr with comp\n");
	printf("duration = %d\n", (finish - start));


	return 0;
}