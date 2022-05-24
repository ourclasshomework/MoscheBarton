Public Class Form1
    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        Label1.Text = Str(e.X) + ", " + Str(e.Y)
        PictureBox1.Left = e.X
        PictureBox1.Top = e.Y
    End Sub
End Class
