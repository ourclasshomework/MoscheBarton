#include <iostream>
#include <string>

using namespace std;

int main()
{
    string str;
    string res = "Dim IntroText() As String = {";

    cout << "�奻��VB���r��}�C��\n";
    cout << "�п�J�奻�A��J������п�J ###\n";

    while (getline(cin, str) && str != "###"){
        res += "\"" + str + "\", ";
    }
    res[res.size()-1] = ' ';
    res[res.size()-2] = '}';

    cout << "\n\n\n##########\n" << res << "\n\n";

    system("PAUSE");

    return 0;
}
