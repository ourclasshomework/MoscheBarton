#include <iostream>
#include <string>

using namespace std;

int main()
{
    string str;
    string res = "Dim IntroText() As String = {";

    cout << "文本轉VB的字串陣列機\n";
    cout << "請輸入文本，輸入完畢後請輸入 ###\n";

    while (getline(cin, str) && str != "###"){
        res += "\"" + str + "\", ";
    }
    res[res.size()-1] = ' ';
    res[res.size()-2] = '}';

    cout << "\n\n\n##########\n" << res << "\n\n";

    system("PAUSE");

    return 0;
}
