Public Class Form1
    Structure MyPicBox
        Dim Pic As Bitmap
        Dim Box As RectangleF
        Dim angle As Integer
        Dim isPressed As Boolean
        Dim isActivated As Boolean
    End Structure

    Dim ScaleRatio As Integer = 1

    Dim Keyboard(256) As Boolean
    Dim MyWidth As Integer = 800 * ScaleRatio
    Dim MyHeight As Integer = 450 * ScaleRatio


    Dim CameraBoarderSizeX As Integer = 400
    Dim CameraBoarderSizeY As Integer = 200
    Dim CameraBoarder As New RectangleF(CameraBoarderSizeX * ScaleRatio, CameraBoarderSizeY * ScaleRatio, MyWidth - CameraBoarderSizeX * 2 * ScaleRatio, MyHeight - CameraBoarderSizeY * 2 * ScaleRatio)
    Dim Map As New MyPicBox With {.Pic = My.Resources.Game.Map, .Box = New RectangleF(-800 * ScaleRatio, -500 * ScaleRatio, 1690 * ScaleRatio, 1151 * ScaleRatio)}
    Dim MapBoarder As New RectangleF(50 * ScaleRatio, 50 * ScaleRatio, MyWidth - 50 * 2 * ScaleRatio, MyHeight - 50 * 2 * ScaleRatio)


    Dim Player As New MyPicBox With {.Pic = My.Resources.Game.Character, .Box = New RectangleF(400 * ScaleRatio, 200 * ScaleRatio, 64 * ScaleRatio, 64 * ScaleRatio)}
    Dim CharacterSpeed As Integer = 8
    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        e.Graphics.DrawImage(Map.Pic, Map.Box)

        e.Graphics.TranslateTransform(Player.Box.X + Player.Box.Width / 2, Player.Box.Y + Player.Box.Width / 2)
        e.Graphics.RotateTransform(Player.angle)
        e.Graphics.TranslateTransform(-Player.Box.X - Player.Box.Width / 2, -Player.Box.Y - Player.Box.Width / 2)
        e.Graphics.DrawImage(Player.Pic, Player.Box)

        e.Graphics.ResetTransform()


    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick



        If Keyboard(Keys.W) Then
            Player.angle = 0
            If Player.Box.Y <= CameraBoarder.Y Then
                If Map.Box.Y >= 0 Then
                    Map.Box.Y = 0
                    If Player.Box.Y <= MapBoarder.Y Then
                        Player.Box.Y = MapBoarder.Y
                    Else
                        Player.Box.Y -= CharacterSpeed * ScaleRatio
                    End If
                Else
                        Map.Box.Y += CharacterSpeed * ScaleRatio
                End If
            Else
                Player.Box.Y -= CharacterSpeed * ScaleRatio
            End If
        End If
        If Keyboard(Keys.A) Then
            Player.angle = 270
            If Player.Box.X <= CameraBoarder.X Then
                If Map.Box.X >= 0 Then
                    Map.Box.X = 0
                    If Player.Box.X <= MapBoarder.X Then
                        Player.Box.X = MapBoarder.X
                    Else
                        Player.Box.X -= CharacterSpeed * ScaleRatio
                    End If
                Else
                    Map.Box.X += CharacterSpeed * ScaleRatio
                End If
            Else
                Player.Box.X -= CharacterSpeed * ScaleRatio
            End If
        End If
        If Keyboard(Keys.S) Then
            Player.angle = 180
            If Player.Box.Y + Player.Box.Height >= CameraBoarder.Y + CameraBoarder.Height Then
                If Map.Box.Y + Map.Box.Height <= MyHeight Then
                    Map.Box.Y = MyHeight - Map.Box.Height
                    If Player.Box.Y + Player.Box.Height >= MapBoarder.Y + MapBoarder.Height Then
                        Player.Box.Y = MapBoarder.Y + MapBoarder.Height - Player.Box.Height
                    Else
                        Player.Box.Y += CharacterSpeed * ScaleRatio
                    End If
                Else
                    Map.Box.Y -= CharacterSpeed * ScaleRatio
                End If
            Else
                Player.Box.Y += CharacterSpeed * ScaleRatio
            End If
        End If
        If Keyboard(Keys.D) Then
            Player.angle = 90
            If Player.Box.X + Player.Box.Width >= CameraBoarder.X + CameraBoarder.Width Then
                If Map.Box.X + Map.Box.Width <= MyWidth Then
                    Map.Box.X = MyWidth - Map.Box.Width
                    If Player.Box.X + Player.Box.Width >= MapBoarder.X + MapBoarder.Width Then
                        Player.Box.X = MapBoarder.X + MapBoarder.Width - Player.Box.Width
                    Else
                        Player.Box.X += CharacterSpeed * ScaleRatio
                    End If
                Else
                    Map.Box.X -= CharacterSpeed * ScaleRatio
                End If
            Else
                Player.Box.X += CharacterSpeed * ScaleRatio
            End If

        End If
        If Keyboard(Keys.W) And Keyboard(Keys.A) Then
            Player.angle = 315
        ElseIf Keyboard(Keys.W) And Keyboard(Keys.D) Then
            Player.angle = 45
        ElseIf Keyboard(Keys.S) And Keyboard(Keys.A) Then
            Player.angle = 225
        ElseIf Keyboard(Keys.S) And Keyboard(Keys.D) Then
            Player.angle = 135
        End If
        Me.Invalidate()
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Keyboard(e.KeyValue) = True
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        Keyboard(e.KeyValue) = False
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For i = 0 To 255
            Keyboard(i) = False
        Next

    End Sub
End Class

