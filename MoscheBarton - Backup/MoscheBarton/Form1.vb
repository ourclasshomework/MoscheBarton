Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Runtime.InteropServices
Imports System.Timers

Public Class Form1
    Public Class MyTextBox '自定義文字方塊
        Public font As String '選擇要繪製的字體
        Public font_size As Double '字體大小
        Private DrawFont As Font '要繪製文字的字體
        Public color As Color '顏色
        Public opacity As Double = 255.0 '透明度，預設是255.0 (完全不透明)
        Public point As PointF '繪製的左上角座標
        Private format As New StringFormat '字體集
        Public Align As String = "Left" '垂直對齊
        Public LineAlign As String = "Top" '水平對齊

        '查詢字體高度
        Public Function FontHeight(ByRef e As PaintEventArgs, ByRef myfont As PrivateFontCollection, ScaleRatio As Double) As Integer
            Select Case font
                Case "Cubic11" '如果選擇 "Cubic11" 就把 DrawFont 設成 Cubic
                    DrawFont = New Font(myfont.Families(1), font_size * ScaleRatio)
                Case "Monoton" '如果選擇 "Monoton" 就把 DrawFont 設成 Monoton
                    DrawFont = New Font(myfont.Families(0), font_size * ScaleRatio)
                Case Else '如果未設定就設定成 微軟正黑體
                    If font = Nothing Then
                        DrawFont = New Font("微軟正黑體", font_size * ScaleRatio)
                    Else '如果選擇 其他 就把 DrawFont 設成 其他
                        DrawFont = New Font(font, font_size * ScaleRatio)
                    End If
            End Select
            Return DrawFont.Height '回傳自體高度
        End Function

        '設定對齊
        Public Sub AlignSetting(alignment As String, linealignment As String)
            Align = alignment
            LineAlign = linealignment
        End Sub

        '繪製文字
        Public Sub Draw(Text As String, ByRef e As PaintEventArgs, myfont As PrivateFontCollection, ScaleRatio As Double)
            Select Case font
                Case "Cubic11" '如果選擇 "Cubic11" 就把 DrawFont 設成 Cubic
                    DrawFont = New Font(myfont.Families(1), font_size * ScaleRatio)
                Case "Monoton" '如果選擇 "Monoton" 就把 DrawFont 設成 Monoton
                    DrawFont = New Font(myfont.Families(0), font_size * ScaleRatio)
                Case Else '如果未設定就設定成 微軟正黑體
                    If font = Nothing Then
                        DrawFont = New Font("微軟正黑體", font_size * ScaleRatio)
                    Else '如果選擇 其他 就把 DrawFont 設成 其他
                        DrawFont = New Font(font, font_size * ScaleRatio)
                    End If
            End Select

            Select Case Align '垂直對齊
                Case "Left" '左
                    format.Alignment = StringAlignment.Near
                Case "Center" '中
                    format.Alignment = StringAlignment.Center
                Case "Right" '右
                    format.Alignment = StringAlignment.Far
            End Select

            Select Case LineAlign '水平對齊
                Case "Top" '上
                    format.LineAlignment = StringAlignment.Near
                Case "Center" '中
                    format.LineAlignment = StringAlignment.Center
                Case "Bottom" '下
                    format.LineAlignment = StringAlignment.Far
            End Select

            If opacity > 255.0 Then '如果 透明度 > 255.0 就把 透明度 設成 255.0
                opacity = 255.0
            ElseIf opacity < 0.0 Then '如果 透明度 < 0.0 就把 透明度 設成 0.0
                opacity = 0.0
            End If

            e.Graphics.DrawString(Text, DrawFont, New SolidBrush(Color.FromArgb(opacity, color)), point, format) '繪製文字
        End Sub
    End Class

    Public Class MyPictureBox
        Public Box As RectangleF '圖片的繪製框
        Public Image As Bitmap '圖片

        Public Sub BoxSetting(x1 As Single, y1 As Single, Height As Single, Width As Single) '繪製框設定
            Box = New RectangleF(x1, y1, Height, Width)
        End Sub

        Public Sub Draw(ByRef e As PaintEventArgs) '在繪製框內畫圖片
            e.Graphics.DrawImage(Image, Box)
        End Sub
    End Class

    Public Class MyButton
        Public Box As RectangleF '按鈕框
        Public Image As Bitmap '按鈕圖片
        Public PressedImage As Bitmap '壓下的按鈕的圖片
        Public Activated As Boolean = False '是否被按下過，預設是 False
        Public Player As WMPLib.WindowsMediaPlayer

        '繪製按鈕
        Public Function Draw(ByRef e As PaintEventArgs, Mouse As Point, MousePressed As Boolean, Volume As Double) As Integer
            If Box.X <= Mouse.X And Mouse.X <= Box.X + Box.Width And Box.Y <= Mouse.Y And Mouse.Y <= Box.Y + Box.Height Then '游標在按鈕上
                If MousePressed Then '滑鼠按下
                    Activated = True '曾按過 = True
                    e.Graphics.DrawImage(PressedImage, Box) '繪製已按過的圖片
                    Return 1 '回傳 「按下，但還沒放開」
                Else '滑鼠未按
                    e.Graphics.DrawImage(Image, Box) '繪製未按過的圖片
                    If Activated Then '如果曾按過
                        Activated = False '曾按過 = False
                        '播放音效
                        Player.URL = My.Application.Info.DirectoryPath & "\Music\Click.wav"
                        Player.settings.volume = Volume
                        Player.controls.play()
                        Return 3 '回傳 「放開，已按過，需觸發事件」
                    Else
                        Return 0 '回傳 「放開，未曾按過」
                    End If
                End If
            Else
                If MousePressed Then '滑鼠按下
                    If Activated Then '曾按過
                        Activated = True '按過 = True
                        e.Graphics.DrawImage(PressedImage, Box) '繪製已按過的圖片
                        Return 2 '回傳「不再按鈕上，但曾按過，且未放開」
                    Else
                        e.Graphics.DrawImage(Image, Box) '繪製放開的圖片
                        Return 0 '回傳 「放開，未曾按過」
                    End If
                Else
                    Activated = False '曾按過 = False
                    e.Graphics.DrawImage(Image, Box) '繪製放開的圖片
                    Return 0 '回傳 「放開，未曾按過」
                End If
            End If

            e.Graphics.DrawImage(Image, Box)
            Return 0 '回傳 「放開，未曾按過」
        End Function
    End Class

    Public Sub PlaySound(ByRef Player As WMPLib.WindowsMediaPlayer, File As String, ShouldLoop As Boolean)
        Player.URL = My.Application.Info.DirectoryPath & "\Music\" & File '選擇路徑
        Player.settings.setMode("loop", ShouldLoop) '設定是否循環
        Player.controls.play() '播放
    End Sub

    Public Sub WMPExit(ByRef Player As WMPLib.WindowsMediaPlayer)
        Player.controls.stop() '停止
        Player.close() '結束
    End Sub

    Public Class Slider
        Public X1 As Double '左邊點的X座標
        Public X2 As Double '右邊點的X座標
        Public Y As Double '左右邊點的Y座標
        Private HeadX As Double '控制頭的X座標
        Private Activated As Boolean = False '是否按下控制頭
        Public value As Integer = 100 '值
        Public color As Color = Color.White '顏色
        Public Activated_color As Color = Color.FromArgb(157, 157, 157) '控制頭按下的顏色
        Public width As Double = 4 '線寬
        Public head_width As Double = width * 3.6 '頭寬

        '控制桿的文字
        Public Name As New MyTextBox With {.LineAlign = "Center", .color = color}
        Public NameText As String
        '數值的文字
        Public ValueText As New MyTextBox With {.LineAlign = "Center", .color = color}

        '控制桿的文字設定
        Public Sub NameSetting(Text As String, font As String, font_size As String, xpt1 As Double, ypt1 As Double)
            NameText = Text
            Name.font = font
            Name.font_size = font_size
            Name.point.X = xpt1
            Name.point.Y = ypt1
        End Sub

        '數值的文字設定
        Public Sub ValueTextSetting(font As String, font_size As String, xpt1 As Double, ypt1 As Double)
            ValueText.font = font
            ValueText.font_size = font_size
            ValueText.point.X = xpt1
            ValueText.point.Y = ypt1
        End Sub

        '座標設定
        Public Sub Setting(xpt1 As Double, xpt2 As Double, ypt12 As Double)
            X1 = xpt1
            X2 = xpt2
            Y = ypt12
        End Sub

        '繪製控制桿
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

    Public Class Game
        Public x As Double
        Public y As Double
        Public width As Double
        Public height As Double
        Public BG As Bitmap
        Private Const G As Double = 9.80665 / 1000 '重力加速度常數 (格/ms^2)
        Public LastUpdate As Long '(ms)
        Private CameraX As Double = 0
        Private CameraY As Double = 0

        Public SoundEffect As WMPLib.WindowsMediaPlayer

        Private WallDetectX0 As Integer
        Private WallDetectX08 As Integer
        Private WallDetectY0 As Integer
        Private WallDetectY09 As Integer
        Private WallDetectY18 As Integer
        Private CharacterLeft As Boolean
        Private CharacterRight As Boolean
        Private CharacterTop As Boolean
        Private CharacterBottom As Boolean

        Public Map As Integer(,)
        Public Map_Width As Integer
        Public Map_Height As Integer
        Private Map_Texture As Bitmap() = {My.Resources.Game.Cobblestone, My.Resources.Game.Cobblestone}
        Private MapDx As Double = 0
        Private MapDy As Double = 0

        Public myfont As PrivateFontCollection
        Private DamageText As New MyTextBox With {.font = "Cubic11", .font_size = 24, .color = Color.FromArgb(244, 67, 54), .Align = "Center", .LineAlign = "Center"}
        Private ShowDamage As Boolean = False
        Private LastShowDamage As Long = 0

        Private CharacterImg As Bitmap = My.Resources.Game.CRNN
        Private CharacterAttackWidth As Double = 0
        Public CharacterHealth As Double = 100
        Private CharacterWidth As Double = 27
        Private CharacterHeight As Double = 84
        Private CharacterHitBoxWidth As Double = 27 / 48
        Private CharacterHitBoxHeight As Double = 84 / 48
        Private CharacterMoveAni As Integer = 0
        Private CharacterMoveAniTimer As Long
        Private CharacterDirection As Boolean = False 'False = 右，True = 左

        Public CharacterBox As RectangleF
        Public CharacterX As Double
        Public CharacterY As Double
        Private Character_Speed As Double = 0.075 '(格/每次更新)
        Private CharacterYv As Double = 0

        Private BulletImg As Bitmap = My.Resources.Game.BulletR
        Public BulletBox As RectangleF
        Public BulletX As Double
        Public BulletY As Double
        Private BulletDetectX As Integer
        Private BulletDetectY As Integer
        Private Const BulletSpeed As Double = 0.3 '格/每次更新
        Private BulletDirection As Boolean = False 'False = 右，True = 左
        Private CanShoot As Boolean = True
        Private LastShoot As Long = 0
        Private Const ShootGap As Long = 250 'ms
        Private HadShot As Boolean = False

        Private Character_Jump As Boolean = False
        Private Character_CanJump As Boolean = True
        Private Const Character_Jump_Speed As Double = 0.075 '(格/ms)
        Private Jump_Delay As Integer = 350 '(ms)
        Private Last_Jump As Long = 0

        Private MonsterImg As Bitmap = My.Resources.Game.SlimeL
        Private Monster_Texture As Bitmap() = {My.Resources.Game.SlimeL, My.Resources.Game.SlimeR}
        Private MonsterDirection As Boolean = False 'False = 右，True = 左
        Private Monster_Speed As Double = 0.01 '(格/每次更新)
        Public MonsterBox As RectangleF
        Public MonsterX As Double = 9
        Public MonsterY As Double = 4
        Private MonsterYv As Double = 0
        Public MonsterWidth As Double = 63
        Public MonsterHeight As Double = 84
        Public MonsterType As Integer = 0
        Public MonsterHealth As Double = 100
        Private MonsterDetectArea As Double = 3.5 '格

        Private MonsterLeft As Boolean
        Private MonsterRight As Boolean
        Private MonsterTop As Boolean
        Private MonsterBottom As Boolean

        Public Sub CameraReset()
            CameraX = 0
            CameraY = 0
        End Sub

        Public Sub BoxSetting(ptx As Double, pty As Double, ptwidth As Double, ptheight As Double)
            x = ptx
            y = pty
            width = ptwidth
            height = ptheight
        End Sub

        Private Sub Detect()
            WallDetectX0 = Math.Floor(CharacterX)
            WallDetectX08 = Math.Floor(CharacterX + CharacterHitBoxWidth - 0.000001)
            WallDetectY0 = Math.Floor(Map_Height - (CharacterY + 0.000001))
            WallDetectY09 = Math.Floor(Map_Height - (CharacterY + CharacterHitBoxHeight / 2))
            WallDetectY18 = Math.Floor(Map_Height - (CharacterY + CharacterHitBoxHeight - 0.000001))

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

            If WallDetectY0 <= 0 Then
                WallDetectY0 = 0
            ElseIf WallDetectY0 > Map_Height - 1 Then
                WallDetectY0 = Map_Height - 1
            End If

            If WallDetectY09 <= 0 Then
                WallDetectY09 = 0
            ElseIf WallDetectY09 > Map_Height - 1 Then
                WallDetectY09 = Map_Height - 1
            End If

            If WallDetectY18 <= 0 Then
                WallDetectY18 = 0
            ElseIf WallDetectY18 > Map_Height - 1 Then
                WallDetectY18 = Map_Height - 1
            End If

            CharacterLeft = (Map(WallDetectY0, WallDetectX0) <> 0 Or Map(WallDetectY09, WallDetectX0) <> 0 Or Map(WallDetectY18, WallDetectX0) <> 0) And Math.Floor(CharacterX) < CharacterX And CharacterX < Math.Floor(CharacterX) + 1
            CharacterRight = (Map(WallDetectY0, WallDetectX08) <> 0 Or Map(WallDetectY09, WallDetectX08) <> 0 Or Map(WallDetectY18, WallDetectX08) <> 0) And Math.Floor(CharacterX + CharacterHitBoxWidth) < CharacterX + CharacterHitBoxWidth And CharacterX + CharacterHitBoxWidth < Math.Floor(CharacterX + CharacterHitBoxWidth) + 1
            CharacterTop = (Map(WallDetectY18, WallDetectX0) <> 0 Or Map(WallDetectY18, WallDetectX08) <> 0) And Math.Floor(CharacterY + CharacterHitBoxHeight) < CharacterY + CharacterHitBoxHeight And CharacterY + CharacterHitBoxHeight < Math.Floor(CharacterY + CharacterHitBoxHeight) + 1
            CharacterBottom = (Map(WallDetectY0, WallDetectX0) <> 0 Or Map(WallDetectY0, WallDetectX08) <> 0) And Math.Floor(CharacterY) < CharacterY And CharacterY < Math.Floor(CharacterY) + 1
        End Sub

        Public Sub MonsterDetect()
            WallDetectX0 = Math.Floor(MonsterX)
            WallDetectX08 = Math.Floor(MonsterX + MonsterWidth / 48 - 0.000001)
            WallDetectY0 = Math.Floor(Map_Height - (MonsterY + 0.000001))
            WallDetectY09 = Math.Floor(Map_Height - (MonsterY + MonsterHeight / 48 / 2))
            WallDetectY18 = Math.Floor(Map_Height - (MonsterY + MonsterHeight / 48 - 0.000001))

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

            If WallDetectY0 <= 0 Then
                WallDetectY0 = 0
            ElseIf WallDetectY0 > Map_Height - 1 Then
                WallDetectY0 = Map_Height - 1
            End If

            If WallDetectY09 <= 0 Then
                WallDetectY09 = 0
            ElseIf WallDetectY09 > Map_Height - 1 Then
                WallDetectY09 = Map_Height - 1
            End If

            If WallDetectY18 <= 0 Then
                WallDetectY18 = 0
            ElseIf WallDetectY18 > Map_Height - 1 Then
                WallDetectY18 = Map_Height - 1
            End If

            MonsterLeft = (Map(WallDetectY0, WallDetectX0) <> 0 Or Map(WallDetectY09, WallDetectX0) <> 0 Or Map(WallDetectY18, WallDetectX0) <> 0) And Math.Floor(MonsterX) < MonsterX And MonsterX < Math.Floor(MonsterX) + 1
            MonsterRight = (Map(WallDetectY0, WallDetectX08) <> 0 Or Map(WallDetectY09, WallDetectX08) <> 0 Or Map(WallDetectY18, WallDetectX08) <> 0) And Math.Floor(MonsterX + MonsterWidth / 48) < MonsterX + MonsterWidth / 48 And MonsterX + MonsterWidth / 48 < Math.Ceiling(MonsterX + MonsterWidth / 48)
            MonsterTop = (Map(WallDetectY18, WallDetectX0) <> 0 Or Map(WallDetectY18, WallDetectX08) <> 0) And Math.Floor(MonsterY + MonsterHeight / 48) < MonsterY + MonsterHeight / 48 And MonsterY + MonsterHeight / 48 < Math.Floor(MonsterY + MonsterHeight / 48) + 1
            MonsterBottom = (Map(WallDetectY0, WallDetectX0) <> 0 Or Map(WallDetectY0, WallDetectX08) <> 0) And Math.Floor(MonsterY) < MonsterY And MonsterY < Math.Floor(MonsterY) + 1
        End Sub

        Private Sub CharacterImgChange(ByRef e As PaintEventArgs, ByRef Keyboard() As Boolean, ByRef Mouse As Point, ByRef MousePressed As Boolean, ByRef ScaleRatio As Double)
            If MousePressed Then
                If Keyboard(Keys.A) Then
                    CharacterWidth = 67.0383
                    CharacterHeight = 84.5916
                    CharacterAttackWidth = 27
                    CharacterHitBoxWidth = 27 / 48
                    CharacterHitBoxHeight = 84.5916 / 48

                    Select Case CharacterMoveAni
                        Case 0
                            CharacterImg = My.Resources.Game.CLAW0
                        Case 1
                            CharacterImg = My.Resources.Game.CLAW1
                        Case 2
                            CharacterImg = My.Resources.Game.CLAW2
                    End Select

                    If CanShoot Then
                        BulletDirection = True
                    End If
                End If

                If Keyboard(Keys.D) Then
                    CharacterWidth = 67.0383
                    CharacterHeight = 84.5916
                    CharacterAttackWidth = 0
                    CharacterHitBoxWidth = 40.0383 / 48
                    CharacterHitBoxHeight = 84.5916 / 48

                    Select Case CharacterMoveAni
                        Case 0
                            CharacterImg = My.Resources.Game.CRAW0
                        Case 1
                            CharacterImg = My.Resources.Game.CRAW1
                        Case 2
                            CharacterImg = My.Resources.Game.CRAW2
                    End Select

                    If CanShoot Then
                        BulletDirection = False
                    End If
                End If


                If Not Keyboard(Keys.A) And Not Keyboard(Keys.D) Then
                    CharacterWidth = 58.0383
                    CharacterHeight = 84
                    CharacterHitBoxWidth = 27 / 48
                    CharacterHitBoxHeight = 84 / 48

                    If CharacterDirection Then
                        CharacterAttackWidth = -12.1249
                        CharacterImg = My.Resources.Game.CRAN
                        If CanShoot Then
                            BulletDirection = False
                        End If
                    Else
                        CharacterAttackWidth = 31.0383 - 12.1249
                        CharacterImg = My.Resources.Game.CLAN
                        If CanShoot Then
                            BulletDirection = True
                        End If
                    End If
                End If

                If CanShoot And Now.Ticks() / 10000 - LastShoot > ShootGap And Not HadShot Then
                    LastShoot = Now.Ticks() / 10000
                    If BulletDirection Then
                        BulletX = CharacterX
                    Else
                        BulletX = CharacterX + 1.35
                    End If
                    BulletY = CharacterY + 0.9
                    CanShoot = False
                    HadShot = True
                    SoundEffect.URL = My.Application.Info.DirectoryPath & "\Music\射出.mp3" '選擇路徑
                    SoundEffect.settings.setMode("loop", False) '設定是否循環
                    SoundEffect.controls.play() '播放
                End If
            Else
                HadShot = False
                If Keyboard(Keys.A) Then
                    CharacterWidth = 39.1249
                    CharacterHeight = 84.5916
                    CharacterAttackWidth = 0
                    CharacterHitBoxWidth = 39.1249 / 48
                    CharacterHitBoxHeight = 84.5916 / 48

                    Select Case CharacterMoveAni
                        Case 0
                            CharacterImg = My.Resources.Game.CLNW0
                        Case 1
                            CharacterImg = My.Resources.Game.CLNW1
                        Case 2
                            CharacterImg = My.Resources.Game.CLNW2
                    End Select
                End If

                If Keyboard(Keys.D) Then
                    CharacterWidth = 39.1249
                    CharacterHeight = 84.5916
                    CharacterAttackWidth = 0
                    CharacterHitBoxWidth = 39.1249 / 48
                    CharacterHitBoxHeight = 84.5916 / 48

                    Select Case CharacterMoveAni
                        Case 0
                            CharacterImg = My.Resources.Game.CRNW0
                        Case 1
                            CharacterImg = My.Resources.Game.CRNW1
                        Case 2
                            CharacterImg = My.Resources.Game.CRNW2
                    End Select
                End If

                If Not Keyboard(Keys.A) And Not Keyboard(Keys.D) Then
                    CharacterWidth = 27
                    CharacterHeight = 84
                    CharacterAttackWidth = -12.1249
                    CharacterHitBoxWidth = 27 / 48
                    CharacterHitBoxHeight = 84.5916 / 48

                    If CharacterDirection Then
                        CharacterImg = My.Resources.Game.CRNN
                    Else
                        CharacterImg = My.Resources.Game.CLNN
                    End If
                End If
            End If

            If CanShoot = False Then
                If BulletDirection Then
                    BulletImg = My.Resources.Game.BulletL
                    BulletX -= BulletSpeed
                Else
                    BulletImg = My.Resources.Game.BulletR
                    BulletX += BulletSpeed
                End If

                If BulletDirection Then
                    If BulletX < MonsterX And BulletY - 0.9 < MonsterY And MonsterY < BulletY - 0.9 + MonsterHeight / 48 Then
                        CanShoot = True
                        MonsterDamage(3, e, ScaleRatio)
                        MonsterHealth -= 3
                        LastShowDamage = Now.Ticks() / 10000
                        ShowDamage = True
                    End If
                Else
                    If BulletX > MonsterX And BulletY - 0.9 < MonsterY And MonsterY < BulletY - 0.9 + MonsterHeight / 48 Then
                        CanShoot = True
                        MonsterDamage(3, e, ScaleRatio)
                        MonsterHealth -= 3
                        LastShowDamage = Now.Ticks() / 10000
                        ShowDamage = True
                    End If
                End If

                BulletDetectY = Math.Floor(Map_Height - BulletY - 1)
                BulletDetectX = Math.Floor(BulletX + 0.25)
                If BulletDetectY >= Map_Height Then
                    BulletDetectY = Map_Height - 1
                End If
                If BulletDetectY < 0 Then
                    BulletDetectY = 0
                End If
                If BulletDetectX >= Map_Width Then
                    BulletDetectX = Map_Width - 1
                End If
                If BulletDetectX < 0 Then
                    BulletDetectX = 0
                End If

                If Map(BulletDetectY, BulletDetectX) <> 0 And Not Map(BulletDetectY, BulletDetectX) >= 100 Then
                    CanShoot = True
                End If

                BulletBox = New RectangleF(x + CameraX + BulletX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                       y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - BulletY) * 48 * ScaleRatio - 24 * ScaleRatio - 10 * ScaleRatio,
                                                       12 * ScaleRatio, 24 * ScaleRatio)
                e.Graphics.DrawImage(BulletImg, BulletBox)
            End If

            If ShowDamage And Now.Ticks() / 10000 - LastShowDamage < 500 Then
                MonsterDamage(3, e, ScaleRatio)
            End If
        End Sub

        Private Sub MonsterAppearanceUpdate(id As Integer, ScaleRatio As Double)
            Select Case id
                Case 0
                    MonsterImg = My.Resources.Game.SlimeL
                    MonsterWidth = 63
                    MonsterHeight = 84
                Case 1
                    MonsterImg = My.Resources.Game.SlimeR
                    MonsterWidth = 63
                    MonsterHeight = 84
            End Select
        End Sub

        Private Sub MonsterDamage(damage As Double, ByRef e As PaintEventArgs, ScaleRatio As Double)
            DamageText.point.X = x + MonsterX * 48 * ScaleRatio + MonsterWidth / 2 + CameraX
            DamageText.point.Y = y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight / 2 * ScaleRatio
            DamageText.Draw(CStr(damage), e, myfont, ScaleRatio)
        End Sub

        Public Sub DrawGame(ByRef e As PaintEventArgs, ScaleRatio As Double, ByRef Keyboard() As Boolean, ByRef Mouse As Point, ByRef MousePressed As Boolean)
            '畫背景
            e.Graphics.DrawImage(BG, New RectangleF(x, y, width, height))

            '偵測角色偏移以偏移鏡頭
            If x + CharacterX * 48 * ScaleRatio + CharacterBox.Width / 2 + CameraX > x + width / 2 And x + width < x + Map_Width * 48 * ScaleRatio + CameraX Then
                CameraX -= 3 * ScaleRatio
            End If
            If x + CharacterX * 48 * ScaleRatio + CharacterBox.Width / 2 + CameraX < x + width / 2 And x > x + CameraX Then
                CameraX += 3 * ScaleRatio
            End If
            If y + height - CharacterY * 48 * ScaleRatio - CharacterBox.Height / 2 + CameraY < y + height / 2 And y > y + height - Map_Height * 48 * ScaleRatio + CameraY Then
                CameraY += 3 * ScaleRatio
            End If
            If y + height - CharacterY * 48 * ScaleRatio - CharacterBox.Height / 2 + CameraY > y + height / 2 And y + height < y + height + CameraY Then
                CameraY -= 3 * ScaleRatio
            End If

            '畫地圖
            MapDx = 0
            MapDy = height - Map_Height * 48 * ScaleRatio
            For i = 0 To Map_Height - 1
                For j = 0 To Map_Width - 1
                    If Map(i, j) <> 0 Then
                        e.Graphics.DrawImage(Map_Texture(Map(i, j)), New RectangleF(x + MapDx + CameraX, y + MapDy + CameraY, 48 * ScaleRatio, 48 * ScaleRatio))
                    End If

                    MapDx += 48 * ScaleRatio
                Next
                MapDx = 0
                MapDy += 48 * ScaleRatio
            Next

            '怪物部分---------------------------------------------------------------------
            If MonsterHealth > 0 Then
                '怪物擋右牆
                If CharacterX > MonsterX And CharacterX - MonsterX < MonsterDetectArea Then
                    MonsterDirection = True
                    MonsterX += Monster_Speed
                    MonsterAppearanceUpdate(1, ScaleRatio)
                    MonsterDetect()
                    If MonsterRight Then
                        MonsterX = Math.Floor(MonsterX) + (Math.Ceiling(MonsterWidth / 48) - MonsterWidth / 48 - 0.000001)
                    End If
                End If
                '怪物擋左牆
                If CharacterX < MonsterX And MonsterX - CharacterX < MonsterDetectArea Then
                    MonsterDirection = False
                    MonsterX -= Monster_Speed
                    MonsterAppearanceUpdate(0, ScaleRatio)
                    MonsterDetect()
                    If MonsterLeft Then
                        MonsterX = Math.Floor(MonsterX) + 1
                    End If
                End If

                '怪物重力
                MonsterYv -= G
                MonsterY += MonsterYv * (Now.Ticks() / 10000 - LastUpdate)
                MonsterDetect()
                If MonsterTop Then
                    MonsterY = Math.Floor(MonsterY + MonsterHeight / 48) - MonsterHeight / 48
                End If
                MonsterDetect()
                If MonsterBottom Then
                    MonsterY = Math.Floor(MonsterY) + 1
                    MonsterYv = 0
                End If

                '設定怪物要畫的地方
                MonsterBox = New RectangleF(x + CameraX + MonsterX * 48 * ScaleRatio,
                                            y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight * ScaleRatio,
                                            MonsterWidth * ScaleRatio,
                                            MonsterHeight * ScaleRatio)

                '畫怪物
                e.Graphics.DrawImage(MonsterImg, MonsterBox)

                '畫怪物血量
                e.Graphics.FillRectangle(Brushes.White, New RectangleF(x + CameraX + MonsterX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                                       y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight * ScaleRatio - 10 * ScaleRatio,
                                                                       MonsterWidth * ScaleRatio,
                                                                       5 * ScaleRatio))
                e.Graphics.FillRectangle(Brushes.Red, New RectangleF(x + CameraX + MonsterX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                                       y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight * ScaleRatio - 10 * ScaleRatio,
                                                                       MonsterWidth * ScaleRatio * MonsterHealth / 100,
                                                                       5 * ScaleRatio))
            End If

            '角色部分--------------------------------------------------------------------
            '角色向右
            If Keyboard(Keys.D) Then
                CharacterDirection = True
                CharacterX += Character_Speed
                Detect()
                If CharacterRight Then
                    CharacterX = Math.Floor(CharacterX) + (1 - CharacterHitBoxWidth - 0.000001)
                End If
            End If
            '角色向左
            If Keyboard(Keys.A) Then
                CharacterDirection = False
                CharacterX -= Character_Speed
                Detect()
                If CharacterLeft Then
                    CharacterX = Math.Floor(CharacterX) + 1
                End If
            End If
            '角色跳
            If Keyboard(Keys.Space) And Character_Jump = False And Character_CanJump And Now.Ticks() / 10000 - Last_Jump > Jump_Delay Then
                CharacterYv = Character_Jump_Speed
                Character_Jump = True
                Character_CanJump = False
                Last_Jump = Now.Ticks() / 10000
            End If
            If Not Keyboard(Keys.Space) Then
                Character_CanJump = True
            End If

            '角色重力
            CharacterYv -= G
            CharacterY += CharacterYv * (Now.Ticks() / 10000 - LastUpdate)
            Detect()
            If CharacterTop And Character_Jump Then
                CharacterY = Math.Floor(CharacterY + CharacterHitBoxHeight) - CharacterHitBoxHeight
            End If
            Detect()
            If CharacterBottom Then
                CharacterY = Math.Floor(CharacterY) + 1
                CharacterYv = 0
                Character_Jump = False
            End If

            '角色走路動畫
            If Now.Ticks() / 10000 - CharacterMoveAniTimer > 100 Then
                CharacterMoveAniTimer = Now.Ticks() / 10000
                CharacterMoveAni += 1
                If CharacterMoveAni > 2 Then
                    CharacterMoveAni = 0
                End If
            End If

            '偵測滑鼠與鍵盤動作，以更換角色動作與畫子彈
            CharacterImgChange(e, Keyboard, Mouse, MousePressed, ScaleRatio)

            '設定角色要畫的地方
            CharacterBox = New RectangleF(x + CameraX + CharacterX * 48 * ScaleRatio - CharacterAttackWidth * ScaleRatio,
                                          y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - CharacterY) * 48 * ScaleRatio - CharacterHeight * ScaleRatio,
                                          CharacterWidth * ScaleRatio,
                                          CharacterHeight * ScaleRatio)

            '畫角色
            e.Graphics.DrawImage(CharacterImg, CharacterBox)

            '畫角色血量
            e.Graphics.FillRectangle(Brushes.White, New RectangleF(x + CameraX + CharacterX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                                   y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - CharacterY) * 48 * ScaleRatio - CharacterHeight * ScaleRatio - 10 * ScaleRatio,
                                                                   37 * ScaleRatio,
                                                                   5 * ScaleRatio))
            e.Graphics.FillRectangle(Brushes.Red, New RectangleF(x + CameraX + CharacterX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                                   y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - CharacterY) * 48 * ScaleRatio - CharacterHeight * ScaleRatio - 10 * ScaleRatio,
                                                                   37 * ScaleRatio * CharacterHealth / 100,
                                                                   5 * ScaleRatio))

            LastUpdate = Now.Ticks() / 10000
        End Sub
    End Class

    '快速調整設定區
    Dim State As String = "Start"

    Const Version As String = "Insider Preview 1.25"
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

    Dim StartButton As New MyButton With {.Image = My.Resources.Menu.Start, .PressedImage = My.Resources.Menu.Start_Pressed, .Player = Ding}
    Dim HowToPlayButton As New MyButton With {.Image = My.Resources.Menu.HowToPlay, .PressedImage = My.Resources.Menu.HowToPlay_Pressed, .Player = Ding}
    Dim SettingButton As New MyButton With {.Image = My.Resources.Menu.Setting, .PressedImage = My.Resources.Menu.Setting_Pressed, .Player = Ding}
    Dim ExitButton As New MyButton With {.Image = My.Resources.Menu._Exit, .PressedImage = My.Resources.Menu.Exit_Pressed, .Player = Ding}

    'HowToPlay1 初始化
    Dim NextPageButton As New MyButton With {.Image = My.Resources.HowToPlay.NextPage, .PressedImage = My.Resources.HowToPlay.NextPage_Pressed, .Player = Ding}
    Dim HowToPlay_Text As New MyTextBox With {.font = "Cubic11", .font_size = 21.5, .color = Color.Black}
    Dim HowToPlay_Img As New MyPictureBox With {.Image = My.Resources.HowToPlay.HowToPlay1}
    Dim HowToPlay1_Map As Integer(,) = {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                                        {1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1},
                                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}
    Dim DemoGame As New Game With {.Map = HowToPlay1_Map, .Map_Width = 16, .Map_Height = 8, .BG = My.Resources.HowToPlay.Sky, .SoundEffect = SoundEffect}

    'HowToPlay2初始化
    Dim HowToPlay2_Map As Integer(,) = {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}

    'Setting 初始化
    Dim SettingText As New MyTextBox With {.font = "Cubic11", .font_size = 30.9, .color = Color.White}
    Dim MusicSlider As New Slider
    Dim SoundEffectSlider As New Slider
    Dim DoneSettingButton As New MyButton With {.Image = My.Resources.Setting.DoneSetting, .PressedImage = My.Resources.Setting.DoneSetting_Activated, .Player = Ding}

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

            Loading.BoxSetting(MyWidth - 50 * ScaleRatio, MyHeight - 50 * ScaleRatio, 30 * ScaleRatio, 30 * ScaleRatio)
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
                If StartButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume) = 3 Then
                    State = "Intro"
                    IntroStartTime = Now.Ticks() / 10000
                    TextIndex = 0
                    TextAnimationIndex = 0
                    DimScreen = 0
                    BGM.controls.stop()
                End If

                HowToPlayButton.Box = New RectangleF(MyWidth - 289 * ScaleRatio, 170 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If HowToPlayButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume) = 3 Then
                    HowToPlay_Img.Image = My.Resources.HowToPlay.HowToPlay1
                    BGisOn = False
                    State = "HowToPlay1"
                    DemoGame.CharacterX = 1.1
                    DemoGame.CharacterY = 1
                    DemoGame.Map_Width = 16
                    DemoGame.Map_Height = 8
                    DemoGame.CameraReset()
                    DemoGame.CharacterHealth = 100
                    DemoGame.MonsterHealth = 0
                    DemoGame.Map = HowToPlay1_Map
                End If

                SettingButton.Box = New RectangleF(MyWidth - 258 * ScaleRatio, 245 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If SettingButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume) = 3 Then
                    State = "Setting"
                End If

                ExitButton.Box = New RectangleF(MyWidth - 238 * ScaleRatio, 319 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If ExitButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume) = 3 Then
                    State = "Exit"
                End If


            Case "HowToPlay1"
                DemoGame.BoxSetting(MyWidth / 2 - 335 * ScaleRatio, MyHeight / 2 - 112 * ScaleRatio, 670 * ScaleRatio, 287 * ScaleRatio)
                DemoGame.LastUpdate = Now.Ticks() / 10000
                DemoGame.DrawGame(e, ScaleRatio, Keyboard, Mouse, MousePressed)

                HowToPlay_Img.BoxSetting(MyWidth / 2 - 702 / 2 * ScaleRatio, MyHeight / 2 - 382.811 / 2 * ScaleRatio, 702 * ScaleRatio, 382.811 * ScaleRatio)
                HowToPlay_Img.Draw(e)

                HowToPlay_Text.point.Y = MyHeight / 2 - 167 * ScaleRatio

                HowToPlay_Text.point.X = MyWidth / 2 - 330 * ScaleRatio
                HowToPlay_Text.Draw("使用", e, myfont, ScaleRatio)
                HowToPlay_Text.point.X = MyWidth / 2 - 182 * ScaleRatio
                HowToPlay_Text.Draw("移動角色，", e, myfont, ScaleRatio)
                HowToPlay_Text.point.X = MyWidth / 2 + 70 * ScaleRatio
                HowToPlay_Text.Draw("跳躍", e, myfont, ScaleRatio)

                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(0, 0, MyWidth, MyHeight / 2 - 382.811 / 2 * ScaleRatio))
                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(0, MyHeight / 2 - 382.811 / 2 * ScaleRatio + 382.811 * ScaleRatio, MyWidth, MyHeight - (MyHeight / 2 - 382.811 / 2 * ScaleRatio + 382.811 * ScaleRatio)))
                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(0, MyHeight / 2 - 382.811 / 2 * ScaleRatio, MyWidth / 2 - 702 / 2 * ScaleRatio, 382.811 * ScaleRatio))
                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(MyWidth / 2 - 702 / 2 * ScaleRatio + 702 * ScaleRatio, MyHeight / 2 - 382.811 / 2 * ScaleRatio, MyWidth / 2 - 702 / 2 * ScaleRatio, 382.811 * ScaleRatio))

                NextPageButton.Box = New RectangleF(MyWidth / 2 + 175 * ScaleRatio, MyHeight / 2 + 150 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If NextPageButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume) = 3 Then
                    HowToPlay_Img.Image = My.Resources.HowToPlay.HowToPlay2
                    State = "HowToPlay2"
                    DemoGame.CharacterX = 1.1
                    DemoGame.CharacterY = 1
                    DemoGame.Map_Width = 14
                    DemoGame.Map_Height = 8
                    DemoGame.CameraReset()
                    DemoGame.CharacterHealth = 100
                    DemoGame.MonsterHealth = 100
                    DemoGame.Map = HowToPlay2_Map
                End If

            Case "HowToPlay2"
                DemoGame.BoxSetting(MyWidth / 2 - 335 * ScaleRatio, MyHeight / 2 - 112 * ScaleRatio, 670 * ScaleRatio, 287 * ScaleRatio)
                DemoGame.LastUpdate = Now.Ticks() / 10000
                DemoGame.DrawGame(e, ScaleRatio, Keyboard, Mouse, MousePressed)

                HowToPlay_Img.BoxSetting(MyWidth / 2 - 702 / 2 * ScaleRatio, MyHeight / 2 - 382.811 / 2 * ScaleRatio, 702 * ScaleRatio, 382.811 * ScaleRatio)
                HowToPlay_Img.Draw(e)

                HowToPlay_Text.point.Y = MyHeight / 2 - 167 * ScaleRatio

                HowToPlay_Text.point.X = MyWidth / 2 - 330 * ScaleRatio
                HowToPlay_Text.Draw("按下右鍵攻擊敵人，", e, myfont, ScaleRatio)
                HowToPlay_Text.point.X = MyWidth / 2 - 4 * ScaleRatio
                HowToPlay_Text.Draw("釋放技能", e, myfont, ScaleRatio)

                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(0, 0, MyWidth, MyHeight / 2 - 382.811 / 2 * ScaleRatio))
                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(0, MyHeight / 2 - 382.811 / 2 * ScaleRatio + 382.811 * ScaleRatio, MyWidth, MyHeight - (MyHeight / 2 - 382.811 / 2 * ScaleRatio + 382.811 * ScaleRatio)))
                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(0, MyHeight / 2 - 382.811 / 2 * ScaleRatio, MyWidth / 2 - 702 / 2 * ScaleRatio, 382.811 * ScaleRatio))
                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(MyWidth / 2 - 702 / 2 * ScaleRatio + 702 * ScaleRatio, MyHeight / 2 - 382.811 / 2 * ScaleRatio, MyWidth / 2 - 702 / 2 * ScaleRatio, 382.811 * ScaleRatio))

                NextPageButton.Box = New RectangleF(MyWidth / 2 + 175 * ScaleRatio, MyHeight / 2 + 150 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If NextPageButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume) = 3 Then
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
                If DoneSettingButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume) = 3 Then
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

        DemoGame.myfont = myfont
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
