﻿Imports System.Drawing.Drawing2D
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

        Public Function FontHeight(ByRef e As PaintEventArgs, myfont As PrivateFontCollection, ScaleRatio As Double) As Integer
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
            Return DrawFont.Height
        End Function
        Public Sub AlignSetting(alignment As String, linealignment As String)
            Align = alignment
            LineAlign = linealignment
        End Sub

        Public Sub Draw(Text As String, ByRef e As PaintEventArgs, myfont As PrivateFontCollection, ScaleRatio As Double)
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

        Public Sub Draw(ByRef e As PaintEventArgs)
            e.Graphics.DrawImage(Image, Box)
        End Sub
    End Class

    Public Class MyButton
        Public Box As RectangleF
        Public Image As Bitmap
        Public PressedImage As Bitmap
        Public Activated As Boolean = False

        Public Function Draw(ByRef e As PaintEventArgs, Mouse As Point, MousePressed As Boolean, ByRef Player As WMPLib.WindowsMediaPlayer, Volume As Double) As Integer
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

    Public Class Game
        Public x As Double
        Public y As Double
        Public width As Double
        Public height As Double
        Public BG As Bitmap
        Private Const G As Double = 9.80665 / 1000 '重力加速度常數 (格/ms^2)
        Public LastUpdate As Long '(ms)

        Private WallDetectX0 As Integer
        Private WallDetectX08 As Integer
        Private WallDetectY0 As Integer
        Private WallDetectY09 As Integer
        Private WallDetectY18 As Integer

        Public Map As Integer(,)
        Public Map_Width As Integer
        Public Map_Height As Integer
        Private Map_Texture As Bitmap() = {My.Resources.Game.Cobblestone, My.Resources.Game.Cobblestone}
        Private MapDx As Double = 0
        Private MapDy As Double = 0

        Public CharacterBox As RectangleF
        Public CharacterX As Double = 5.1
        Public CharacterY As Double = 1
        Private Character_Speed As Double = 0.1
        Private CharacterYv As Double = 0


        Public Character_Jump As Boolean = False
        Public Const Character_Jump_Speed As Double = 0.08 '(格/ms)
        Public Jump_Delay As Integer = 250 '(ms)
        Public Last_Jump As Long = 0

        Public Sub BoxSetting(ptx As Double, pty As Double, ptwidth As Double, ptheight As Double)
            x = ptx
            y = pty
            width = ptwidth
            height = ptheight
        End Sub

        Public Sub Detect()
            WallDetectX0 = Math.Floor(CharacterX)
            WallDetectX08 = Math.Floor(CharacterX + 0.8)
            WallDetectY0 = Math.Floor(Map_Height - CharacterY)
            WallDetectY09 = Math.Floor(Map_Height - (CharacterY + 0.9))
            WallDetectY18 = Math.Floor(Map_Height - (CharacterY + 1.8))

            If WallDetectX0 < 0 Then
                WallDetectX0 = 0
            ElseIf WallDetectX0 > Map_Width - 1 Then
                WallDetectX0 = Map_Width - 1
            End If

            If WallDetectX08 < 0 Then
                WallDetectX08 = 0
            ElseIf WallDetectX08 > Map_Width - 1 Then
                WallDetectX08 = Map_Width - 1
            End If

            If WallDetectY0 < 0 Then
                WallDetectY0 = 0
            ElseIf WallDetectY0 > Map_Height - 1 Then
                WallDetectY0 = Map_Height - 1
            End If

            If WallDetectY09 < 0 Then
                WallDetectY09 = 0
            ElseIf WallDetectY09 > Map_Height - 1 Then
                WallDetectY09 = Map_Height - 1
            End If

            If WallDetectY18 < 0 Then
                WallDetectY18 = 0
            ElseIf WallDetectY18 > Map_Height - 1 Then
                WallDetectY18 = Map_Height - 1
            End If
        End Sub

        Public Sub DrawGame(ByRef e As PaintEventArgs, ScaleRatio As Double, ByRef Keyboard() As Boolean)
            '畫背景
            e.Graphics.DrawImage(BG, New RectangleF(x + MapDx * ScaleRatio, y + MapDy * ScaleRatio, width, height))

            '畫地圖
            For i = 0 To Map_Height - 1
                For j = 0 To Map_Width - 1
                    If Map(i, j) > 0 Then
                        e.Graphics.DrawImage(Map_Texture(Map(i, j)), New RectangleF(x + MapDx * ScaleRatio, y + MapDy * ScaleRatio, 48 * ScaleRatio, 48 * ScaleRatio))
                    End If

                    MapDx += 48
                Next
                MapDx = 0
                MapDy += 48
            Next
            MapDx = 0
            MapDy = 0

            '畫角色
            If Keyboard(Keys.D) Then
                CharacterX += Character_Speed
            End If
            If Keyboard(Keys.A) Then
                CharacterX -= Character_Speed
            End If
            If Keyboard(Keys.S) Then
                CharacterY -= Character_Speed
            End If
            If Keyboard(Keys.Space) And Character_Jump = False And Now.Ticks() / 10000 - Last_Jump > Jump_Delay Then
                CharacterY += Character_Speed
                CharacterYv = Character_Jump_Speed
                Character_Jump = True
                Last_Jump = Now.Ticks() / 10000
            End If

            CharacterYv -= G
            CharacterY += CharacterYv * (Now.Ticks() / 10000 - LastUpdate)

            Detect()
            If (Map(WallDetectY0, WallDetectX0) <> 0 Or Map(WallDetectY0, WallDetectX08) <> 0) And Math.Floor(CharacterY) <= CharacterY And CharacterY < Math.Floor(CharacterY) + 1 Then
                CharacterYv = 0
                CharacterY = Math.Floor(CharacterY) + 1
                Character_Jump = False
            End If



            CharacterBox = New RectangleF(x + CharacterX * 48 * ScaleRatio, y + (Map_Height - CharacterY) * 48 * ScaleRatio - 86.4 * ScaleRatio, 38.4 * ScaleRatio, 86.4 * ScaleRatio)

            e.Graphics.DrawImage(My.Resources.Game.Character, CharacterBox)
            LastUpdate = Now.Ticks() / 10000
        End Sub
    End Class

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

        Public Sub Draw(ByRef e As PaintEventArgs, myfont As PrivateFontCollection, ScaleRatio As Double, ByRef Player As WMPLib.WindowsMediaPlayer, Mouse As Point, MousePressed As Boolean)
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
    Dim State As String = "HowToPlay1"

    Const Version As String = "Insider Preview 1.1"
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
    Dim DebugInfo As New MyTextBox With {.font = "Consolas", .font_size = 10, .color = Color.Yellow}
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

    'Loading 初始化
    Dim LoadingIndex As Integer = 0
    Dim LoadingShow As Boolean = False
    Dim LoadingSeq() As Bitmap = {My.Resources.Loading._0, My.Resources.Loading._1, My.Resources.Loading._2, My.Resources.Loading._3, My.Resources.Loading._4, My.Resources.Loading._5, My.Resources.Loading._6, My.Resources.Loading._7, My.Resources.Loading._8, My.Resources.Loading._9, My.Resources.Loading._10, My.Resources.Loading._11, My.Resources.Loading._12, My.Resources.Loading._13, My.Resources.Loading._14, My.Resources.Loading._15, My.Resources.Loading._16, My.Resources.Loading._17, My.Resources.Loading._18, My.Resources.Loading._19, My.Resources.Loading._20, My.Resources.Loading._21, My.Resources.Loading._22, My.Resources.Loading._23, My.Resources.Loading._24, My.Resources.Loading._25, My.Resources.Loading._26, My.Resources.Loading._27, My.Resources.Loading._28, My.Resources.Loading._29}
    Dim Loading As New MyPictureBox

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
    Dim DemoGame As New Game With {.Map = {{1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                           {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                           {1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1},
                                           {1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1},
                                           {1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 1},
                                           {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}},
                                   .Map_Width = 14, .Map_Height = 6, .BG = My.Resources.HowToPlay.Sky}

    'Setting 初始化
    Dim SettingText As New MyTextBox With {.font = "Cubic11", .font_size = 30.9, .color = Color.White}
    Dim MusicSlider As New Slider
    Dim SoundEffectSlider As New Slider
    Dim DoneSettingButton As New MyButton With {.Image = My.Resources.Setting.DoneSetting, .PressedImage = My.Resources.Setting.DoneSetting_Activated}

    'Intro 初始化
    Dim IntroStartTime As Long
    Dim IntroText() As String = {"我曾踏過千山萬水", "行過無數市鎮", "與牧人一起唱著山歌", "和漁夫共同經歷風暴", "只為求得那", "不存於人世的音樂", "那是我年輕時的故事了", "呵呵，別看我現在這糟老頭樣", "我年輕時可是才華洋溢的提琴手呢！", "孩子們", "靠過來點", "今天老莫爺爺我啊", "要說的是我年輕時最不可思議的事", "那就是跟使用邪惡音樂", "攻擊村莊的魔王死戰", "嘿！誰說英雄一定要拿劍的", "坐好坐好，要開始囉！", "嗯咳！", "那，是一個陰暗的日子。。。。。"}
    Dim IntroTimeCodeStart As Integer() = {12000, 15040, 16704, 19104, 21696, 23392, 26656, 28736, 31360, 34592, 35488, 36864, 38784, 42432, 44150, 46478, 49166, 51086, 52270}
    Dim IntroTimeCodeEnd As Integer() = {14816, 16640, 18912, 21632, 23232, 26112, 28608, 31104, 34016, 35296, 36512, 38432, 41920, 44088, 45710, 48878, 50510, 51534, 56750}
    Dim TextSpeed As Double = 15
    Dim TextIndex As Integer = 0
    Dim TextAnimationIndex As Integer = 0
    Dim ShowText As New MyTextBox With {.font = "Cubic11", .font_size = 30, .color = Color.White, .LineAlign = "Center"}
    Dim NowShowText As String
    Dim DimScreen As Integer = 0


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

        If LoadingShow Then
            LoadingIndex += 1
            If LoadingIndex >= LoadingSeq.Length() - 1 Then
                LoadingIndex = 0
            End If

            Loading.BoxSetting(MyWidth - 50, MyHeight - 50, 30, 30)
            Loading.Image = LoadingSeq(LoadingIndex)
            Loading.Draw(e)
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
                    State = "Intro"
                    IntroStartTime = Now.Ticks() / 10000
                    TextIndex = 0
                    TextAnimationIndex = 0
                    DimScreen = 0
                    BGM.controls.stop()
                End If

                HowToPlayButton.Box = New RectangleF(MyWidth - 289 * ScaleRatio, 170 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If HowToPlayButton.Draw(e, Mouse, MousePressed, Ding, SoundEffect.settings.volume) = 3 Then
                    HowToPlay_Img.Image = My.Resources.HowToPlay.HowToPlay1
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

                DemoGame.BoxSetting(MyWidth / 2 - 335 * ScaleRatio, MyHeight / 2 - 112 * ScaleRatio, 670 * ScaleRatio, 287 * ScaleRatio)
                DemoGame.LastUpdate = Now.Ticks() / 10000
                DemoGame.DrawGame(e, ScaleRatio, Keyboard)

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

                MusicSlider.NameSetting("音樂設定", "Cubic11", 16.98, 50 * ScaleRatio, 143 * ScaleRatio)
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

            Case "Intro"
                Select Case Now.Ticks() / 10000 - IntroStartTime
                    Case 0 To 3000
                        LoadingShow = True

                    Case 3000 To 3250
                        PlaySound(BGM, "Bach Air on the G String.mp3", False)
                        LoadingShow = False

                    Case IntroTimeCodeStart(0) To IntroTimeCodeStart(0) + 250
                        PlaySound(SoundEffect, "開場配音.wav", False)

                    Case IntroTimeCodeEnd(IntroText.Length() - 1) To IntroTimeCodeEnd(IntroText.Length() - 1) + 2000

                        If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                            DimScreen += 5
                            If DimScreen >= 255 Then
                                DimScreen = 255
                            End If
                        End If
                    Case IntroTimeCodeEnd(IntroText.Length() - 1) + 2000 To IntroTimeCodeEnd(IntroText.Length() - 1) + 2000 + 2000
                        State = "Menu"
                        PlaySound(BGM, "MenuBGM.wav", True)

                    Case IntroTimeCodeStart(TextIndex) To IntroTimeCodeEnd(TextIndex)
                        If TextAnimationIndex < IntroText(TextIndex).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                NowShowText += IntroText(TextIndex)(TextAnimationIndex)
                                TextAnimationIndex += 1
                            End If
                        ElseIf Now.Ticks() / 10000 - IntroStartTime >= IntroTimeCodeEnd(TextIndex) - 30 Then
                            TextIndex += 1
                            If TextIndex >= IntroText.Length() - 1 Then
                                TextIndex = IntroText.Length() - 1
                            End If
                        End If

                    Case Else
                        TextAnimationIndex = 0
                        NowShowText = ""
                End Select
                ShowText.point.X = MyWidth / 2 - IntroText(TextIndex).Length() / 2 * ShowText.FontHeight(e, myfont, ScaleRatio) + 1.15 * IntroText(TextIndex).Length() * ScaleRatio
                ShowText.point.Y = MyHeight / 2
                ShowText.Draw(NowShowText, e, myfont, ScaleRatio)
                e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(DimScreen, 0, 0, 0)), New Rectangle(0, 0, MyWidth, MyHeight))

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
                           CStr(MyWidth) & "x" & CStr(MyHeight) & vbNewLine &
                           "Scale by " & CStr(ScaleRatio) & vbNewLine &
                           "FPS " & CStr(Math.Round(FPS, 2)) & vbNewLine &
                           "Frametime " & CStr(Math.Round(Frametime, 2)) & vbNewLine &
                           "State " & State & vbNewLine &
                           "Character (" & CStr(Math.Round(DemoGame.CharacterX, 2)) & ", " & CStr(Math.Round(DemoGame.CharacterY, 2)) & ")",
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
