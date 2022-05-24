Imports System.Timers

Public Class Form1
    Dim lastTime As Long
    Dim ms As Long
    Dim txt As String
    Dim tRefresh As New Timer(1)
    Dim myFont As New Font("Cubic 11", 20)
    Dim myBrush As New SolidBrush(Color.Black)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tRefresh.SynchronizingObject = Me
        AddHandler tRefresh.Elapsed, AddressOf tRefresh_Tick
        tRefresh.AutoReset = True
        tRefresh.Enabled = True
    End Sub

    Private Sub tRefresh_Tick(source As Object, e As ElapsedEventArgs)
        ms = Now.Ticks / 10000 - lastTime
        txt = "Framrate " + Str(Math.Round(1 / ms * 1000, 2)) + vbNewLine + "Framtime " + Str(ms)
        lastTime = Now.Ticks / 10000
        Me.Invalidate()
    End Sub

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        e.Graphics.DrawString(txt, myFont, myBrush, 100.0, 100.0)
    End Sub
End Class
