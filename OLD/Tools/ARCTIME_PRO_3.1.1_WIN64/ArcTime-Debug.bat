@echo off
set javapath=\tools\jre1.8\bin\java.exe
echo --------------------------
echo �ð汾�Ѱ�����Java��VLC���������������а�װ
echo --------------------------
echo ���������Java�汾��Ϣ��
echo.
echo Path=%cd%%javapath%
"%cd%%javapath%" -version
echo --------------------------
echo.
echo ��������Arctime...
"%cd%%javapath%" -Xms256m -Xmx4800m -jar ArcTime.jar
echo.
echo --------------------------
echo �����ѽ������У����Arctime���������г������쳣����콫����Ļ��ͼ���ϱ���
echo --------------------------
pause