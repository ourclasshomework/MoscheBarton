Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.Runtime.InteropServices
Imports System.Timers

Public Class Form1
    Structure MyPictureBox
        Dim Pic As Bitmap
        Dim Box As Rectangle
        Dim isPressed As Boolean
        Dim isBlinking As Boolean
        Dim angle As Integer
    End Structure

    '預設狀態值
    Dim State As String = "Logo"

    'Timer啟動
    Dim tRefresh As New Timer(1)

    '音量設定
    Dim Volume As Integer = 100

    '滑鼠初始化
    Dim Mouse As Point
    Dim MousePressed As Boolean = False

    '鍵盤初始化
    Dim KeyPressed(256) As Boolean

    '淡入淡出初始化
    Dim LogoMatrixItems As Single()() = {New Single() {1, 0, 0, 0, 0}, New Single() {0, 1, 0, 0, 0}, New Single() {0, 0, 1, 0, 0}, New Single() {0, 0, 0, 0, 0}, New Single() {0, 0, 0, 0, 1}}
    Dim LogoMatrix As New ColorMatrix(LogoMatrixItems)
    Dim LogoAttr As New ImageAttributes()

    '字體初始化
    Dim Cubic(3) As Font

    '背景初始化
    Dim BG As New MyPictureBox With {.Pic = My.Resources.Logos.bg}

    '開始畫面Logo淡入的特效初始化
    Dim Logo As Bitmap = My.Resources.Logos.Team
    Dim LogoBox As Rectangle
    Dim LogoNext As Boolean = False

    'Menu 初始化
    Dim MenuBG As New MyPictureBox With {.Pic = My.Resources.Menu.bg}
    Dim MenuLogo As New MyPictureBox With {.Pic = My.Resources.Menu.Logo}
    Dim ExitButton As New MyPictureBox With {.Pic = My.Resources.Menu._Exit, .isPressed = False}
    Dim SettingButton As New MyPictureBox With {.Pic = My.Resources.Menu.Setting, .isPressed = False}
    Dim EnterButton As New MyPictureBox With {.Pic = My.Resources.Menu.Enter, .isPressed = False, .isBlinking = True}
    Dim Version As New MyPictureBox With {.Pic = My.Resources.Menu.Version}
    Dim Copyright As New MyPictureBox With {.Pic = My.Resources.Menu.Copyright, .isPressed = False}

    'Setting 初始化
    Dim SettingLinePen As New Pen(Color.White, 3.55)
    Dim SettingBrush As New SolidBrush(Color.White)
    Dim VolumeLinePressed As Boolean = False
    Dim VolumeSlideSize As Integer = 20
    Dim VolumeSlideBox As New Rectangle(293 - VolumeSlideSize / 2, 256 - VolumeSlideSize / 2, VolumeSlideSize, VolumeSlideSize)
    Dim VolumeSlidePressed As Boolean = False
    Dim DoneSetting As New MyPictureBox With {.Pic = My.Resources.Menu.DoneSetting}

    'HowToPlay 初始化
    Dim TextTime As Long = 0
    Dim TextStart As Long = 0
    Dim TextIndex As Integer = 0
    Dim ShowText As String
    Dim TextSpeed As Integer = 100
    Dim CanTextRun As Boolean
    Dim TextPoint As PointF

    '遊戲本體初始化
    Dim Character As New MyPictureBox With {.Pic = My.Resources.Game.Character, .Box = New Rectangle(Me.ClientSize.Width / 2 - .Pic.Width / 2, Me.ClientSize.Height / 2 - .Pic.Height / 2, .Pic.Width, .Pic.Height), .angle = 0}
    Dim CharacterSpeed As Integer = 1

    '繪製畫面
    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        Select Case State
            Case "Logo"
                BG.Box = New Rectangle(0, 0, Me.ClientSize.Width, Me.ClientSize.Width / BG.Pic.Width * BG.Pic.Height)
                e.Graphics.DrawImage(BG.Pic, BG.Box)
                LogoBox = New Rectangle(Me.ClientSize.Width / 2 - Logo.Width / 3 / 2, Me.ClientSize.Height / 2 - Logo.Height / 3 / 2, Logo.Width / 3, Logo.Height / 3)
                LogoAttr.SetColorMatrix(LogoMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
                e.Graphics.DrawImage(Logo, LogoBox, 0, 0, Logo.Width, Logo.Height, GraphicsUnit.Pixel, LogoAttr)
            Case "Menu"
                MenuBG.Box = New Rectangle(0, 0, Me.ClientSize.Width, Me.ClientSize.Width / MenuBG.Pic.Width * MenuBG.Pic.Height)
                e.Graphics.DrawImage(MenuBG.Pic, MenuBG.Box)

                MenuLogo.Box = New Rectangle(Me.ClientSize.Width / 2 - MenuLogo.Pic.Width / 3 / 2, 107, MenuLogo.Pic.Width / 3, MenuLogo.Pic.Height / 3) '在 (畫面中央, 107) 顯示 Logo
                e.Graphics.DrawImage(MenuLogo.Pic, MenuLogo.Box)

                e.Graphics.DrawImage(ExitButton.Pic, ExitButton.Box)

                e.Graphics.DrawImage(SettingButton.Pic, SettingButton.Box)

                EnterButton.Box = New Rectangle(Me.ClientSize.Width / 2 - EnterButton.Pic.Width / 3 / 2, Me.ClientSize.Height - 187, EnterButton.Pic.Width / 3, EnterButton.Pic.Height / 3)
                LogoAttr.SetColorMatrix(LogoMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
                e.Graphics.DrawImage(EnterButton.Pic, EnterButton.Box, 0, 0, EnterButton.Pic.Width, EnterButton.Pic.Height, GraphicsUnit.Pixel, LogoAttr)

                Version.Box = New Rectangle(18, Me.ClientSize.Height - 44, Version.Pic.Width / 3, Version.Pic.Height / 3)
                e.Graphics.DrawImage(Version.Pic, Version.Box)

                Copyright.Box = New Rectangle(Me.ClientSize.Width - 535, Me.ClientSize.Height - 44, Copyright.Pic.Width / 3, Copyright.Pic.Height / 3)
                e.Graphics.DrawImage(Copyright.Pic, Copyright.Box)
            Case "Setting"
                BG.Box = New Rectangle(0, 0, Me.ClientSize.Width, Me.ClientSize.Width / BG.Pic.Width * BG.Pic.Height)
                e.Graphics.DrawImage(BG.Pic, BG.Box)

                e.Graphics.DrawString("設定", Cubic(0), SettingBrush, New PointF(109.66, 71.0))
                e.Graphics.DrawLine(SettingLinePen, 84, 171, Me.ClientSize.Width - 84, 171)

                e.Graphics.DrawString("音量", Cubic(1), SettingBrush, New PointF(151.0, 231.66))
                e.Graphics.DrawLine(SettingLinePen, 293, 256, Me.ClientSize.Width - 283, 256)
                e.Graphics.DrawString(Str(Volume), Cubic(1), SettingBrush, New PointF(Me.ClientSize.Width - 247.32, 231.66))
                e.Graphics.FillEllipse(SettingBrush, VolumeSlideBox)

                e.Graphics.DrawString("遊戲版本", Cubic(1), SettingBrush, New PointF(151.0, 314.66))
                e.Graphics.DrawString("v1.0", Cubic(1), SettingBrush, New PointF(Me.ClientSize.Width - 231.0, 314.66))

                e.Graphics.DrawImage(DoneSetting.Pic, DoneSetting.Box)
            Case "Introduction"
                BG.Box = New Rectangle(0, 0, Me.ClientSize.Width, Me.ClientSize.Width / BG.Pic.Width * BG.Pic.Height)
                e.Graphics.DrawImage(BG.Pic, BG.Box)

                e.Graphics.DrawString(ShowText, Cubic(1), SettingBrush, TextPoint)
            Case "Game3"
                BG.Box = New Rectangle(0, 0, Me.ClientSize.Width, Me.ClientSize.Width / BG.Pic.Width * BG.Pic.Height)
                e.Graphics.DrawImage(BG.Pic, BG.Box)

                e.Graphics.TranslateTransform(Character.Box.Left + Character.Box.Width / 2, Character.Box.Top + Character.Box.Height / 2)
                e.Graphics.RotateTransform(Character.angle)
                e.Graphics.TranslateTransform(-(Character.Box.Left + Character.Box.Width / 2), -(Character.Box.Top + Character.Box.Height / 2))
                e.Graphics.DrawImage(Character.Pic, Character.Box)
        End Select
    End Sub

    '畫面更新與遊戲判定
    Private Sub tRefresh_Tick(source As Object, e As ElapsedEventArgs)
        Select Case State
            '載入畫面
            Case "Logo"
                LogoMatrix.Matrix33 += 0.012
                If LogoMatrix.Matrix33 > 2 Then
                    If LogoNext Then
                        State = "Menu"
                        LogoMatrix.Matrix33 = 0

                        'Menu BGM播放
                        WMP.URL = ".\Music\MenuBGM.flac"
                        Do While WMP.playState = 6
                        Loop
                        WMP.Ctlcontrols.play()
                        WMP.settings.setMode("Loop", True)
                    End If
                    LogoMatrix.Matrix33 = 0
                    Logo = My.Resources.Logos.Logo
                    LogoNext = True
                End If
            '遊戲選單
            Case "Menu"
                '進入遊戲按鈕
                If LogoMatrix.Matrix33 <= 0 Then
                    EnterButton.isBlinking = True
                End If
                If LogoMatrix.Matrix33 >= 1.2 Then
                    EnterButton.isBlinking = False
                End If
                If EnterButton.isBlinking Then
                    LogoMatrix.Matrix33 += 0.012
                Else
                    LogoMatrix.Matrix33 -= 0.012
                End If
                If MouseInBox(323, MenuLogo.Box.Y + MenuLogo.Box.Height + 71, Me.ClientSize.Width - 323, Me.ClientSize.Height - 68) And MousePressed Then
                    EnterButton.isPressed = True
                ElseIf EnterButton.isPressed And MousePressed = False Then
                    EnterButton.isPressed = False
                    State = "Introduction"
                    WMP.URL = ".\Music\Introduction.mp3"
                    Do While WMP.playState = 6
                    Loop
                    WMP.Ctlcontrols.play()
                    WMP.settings.setMode("Loop", False)
                    TextStart = Now.Ticks
                Else
                    EnterButton.isPressed = False
                End If

                '退出遊戲按鈕
                If DistantFromMouse(87, Me.ClientSize.Height - 160, 28) And MousePressed Then
                    ExitButton.Pic = My.Resources.Menu.ExitActivated
                    ExitButton.Box = New Rectangle(56, Me.ClientSize.Height - 190, ExitButton.Pic.Width / 3, ExitButton.Pic.Height / 3)
                    ExitButton.isPressed = True
                ElseIf ExitButton.isPressed And MousePressed = False Then
                    ExitButton.isPressed = False
                    State = "Exit"
                Else
                    ExitButton.Pic = My.Resources.Menu._Exit
                    ExitButton.Box = New Rectangle(59, Me.ClientSize.Height - 187, ExitButton.Pic.Width / 3, ExitButton.Pic.Height / 3)
                    ExitButton.isPressed = False
                End If

                '設定按鈕
                If DistantFromMouse(Me.ClientSize.Width - 87, Me.ClientSize.Height - 160, 28) And MousePressed Then
                    SettingButton.Pic = My.Resources.Menu.SettingActivated
                    SettingButton.Box = New Rectangle(Me.ClientSize.Width - 118, Me.ClientSize.Height - 191, SettingButton.Pic.Width / 3, SettingButton.Pic.Height / 3)
                    SettingButton.isPressed = True
                ElseIf SettingButton.isPressed And MousePressed = False Then
                    SettingButton.isPressed = False
                    State = "Setting"
                Else
                    SettingButton.Pic = My.Resources.Menu.Setting
                    SettingButton.Box = New Rectangle(Me.ClientSize.Width - 115, Me.ClientSize.Height - 187, SettingButton.Pic.Width / 3, SettingButton.Pic.Height / 3)
                    SettingButton.isPressed = False
                End If

            '設定畫面
            Case "Setting"
                '音量控制條的控制頭
                If MousePressed = False Then
                    VolumeSlidePressed = False
                ElseIf VolumeSlidePressed Then
                    VolumeSlideBox.X = Mouse.X - VolumeSlideSize / 2
                    If VolumeSlideBox.X + VolumeSlideSize / 2 > Me.ClientSize.Width - 283 - VolumeSlideSize / 2 Then
                        VolumeSlideBox.X = Me.ClientSize.Width - 283 - VolumeSlideSize / 2
                    End If
                    If VolumeSlideBox.X + VolumeSlideSize / 2 < 293 - VolumeSlideSize / 2 Then
                        VolumeSlideBox.X = 293 - VolumeSlideSize / 2
                    End If
                    Volume = (VolumeSlideBox.X + VolumeSlideSize / 2 - 293) / (Me.ClientSize.Width - 283 - 293) * 100
                ElseIf DistantFromMouse(VolumeSlideBox.X + VolumeSlideSize / 2, VolumeSlideBox.Y + VolumeSlideSize / 2, VolumeSlideSize / 2) Then
                    VolumeSlidePressed = True
                Else
                    VolumeSlidePressed = False
                End If

                '音量控制條的條
                If MousePressed = False Then
                    VolumeLinePressed = False
                ElseIf VolumeLinePressed Then
                    VolumeSlideBox.X = Mouse.X - VolumeSlideSize / 2
                    If VolumeSlideBox.X + VolumeSlideSize / 2 > Me.ClientSize.Width - 283 Then
                        VolumeSlideBox.X = Me.ClientSize.Width - 283 - VolumeSlideSize / 2
                    End If
                    If VolumeSlideBox.X + VolumeSlideSize / 2 < 293 Then
                        VolumeSlideBox.X = 293 - VolumeSlideSize / 2
                    End If
                    Volume = (VolumeSlideBox.X + VolumeSlideSize / 2 - 293) / (Me.ClientSize.Width - 283 - 293) * 100
                ElseIf MouseInBox(293, 252, Me.ClientSize.Width - 283, 260) Then
                    VolumeLinePressed = True
                Else
                    VolumeLinePressed = False
                End If

                '完成設定按鈕
                If MouseInBox(Me.ClientSize.Width - 339, Me.ClientSize.Height - 141, Me.ClientSize.Width - 102, Me.ClientSize.Height - 57) And MousePressed Then
                    DoneSetting.Pic = My.Resources.Menu.DoneSettingActivated
                    DoneSetting.Box = New Rectangle(Me.ClientSize.Width - 341, Me.ClientSize.Height - 143, DoneSetting.Pic.Width / 3, DoneSetting.Pic.Height / 3)
                    DoneSetting.isPressed = True
                ElseIf DoneSetting.isPressed And MousePressed = False Then
                    DoneSetting.isPressed = False
                    State = "Menu"
                Else
                    DoneSetting.Pic = My.Resources.Menu.DoneSetting
                    DoneSetting.Box = New Rectangle(Me.ClientSize.Width - 339, Me.ClientSize.Height - 141, DoneSetting.Pic.Width / 3, DoneSetting.Pic.Height / 3)
                    DoneSetting.isPressed = False
                End If

                '音量更新
                WMP.settings.volume = Volume

            '如何遊玩畫面
            Case "Introduction"
                '文字動畫
                TextTime = Now.Ticks
                CanTextRun = Int((TextTime - TextStart) / 10000) Mod TextSpeed <= 25
                Select Case Int((TextTime - TextStart) / 10000)
                    Case 316 To 972
                        If ShowText.Length < My.Resources.Game.Intro1.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro1(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro1.Length * 44 / 2
                    Case 2118 To 5000
                        If ShowText.Length < My.Resources.Game.Intro2.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro2(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro2.Length * 44 / 2
                    Case 6608 To 8996
                        If ShowText.Length < My.Resources.Game.Intro3.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro3(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro3.Length * 44 / 2
                    Case 9359 To 12598
                        If ShowText.Length < My.Resources.Game.Intro4.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro4(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro4.Length * 44 / 2
                    Case 14500 To 15320
                        If ShowText.Length < My.Resources.Game.Intro5.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro5(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro5.Length * 44 / 2
                    Case 15744 To 20275
                        If ShowText.Length < My.Resources.Game.Intro6.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro6(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro6.Length * 44 / 2
                    Case 22292 To 24388
                        If ShowText.Length < My.Resources.Game.Intro7.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro7(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro7.Length * 44 / 2
                    Case 24806 To 27024
                        If ShowText.Length < My.Resources.Game.Intro8.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro8(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro8.Length * 44 / 2
                    Case 27415 To 30675
                        If ShowText.Length < My.Resources.Game.Intro9.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro9(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro9.Length * 44 / 2
                    Case 33296 To 34671
                        If ShowText.Length < My.Resources.Game.Intro10.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro10(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro10.Length * 44 / 2
                    Case 35399 To 38064
                        If ShowText.Length < My.Resources.Game.Intro11.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro11(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro11.Length * 44 / 2
                    Case 39130 To 41501
                        If ShowText.Length < My.Resources.Game.Intro12.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro12(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro12.Length * 44 / 2
                    Case 42096 To 43776
                        If ShowText.Length < My.Resources.Game.Intro13.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro13(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro13.Length * 44 / 2
                    Case 44016 To 48205
                        If ShowText.Length < My.Resources.Game.Intro14.Length And CanTextRun Then
                            ShowText += My.Resources.Game.Intro14(TextIndex)
                            TextIndex += 1
                        End If
                        TextPoint.X = Me.ClientSize.Width / 2 - My.Resources.Game.Intro14.Length * 44 / 2
                    Case > 48206
                        State = "Game3"
                        WMP.Ctlcontrols.stop()
                        TextIndex = 0
                        TextStart = 0
                    Case Else
                        TextIndex = 0
                        ShowText = ""
                End Select
                TextPoint.Y = Me.ClientSize.Height - 125
            Case "Game3"
                '角色方向改變
                If KeyPressed(Keys.W) Then
                    If KeyPressed(Keys.D) Then
                        Character.Box.X += CharacterSpeed
                        Character.angle = 45
                    ElseIf KeyPressed(Keys.A) Then
                        Character.Box.X -= CharacterSpeed
                        Character.angle = 315
                    Else
                        Character.angle = 0
                    End If
                    Character.Box.Y -= CharacterSpeed
                ElseIf KeyPressed(Keys.S) Then
                    If KeyPressed(Keys.A) Then
                        Character.Box.X -= CharacterSpeed
                        Character.angle = 225
                    ElseIf KeyPressed(Keys.D) Then
                        Character.Box.X += CharacterSpeed
                        Character.angle = 135
                    Else
                        Character.angle = 180
                    End If
                    Character.Box.Y += CharacterSpeed
                ElseIf KeyPressed(Keys.A) Then
                    If KeyPressed(Keys.W) Then
                        Character.Box.Y -= CharacterSpeed
                        Character.angle = 315
                    ElseIf KeyPressed(Keys.S) Then
                        Character.Box.Y += CharacterSpeed
                        Character.angle = 225
                    Else
                        Character.angle = 270
                    End If
                    Character.Box.X -= CharacterSpeed
                ElseIf KeyPressed(Keys.D) Then
                    If KeyPressed(Keys.W) Then
                        Character.Box.Y -= CharacterSpeed
                        Character.angle = 45
                    ElseIf KeyPressed(Keys.S) Then
                        Character.Box.Y += CharacterSpeed
                        Character.angle = 135
                    Else
                        Character.angle = 90
                    End If
                    Character.Box.X += CharacterSpeed
                End If

                '擋牆
                If Character.Box.X < 0 Then
                    Character.Box.X = 0
                End If
                If Character.Box.X > Me.ClientSize.Width - Character.Box.Width Then
                    Character.Box.X = Me.ClientSize.Width - Character.Box.Width
                End If
                If Character.Box.Y < 0 Then
                    Character.Box.Y = 0
                End If
                If Character.Box.Y > Me.ClientSize.Height - Character.Box.Height Then
                    Character.Box.Y = Me.ClientSize.Height - Character.Box.Height
                End If
            Case "Exit"
                Me.Close()
        End Select
        Me.Invalidate()
    End Sub

    Private Function DistantFromMouse(ByVal x As Integer, ByVal y As Integer, ByVal distant As Integer) As Boolean
        If distant >= Math.Sqrt((Mouse.X - x) * (Mouse.X - x) + (Mouse.Y - y) * (Mouse.Y - y)) Then
            Return True
        End If
        Return False
    End Function

    Private Function MouseInBox(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer) As Boolean
        If (Mouse.X >= x1) And (Mouse.X <= x2) And (Mouse.Y >= y1) And (Mouse.Y <= y2) Then
            Return True
        End If
        Return False
    End Function

    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        Mouse.X = e.X
        Mouse.Y = e.Y
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        Me.Invalidate()
    End Sub

    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        MousePressed = True
    End Sub

    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        MousePressed = False
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '載入字體
        Static Dim font As New PrivateFontCollection
        Static Dim bytes As Byte() = My.Resources.Fonts.Cubic
        Static Dim ptr As IntPtr = Marshal.AllocCoTaskMem(bytes.Length)
        Marshal.Copy(bytes, 0, ptr, bytes.Length)
        font.AddMemoryFont(ptr, bytes.Length)
        Cubic(0) = New Font(font.Families(0), 48)
        Cubic(1) = New Font(font.Families(0), 30)
        Cubic(2) = New Font(font.Families(0), 24)

        For i = 0 To 255
            KeyPressed(i) = False
        Next

        tRefresh.SynchronizingObject = Me
        AddHandler tRefresh.Elapsed, AddressOf tRefresh_Tick
        tRefresh.AutoReset = True
        tRefresh.Enabled = True
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        VolumeSlideBox = New Rectangle(Me.ClientSize.Width - 283 - VolumeSlideSize / 2, 256 - VolumeSlideSize / 2, VolumeSlideSize, VolumeSlideSize)
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        KeyPressed(e.KeyCode) = True
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        KeyPressed(e.KeyCode) = False
    End Sub
End Class