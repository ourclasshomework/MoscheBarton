Public Class Form1
    Dim point1 As New Point(100, 100)
    Dim point2 As New Point(10, 150)
    Dim pen As New Pen(Color.Black, 3)
    Dim rectX As Integer = 0
    Dim rectY As Integer = 0
    Dim rect As New Rectangle(rectX, rectY, 20, 20)

    Dim KeyPressed() As Boolean = {False, False, False, False}
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Label1.Text = e.KeyValue
        Select Case e.KeyValue
            Case Keys.W
                KeyPressed(0) = True
                Timer1.Enabled = True
            Case Keys.S
                KeyPressed(1) = True
                Timer1.Enabled = True
            Case Keys.A
                KeyPressed(2) = True
                Timer1.Enabled = True
            Case Keys.D
                KeyPressed(3) = True
                Timer1.Enabled = True
        End Select
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        Select Case e.KeyValue
            Case Keys.W
                KeyPressed(0) = False
            Case Keys.S
                KeyPressed(1) = False
            Case Keys.A
                KeyPressed(2) = False
            Case Keys.D
                KeyPressed(3) = False
        End Select
        If KeyPressed(0) + KeyPressed(1) + KeyPressed(2) + KeyPressed(3) = 0 Then
            Timer1.Enabled = False
            Label2.Text = "Timer1 End"
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label2.Text = "Timer1 Start"
        If KeyPressed(0) Then
            rectY -= 1
        End If
        If KeyPressed(1) Then
            rectY += 1
        End If
        If KeyPressed(2) Then
            rectX -= 1
        End If
        If KeyPressed(3) Then
            rectX += 1
        End If
        Me.Invalidate()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.DoubleBuffered = True
    End Sub

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        Dim g As Graphics = e.Graphics
        g.DrawLine(pen, point1, point2)
        rect = New Rectangle(rectX, rectY, 20, 20)
        g.DrawEllipse(pen, rect)
    End Sub
End Class
