Public Class Form1
    Structure MyTexture
        Dim Pic As Bitmap
        Dim Box As RectangleF
        Dim OriginalBox As RectangleF
    End Structure

    Structure Character
        Dim Pic As Bitmap
        Dim Box As RectangleF
        Dim angle As Integer
    End Structure

    Dim ScaleRatio As Integer = 1

    Dim Keyboard(256) As Boolean
    Dim MyWidth As Integer = 800
    Dim MyHeight As Integer = 450

    Dim Map As Integer(,) = {{1, 1, 1, 1, 1, 1, 1, 1},
                             {0, 0, 1, 0, 0, 1, 0, 1},
                             {0, 1, 0, 0, 1, 0, 1, 0},
                             {0, 0, 1, 1, 0, 0, 1, 1},
                             {1, 1, 1, 1, 1, 1, 1, 1},
                             {0, 0, 1, 0, 0, 1, 0, 1}}
    Dim MapX As Integer = 6
    Dim MapY As Integer = 8

    Dim CameraFar As Double = 1
    Dim CameraShouldMoveTop As Boolean = False
    Dim CameraShouldMoveBottom As Boolean = False
    Dim CameraShouldMoveLeft As Boolean = False
    Dim CameraShouldMoveRight As Boolean = False
    Dim CameraBorder As Rectangle
    Dim CameraBorderWidth As Integer = 60

    Dim Texture As Bitmap() = {My.Resources.Game._0, My.Resources.Game._1}

    Dim BlockSize As Integer = 128
    Dim Block As New MyTexture With {.Pic = My.Resources.Game._1, .Box = New Rectangle(0, 0, BlockSize * ScaleRatio, BlockSize * ScaleRatio), .OriginalBox = New Rectangle(0, 0, BlockSize * ScaleRatio, BlockSize * ScaleRatio)}

    Dim Player As New Character With {.Pic = My.Resources.Game.Character, .Box = New RectangleF(128, 128, 64, 64)}
    Dim CharacterSpeed As Integer = 8
    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        For x = 0 To MapX - 1
            For y = 0 To MapY - 1
                Block.Pic = Texture(Map(x, y))
                e.Graphics.DrawImage(Block.Pic, Block.Box)
                Block.Box.X += Block.Box.Width
            Next
            Block.Box.X -= Block.Box.Height * MapX
            Block.Box.Y += Block.Box.Width
        Next
        Block.Box = Block.OriginalBox

        e.Graphics.DrawImage(Block.Pic, Block.Box)

        e.Graphics.TranslateTransform(Player.Box.X + Player.Box.Width / 2, Player.Box.Y + Player.Box.Width / 2)
        e.Graphics.RotateTransform(Player.angle)
        e.Graphics.TranslateTransform(-Player.Box.X - Player.Box.Width / 2, -Player.Box.Y - Player.Box.Width / 2)
        e.Graphics.DrawImage(Player.Pic, Player.Box)

        e.Graphics.ResetTransform()


    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        CameraBorder = New Rectangle(CameraBorderWidth, CameraBorderWidth, MyWidth - CameraBorderWidth - Player.Box.Width, MyHeight - CameraBorderWidth - Player.Box.Height)
        If CameraBorder.Y <= Player.Box.Y Then
            CameraShouldMoveTop = False
        Else
            CameraShouldMoveTop = True
        End If
        If Player.Box.Y + Player.Box.Height <= CameraBorder.Y + CameraBorder.Height Then
            CameraShouldMoveBottom = False
        Else
            CameraShouldMoveBottom = True
        End If
        If CameraBorder.X <= Player.Box.X Then
            CameraShouldMoveLeft = False
        Else
            CameraShouldMoveLeft = True
        End If
        If Player.Box.X + Player.Box.Width <= CameraBorder.X + CameraBorder.Width Then
            CameraShouldMoveRight = False
        Else
            CameraShouldMoveRight = True
        End If


        If Keyboard(Keys.W) Then
            Player.angle = 0
            If CameraShouldMoveTop Then
                If Block.OriginalBox.Y >= 0 Then
                    Block.OriginalBox.Y -= CharacterSpeed
                End If
            Else
                Player.Box.Y -= CharacterSpeed
            End If
        End If
        If Keyboard(Keys.A) Then
            Player.angle = 270
            If CameraShouldMoveLeft Then
                If Block.OriginalBox.X >= 0 Then
                    Block.OriginalBox.X -= CharacterSpeed
                End If
            Else
                Player.Box.X -= CharacterSpeed
            End If
        End If
        If Keyboard(Keys.S) Then
            Player.angle = 180
            If CameraShouldMoveBottom Then
                If Block.OriginalBox.Y + BlockSize * ScaleRatio * MapY <= MyHeight Then
                    Block.OriginalBox.Y += CharacterSpeed
                End If
            Else
                Player.Box.Y += CharacterSpeed
            End If
        End If
        If Keyboard(Keys.D) Then
            Player.angle = 90
            If CameraShouldMoveRight Then
                If Block.OriginalBox.X + BlockSize * ScaleRatio * MapX <= MyWidth Then
                    Block.OriginalBox.X += CharacterSpeed
                End If
            Else
                Player.Box.X += CharacterSpeed
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

