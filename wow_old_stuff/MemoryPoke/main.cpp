#include <iostream>

using namespace std;

int main()
{
    int * ptr = (int*) 0x003fa000;
    int x = 23;
    ptr = &x;
    cout << ptr;
    return 0;
}
