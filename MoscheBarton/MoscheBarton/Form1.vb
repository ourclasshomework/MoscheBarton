Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Runtime.InteropServices
Imports System.Timers

Public Class Form1
    Public Class MyTextBox
        Public font As String
        Public font_size As Double
        Private DrawFont As Font
        Public color As Color
        Public opacity As Double = 255.0
        Public point As PointF
        Private format As New StringFormat
        Public Align As String = "Left"
        Public LineAlign As String = "Top"

        Public Sub AlignSetting(alignment As String, linealignment As String)
            Align = alignment
            LineAlign = linealignment
        End Sub

        Public Sub Draw(Text As String, e As PaintEventArgs, myfont As PrivateFontCollection, ScaleRatio As Double)
            Select Case font
                Case "Cubic11"
                    DrawFont = New Font(myfont.Families(1), font_size * ScaleRatio)
                Case "Monoton"
                    DrawFont = New Font(myfont.Families(0), font_size * ScaleRatio)
                Case Else
                    If font = Nothing Then
                        DrawFont = New Font("微軟正黑體", font_size * ScaleRatio)
                    Else
                        DrawFont = New Font(font, font_size * ScaleRatio)
                    End If
            End Select

            Select Case Align
                Case "Left"
                    format.Alignment = StringAlignment.Near
                Case "Center"
                    format.Alignment = StringAlignment.Center
                Case "Right"
                    format.Alignment = StringAlignment.Far
            End Select

            Select Case LineAlign
                Case "Top"
                    format.LineAlignment = StringAlignment.Near
                Case "Center"
                    format.LineAlignment = StringAlignment.Center
                Case "Bottom"
                    format.LineAlignment = StringAlignment.Far
            End Select

            If opacity >= 255.0 Then
                opacity = 255.0
            ElseIf opacity <= 0.0 Then
                opacity = 0.0
            End If

            e.Graphics.DrawString(Text, DrawFont, New SolidBrush(Color.FromArgb(opacity, color)), point, format)
        End Sub
    End Class

    Public Class MyPictureBox
        Public Box As RectangleF
        Public Image As Bitmap

        Public Sub BoxSetting(x1 As Single, y1 As Single, Height As Single, Width As Single)
            Box = New RectangleF(x1, y1, Height, Width)
        End Sub

        Public Sub Draw(e As PaintEventArgs)
            e.Graphics.DrawImage(Image, Box)
        End Sub
    End Class

    Public Class MyButton
        Public Box As RectangleF
        Public Image As Bitmap
        Public PressedImage As Bitmap
        Public Activated As Boolean = False

        Public Function Draw(e As PaintEventArgs, Mouse As Point, MousePressed As Boolean, ByRef Player As WMPLib.WindowsMediaPlayer, Volume As Double) As Integer
            If Box.X <= Mouse.X And Mouse.X <= Box.X + Box.Width And Box.Y <= Mouse.Y And Mouse.Y <= Box.Y + Box.Height Then
                If MousePressed Then
                    Activated = True
                    e.Graphics.DrawImage(PressedImage, Box)
                    Return 1
                Else
                    e.Graphics.DrawImage(Image, Box)
                    If Activated Then
                        Activated = False
                        Player.URL = My.Application.Info.DirectoryPath & "\Music\Click.wav"
                        Player.settings.volume = Volume
                        Player.controls.play()
                        Return 3
                    Else
                        Return 0
                    End If
                End If
            Else
                If MousePressed Then
                    If Activated Then
                        Activated = True
                        e.Graphics.DrawImage(PressedImage, Box)
                        Return 2
                    Else
                        e.Graphics.DrawImage(Image, Box)
                        Return 0
                    End If
                Else
                    Activated = False
                    e.Graphics.DrawImage(Image, Box)
                    Return 0
                End If
            End If

            e.Graphics.DrawImage(Image, Box)
            Return 0
        End Function
    End Class

    Public Sub PlaySound(ByRef Player As WMPLib.WindowsMediaPlayer, File As String, ShouldLoop As Boolean)
        Player.URL = My.Application.Info.DirectoryPath & "\Music\" & File
        Player.settings.setMode("loop", ShouldLoop)
        Player.controls.play()
    End Sub

    Public Sub WMPExit(ByRef Player As WMPLib.WindowsMediaPlayer)
        Player.controls.stop()
        Player.close()
    End Sub

    Public Sub DrawGame(e As PaintEventArgs, ByRef Map(,) As Integer, x As Double, y As Double, MapWidth As Integer, MapHeight As Integer, ScaleRatio As Double, ByRef Character As MyPictureBox)
        Dim image As Bitmap = My.Resources.Game.Cobblestone
        Dim Box As RectangleF
        Dim dx As Double = 0
        Dim dy As Double = 0

        For i = 0 To MapHeight - 1
            For j = 0 To MapWidth - 1
                Select Case Map(i, j)
                    Case 1
                        image = My.Resources.Game.Cobblestone
                End Select
                If Not (Map(i, j) = 0) Then
                    Box = New RectangleF(x + dx, y + dy, 48 * ScaleRatio, 48 * ScaleRatio)
                    e.Graphics.DrawImage(image, Box)
                    dx += 48 * ScaleRatio
                Else
                    dx += 48 * ScaleRatio
                End If
            Next
            dy += 48 * ScaleRatio
            dx = 0
        Next

        If Keyboard(Keys.A) Then
            Character.Box.X -= 1 * ScaleRatio
        ElseIf Keyboard(Keys.D) Then
            Character.Box.X += 1 * ScaleRatio
        End If
        Character.Draw(e)
    End Sub

    Public Class Slider
        Public X1 As Double
        Public X2 As Double
        Public Y As Double
        Private HeadX As Double
        Private Activated As Boolean = False
        Public value As Integer = 100
        Public color As Color = Color.White
        Public Activated_color As Color = Color.FromArgb(157, 157, 157)
        Public width As Double = 4
        Public head_width As Double = width * 3.6

        Public Name As New MyTextBox With {.LineAlign = "Center", .color = color}
        Public NameText As String
        Public ValueText As New MyTextBox With {.LineAlign = "Center", .color = color}

        Public Sub NameSetting(Text As String, font As String, font_size As String, xpt1 As Double, ypt1 As Double)
            NameText = Text
            Name.font = font
            Name.font_size = font_size
            Name.point.X = xpt1
            Name.point.Y = ypt1
        End Sub

        Public Sub ValueTextSetting(font As String, font_size As String, xpt1 As Double, ypt1 As Double)
            ValueText.font = font
            ValueText.font_size = font_size
            ValueText.point.X = xpt1
            ValueText.point.Y = ypt1
        End Sub

        Public Sub Setting(xpt1 As Double, xpt2 As Double, ypt12 As Double)
            X1 = xpt1
            X2 = xpt2
            Y = ypt12
        End Sub

        Public Sub Draw(e As PaintEventArgs, myfont As PrivateFontCollection, ScaleRatio As Double, ByRef Player As WMPLib.WindowsMediaPlayer, Mouse As Point, MousePressed As Boolean)
            e.Graphics.DrawLine(New Pen(color, width), New PointF(X1, Y), New PointF(X2, Y))
            If X1 + (X2 - X1) / 100 * value - head_width / 2 <= Mouse.X And Mouse.X <= X1 + (X2 - X1) / 100 * value + head_width / 2 And Y - head_width / 2 <= Mouse.Y And Mouse.Y <= Y + head_width / 2 And MousePressed Then
                Activated = True
                HeadX = Mouse.X
                If HeadX > X2 Then
                    HeadX = X2
                End If
                If HeadX < X1 Then
                    HeadX = X1
                End If
                value = (Mouse.X - X1) / (X2 - X1) * 100

                If value > 100 Then
                    value = 100
                End If
                If value < 0 Then
                    value = 0
                End If

                e.Graphics.FillEllipse(New SolidBrush(Activated_color), New RectangleF(HeadX - head_width * 0.8 / 2, Y - head_width * 0.8 / 2, head_width * 0.8, head_width * 0.8))
            ElseIf Activated And MousePressed Then
                HeadX = Mouse.X
                If HeadX > X2 Then
                    HeadX = X2
                End If
                If HeadX < X1 Then
                    HeadX = X1
                End If
                value = (Mouse.X - X1) / (X2 - X1) * 100

                If value > 100 Then
                    value = 100
                End If
                If value < 0 Then
                    value = 0
                End If
                e.Graphics.FillEllipse(New SolidBrush(Activated_color), New RectangleF(HeadX - head_width * 0.8 / 2, Y - head_width * 0.8 / 2, head_width * 0.8, head_width * 0.8))
            ElseIf X1 <= Mouse.X And Mouse.X <= X2 And Y - head_width / 2 <= Mouse.Y And Mouse.Y <= Y + head_width / 2 And MousePressed Then
                HeadX = Mouse.X
                If HeadX > X2 Then
                    HeadX = X2
                End If
                If HeadX < X1 Then
                    HeadX = X1
                End If
                value = (Mouse.X - X1) / (X2 - X1) * 100

                If value > 100 Then
                    value = 100
                End If
                If value < 0 Then
                    value = 0
                End If
                e.Graphics.FillEllipse(New SolidBrush(Activated_color), New RectangleF(HeadX - head_width * 0.8 / 2, Y - head_width * 0.8 / 2, head_width * 0.8, head_width * 0.8))
            ElseIf Activated And MousePressed = False Then
                Player.settings.volume = value
                Player.settings.setMode("loop", False)
                Player.URL = My.Application.Info.DirectoryPath & "\Music\Click.wav"



                e.Graphics.FillEllipse(New SolidBrush(color), New RectangleF(X1 + (X2 - X1) / 100 * value - head_width / 2, Y - head_width / 2, head_width, head_width))
                Activated = False
            Else
                e.Graphics.FillEllipse(New SolidBrush(color), New RectangleF(X1 + (X2 - X1) / 100 * value - head_width / 2, Y - head_width / 2, head_width, head_width))
                Activated = False
            End If

            Name.Draw(NameText, e, myfont, ScaleRatio)
            ValueText.Draw(CStr(value), e, myfont, ScaleRatio)
        End Sub
    End Class

    '快速調整設定區
    Dim State As String = "Start"

    Const Version As String = "Insider Preview 1.0"
    Const Copyright As String = "III Studio 製作"

    Const DefaultWidth As Integer = 800 '預設的寬度
    Const DefaultHeight As Integer = 450 '預設的高度
    Const DefaultDebugPanelOn As Boolean = True '預設除錯面板開啟狀態

    Const OpeningDimSpeed As Double = 7.5 '開場動畫淡入淡出的速度
    Const OpeningGapSpeed As Double = 1000 '開場動畫淡入淡出的速度 (ms)
    Const OpeningSpeed As Integer = 2000 '每個開場動畫持續的時間 (ms)

    '初始化Timer
    Dim ScreenRefresh As New Timer(1)

    '字體初始化
    Dim myfont As New PrivateFontCollection
    Dim bytes As Byte()
    Dim ptr As IntPtr

    '螢幕縮放比例初始化
    Dim MyWidth As Integer = DefaultWidth
    Dim MyHeight As Integer = DefaultHeight
    Dim ScaleRatio As Double = 1.0

    '鍵盤初始化
    Dim Keyboard(256) As Boolean

    '滑鼠初始化
    Dim Mouse As New Point(0, 0)
    Dim MousePressed As Boolean = False

    '音樂初始化
    Dim BGM As New WMPLib.WindowsMediaPlayer
    Dim SoundEffect As New WMPLib.WindowsMediaPlayer
    Dim Ding As New WMPLib.WindowsMediaPlayer

    'DebugPanel 初始化
    Dim DebugPanelOn As Boolean = DefaultDebugPanelOn
    Dim DebugInfo As New MyTextBox With {.font = "Consolas", .font_size = 12, .color = Color.Yellow}
    Dim lastUpdate As Long
    Dim FPS As Double
    Dim Frametime As Double

    '背景初始化
    Dim BGisOn As Boolean = True
    Dim BGColorA As Double = 0
    Dim BGColors As Color() = {Color.FromArgb(35, 35, 35), Color.FromArgb(0, 0, 0), Color.FromArgb(35, 35, 35)}
    Dim BGColorPosition As Single() = {0.0F, 0.5F, 1.0F}
    Dim BGColorBlend As New ColorBlend
    Dim BGBrush As New LinearGradientBrush(New Point(0, 0), New Point(100, 500), Color.Black, Color.White)
    Dim BGAnimation As Single = 0

    '開場動畫計時器
    Dim OpeningStartTime As Long

    'StudioLogo 初始化
    Dim StudioLogo As New MyTextBox With {.font = "Monoton", .font_size = 50, .color = Color.White, .opacity = 0, .Align = "Center", .LineAlign = "Center"}

    'GameLogo 初始化
    Dim GameLogo As New MyTextBox With {.font = "Cubic11", .font_size = 50, .color = Color.White, .opacity = 0, .Align = "Center", .LineAlign = "Center"}
    Dim GameSubLogo As New MyTextBox With {.font = "Cubic11", .font_size = 30.9, .color = Color.FromArgb(157, 157, 157), .opacity = 0, .Align = "Center", .LineAlign = "Center"}

    'Menu 初始化
    Dim MenuCaption As New MyPictureBox With {.Image = My.Resources.Menu.Caption}
    Dim VersionInfo As New MyTextBox With {.font = "Cubic11", .font_size = 8.5, .color = Color.FromArgb(0, 157, 157, 157), .Align = "Left", .LineAlign = "Bottom"}
    Dim CopyrightInfo As New MyTextBox With {.font = "Cubic11", .font_size = 8.5, .color = Color.FromArgb(0, 157, 157, 157), .Align = "Right", .LineAlign = "Bottom"}

    Dim StartButton As New MyButton With {.Image = My.Resources.Menu.Start, .PressedImage = My.Resources.Menu.Start_Pressed}
    Dim HowToPlayButton As New MyButton With {.Image = My.Resources.Menu.HowToPlay, .PressedImage = My.Resources.Menu.HowToPlay_Pressed}
    Dim SettingButton As New MyButton With {.Image = My.Resources.Menu.Setting, .PressedImage = My.Resources.Menu.Setting_Pressed}
    Dim ExitButton As New MyButton With {.Image = My.Resources.Menu._Exit, .PressedImage = My.Resources.Menu.Exit_Pressed}

    'HowToPlay1 初始化
    Dim NextPageButton As New MyButton With {.Image = My.Resources.HowToPlay.NextPage, .PressedImage = My.Resources.HowToPlay.NextPage_Pressed}
    Dim HowToPlay_Text As New MyTextBox With {.font = "Cubic11", .font_size = 21.5, .color = Color.Black}
    Dim HowToPlay_Img As New MyPictureBox With {.Image = My.Resources.HowToPlay.HowToPlay1}
    Dim HowToPlay1_Map(,) As Integer = {{1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1},
                                        {1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1},
                                        {1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1},
                                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}
    Dim HowToPlay1_MapWidth As Integer = 15
    Dim HowToPlay1_MapHeight As Integer = 6
    Dim Sky As New MyPictureBox With {.Image = My.Resources.HowToPlay.Sky}
    Dim DemoCharacter As New MyPictureBox With {.Image = My.Resources.Game.Character}

    'Setting 初始化
    Dim SettingText As New MyTextBox With {.font = "Cubic11", .font_size = 30.9, .color = Color.White}
    Dim MusicSlider As New Slider
    Dim SoundEffectSlider As New Slider
    Dim DoneSettingButton As New MyButton With {.Image = My.Resources.Setting.DoneSetting, .PressedImage = My.Resources.Setting.DoneSetting_Activated}

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        If BGisOn Then
            '調整背景白色條紋的角度
            BGAnimation += 0.075
            If BGAnimation >= 360 Then
                BGAnimation = 0
            End If

            '繪製背景
            BGBrush = New LinearGradientBrush(New Point(0, 0), New Point(0, MyHeight), Color.Black, Color.Black) '更新背景筆刷位置
            BGColorBlend.Colors = BGColors '更新背景漸層筆刷顏色
            BGColorBlend.Positions = BGColorPosition '更新背景漸層筆刷的顏色的位置
            BGBrush.InterpolationColors = BGColorBlend '更新背景成多色漸層
            BGBrush.RotateTransform(33 + BGAnimation) '旋轉背景
            e.Graphics.FillRectangle(BGBrush, New RectangleF(0, 0, MyWidth, MyHeight)) '繪製背景
        End If

        Select Case State
            Case "Start"
                BGisOn = True
                State = "StudioLogo"
                GameLogo.opacity = 0
                GameLogo.AlignSetting("Center", "Center")
                GameSubLogo.opacity = 0
                GameSubLogo.AlignSetting("Center", "Center")
                OpeningStartTime = Now.Ticks()

            Case "StudioLogo"
                StudioLogo.opacity += OpeningDimSpeed
                If (Now.Ticks() - OpeningStartTime) / 10000 >= OpeningSpeed Then
                    State = "StudioLogoDim"
                    OpeningStartTime = Now.Ticks()
                End If

                StudioLogo.point.X = MyWidth / 2
                StudioLogo.point.Y = MyHeight / 2
                StudioLogo.Draw("III Studio", e, myfont, ScaleRatio)

            Case "StudioLogoDim"
                StudioLogo.opacity -= OpeningDimSpeed
                If (Now.Ticks() - OpeningStartTime) / 10000 >= OpeningGapSpeed Then
                    State = "GameLogo"
                    OpeningStartTime = Now.Ticks()
                End If

                StudioLogo.point.X = MyWidth / 2
                StudioLogo.point.Y = MyHeight / 2
                StudioLogo.Draw("III Studio", e, myfont, ScaleRatio)

            Case "GameLogo"
                GameLogo.opacity += OpeningDimSpeed
                GameSubLogo.opacity += OpeningDimSpeed
                If (Now.Ticks() - OpeningStartTime) / 10000 >= OpeningSpeed Then
                    State = "GameLogoDim"
                    OpeningStartTime = Now.Ticks()
                End If

                GameLogo.point.X = MyWidth / 2
                GameLogo.point.Y = MyHeight / 2 - 28 * ScaleRatio
                GameLogo.Draw("莫舒巴頓", e, myfont, ScaleRatio)

                GameSubLogo.point.X = MyWidth / 2
                GameSubLogo.point.Y = MyHeight / 2 + 37.5 * ScaleRatio
                GameSubLogo.Draw("Mosche Barton", e, myfont, ScaleRatio)

            Case "GameLogoDim"
                GameLogo.opacity -= OpeningDimSpeed
                GameSubLogo.opacity -= OpeningDimSpeed
                If (Now.Ticks() - OpeningStartTime) / 10000 >= OpeningGapSpeed Then
                    State = "Menu"
                    PlaySound(BGM, "MenuBGM.wav", True)
                End If

                GameLogo.point.X = MyWidth / 2
                GameLogo.point.Y = MyHeight / 2 - 28 * ScaleRatio
                GameLogo.Draw("莫舒巴頓", e, myfont, ScaleRatio)

                GameSubLogo.point.X = MyWidth / 2
                GameSubLogo.point.Y = MyHeight / 2 + 37.5 * ScaleRatio
                GameSubLogo.Draw("Mosche Barton", e, myfont, ScaleRatio)

            Case "Menu"
                GameLogo.opacity = 255
                GameLogo.AlignSetting("Center", "Top")
                GameSubLogo.opacity = 255
                GameSubLogo.AlignSetting("Center", "Top")

                MenuCaption.BoxSetting(48 * ScaleRatio, MyHeight - 229 * ScaleRatio, 467.112 * ScaleRatio, 232.3734 * ScaleRatio)
                MenuCaption.Draw(e)

                GameLogo.point.X = 239 * ScaleRatio
                GameLogo.point.Y = 62 * ScaleRatio
                GameLogo.Draw("莫舒巴頓", e, myfont, ScaleRatio)

                GameSubLogo.point.X = 239 * ScaleRatio
                GameSubLogo.point.Y = 140 * ScaleRatio
                GameSubLogo.Draw("Mosche Barton", e, myfont, ScaleRatio)

                VersionInfo.point.X = 12 * ScaleRatio
                VersionInfo.point.Y = MyHeight - 10 * ScaleRatio
                VersionInfo.Draw(Version, e, myfont, ScaleRatio)

                CopyrightInfo.point.X = MyWidth - 12 * ScaleRatio
                CopyrightInfo.point.Y = MyHeight - 10 * ScaleRatio
                CopyrightInfo.Draw(Copyright, e, myfont, ScaleRatio)

                StartButton.Box = New RectangleF(MyWidth - 332 * ScaleRatio, 97 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If StartButton.Draw(e, Mouse, MousePressed, Ding, SoundEffect.settings.volume) = 3 Then
                    State = "Start"
                End If

                HowToPlayButton.Box = New RectangleF(MyWidth - 289 * ScaleRatio, 170 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If HowToPlayButton.Draw(e, Mouse, MousePressed, Ding, SoundEffect.settings.volume) = 3 Then
                    HowToPlay_Img.Image = My.Resources.HowToPlay.HowToPlay1
                    DemoCharacter.BoxSetting(MyWidth / 2 - 268 * ScaleRatio, MyHeight / 2 + 32 * ScaleRatio, 48 * ScaleRatio, 96 * ScaleRatio)
                    BGisOn = False
                    State = "HowToPlay1"
                End If

                SettingButton.Box = New RectangleF(MyWidth - 258 * ScaleRatio, 245 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If SettingButton.Draw(e, Mouse, MousePressed, Ding, SoundEffect.settings.volume) = 3 Then
                    State = "Setting"
                End If

                ExitButton.Box = New RectangleF(MyWidth - 238 * ScaleRatio, 319 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If ExitButton.Draw(e, Mouse, MousePressed, Ding, SoundEffect.settings.volume) = 3 Then
                    State = "Exit"
                End If


            Case "HowToPlay1"
                Sky.BoxSetting(MyWidth / 2 - 684 * ScaleRatio / 2, MyHeight / 2 - 362 * ScaleRatio / 2, 684 * ScaleRatio, 362 * ScaleRatio)
                Sky.Draw(e)

                DrawGame(e, HowToPlay1_Map, MyWidth / 2 - 340 * ScaleRatio, MyHeight / 2 - 112 * ScaleRatio, HowToPlay1_MapWidth, HowToPlay1_MapHeight, ScaleRatio, DemoCharacter)

                DemoCharacter.Draw(e)

                HowToPlay_Img.BoxSetting(MyWidth / 2 - 702 / 2 * ScaleRatio, MyHeight / 2 - 382.811 / 2 * ScaleRatio, 702 * ScaleRatio, 382.811 * ScaleRatio)
                HowToPlay_Img.Draw(e)

                HowToPlay_Text.point.Y = MyHeight / 2 - 167 * ScaleRatio

                HowToPlay_Text.point.X = MyWidth / 2 - 330 * ScaleRatio
                HowToPlay_Text.Draw("使用", e, myfont, ScaleRatio)
                HowToPlay_Text.point.X = MyWidth / 2 - 182 * ScaleRatio
                HowToPlay_Text.Draw("移動角色，", e, myfont, ScaleRatio)
                HowToPlay_Text.point.X = MyWidth / 2 + 70 * ScaleRatio
                HowToPlay_Text.Draw("跳躍", e, myfont, ScaleRatio)

                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(MyWidth / 2 + 702 / 2 * ScaleRatio, MyHeight / 2 - 382.811 / 2 * ScaleRatio, MyWidth - (MyWidth / 2 + 702 / 2 * ScaleRatio), 382.811 * ScaleRatio))

                NextPageButton.Box = New RectangleF(MyWidth / 2 + 175 * ScaleRatio, MyHeight / 2 + 150 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If NextPageButton.Draw(e, Mouse, MousePressed, Ding, SoundEffect.settings.volume) = 3 Then
                    HowToPlay_Img.Image = My.Resources.HowToPlay.HowToPlay2
                    State = "HowToPlay2"
                End If

            Case "HowToPlay2"
                HowToPlay_Img.BoxSetting(MyWidth / 2 - 702 / 2 * ScaleRatio, MyHeight / 2 - 382.811 / 2 * ScaleRatio, 702 * ScaleRatio, 382.811 * ScaleRatio)
                HowToPlay_Img.Draw(e)

                HowToPlay_Text.point.Y = MyHeight / 2 - 167 * ScaleRatio

                HowToPlay_Text.point.X = MyWidth / 2 - 330 * ScaleRatio
                HowToPlay_Text.Draw("按下右鍵攻擊敵人，", e, myfont, ScaleRatio)
                HowToPlay_Text.point.X = MyWidth / 2 - 4 * ScaleRatio
                HowToPlay_Text.Draw("釋放技能", e, myfont, ScaleRatio)

                NextPageButton.Box = New RectangleF(MyWidth / 2 + 175 * ScaleRatio, MyHeight / 2 + 150 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If NextPageButton.Draw(e, Mouse, MousePressed, Ding, SoundEffect.settings.volume) = 3 Then
                    BGisOn = True
                    State = "Menu"
                End If

            Case "Setting"
                SettingText.point.X = 50 * ScaleRatio
                SettingText.point.Y = 42 * ScaleRatio
                SettingText.Draw("設定", e, myfont, ScaleRatio)

                MusicSlider.NameSetting("音量設定", "Cubic11", 16.98, 50 * ScaleRatio, 143 * ScaleRatio)
                MusicSlider.Setting(177 * ScaleRatio, MyWidth - 105 * ScaleRatio, 143 * ScaleRatio)
                MusicSlider.ValueTextSetting("Cubic11", 16.98, MyWidth - 88 * ScaleRatio, 143 * ScaleRatio)
                MusicSlider.Draw(e, myfont, ScaleRatio, Ding, Mouse, MousePressed)
                BGM.settings.volume = MusicSlider.value

                SoundEffectSlider.NameSetting("音效設定", "Cubic11", 16.98, 50 * ScaleRatio, 198 * ScaleRatio)
                SoundEffectSlider.Setting(177 * ScaleRatio, MyWidth - 105 * ScaleRatio, 198 * ScaleRatio)
                SoundEffectSlider.ValueTextSetting("Cubic11", 16.98, MyWidth - 88 * ScaleRatio, 198 * ScaleRatio)
                SoundEffectSlider.Draw(e, myfont, ScaleRatio, Ding, Mouse, MousePressed)
                SoundEffect.settings.volume = SoundEffectSlider.value

                DoneSettingButton.Box = New RectangleF(566 * ScaleRatio, 362 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If DoneSettingButton.Draw(e, Mouse, MousePressed, Ding, SoundEffect.settings.volume) = 3 Then
                    State = "Menu"
                End If

            Case "Exit"
                WMPExit(BGM)
                WMPExit(SoundEffect)
                WMPExit(Ding)
                Me.Close()
        End Select

        '除錯面板開
        If DebugPanelOn Then
            Frametime = Now.Ticks() / 10000 - lastUpdate
            FPS = 1 / (Frametime / 1000)
            DebugInfo.Draw("Mosche Barton " & vbNewLine &
                           Version & vbNewLine &
                           "FPS " & CStr(Math.Round(FPS, 2)) & vbNewLine &
                           "Frametime " & CStr(Math.Round(Frametime, 2)) & vbNewLine &
                           "State " & State,
                           e, myfont, ScaleRatio)

            e.Graphics.DrawLine(New Pen(Color.Yellow), New PointF(0, MyHeight / 2), New PointF(MyWidth, MyHeight / 2))
            e.Graphics.DrawLine(New Pen(Color.Yellow), New PointF(MyWidth / 2, 0), New PointF(MyWidth / 2, MyHeight))

            lastUpdate = Now.Ticks() / 10000
        End If
    End Sub

    Private Sub ScreenRefresh_Tick(sender As Object, e As EventArgs)
        Me.Invalidate()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ScreenRefresh.SynchronizingObject = Me '使Timer的事件依附在本體的執行續
        AddHandler ScreenRefresh.Elapsed, AddressOf ScreenRefresh_Tick '分派事件到ScreenRefresh_Tick
        ScreenRefresh.AutoReset = True 'Timer的自動重新計時開啟
        ScreenRefresh.Enabled = True 'Timer啟動

        '從Font.resx取出字體，並在記憶體中建立Font物件
        bytes = My.Resources.Fonts.Cubic11 '從Font.resx讀取字體，並寫入記憶體
        ptr = Marshal.AllocCoTaskMem(bytes.Length) '在記憶體中找位置放字體
        Marshal.Copy(bytes, 0, ptr, bytes.Length) '複製字體到記憶體中
        myfont.AddMemoryFont(ptr, bytes.Length) '物件化字體

        bytes = My.Resources.Fonts.Monoton
        ptr = Marshal.AllocCoTaskMem(bytes.Length)
        Marshal.Copy(bytes, 0, ptr, bytes.Length)
        myfont.AddMemoryFont(ptr, bytes.Length)

        '記錄鍵盤按下的變數歸零
        For i = 0 To Keyboard.Length - 1
            Keyboard(i) = False
        Next
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        MyWidth = Me.ClientSize.Width
        MyHeight = Me.ClientSize.Height
        ScaleRatio = (MyWidth / DefaultWidth + MyHeight / DefaultHeight) / 2
        If ScaleRatio > 2 Then
            ScaleRatio = 2
        End If
        Me.Invalidate()
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyValue <= 255 Then
            Keyboard(e.KeyValue) = True
        End If
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        If e.KeyValue <= 255 Then
            Keyboard(e.KeyValue) = False
        End If

        If e.KeyCode = Keys.F3 Then
            DebugPanelOn = Not DebugPanelOn
        End If
    End Sub

    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        Mouse.X = e.X
        Mouse.Y = e.Y
    End Sub

    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        MousePressed = True
    End Sub

    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        MousePressed = False
    End Sub
End Class
