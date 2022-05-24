#include <iostream>
#include <string>

using namespace std;

int main()
{
    string str;
    string res = "Dim IntroText() As String = {";
    string start;

    cout << "圖片序列轉VB的圖片陣列機\n";
    cout << "請輸入開頭: ";
    cin >> start;
    cout << "請輸入序列，輸入完畢後請輸入 ###\n";

    while (getline(cin, str) && str != "###"){
        res += start + str + ", ";
    }
    res[res.size()-1] = ' ';
    res[res.size()-2] = '}';

    cout << "\n\n\n##########\n" << res << "\n\n";

    system("PAUSE");

    return 0;
}
