#include <iostream>
#include <string>

using namespace std;

int main()
{
    int head, HH, MM, SS ,mmm, StartTime, EndTime, d = 0;
    string arrow;
    char sep;
    string text;
    string TimeCodeStart = "Dim TimeCodeStart As Integer() = {";
    string TimeCodeEnd = "Dim TimeCodeEnd As Integer() = {";

    cout << "SRT��VB�ʵe�ɶ��X��\n";
    cout << "�п�J����}�l�ɶ�: ";
    cin >> d;
    cout << "�п�JSRT�ɮפ��e�A��J������п�J -1\n\n";

    while (cin >> head &&head != -1){
        cin >> HH >> sep >> MM >> sep >> SS >> sep >> mmm >> arrow;
        StartTime = HH*60*60*1000 + MM*60*1000 + SS*1000 + mmm + d;
        cin >> HH >> sep >> MM >> sep >> SS >> sep >> mmm >> text;
        EndTime = HH*60*60*1000 + MM*60*1000 + SS*1000 + mmm + d;
        TimeCodeStart += to_string(StartTime) + ", ";
        TimeCodeEnd += to_string(EndTime) + ", ";
    }

    TimeCodeStart[TimeCodeStart.size()-1] = ' ';
    TimeCodeStart[TimeCodeStart.size()-2] = '}';
    TimeCodeEnd[TimeCodeEnd.size()-1] = ' ';
    TimeCodeEnd[TimeCodeEnd.size()-2] = '}';

    cout << "##############\n" << TimeCodeStart << "\n##############\n" << TimeCodeEnd;

    system("PAUSE");

    return 0;
}

/*
Case 0 To 1000
    ShowTextLen = IntroText(0).Length
    If TextIndex < IntroText(0).Length Then
        If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
            ShowText += IntroText(0)(TextIndex)
            TextIndex += 1
        End If
    End If
*/
