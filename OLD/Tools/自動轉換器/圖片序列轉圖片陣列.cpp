#include <iostream>
#include <string>

using namespace std;

int main()
{
    string str;
    string res = "Dim IntroText() As String = {";
    string start;

    cout << "�Ϥ��ǦC��VB���Ϥ��}�C��\n";
    cout << "�п�J�}�Y: ";
    cin >> start;
    cout << "�п�J�ǦC�A��J������п�J ###\n";

    while (getline(cin, str) && str != "###"){
        res += start + str + ", ";
    }
    res[res.size()-1] = ' ';
    res[res.size()-2] = '}';

    cout << "\n\n\n##########\n" << res << "\n\n";

    system("PAUSE");

    return 0;
}
