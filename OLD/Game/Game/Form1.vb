Imports System.Timers
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Runtime.InteropServices

Public Class Form1
    Structure MyPicBox
        Dim Pic As Bitmap
        Dim Box As RectangleF
        Dim angle As Integer
        Dim isPressed As Boolean
        Dim isActivated As Boolean
    End Structure

    '初始狀態
    Dim State As String = "Opening1"

    '遊戲版本
    Const GameVersion As String = "InsiderPreview 0.02"

    '初始化視窗長寬的變數
    Dim MyWidth As Integer
    Dim MyHeight As Integer

    'ScaleSize
    Dim ScaleRatio As Double = 1

    '音量
    Dim BGM As New WMPLib.WindowsMediaPlayer
    Dim SoundEffect As New WMPLib.WindowsMediaPlayer
    Dim Volume As Integer = 100

    '初始化滑鼠的變數
    Dim Mouse As Point
    Dim MousePressed As Boolean = False
    Dim lastClickPoint As Point

    '初始化鍵盤的變數
    Dim Keyboard(256) As Boolean

    '初始化Timer
    Dim ScreenRefresh As New Timer(1)

    '字體初始化
    Dim myfont As New PrivateFontCollection
    Dim bytes As Byte()
    Dim ptr As IntPtr

    '攝影機初始化
    Dim CameraBoarderSizeX As Integer = 400 - 32
    Dim CameraBoarderSizeY As Integer = 225 - 32
    Dim CameraBoarder As RectangleF

    '地圖初始化
    Dim Map As New MyPicBox With {.Pic = My.Resources.Game.Map, .Box = New RectangleF(-800 * ScaleRatio, -500 * ScaleRatio, 1690 * ScaleRatio, 1151 * ScaleRatio)}
    Dim MapBoarder As RectangleF

    '玩家初始化
    Dim Player As New MyPicBox With {.Pic = My.Resources.Game.Character, .Box = New RectangleF(368 * ScaleRatio, 193 * ScaleRatio, 64 * ScaleRatio, 64 * ScaleRatio)}
    Dim CharacterSpeed As Integer = 8

    '初始化效能相關資訊
    Dim lastUpdate As Long = 0
    Dim FPS As Double
    Dim Frametime As Double
    Dim DebugInfo As String

    '初始化載入動畫
    Dim Loading() As Bitmap = {My.Resources.Loading._0, My.Resources.Loading._1, My.Resources.Loading._2, My.Resources.Loading._3, My.Resources.Loading._4, My.Resources.Loading._5, My.Resources.Loading._6, My.Resources.Loading._7, My.Resources.Loading._8, My.Resources.Loading._9, My.Resources.Loading._10, My.Resources.Loading._11, My.Resources.Loading._12, My.Resources.Loading._13, My.Resources.Loading._14, My.Resources.Loading._15, My.Resources.Loading._16, My.Resources.Loading._17, My.Resources.Loading._18, My.Resources.Loading._19, My.Resources.Loading._20, My.Resources.Loading._21, My.Resources.Loading._22, My.Resources.Loading._23, My.Resources.Loading._24, My.Resources.Loading._25, My.Resources.Loading._26, My.Resources.Loading._27, My.Resources.Loading._28, My.Resources.Loading._29}
    Dim LoadingShow As Boolean = False
    Dim LoadingIndex As Integer = 0

    '初始化開始的Logo顯示
    Dim BGColors As Color() = {Color.FromArgb(25, 25, 25), Color.FromArgb(0, 0, 0), Color.FromArgb(25, 25, 25)}
    Dim BGColorPosition As Single() = {0.0F, 0.5F, 1.0F}
    Dim BGColorBlend As New ColorBlend
    Dim BGBrush As New LinearGradientBrush(New Point(0, 0), New Point(100, 500), Color.Black, Color.White)
    Dim BGAnimation As Single = 0
    Dim LogoColorA As Double = 0
    Dim LogoShowingTime As Long = 1750
    Dim StartTime As Long

    '初始化Menu
    Dim StartGameColorA As Single = 0
    Dim StartGameColorA_d As Single = 1.5
    Dim StartGameHasPressed As Boolean = False
    Dim Caption As New MyPicBox With {.Pic = My.Resources.Image.Caption}
    Dim _Exit As New MyPicBox With {.Pic = My.Resources.Image._Exit, .isPressed = False}
    Dim Setting As New MyPicBox With {.Pic = My.Resources.Image.Setting, .isPressed = False}

    '初始化設定介面
    Dim SettingPen As New Pen(Color.White, 3.0F)
    Dim SettingDotBrush As New SolidBrush(Color.White)
    Dim SettingDotRect As RectangleF
    Dim SettingDotSize As Single = 16.0F
    Dim SettingDotPressed As Boolean = False
    Dim F3 As New MyPicBox With {.Pic = My.Resources.Image.SettingF3}
    Dim F3_Switch As New MyPicBox With {.Pic = My.Resources.Image.F3_OFF, .isPressed = False, .isActivated = False}
    Dim DoneSetting As New MyPicBox With {.Pic = My.Resources.Image.DoneSetting, .isPressed = False}

    '初始化Intro
    Dim BGColorA As Double = 0
    Dim IntroStartTime As Long = 0
    Dim ShowText As String = ""
    Dim ShowTextLen As Integer = 0
    Dim TextIndex As Integer = 0
    Dim TextSpeed As Integer = 100
    Dim IntroText() As String = {"我曾踏過千山萬水", "行過無數市鎮", "與牧人一起唱著山歌", "和漁夫共同經歷風暴", "只為求得那", "不存於人世的音樂", "那是我年輕時的故事了", "呵呵，別看我現在這糟老頭樣", "我年輕時可是才華洋溢的提琴手呢！", "孩子們", "靠過來點", "今天老莫爺爺我啊", "要說的是我年輕時最不可思議的事", "那就是跟使用邪惡音樂", "攻擊村莊的魔王死戰", "嘿！誰說英雄一定要拿劍的", "坐好坐好，要開始囉！", "嗯咳！", "那，是一個陰暗的日子．．．．． "}
    Dim SkipisPressed As Boolean = False

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        Select Case State
            Case "Opening1"
                '繪製背景
                BGBrush = New LinearGradientBrush(New Point(0, 0), New Point(0, MyHeight), Color.Black, Color.Black) '更新背景筆刷位置
                BGColorBlend.Colors = BGColors '更新背景漸層筆刷顏色
                BGColorBlend.Positions = BGColorPosition '更新背景漸層筆刷的顏色的位置
                BGBrush.InterpolationColors = BGColorBlend '更新背景成多色漸層
                BGBrush.RotateTransform(33 + BGAnimation) '旋轉背景
                e.Graphics.FillRectangle(BGBrush, New RectangleF(0, 0, MyWidth, MyHeight)) '繪製背景

                e.Graphics.DrawString("III Studio", Fonts("Monoton", 50 * ScaleRatio), New SolidBrush(Color.FromArgb(LogoColorA, 255, 255, 255)),
                                      MyWidth / 2 - Fonts("Monoton", 50 * ScaleRatio).Height * 1.78, 'X
                                      MyHeight / 2 - Fonts("Monoton", 50 * ScaleRatio).Height / 2) 'Y

            Case "Opening2"
                '繪製背景
                BGBrush = New LinearGradientBrush(New Point(0, 0), New Point(0, MyHeight), Color.Black, Color.Black) '更新背景筆刷位置
                BGColorBlend.Colors = BGColors '更新背景漸層筆刷顏色
                BGColorBlend.Positions = BGColorPosition '更新背景漸層筆刷的顏色的位置
                BGBrush.InterpolationColors = BGColorBlend '更新背景成多色漸層
                BGBrush.RotateTransform(33 + BGAnimation) '旋轉背景
                e.Graphics.FillRectangle(BGBrush, New RectangleF(0, 0, MyWidth, MyHeight)) '繪製背景

                e.Graphics.DrawString("莫舒巴頓", Fonts("Cubic", 50 * ScaleRatio), New SolidBrush(Color.FromArgb(LogoColorA, 255, 255, 255)),
                                      MyWidth / 2 - Fonts("Cubic", 50 * ScaleRatio).Height * 2, 'X
                                      MyHeight / 2 - Fonts("Cubic", 50 * ScaleRatio).Height + 15 * ScaleRatio) 'Y
                e.Graphics.DrawString("Mosche Barton", Fonts("Cubic", 30.9 * ScaleRatio), New SolidBrush(Color.FromArgb(LogoColorA, 157, 157, 157)),
                                      MyWidth / 2 - Fonts("Cubic", 30.9 * ScaleRatio).Height * 2.91, 'X
                                      MyHeight / 2 + 15 * ScaleRatio) 'Y

            Case "Menu"
                '繪製背景
                BGBrush = New LinearGradientBrush(New Point(0, 0), New Point(0, MyHeight), Color.Black, Color.Black) '更新背景筆刷位置
                BGColorBlend.Colors = BGColors '更新背景漸層筆刷顏色
                BGColorBlend.Positions = BGColorPosition '更新背景漸層筆刷的顏色的位置
                BGBrush.InterpolationColors = BGColorBlend '更新背景成多色漸層
                BGBrush.RotateTransform(33 + BGAnimation) '旋轉背景
                e.Graphics.FillRectangle(BGBrush, New RectangleF(0, 0, MyWidth, MyHeight)) '繪製背景

                Caption.Box = New RectangleF(MyWidth / 2 - 466.5714 * ScaleRatio / 2, MyHeight - 256 * ScaleRatio, 466.5714 * ScaleRatio, 232.1045 * ScaleRatio)
                e.Graphics.DrawImage(Caption.Pic, Caption.Box)

                e.Graphics.DrawString("莫舒巴頓", Fonts("Cubic", 50 * ScaleRatio), New SolidBrush(Color.FromArgb(255, 255, 255)),
                                      MyWidth / 2 - Fonts("Cubic", 50 * ScaleRatio).Height * 2, 'X
                                      70 * ScaleRatio) 'Y
                e.Graphics.DrawString("Mosche Barton", Fonts("Cubic", 30.9 * ScaleRatio), New SolidBrush(Color.FromArgb(157, 157, 157)),
                                      MyWidth / 2 - Fonts("Cubic", 30.9 * ScaleRatio).Height * 2.91, 'X
                                      70 * ScaleRatio + Fonts("Cubic", 50 * ScaleRatio).Height) 'Y

                e.Graphics.DrawString("點選進入遊戲", Fonts("Cubic", 30.9 * ScaleRatio), New SolidBrush(Color.FromArgb(StartGameColorA, 255, 255, 255)),
                                      MyWidth / 2 - Fonts("Cubic", 30.9 * ScaleRatio).Height * 3 + 6.35 * ScaleRatio, 'X
                                      MyHeight - Fonts("Cubic", 30.9 * ScaleRatio).Height - 70 * ScaleRatio) 'Y
                _Exit.Box = New RectangleF(-Fonts("Cubic", 30.9 * ScaleRatio).Height / 2 + 70 * ScaleRatio, 'X
                                          MyHeight - Fonts("Cubic", 30.9 * ScaleRatio).Height - 70 * ScaleRatio, 'Y
                                          Fonts("Cubic", 30.9 * ScaleRatio).Height, '寬
                                          Fonts("Cubic", 30.9 * ScaleRatio).Height) '高
                Setting.Box = New RectangleF(MyWidth - Fonts("Cubic", 30.9 * ScaleRatio).Height / 2 - 70 * ScaleRatio, 'X
                                          MyHeight - Fonts("Cubic", 30.9 * ScaleRatio).Height - 70 * ScaleRatio, 'Y
                                          Fonts("Cubic", 30.9 * ScaleRatio).Height, '寬
                                          Fonts("Cubic", 30.9 * ScaleRatio).Height) '高
                e.Graphics.DrawImage(_Exit.Pic, _Exit.Box)
                e.Graphics.DrawImage(Setting.Pic, Setting.Box)

                e.Graphics.DrawString(GameVersion, Fonts("Cubic", 12 * ScaleRatio), New SolidBrush(Color.FromArgb(127, 255, 255, 255)), 8 * ScaleRatio, MyHeight - Fonts("Cubic", 12 * ScaleRatio).Height - 8 * ScaleRatio)
                e.Graphics.DrawString("III Studio™ 版權所有", Fonts("Cubic", 12 * ScaleRatio), New SolidBrush(Color.FromArgb(127, 255, 255, 255)), MyWidth - Fonts("Cubic", 12 * ScaleRatio).Height * 8.42 - 8 * ScaleRatio, MyHeight - Fonts("Cubic", 12 * ScaleRatio).Height - 8 * ScaleRatio)

            Case "Setting"
                '繪製背景
                BGBrush = New LinearGradientBrush(New Point(0, 0), New Point(0, MyHeight), Color.Black, Color.Black) '更新背景筆刷位置
                BGColorBlend.Colors = BGColors '更新背景漸層筆刷顏色
                BGColorBlend.Positions = BGColorPosition '更新背景漸層筆刷的顏色的位置
                BGBrush.InterpolationColors = BGColorBlend '更新背景成多色漸層
                BGBrush.RotateTransform(33 + BGAnimation) '旋轉背景
                e.Graphics.FillRectangle(BGBrush, New RectangleF(0, 0, MyWidth, MyHeight)) '繪製背景

                e.Graphics.DrawString("設定", Fonts("Cubic", 34 * ScaleRatio), New SolidBrush(Color.FromArgb(255, 255, 255)),
                                      67 * ScaleRatio, 'X
                                      45 * ScaleRatio) 'Y
                e.Graphics.DrawLine(SettingPen, CSng(49 * ScaleRatio), CSng(116 * ScaleRatio), CSng(MyWidth - 49 * ScaleRatio), CSng(116 * ScaleRatio))

                '音量控制部分
                e.Graphics.DrawString("音量", Fonts("Cubic", 23 * ScaleRatio), New SolidBrush(Color.FromArgb(255, 255, 255)),
                                      67 * ScaleRatio, 'X
                                      163 * ScaleRatio) 'Y
                e.Graphics.DrawLine(SettingPen, CSng(165 * ScaleRatio), CSng(163 * ScaleRatio + Fonts("Cubic", 23 * ScaleRatio).Height / 2), CSng(MyWidth - 150 * ScaleRatio), CSng(163 * ScaleRatio + Fonts("Cubic", 23 * ScaleRatio).Height / 2))
                e.Graphics.FillEllipse(SettingDotBrush, SettingDotRect)
                e.Graphics.DrawString(CStr(Volume), Fonts("Cubic", 23 * ScaleRatio), New SolidBrush(Color.FromArgb(255, 255, 255)),
                                      MyWidth - 130 * ScaleRatio, 'X
                                      163 * ScaleRatio) 'Y

                '除錯面板部分
                e.Graphics.DrawString("除錯面板", Fonts("Cubic", 23 * ScaleRatio), New SolidBrush(Color.FromArgb(255, 255, 255)),
                                      67 * ScaleRatio, 'X
                                      238 * ScaleRatio) 'Y
                F3.Box = New RectangleF(218 * ScaleRatio, 238 * ScaleRatio + Fonts("Cubic", 23 * ScaleRatio).Height / 2 - 30.3 * ScaleRatio / 2, 30.3 * ScaleRatio, 30.3 * ScaleRatio)
                e.Graphics.DrawImage(F3.Pic, F3.Box)
                F3_Switch.Box = New RectangleF(292 * ScaleRatio, 238 * ScaleRatio + Fonts("Cubic", 23 * ScaleRatio).Height / 2 - 26.42 * ScaleRatio / 2, 57.96 * ScaleRatio, 26.42 * ScaleRatio)
                e.Graphics.DrawImage(F3_Switch.Pic, F3_Switch.Box)

                '完成設定按鈕部分
                DoneSetting.Box = New RectangleF(MyWidth - 220 * ScaleRatio, MyHeight - 110 * ScaleRatio, 144 * ScaleRatio, 51.4 * ScaleRatio)
                e.Graphics.DrawImage(DoneSetting.Pic, DoneSetting.Box)

            Case "Intro"
                '繪製背景
                BGBrush = New LinearGradientBrush(New Point(0, 0), New Point(0, MyHeight), Color.Black, Color.Black) '更新背景筆刷位置
                BGColorBlend.Colors = BGColors '更新背景漸層筆刷顏色
                BGColorBlend.Positions = BGColorPosition '更新背景漸層筆刷的顏色的位置
                BGBrush.InterpolationColors = BGColorBlend '更新背景成多色漸層
                BGBrush.RotateTransform(33 + BGAnimation) '旋轉背景
                e.Graphics.FillRectangle(BGBrush, New RectangleF(0, 0, MyWidth, MyHeight)) '繪製背景

                e.Graphics.DrawString(ShowText, Fonts("Cubic", 30 * ScaleRatio), New SolidBrush(Color.White),
                                      MyWidth / 2 - ShowTextLen * Fonts("Cubic", 30 * ScaleRatio).Height / 2 + 2.18 * ScaleRatio * ShowTextLen / 2,
                                      MyHeight / 2 - Fonts("Cubic", 30 * ScaleRatio).Height / 2)

                e.Graphics.DrawString("跳過 >", Fonts("Cubic", 12 * ScaleRatio), New SolidBrush(Color.FromArgb(127, 255, 255, 255)),
                                      MyWidth - 80 * ScaleRatio,
                                      24 * ScaleRatio)

                e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(BGColorA, 0, 0, 0)), New RectangleF(0, 0, MyWidth, MyHeight))

            Case "Game"
                e.Graphics.DrawImage(Map.Pic, Map.Box)

                e.Graphics.TranslateTransform(Player.Box.X + Player.Box.Width / 2, Player.Box.Y + Player.Box.Width / 2)
                e.Graphics.RotateTransform(Player.angle)
                e.Graphics.TranslateTransform(-Player.Box.X - Player.Box.Width / 2, -Player.Box.Y - Player.Box.Width / 2)
                e.Graphics.DrawImage(Player.Pic, Player.Box)

                e.Graphics.ResetTransform()
        End Select

        If LoadingShow Then
            e.Graphics.DrawImage(Loading(LoadingIndex), New RectangleF(MyWidth - 30 * ScaleRatio - 20 * ScaleRatio, MyHeight - 30 * ScaleRatio - 20 * ScaleRatio, 30 * ScaleRatio, 30 * ScaleRatio))
        End If

        '顯示效能相關資訊
        If F3_Switch.isActivated Then
            Frametime = Math.Round(Now.Ticks / 10000 - lastUpdate, 2)
            FPS = Math.Round(1 / Frametime * 1000, 2)
            e.Graphics.DrawString(DebugInfo, New Font("Consolas", 12), New SolidBrush(Color.Yellow), 10.0, 10.0)
            e.Graphics.DrawLine(New Pen(Color.Wheat), New Point(MyWidth / 2, 0), New Point(MyWidth / 2, MyHeight))
            e.Graphics.DrawLine(New Pen(Color.Wheat), New Point(0, MyHeight / 2), New Point(MyWidth, MyHeight / 2))
            lastUpdate = Now.Ticks / 10000
        End If
    End Sub

    '畫面的更新
    Private Sub ScreenRefresh_Tick(source As Object, e As EventArgs)
        '更新效能相關資訊
        If F3_Switch.isActivated Then
            DebugInfo = CStr(MyWidth) + "x" + CStr(MyHeight) + vbNewLine +
                "ScaleRatio " + CStr(ScaleRatio) + vbNewLine +
                "FPS " + CStr(FPS) + vbNewLine +
                "Framtime " + CStr(Frametime) + vbNewLine +
                "Scene """ + State + """" + vbNewLine +
                "Mouse (" + CStr(Mouse.X) + ", " + CStr(Mouse.Y) + ")" + vbNewLine +
                "lastClick (" + CStr(lastClickPoint.X) + ", " + CStr(lastClickPoint.Y) + ")" + vbNewLine +
                "Volume " + CStr(Volume)
        End If

        If LoadingShow Then
            LoadingIndex += 1
            If LoadingIndex >= 30 Then
                LoadingIndex = 0
            End If
        End If

        Select Case State
            Case "Opening1"
                BGAnimation += 0.075
                If BGAnimation >= 360 Then
                    BGAnimation = 0
                End If

                If LogoColorA >= 255 Then
                    LogoColorA = 255
                    If Now.Ticks > StartTime + LogoShowingTime * 10000 Then
                        State = "Opening2"
                        LogoColorA = 0
                    End If
                Else
                    LogoColorA += 2.5
                    StartTime = Now.Ticks
                End If

            Case "Opening2"
                BGAnimation += 0.075
                If BGAnimation >= 360 Then
                    BGAnimation = 0
                End If

                If LogoColorA >= 255 Then
                    LogoColorA = 255
                    If Now.Ticks > StartTime + LogoShowingTime * 10000 Then
                        State = "Menu"
                        Play(BGM, "MenuBGM.wav", True)
                        LogoColorA = 0
                    End If
                Else
                    LogoColorA += 2.5
                    StartTime = Now.Ticks
                End If

            Case "Menu"
                BGAnimation += 0.075
                If BGAnimation >= 360 Then
                    BGAnimation = 0
                End If

                StartGameColorA += StartGameColorA_d
                If StartGameColorA >= 200 Then
                    StartGameColorA = 200
                    StartGameColorA_d = -1.5
                ElseIf StartGameColorA <= 0 Then
                    StartGameColorA = 0
                    StartGameColorA_d = 1.5
                End If

                Select Case isPressed(MouseInArea(_Exit.Box.Height, _Exit.Box.Left + _Exit.Box.Width / 2, _Exit.Box.Top + _Exit.Box.Height / 2), _Exit.isPressed, _Exit.Box)
                    Case 2
                        _Exit.Pic = My.Resources.Image.ExitPress
                    Case 3
                        Me.Close()
                        _Exit.isPressed = False
                    Case Else
                        _Exit.Pic = My.Resources.Image._Exit
                End Select
                Select Case isPressed(MouseInArea(Setting.Box.Height, Setting.Box.Left + Setting.Box.Width / 2, Setting.Box.Top + Setting.Box.Height / 2), Setting.isPressed, Setting.Box)
                    Case 2
                        Setting.Pic = My.Resources.Image.SettingPress
                    Case 3
                        State = "Setting"
                        Setting.isPressed = False
                    Case Else
                        Setting.Pic = My.Resources.Image.Setting
                End Select
                Select Case isPressed(MouseInArea(0, 0, MyWidth, MyHeight), StartGameHasPressed, New RectangleF(0, 0, MyWidth, MyHeight), Setting.Box.Height, Setting.Box.Left + Setting.Box.Width / 2, Setting.Box.Top + Setting.Box.Height / 2, _Exit.Box.Left + _Exit.Box.Width / 2, _Exit.Box.Top + _Exit.Box.Height / 2)
                    Case 3
                        State = "Intro"
                        BGM.controls.pause()
                        LoadingShow = True
                        IntroStartTime = Now.Ticks / 10000
                End Select

            Case "Setting"
                BGAnimation += 0.075
                If BGAnimation >= 360 Then
                    BGAnimation = 0
                End If

                If MousePressed And PointInArea(lastClickPoint,
                                                165 * ScaleRatio - 5 * ScaleRatio,
                                                163 * ScaleRatio + Fonts("Cubic", 23 * ScaleRatio).Height / 2 - 5 * ScaleRatio,
                                                MyWidth - 150 * ScaleRatio + 5 * ScaleRatio,
                                                163 * ScaleRatio + Fonts("Cubic", 23 * ScaleRatio).Height / 2 + 5 * ScaleRatio) Then
                    If Not SettingDotPressed Then
                        SettingDotSize = 12.0F
                        SettingDotBrush = New SolidBrush(Color.FromArgb(128, 128, 128))
                        SettingDotPressed = True
                        SettingDotRect = New RectangleF(165 * ScaleRatio - SettingDotSize * ScaleRatio / 2 + ((MyWidth - 150 * ScaleRatio) - (165 * ScaleRatio)) / 100 * Volume,
                                                    163 * ScaleRatio + Fonts("Cubic", 23 * ScaleRatio).Height / 2 - SettingDotSize * ScaleRatio / 2,
                                                    SettingDotSize * ScaleRatio,
                                                    SettingDotSize * ScaleRatio)
                    End If
                    SettingDotRect.X = Mouse.X - SettingDotRect.Width / 2
                    If SettingDotRect.X < 165 * ScaleRatio - SettingDotRect.Width / 2 Then
                        SettingDotRect.X = 165 * ScaleRatio - SettingDotRect.Width / 2
                    End If
                    If SettingDotRect.X > MyWidth - 150 * ScaleRatio - SettingDotRect.Width / 2 Then
                        SettingDotRect.X = MyWidth - 150 * ScaleRatio - SettingDotRect.Width / 2
                    End If
                    Volume = (SettingDotRect.X + SettingDotRect.Width / 2 - 165 * ScaleRatio) / (MyWidth - 150 * ScaleRatio - 165 * ScaleRatio) * 100
                    BGM.settings.volume = Volume
                    SoundEffect.settings.volume = Volume
                Else
                    If SettingDotPressed Then
                        Play(SoundEffect, "Click.wav", False)
                        SettingDotSize = 16.0F
                        SettingDotBrush = New SolidBrush(Color.FromArgb(255, 255, 255))
                        SettingDotPressed = False
                    End If
                    SettingDotRect = New RectangleF(165 * ScaleRatio - SettingDotSize * ScaleRatio / 2 + ((MyWidth - 150 * ScaleRatio) - (165 * ScaleRatio)) / 100 * Volume,
                                                    163 * ScaleRatio + Fonts("Cubic", 23 * ScaleRatio).Height / 2 - SettingDotSize * ScaleRatio / 2,
                                                    SettingDotSize * ScaleRatio,
                                                    SettingDotSize * ScaleRatio)
                End If

                Select Case isPressed(MouseInArea(DoneSetting.Box), DoneSetting.isPressed, DoneSetting.Box)
                    Case 2
                        DoneSetting.Pic = My.Resources.Image.DoneSetting_Pressed
                    Case 3
                        State = "Menu"
                        DoneSetting.isPressed = False
                    Case Else
                        DoneSetting.Pic = My.Resources.Image.DoneSetting
                End Select

                Select Case isPressed(MouseInArea(F3_Switch.Box), F3_Switch.isPressed, F3_Switch.Box)
                    Case 2
                        If F3_Switch.isActivated Then
                            F3_Switch.Pic = My.Resources.Image.F3_ON_Pressed
                        Else
                            F3_Switch.Pic = My.Resources.Image.F3_OFF_Pressed
                        End If
                    Case 3
                        If F3_Switch.isActivated Then
                            F3_Switch.Pic = My.Resources.Image.F3_OFF
                            F3_Switch.isActivated = False
                        Else
                            F3_Switch.Pic = My.Resources.Image.F3_ON
                            F3_Switch.isActivated = True
                        End If
                        F3_Switch.isPressed = False
                    Case Else
                        If F3_Switch.isActivated Then
                            F3_Switch.Pic = My.Resources.Image.F3_ON
                        Else
                            F3_Switch.Pic = My.Resources.Image.F3_OFF
                        End If
                End Select

            Case "Intro"
                BGAnimation += 0.075
                If BGAnimation >= 360 Then
                    BGAnimation = 0
                End If

                Select Case Now.Ticks / 10000 - IntroStartTime
                    '音樂的撥放
                    Case 2875 To 3000
                        LoadingShow = False
                        Play(BGM, "Bach Air on the G String.mp3", False)
                    Case 12750 To 12875
                        Play(SoundEffect, "開場配音.wav", False)

                    '文字的動畫
                    Case 13000 To 15816
                        ShowTextLen = IntroText(0).Length
                        If TextIndex < IntroText(0).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(0)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 16040 To 17640
                        ShowTextLen = IntroText(1).Length
                        If TextIndex < IntroText(1).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(1)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 17704 To 19912
                        ShowTextLen = IntroText(2).Length
                        If TextIndex < IntroText(2).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(2)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 20104 To 22632
                        ShowTextLen = IntroText(3).Length
                        If TextIndex < IntroText(3).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(3)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 22696 To 24232
                        ShowTextLen = IntroText(4).Length
                        If TextIndex < IntroText(4).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(4)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 24392 To 27112
                        ShowTextLen = IntroText(5).Length
                        If TextIndex < IntroText(5).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(5)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 27656 To 29608
                        ShowTextLen = IntroText(6).Length
                        If TextIndex < IntroText(6).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(6)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 29736 To 32104
                        ShowTextLen = IntroText(7).Length
                        If TextIndex < IntroText(7).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(7)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 32360 To 35016
                        ShowTextLen = IntroText(8).Length
                        If TextIndex < IntroText(8).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(8)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 35592 To 36296
                        ShowTextLen = IntroText(9).Length
                        If TextIndex < IntroText(9).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(9)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 36488 To 37512
                        ShowTextLen = IntroText(10).Length
                        If TextIndex < IntroText(10).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(10)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 37864 To 39432
                        ShowTextLen = IntroText(11).Length
                        If TextIndex < IntroText(11).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(11)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 39784 To 42920
                        ShowTextLen = IntroText(12).Length
                        If TextIndex < IntroText(12).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(12)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 43432 To 45088
                        ShowTextLen = IntroText(13).Length
                        If TextIndex < IntroText(13).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(13)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 45150 To 46710
                        ShowTextLen = IntroText(14).Length
                        If TextIndex < IntroText(14).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(14)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 47478 To 49878
                        ShowTextLen = IntroText(15).Length
                        If TextIndex < IntroText(15).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(15)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 50166 To 51510
                        ShowTextLen = IntroText(16).Length
                        If TextIndex < IntroText(16).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(16)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 52086 To 52534
                        ShowTextLen = IntroText(17).Length
                        If TextIndex < IntroText(17).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(17)(TextIndex)
                                TextIndex += 1
                            End If
                        End If
                    Case 53270 To 57750
                        ShowTextLen = IntroText(18).Length
                        If TextIndex < IntroText(18).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                ShowText += IntroText(18)(TextIndex)
                                TextIndex += 1
                            End If
                        End If

                    '變暗的動畫
                    Case 54750 To 57000
                        BGColorA += 2
                        If BGColorA >= 255 Then
                            BGColorA = 255
                        End If
                        BGM.settings.volume -= 1
                        If BGM.settings.volume <= 0 Then
                            BGM.settings.volume = 0
                        End If
                    Case 57000 To 61000
                        Try
                            BGM.settings.volume = Volume
                        Catch
                        End Try
                        State = "Game"
                    Case Else
                        ShowText = ""
                        TextIndex = 0
                End Select

                Select Case isPressed(MouseInArea(MyWidth - 80 * ScaleRatio, 24 * ScaleRatio, MyWidth - 80 * ScaleRatio + Fonts("Cubic", 12 * ScaleRatio).Height * 3.5, 24 * ScaleRatio + Fonts("Cubic", 12 * ScaleRatio).Height), SkipisPressed, New RectangleF(MyWidth - 80 * ScaleRatio, 24 * ScaleRatio, MyWidth - 80 * ScaleRatio + Fonts("Cubic", 12 * ScaleRatio).Height * 3.5, 24 * ScaleRatio + Fonts("Cubic", 12 * ScaleRatio).Height))
                    Case 3
                        State = "Game"
                        LoadingShow = False
                        SkipisPressed = False
                End Select

            Case "Game"
                Map.Box.Width = 1690 * ScaleRatio
                Map.Box.Height = 1151 * ScaleRatio

                Player.Box.Width = 64 * ScaleRatio
                Player.Box.Height = 64 * ScaleRatio

                CameraBoarderSizeX = MyWidth / 2 - Player.Box.Width / 2
                CameraBoarderSizeY = MyHeight / 2 - Player.Box.Height / 2
                CameraBoarder = New RectangleF(CameraBoarderSizeX, CameraBoarderSizeY, MyWidth - CameraBoarderSizeX * 2, MyHeight - CameraBoarderSizeY * 2)
                MapBoarder = New RectangleF(50 * ScaleRatio, 50 * ScaleRatio, MyWidth - 50 * 2 * ScaleRatio, MyHeight - 50 * 2 * ScaleRatio)

                If Keyboard(Keys.W) Or Keyboard(Keys.Up) Then
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
                If Keyboard(Keys.A) Or Keyboard(Keys.Left) Then
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
                If Keyboard(Keys.S) Or Keyboard(Keys.Down) Then
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
                If Keyboard(Keys.D) Or Keyboard(Keys.Right) Then
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
                If Keyboard(Keys.W) And Keyboard(Keys.A) Or Keyboard(Keys.Up) And Keyboard(Keys.Left) Then
                    Player.angle = 315
                ElseIf Keyboard(Keys.W) And Keyboard(Keys.D) Or Keyboard(Keys.Up) And Keyboard(Keys.Right) Then
                    Player.angle = 45
                ElseIf Keyboard(Keys.S) And Keyboard(Keys.A) Or Keyboard(Keys.Down) And Keyboard(Keys.Left) Then
                    Player.angle = 225
                ElseIf Keyboard(Keys.S) And Keyboard(Keys.D) Or Keyboard(Keys.Down) And Keyboard(Keys.Right) Then
                    Player.angle = 135
                End If
        End Select
        Me.Invalidate()
    End Sub

    Private Sub Play(ByRef player As WMPLib.WindowsMediaPlayer, ByVal File As String, ByVal shouldLoop As Boolean)
        Try
            player.URL = My.Application.Info.DirectoryPath & "\Music\" + File
            Do While player.playState = 6 '等待載入音樂
            Loop
            player.controls.play()
            player.settings.setMode("Loop", shouldLoop)
        Catch
        End Try
    End Sub

    Overloads Function isPressed(ByRef InsideArea As Boolean, ByRef HasBeenPressed As Boolean, ByRef area As RectangleF) As Integer '0 = Nothing, 1 = 靠近, 2 = 按下, 3 = 放開
        If InsideArea Then
            If MousePressed And PointInArea(lastClickPoint, area) Then
                HasBeenPressed = True
                Return 2
            Else
                If HasBeenPressed Then
                    Play(SoundEffect, "Click.wav", False)
                    Return 3
                Else
                    Return 1
                End If
            End If
        End If
        HasBeenPressed = False
        Return 0
    End Function
    Overloads Function isPressed(ByRef InsideArea As Boolean, ByRef HasBeenPressed As Boolean, ByRef area As RectangleF, ByVal diameter As Double, ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Integer '0 = Nothing, 1 = 靠近, 2 = 按下, 3 = 放開
        Dim distant1 As Double = ((lastClickPoint.X - x1) ^ 2 + (lastClickPoint.Y - y1) ^ 2) ^ 0.5
        Dim distant2 As Double = ((lastClickPoint.X - x2) ^ 2 + (lastClickPoint.Y - y2) ^ 2) ^ 0.5
        If InsideArea Then
            If MousePressed And PointInArea(lastClickPoint, area) And Not distant1 <= diameter / 2 And Not distant2 <= diameter / 2 Then
                HasBeenPressed = True
                Return 2
            Else
                If HasBeenPressed Then
                    Return 3
                Else
                    Return 1
                End If
            End If
        End If
        HasBeenPressed = False
        Return 0
    End Function

    Overloads Function MouseInArea(ByRef rect As RectangleF) As Boolean
        If Mouse.X >= rect.Left And Mouse.X <= rect.Left + rect.Width And Mouse.Y >= rect.Top And Mouse.Y <= rect.Top + rect.Height Then
            Return True
        End If
        Return False
    End Function
    Overloads Function MouseInArea(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Boolean
        Dim tmp As Double
        If x1 < x2 Then
            tmp = x1
            x1 = x2
            x2 = tmp
        End If
        If y1 < y2 Then
            tmp = y1
            y1 = y2
            y2 = tmp
        End If
        If x1 >= Mouse.X And Mouse.X >= x2 And y1 >= Mouse.Y And Mouse.Y >= y2 Then
            Return True
        End If
        Return False
    End Function
    Overloads Function MouseInArea(ByVal diameter As Double, ByVal x As Double, ByVal y As Double) As Boolean
        Dim distant As Double = ((Mouse.X - x) ^ 2 + (Mouse.Y - y) ^ 2) ^ 0.5
        If distant <= diameter / 2 Then
            Return True
        End If
        Return False
    End Function
    Overloads Function PointInArea(ByVal pt As Point, ByVal rect As RectangleF) As Boolean
        If rect.Left <= pt.X And pt.X <= rect.Left + rect.Width And rect.Top <= pt.Y And pt.Y <= rect.Top + rect.Height Then
            Return True
        End If
        Return False
    End Function
    Overloads Function PointInArea(ByVal pt As Point, ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Boolean
        If x1 <= pt.X And pt.X <= x2 And y1 <= pt.Y And pt.Y <= y2 Then
            Return True
        End If
        Return False
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ScreenRefresh.SynchronizingObject = Me '使Timer的事件依附在本體的執行續
        AddHandler ScreenRefresh.Elapsed, AddressOf ScreenRefresh_Tick '分派事件到ScreenRefresh_Tick
        ScreenRefresh.AutoReset = True 'Timer的自動重新計時開啟

        MyWidth = Me.ClientSize.Width
        MyHeight = Me.ClientSize.Height

        '從Font.resx取出字體，並在記憶體中建立Font物件
        bytes = My.Resources.Font.Cubic11 '從Font.resx讀取字體，並寫入記憶體
        ptr = Marshal.AllocCoTaskMem(bytes.Length) '在記憶體中找位置放字體
        Marshal.Copy(bytes, 0, ptr, bytes.Length) '複製字體到記憶體中
        myfont.AddMemoryFont(ptr, bytes.Length) '物件化字體

        bytes = My.Resources.Font.Monoton
        ptr = Marshal.AllocCoTaskMem(bytes.Length)
        Marshal.Copy(bytes, 0, ptr, bytes.Length)
        myfont.AddMemoryFont(ptr, bytes.Length)

        For i = 0 To Keyboard.Length - 1
            Keyboard(i) = False
        Next
    End Sub

    Private Function Fonts(ByVal name As String, ByVal size As Integer) As Font
        Select Case name
            Case "Cubic"
                Return New Font(myfont.Families(1), size)
            Case "Monoton"
                Return New Font(myfont.Families(0), size)
        End Select
        Return New Font(myfont.Families(1), size)
    End Function
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        MyWidth = Me.ClientSize.Width
        MyHeight = Me.ClientSize.Height
        ScaleRatio = (MyWidth / 800 + MyHeight / 450) / 2
        If ScaleRatio > 2 Then
            ScaleRatio = 2
        End If
        Me.Invalidate()
    End Sub

    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        Mouse.X = e.X
        Mouse.Y = e.Y
    End Sub

    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        If Not MousePressed Then
            lastClickPoint = Mouse
        End If
        MousePressed = True
    End Sub

    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        MousePressed = False
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        ScreenRefresh.Enabled = True 'Timer啟動
        SoundEffect.settings.volume = Volume
        BGM.settings.volume = Volume
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Keyboard(e.KeyValue) = True
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        Keyboard(e.KeyValue) = False
        If e.KeyCode = Keys.F3 Then
            F3_Switch.isActivated = Not F3_Switch.isActivated
        End If
    End Sub
End Class
