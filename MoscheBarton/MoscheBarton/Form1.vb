Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Runtime.InteropServices
Imports System.Timers

Public Class Form1
    '自製TextBox
    Public Class MyTextBox '自定義文字方塊
        Public font As String '選擇要繪製的字體
        Public font_size As Double '字體大小
        Private DrawFont As Font '要繪製文字的字體
        Public color As Color '文字顏色
        Public opacity As Double = 255.0 '透明度，預設是255.0 (完全不透明)
        Public point As PointF '繪製的左上角座標
        Private format As New StringFormat '字體設定
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
            Return DrawFont.Height '回傳字體高度
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

    '自製PictureBox
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

    '自製Button
    Dim SomeButtonClick As Boolean = False
    Public Class MyButton
        Public Box As RectangleF '按鈕框
        Public Image As Bitmap '按鈕圖片
        'Public MeClicked As Boolean = False
        Public PressedImage As Bitmap '壓下的按鈕的圖片
        Public Activated As Boolean = False '是否被按下過，預設是 False
        Public Player As WMPLib.WindowsMediaPlayer

        Private Sub Activity(Volume As Double, ByRef SomeButtonClick As Boolean)
            Activated = False '曾按過 = False
            '播放音效
            Player.URL = My.Application.Info.DirectoryPath & "\Music\Click.wav"
            Player.settings.volume = Volume
            Player.controls.play()
            SomeButtonClick = False
        End Sub

        '繪製按鈕
        Public Function Draw(ByRef e As PaintEventArgs, Mouse As Point, MousePressed As Boolean, Volume As Double, ByRef SomeButtonClick As Boolean) As Integer
            If Not Activated And SomeButtonClick Then
                e.Graphics.DrawImage(Image, Box)
                Return 0
            End If
            If Box.X <= Mouse.X And Mouse.X <= Box.X + Box.Width And Box.Y <= Mouse.Y And Mouse.Y <= Box.Y + Box.Height Then '游標在按鈕上
                If MousePressed Then '滑鼠按下
                    Activated = True '曾按過 = True
                    SomeButtonClick = True
                    e.Graphics.DrawImage(PressedImage, Box) '繪製已按過的圖片
                    Return 1 '回傳 「按下，但還沒放開」
                Else '滑鼠未按
                    e.Graphics.DrawImage(Image, Box) '繪製未按過的圖片
                    If Activated Then '如果曾按過
                        SomeButtonClick = False
                        Activity(Volume, SomeButtonClick)
                        Return 3 '回傳 「放開，已按過，需觸發事件」
                    Else
                        Return 0 '回傳 「放開，未曾按過」
                    End If
                End If
            Else
                If MousePressed Then '滑鼠按下
                    If Activated Then '曾按過
                        Activated = True '按過 = True
                        SomeButtonClick = True
                        e.Graphics.DrawImage(PressedImage, Box) '繪製已按過的圖片
                        Return 2 '回傳「不再按鈕上，但曾按過，且未放開」
                    Else
                        e.Graphics.DrawImage(Image, Box) '繪製放開的圖片
                        Return 0 '回傳 「放開，未曾按過」
                    End If
                Else
                    If Activated Then
                        SomeButtonClick = False
                    End If
                    Activated = False '曾按過 = False
                    e.Graphics.DrawImage(Image, Box) '繪製放開的圖片
                    Return 0 '回傳 「放開，未曾按過」
                End If
            End If

            e.Graphics.DrawImage(Image, Box)
            Return 0 '回傳 「放開，未曾按過」
        End Function
    End Class

    '使用WMP播放聲音
    Public Sub PlaySound(ByRef Player As WMPLib.WindowsMediaPlayer, File As String, ShouldLoop As Boolean)
        Player.URL = My.Application.Info.DirectoryPath & "\Music\" & File '選擇路徑
        Player.settings.setMode("loop", ShouldLoop) '設定是否循環
        Player.controls.play() '播放
    End Sub

    '關閉WMP
    Public Sub WMPExit(ByRef Player As WMPLib.WindowsMediaPlayer)
        Player.controls.stop() '停止
        Player.close() '結束
    End Sub

    '自製Slider
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
            e.Graphics.DrawLine(New Pen(color, width), New PointF(X1, Y), New PointF(X2, Y)) '畫桿
            '如果滑鼠在滑桿頭上按下
            If X1 + (X2 - X1) / 100 * value - head_width / 2 <= Mouse.X And Mouse.X <= X1 + (X2 - X1) / 100 * value + head_width / 2 And Y - head_width / 2 <= Mouse.Y And Mouse.Y <= Y + head_width / 2 And MousePressed Then
                Activated = True   '曾按過 = True
                HeadX = Mouse.X    '滑桿頭X座標 = 滑鼠X座標
                If HeadX > X2 Then '如果超出上限
                    HeadX = X2     '設為上限
                End If
                If HeadX < X1 Then '如果超出下限
                    HeadX = X1     '設為下限
                End If
                value = (Mouse.X - X1) / (X2 - X1) * 100 '計算值 (%)

                If value > 100 Then '如果值>100
                    value = 100     '值 = 100
                End If
                If value < 0 Then   '如果值>100
                    value = 0       '值 = 100
                End If

                e.Graphics.FillEllipse(New SolidBrush(Activated_color), New RectangleF(HeadX - head_width * 0.8 / 2, Y - head_width * 0.8 / 2, head_width * 0.8, head_width * 0.8)) '畫滑桿頭

                '如果滑鼠按下且曾按過
            ElseIf Activated And MousePressed Then
                HeadX = Mouse.X    '滑桿頭X座標 = 滑鼠X座標
                If HeadX > X2 Then '如果超出上限
                    HeadX = X2     '設為上限
                End If
                If HeadX < X1 Then '如果超出下限
                    HeadX = X1     '設為下限
                End If
                value = (Mouse.X - X1) / (X2 - X1) * 100 '計算值 (%)

                If value > 100 Then '如果值>100
                    value = 100     '值 = 100
                End If
                If value < 0 Then   '如果值>100
                    value = 0       '值 = 100
                End If
                e.Graphics.FillEllipse(New SolidBrush(Activated_color), New RectangleF(HeadX - head_width * 0.8 / 2, Y - head_width * 0.8 / 2, head_width * 0.8, head_width * 0.8)) '畫滑桿頭

                '如果在滑桿上按下
            ElseIf X1 <= Mouse.X And Mouse.X <= X2 And Y - head_width / 2 <= Mouse.Y And Mouse.Y <= Y + head_width / 2 And MousePressed Then
                HeadX = Mouse.X    '滑桿頭X座標 = 滑鼠X座標
                If HeadX > X2 Then '如果超出上限
                    HeadX = X2     '設為上限
                End If
                If HeadX < X1 Then '如果超出下限
                    HeadX = X1     '設為下限
                End If
                value = (Mouse.X - X1) / (X2 - X1) * 100 '計算值 (%)

                If value > 100 Then '如果值>100
                    value = 100     '值 = 100
                End If
                If value < 0 Then   '如果值>100
                    value = 0       '值 = 100
                End If
                e.Graphics.FillEllipse(New SolidBrush(Activated_color), New RectangleF(HeadX - head_width * 0.8 / 2, Y - head_width * 0.8 / 2, head_width * 0.8, head_width * 0.8)) '畫滑桿頭

                '如果曾按過，但滑鼠放開
            ElseIf Activated And MousePressed = False Then
                Player.settings.volume = value '將音量值設定到 WMP
                Player.settings.setMode("loop", False) '設定是否循環撥放
                Player.URL = My.Application.Info.DirectoryPath & "\Music\Click.wav" '撥放音效

                e.Graphics.FillEllipse(New SolidBrush(color), New RectangleF(X1 + (X2 - X1) / 100 * value - head_width / 2, Y - head_width / 2, head_width, head_width)) '畫滑桿頭
                Activated = False '曾按過 = False
            Else
                e.Graphics.FillEllipse(New SolidBrush(color), New RectangleF(X1 + (X2 - X1) / 100 * value - head_width / 2, Y - head_width / 2, head_width, head_width)) '畫滑桿頭
                Activated = False '曾按過 = False
            End If

            Name.Draw(NameText, e, myfont, ScaleRatio) '畫名字
            ValueText.Draw(CStr(value), e, myfont, ScaleRatio) '畫值
        End Sub
    End Class

    '取亂數
    Public Function RandomInt(min As Double, max As Double) As Integer
        Randomize()
        Return Int(min + Rnd() * (max - min + 1))
    End Function

    '遊戲地圖生成、角色移動、鏡頭移動----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    Public Class Game
        '遊戲畫面基本設定--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Public x As Double '左上角X座標 (px)
        Public y As Double '左上角Y座標 (px)
        Public width As Double  '寬 (px)
        Public height As Double '高 (px)
        Public BG As Bitmap '天空圖片
        Public level As Double = 0 '第幾關
        Private Const G As Double = 9.80665 / 1000 '重力加速度常數 (格/ms^2)

        Public CameraX As Double = 0        '鏡頭X偏移 (px)
        Public CameraY As Double = 0        '鏡頭Y偏移 (px)
        Private CameraSpeed As Double = 4.5 '鏡頭移動速度 (pixel/每次更新)

        Public Pause As Boolean = False '是否暫停遊戲

        '遊戲音效的撥放器--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Public Attack As New WMPLib.WindowsMediaPlayer

        '所有角色的偵測--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Private WallDetectX0 As Integer
        Private WallDetectX08 As Integer
        Private WallDetectY0 As Integer
        Private WallDetectY09 As Integer
        Private WallDetectY18 As Integer

        '主角偵測--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Private CharacterLeft As Boolean
        Private CharacterRight As Boolean
        Private CharacterTop As Boolean
        Private CharacterBottom As Boolean

        '地圖--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Public Map As Integer(,) '地圖
        Public Map_Width As Integer '寬 (格)
        Public Map_Height As Integer '高 (格)
        Private Map_Texture As Bitmap() = {My.Resources.Game.Cobblestone, My.Resources.Game.Cobblestone, My.Resources.Game.Planks, My.Resources.Game.Log, My.Resources.Game.Fence, My.Resources.Game.NextLevel} '材質
        Private MapDx As Double = 0 '地圖X偏移 (生成用)(格)
        Private MapDy As Double = 0 '地圖Y偏移 (生成用)(格)

        '受傷數值文字--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Public myfont As PrivateFontCollection '字體集
        Private DamageText As New MyTextBox With {.font = "Cubic11", .font_size = 24, .Align = "Center", .LineAlign = "Center"} '傷害值的顯示
        Public Damage As Double '傷害
        Public ShowDamage As Boolean = False '顯示傷害
        Public LastShowDamage As Long = 0 '初次顯示傷害的時間 (計時用)

        '角色--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Private CharacterImg As Bitmap = My.Resources.Game.CRNN '角色圖片
        Private CharacterMoveAni As Integer = 0 '角色動畫
        Private CharacterMoveAniTimer As Long '角色動畫的計時器
        Private CharacterDirection As Boolean = True 'False = 右，True = 左

        Private Character_Speed As Double = 0.09 '(格/每次更新)
        Private CharacterYv As Double = 0 '角色Y軸速度

        Public Const CharacterMaxHealth As Double = 100 '角色預設血量
        Public CharacterHealth As Double = CharacterMaxHealth '角色血量

        Public CharacterBox As RectangleF '角色外框
        Public CharacterX As Double '角色X座標 (格)
        Public CharacterY As Double '角色Y座標 (格)
        Private CharacterWidth As Double = 27 '角色寬度 (px)
        Private CharacterHeight As Double = 84 '角色高度 (px)
        Private CharacterHitBoxWidth As Double = 27 / 48 '角色碰撞箱寬度 (px)
        Private CharacterHitBoxHeight As Double = 84 / 48 '角色碰撞相高度 (px)
        Private CharacterAttackWidth As Double = 0 '角色左右邊攻擊的繪製偏移X值

        Public CharacterHarmDamage As Double = 10 '角色普通攻擊傷害
        Const OriginalCharacterATKDamage As Double = 10
        Const OriginalCharacterSkillDamage As Double = 32
        Public CharacterATKDamage As Double = 10
        Public CharacterSkillDamage As Double = 32

        Public CharacterDied As Boolean = False '角色是否死亡

        Private Character_Jump As Boolean = False '角色是否跳躍
        Private Character_CanJump As Boolean = True '角色是否可以跳躍
        Private Const Character_Jump_Speed As Double = 0.105 '角色跳躍初速 (格/每次更新)
        Private Jump_Delay As Integer = 350 '跳躍與跳躍之間的延遲 (ms)
        Private Last_Jump As Long = 0 '上一次跳躍的時間

        '子彈--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Private BulletImg As Bitmap = My.Resources.Game.BulletR '子彈圖
        Public BulletDirection As Boolean = False 'False = 右，True = 左

        Public BulletBox As RectangleF '子彈外框
        Public BulletX As Double '子彈X座標 (格)
        Public BulletY As Double '子彈Y座標 (格)
        Public BulletWidth As Double = 12 '子彈寬度 (px)
        Public BulletHeight As Double = 24 '子彈高度 (px)

        Public CharacterShootX As Double
        Public CharacterShootY As Double

        '子彈偵測
        Private BulletDetectX As Integer
        Private BulletDetectY As Integer
        Private Const BulletSpeed As Double = 0.3 '格/每次更新

        Public CanShoot As Boolean = True '可以射子彈
        Private LastShoot As Long = 0 '上次射子彈時間
        Private ShootGap As Long = 400 'ms
        Private HadShot As Boolean = False '是否射過

        '攻擊CD條--------------------------------------------------------------------------------------------------------------------------------------------
        Private CDBarFull As Double = 400 'CD條滿
        Private CDShowValue As Double = 400 'CD條顯示

        '取隨機數
        Public Function RandomInt(min As Double, max As Double) As Integer
            Randomize()
            Return Int(min + Rnd() * (max - min + 1))
        End Function

        '角色攻擊力重置
        Public Sub CharacterATKReset()
            CharacterATKDamage = OriginalCharacterATKDamage
            CharacterSkillDamage = OriginalCharacterSkillDamage
        End Sub

        '角色重置
        Public Sub CharacterReset(x As Double, y As Double)
            CharacterX = x
            CharacterY = y
            CharacterDied = False
            Pause = False
        End Sub

        '角色血量重置
        Public Sub CharacterHealthReset()
            CharacterHealth = CharacterMaxHealth
        End Sub

        '地圖設定
        Public Sub MapSetting(w As Integer, h As Integer)
            Map_Width = w
            Map_Height = h
            Randomize()
            Select Case Int(0 + Rnd() * (1 - 0 + 1))
                Case 0
                    BG = My.Resources.Game.Day
                Case 1
                    BG = My.Resources.Game.Sunset
            End Select
        End Sub

        '鏡頭重置
        Public Sub CameraReset(Optional dx As Double = 0, Optional dy As Double = 0)
            CameraX = dx
            CameraY = dy
        End Sub

        '遊戲外框設定
        Public Sub BoxSetting(ptx As Double, pty As Double, ptwidth As Double, ptheight As Double)
            x = ptx
            y = pty
            width = ptwidth
            height = ptheight
        End Sub

        '角色偵測牆
        Private Sub Detect()
            '偵測牆
            WallDetectX0 = Math.Floor(CharacterX)
            WallDetectX08 = Math.Floor(CharacterX + CharacterHitBoxWidth - 0.000001)
            WallDetectY0 = Math.Floor(Map_Height - (CharacterY + 0.000001))
            WallDetectY09 = Math.Floor(Map_Height - (CharacterY + CharacterHitBoxHeight / 2))
            WallDetectY18 = Math.Floor(Map_Height - (CharacterY + CharacterHitBoxHeight - 0.000001))

            '過大的值固定到最大，過小的值固定到0
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

            '角色上下左右的牆偵測
            CharacterLeft = (Map(WallDetectY0, WallDetectX0) > 0 Or Map(WallDetectY09, WallDetectX0) > 0 Or Map(WallDetectY18, WallDetectX0) > 0) And Math.Floor(CharacterX) < CharacterX And CharacterX < Math.Floor(CharacterX) + 1
            CharacterRight = (Map(WallDetectY0, WallDetectX08) > 0 Or Map(WallDetectY09, WallDetectX08) > 0 Or Map(WallDetectY18, WallDetectX08) > 0) And Math.Floor(CharacterX + CharacterHitBoxWidth) < CharacterX + CharacterHitBoxWidth And CharacterX + CharacterHitBoxWidth < Math.Floor(CharacterX + CharacterHitBoxWidth) + 1
            CharacterTop = (Map(WallDetectY18, WallDetectX0) > 0 Or Map(WallDetectY18, WallDetectX08) > 0) And Math.Floor(CharacterY + CharacterHitBoxHeight) < CharacterY + CharacterHitBoxHeight And CharacterY + CharacterHitBoxHeight < Math.Floor(CharacterY + CharacterHitBoxHeight) + 1
            CharacterBottom = (Map(WallDetectY0, WallDetectX0) > 0 Or Map(WallDetectY0, WallDetectX08) > 0) And Math.Floor(CharacterY) < CharacterY And CharacterY < Math.Floor(CharacterY) + 1
        End Sub

        '角色換圖片，發射子彈
        Private Sub CharacterImgChange(ByRef e As PaintEventArgs, ByRef Keyboard() As Boolean, ByRef Mouse As Point, ByRef MousePressed As Boolean, ByRef ScaleRatio As Double)
            '如果沒有暫停
            If Not Pause Then
                If MousePressed Or Keyboard(Keys.E) Then '按滑鼠或E
                    If Keyboard(Keys.A) Then '按A
                        '設定角色
                        CharacterWidth = 67.0383
                        CharacterHeight = 84.5916
                        CharacterAttackWidth = 27
                        CharacterHitBoxWidth = 27 / 48
                        CharacterHitBoxHeight = 84.5916 / 48

                        '角色移動動畫
                        Select Case CharacterMoveAni
                            Case 0
                                CharacterImg = My.Resources.Game.CLAW0
                            Case 1
                                CharacterImg = My.Resources.Game.CLAW1
                            Case 2
                                CharacterImg = My.Resources.Game.CLAW2
                        End Select

                        '如果可以射子彈
                        If CanShoot Then
                            BulletDirection = True '子彈方向左
                        End If
                    End If

                    If Keyboard(Keys.D) Then '按D
                        '設定角色
                        CharacterWidth = 67.0383
                        CharacterHeight = 84.5916
                        CharacterAttackWidth = 0
                        CharacterHitBoxWidth = 40.0383 / 48
                        CharacterHitBoxHeight = 84.5916 / 48

                        '角色移動動畫
                        Select Case CharacterMoveAni
                            Case 0
                                CharacterImg = My.Resources.Game.CRAW0
                            Case 1
                                CharacterImg = My.Resources.Game.CRAW1
                            Case 2
                                CharacterImg = My.Resources.Game.CRAW2
                        End Select

                        '如果可以射子彈
                        If CanShoot Then
                            BulletDirection = False '子彈方向右
                        End If
                    End If


                    If Not Keyboard(Keys.A) And Not Keyboard(Keys.D) Then '不按A和D
                        '設定角色
                        CharacterWidth = 58.0383
                        CharacterHeight = 84
                        CharacterHitBoxWidth = 27 / 48
                        CharacterHitBoxHeight = 84 / 48

                        '偵測角色方向
                        If CharacterDirection Then '左
                            CharacterAttackWidth = -12.1249
                            CharacterImg = My.Resources.Game.CRAN
                            If CanShoot Then
                                BulletDirection = False
                            End If
                        Else '右
                            CharacterAttackWidth = 31.0383 - 12.1249
                            CharacterImg = My.Resources.Game.CLAN
                            If CanShoot Then
                                BulletDirection = True
                            End If
                        End If
                    End If

                    '如果可以射
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

                        CharacterShootX = CharacterX
                        CharacterShootY = CharacterY

                        '如果按E
                        If Keyboard(Keys.E) Then
                            Attack.URL = My.Application.Info.DirectoryPath & "\Music\技能.mp3" '選擇路徑
                            Attack.settings.setMode("loop", False) '設定是否循環
                            Attack.controls.play() '播放
                            CharacterHarmDamage = RandomInt(CharacterSkillDamage + 8, CharacterSkillDamage - 8)

                            If BulletDirection Then
                                BulletImg = My.Resources.Game.AbilityL
                                BulletWidth = 48
                                BulletHeight = 33
                            Else
                                BulletImg = My.Resources.Game.AbilityR
                                BulletWidth = 52
                                BulletHeight = 33
                            End If

                            ShootGap = 1500
                            CDBarFull = 1500

                            '如果按滑鼠
                        ElseIf MousePressed Then
                            Attack.URL = My.Application.Info.DirectoryPath & "\Music\射出.mp3" '選擇路徑
                            Attack.settings.setMode("loop", False) '設定是否循環
                            Attack.controls.play() '播放
                            CharacterHarmDamage = RandomInt(CharacterATKDamage + 5, CharacterATKDamage - 5)

                            If BulletDirection Then
                                BulletImg = My.Resources.Game.BulletL
                            Else
                                BulletImg = My.Resources.Game.BulletR
                            End If

                            BulletWidth = 12
                            BulletHeight = 24

                            ShootGap = 400
                            CDBarFull = 400

                        End If
                    End If
                Else '沒按滑鼠或E
                    HadShot = False
                    If Keyboard(Keys.A) Then '按A
                        '設定角色
                        CharacterWidth = 39.1249
                        CharacterHeight = 84.5916
                        CharacterAttackWidth = 0
                        CharacterHitBoxWidth = 39.1249 / 48
                        CharacterHitBoxHeight = 84.5916 / 48

                        '角色動畫
                        Select Case CharacterMoveAni
                            Case 0
                                CharacterImg = My.Resources.Game.CLNW0
                            Case 1
                                CharacterImg = My.Resources.Game.CLNW1
                            Case 2
                                CharacterImg = My.Resources.Game.CLNW2
                        End Select
                    End If

                    If Keyboard(Keys.D) Then '按D
                        '設定角色
                        CharacterWidth = 39.1249
                        CharacterHeight = 84.5916
                        CharacterAttackWidth = 0
                        CharacterHitBoxWidth = 39.1249 / 48
                        CharacterHitBoxHeight = 84.5916 / 48

                        '角色動畫
                        Select Case CharacterMoveAni
                            Case 0
                                CharacterImg = My.Resources.Game.CRNW0
                            Case 1
                                CharacterImg = My.Resources.Game.CRNW1
                            Case 2
                                CharacterImg = My.Resources.Game.CRNW2
                        End Select
                    End If

                    If Not Keyboard(Keys.A) And Not Keyboard(Keys.D) Then '沒按A和D
                        '設定角色
                        CharacterWidth = 27
                        CharacterHeight = 84
                        CharacterAttackWidth = -12.1249
                        CharacterHitBoxWidth = 27 / 48
                        CharacterHitBoxHeight = 84.5916 / 48

                        If CharacterDirection Then '角色向左
                            CharacterImg = My.Resources.Game.CRNN
                        Else '角色向右
                            CharacterImg = My.Resources.Game.CLNN
                        End If
                    End If
                End If

                If CanShoot = False Then '如果不能射
                    If BulletDirection Then '方向向左
                        BulletX -= BulletSpeed '子彈向左
                    Else '方向向右
                        BulletX += BulletSpeed '子彈向右
                    End If

                    '子彈偵測撞牆
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
                End If
            End If

            If CanShoot = False Then '如果不能射
                '更新子彈外框
                BulletBox = New RectangleF(x + CameraX + BulletX * 48 * ScaleRatio + 5 * ScaleRatio,
                                           y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - BulletY) * 48 * ScaleRatio - 24 * ScaleRatio - 10 * ScaleRatio,
                                           BulletWidth * ScaleRatio, BulletHeight * ScaleRatio)
                e.Graphics.DrawImage(BulletImg, BulletBox) '畫子彈

                If Map(BulletDetectY, BulletDetectX) > 0 Then '如果子彈在的方塊不是空氣
                    CanShoot = True '可以射
                End If
            End If
        End Sub

        '畫遊戲介面----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Public Sub DrawGame(ByRef e As PaintEventArgs, ScaleRatio As Double, ByRef Keyboard() As Boolean, ByRef Mouse As Point, ByRef MousePressed As Boolean, volume As Integer)
            Attack.settings.volume = volume

            '畫背景--------------------------------------------------------------------------------------------------------------------------------------------
            e.Graphics.DrawImage(BG, New RectangleF(x, y, width, height))

            If Not Pause Then
                '偵測角色偏移以偏移鏡頭--------------------------------------------------------------------------------------------------------------------------------------------
                If x + CharacterX * 48 * ScaleRatio + 27 * ScaleRatio + CameraX > x + width / 2 And x + width < x + Map_Width * 48 * ScaleRatio + CameraX Then
                    CameraX -= CameraSpeed * ScaleRatio
                End If
                If x + CharacterX * 48 * ScaleRatio + 27 * ScaleRatio + CameraX < x + width / 2 And x > x + CameraX Then
                    CameraX += CameraSpeed * ScaleRatio
                End If
                If y + height - CharacterY * 48 * ScaleRatio - 42 * ScaleRatio + CameraY < y + height / 2 And y > y + height - Map_Height * 48 * ScaleRatio + CameraY Then
                    CameraY += CameraSpeed * ScaleRatio
                End If
                If y + height - CharacterY * 48 * ScaleRatio - 42 * ScaleRatio + CameraY > y + height / 2 And y + height < y + height + CameraY Then
                    CameraY -= CameraSpeed * ScaleRatio
                End If
            End If

            '畫地圖--------------------------------------------------------------------------------------------------------------------------------------------
            MapDx = 0
            MapDy = height - Map_Height * 48 * ScaleRatio
            For i = 0 To Map_Height - 1
                For j = 0 To Map_Width - 1
                    If Map(i, j) > 0 Then
                        e.Graphics.DrawImage(Map_Texture(Map(i, j)), New RectangleF(x + MapDx + CameraX, y + MapDy + CameraY, 48 * ScaleRatio, 48 * ScaleRatio))
                    End If
                    If Map(i, j) < 0 Then
                        e.Graphics.DrawImage(Map_Texture(-Map(i, j)), New RectangleF(x + MapDx + CameraX, y + MapDy + CameraY, 48 * ScaleRatio, 48 * ScaleRatio))
                    End If

                    MapDx += 48 * ScaleRatio
                Next
                MapDx = 0
                MapDy += 48 * ScaleRatio
            Next

            '角色部分----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            If Not Pause Then '如果未暫停
                If Not CharacterDied Then '如果角色未死
                    '角色向右
                    If Keyboard(Keys.D) Then '按D
                        CharacterDirection = True '角色向左
                        CharacterX += Character_Speed '移動
                        Detect() '偵測邊界
                        If CharacterRight Then '如果角色右邊有牆
                            CharacterX = Math.Floor(CharacterX) + (1 - CharacterHitBoxWidth - 0.000001) '卡住角色
                        End If
                    End If
                    '角色向左
                    If Keyboard(Keys.A) Then '按A
                        CharacterDirection = False '角色向右
                        CharacterX -= Character_Speed '移動
                        Detect() '偵測邊界
                        If CharacterLeft Then '如果角色左邊有牆
                            CharacterX = Math.Floor(CharacterX) + 1 '卡住牆壁
                        End If
                    End If
                    '角色跳
                    If Keyboard(Keys.Space) And Character_Jump = False And Character_CanJump And Now.Ticks() / 10000 - Last_Jump > Jump_Delay Then '如果可以跳躍，且按下空白鍵
                        CharacterYv = Character_Jump_Speed '向上加速
                        Character_Jump = True '角色正在跳 = True
                        Character_CanJump = False '角色可以跳 = False
                        Last_Jump = Now.Ticks() / 10000 '上次跳躍時間 = 現在
                    End If
                    If Not Keyboard(Keys.Space) Then '如果沒按空白鍵
                        Character_CanJump = True '角色可以跳 = True
                    End If
                End If

                '角色重力--------------------------------------------------------------------------------------------------------------------------------------------
                CharacterYv -= G '向下速度
                CharacterY += CharacterYv * 5 '根據角色Y軸速度移動角色
                Detect() '偵測邊界
                If CharacterTop And Character_Jump Then '如果上面有牆，且正在跳
                    CharacterY = Math.Floor(CharacterY + CharacterHitBoxHeight) - CharacterHitBoxHeight '卡住角色
                End If
                Detect() '偵測邊界
                If CharacterBottom Then '如果下面有強
                    CharacterY = Math.Floor(CharacterY) + 1 '卡住角色
                    CharacterYv = 0 '跳躍速度 = 0
                    Character_Jump = False '正在跳 = False
                End If

                '角色走路動畫--------------------------------------------------------------------------------------------------------------------------------------------
                If Now.Ticks() / 10000 - CharacterMoveAniTimer > 100 Then '如果要換下一張圖片的時間到
                    CharacterMoveAniTimer = Now.Ticks() / 10000 '更新上次換圖片的時間
                    CharacterMoveAni += 1 '下一張圖片
                    If CharacterMoveAni > 2 Then '如果圖片超出2
                        CharacterMoveAni = 0 '換成0
                    End If
                End If
            End If

            '偵測滑鼠與鍵盤動作，以更換角色動作與畫子彈--------------------------------------------------------------------------------------------------------------------------------------------
            CharacterImgChange(e, Keyboard, Mouse, MousePressed, ScaleRatio)

            '設定角色要畫的地方--------------------------------------------------------------------------------------------------------------------------------------------
            CharacterBox = New RectangleF(x + CameraX + CharacterX * 48 * ScaleRatio - CharacterAttackWidth * ScaleRatio,
                                          y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - CharacterY) * 48 * ScaleRatio - CharacterHeight * ScaleRatio,
                                          CharacterWidth * ScaleRatio,
                                          CharacterHeight * ScaleRatio)

            '畫角色--------------------------------------------------------------------------------------------------------------------------------------------
            e.Graphics.DrawImage(CharacterImg, CharacterBox)

            '畫角色血量--------------------------------------------------------------------------------------------------------------------------------------------
            If CharacterHealth > CharacterMaxHealth Then '偵測血量是否過多
                CharacterHealth = CharacterMaxHealth
            End If

            e.Graphics.FillRectangle(Brushes.White, New RectangleF(x + CameraX + CharacterX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                                   y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - CharacterY) * 48 * ScaleRatio - CharacterHeight * ScaleRatio - 15 * ScaleRatio,
                                                                   37 * ScaleRatio,
                                                                   5 * ScaleRatio))
            e.Graphics.FillRectangle(Brushes.Red, New RectangleF(x + CameraX + CharacterX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                                   y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - CharacterY) * 48 * ScaleRatio - CharacterHeight * ScaleRatio - 15 * ScaleRatio,
                                                                   37 * ScaleRatio * CharacterHealth / CharacterMaxHealth,
                                                                   5 * ScaleRatio))

            If ShowDamage Then '如果要顯示傷害(回血)
                If Now.Ticks() / 10000 - LastShowDamage > ShowDamageTime Then '如果顯示傷害(回血)的時間到
                    ShowDamage = False '停止顯示傷害(回血)
                Else
                    '畫傷害(回血)文字
                    DamageText.point.X = x + CameraX + CharacterX * 48 * ScaleRatio - CharacterAttackWidth * ScaleRatio
                    DamageText.point.Y = y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - CharacterY) * 48 * ScaleRatio - CharacterHeight * ScaleRatio
                    If Damage < 0 Then '如果是回血
                        DamageText.color = Color.FromArgb(21, 244, 73) '換成綠色
                        DamageText.Draw("+" & CStr(-Damage), e, myfont, ScaleRatio) '畫回血量
                    Else
                        DamageText.color = Color.FromArgb(244, 67, 54) '換成紅色
                        DamageText.Draw("-" & CStr(Damage), e, myfont, ScaleRatio) '畫扣血量
                    End If
                End If
            End If

            '畫CD條--------------------------------------------------------------------------------------------------------------------------------------------
            If ShootGap / 1000 - (Now.Ticks() / 10000 - LastShoot) / 1000 <= 0 Then '如果CD條時間<0
                CDShowValue = 0 '時間設定為0
            Else
                CDShowValue = ShootGap / 1000 - (Now.Ticks() / 10000 - LastShoot) / 1000
            End If
            '畫CD條本身
            e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(200, 255, 255, 255)), New RectangleF(x + CameraX + CharacterX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                                                                        y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - CharacterY) * 48 * ScaleRatio - CharacterHeight * ScaleRatio - 7.5 * ScaleRatio,
                                                                                                        37 * ScaleRatio * (CDShowValue / (CDBarFull / 1000)),
                                                                                                        5 * ScaleRatio))

            '偵測角色是否死亡----------------------------------------------------------------------------------------------------------------------------------
            If CharacterHealth <= 0 And Not Pause Then
                CharacterDied = True
            End If
        End Sub
    End Class

    '怪物生成
    Public Class Monster
        '遊戲畫面基本設定--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Public x As Double '遊戲畫面左上角X座標
        Public y As Double '遊戲畫面左上角Y座標
        Public width As Double '遊戲顯示畫面寬度
        Public height As Double '遊戲顯示畫面高度
        Private Const G As Double = 9.80665 / 1000 '重力加速度常數 (格/ms^2)
        Private CameraX As Double = 0 '鏡頭偏移X
        Private CameraY As Double = 0 '鏡頭偏移Y

        Public Pause As Boolean = False

        '遊戲音效的撥放器--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Private Hit As New WMPLib.WindowsMediaPlayer
        Private AddScore As New WMPLib.WindowsMediaPlayer
        Private CharacterBeenHit As New WMPLib.WindowsMediaPlayer

        '角色--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Private CharacterX As Double '角色X座標 (格)
        Private CharacterY As Double '角色Y座標 (格)
        Private CharacterShootX As Double
        Private CharacterShootY As Double

        '地圖--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Private Map As Integer(,) '地圖
        Private Map_Width As Integer '地圖寬度 (格)
        Private Map_Height As Integer '地圖高度 (格)

        '所有角色的偵測--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Private WallDetectX0 As Integer
        Private WallDetectX08 As Integer
        Private WallDetectY0 As Integer
        Private WallDetectY09 As Integer
        Private WallDetectY18 As Integer

        '子彈--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Private BulletDirection As Boolean = False 'False = 右，True = 左
        Private BulletX As Double '子彈X座標
        Private BulletY As Double '子彈Y座標

        '受傷數值文字--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        Public myfont As PrivateFontCollection '自訂字體
        Private Damage As Double '傷害值
        Private DamageText As New MyTextBox With {.font = "Cubic11", .font_size = 24, .color = Color.FromArgb(244, 67, 54), .Align = "Center", .LineAlign = "Center"} '顯示傷害的文字
        Private ShowDamage As Boolean = False '是否顯示傷害
        Private DamageTextBulletX As Double '暫時紀錄怪物被子彈擊中的X座標
        Private LastShowDamage As Long = 0 '上次顯示傷害的時間

        '怪物---------------------------------------------------------------------------------------------------------------------------------------------------
        Private MonsterImg As Bitmap = My.Resources.Game.SlimeL '怪物圖片
        Private LeftId As Integer = 0 '怪物向左的圖片編號
        Private RightId As Integer = 1 '怪物向右的圖片編號
        Private MonsterDirection As Boolean = False 'False = 右，True = 左
        Private Monster_Speed As Double = 0.03 ' 怪物速度(格/每次更新)
        Private MonsterBox As RectangleF '怪物外框
        Public MonsterX As Double '怪物X座標 (格)
        Public MonsterY As Double '怪物Y座標 (格)
        Private MonsterYv As Double = 0 '怪物Y軸速度
        Public MonsterWidth As Double = 63 '怪物寬度
        Public MonsterHeight As Double = 84 '怪物高度
        Public MonsterHealth As Double = 100 '怪物血量
        Public MonsterHarm As Double = 0 '怪物造成的傷害
        Private MonsterMaxHealth As Double = 100 '怪物滿血血量
        Private MonsterDetectArea As Double = 3.5 '怪物偵測範圍 (格) 
        Private MonsterHasNotDied As Boolean = True '怪物還沒死 (放音效用)

        Private MonsterLaserDxLeft As Double = 0 'BOSS雷射向左位移量
        Private MonsterLaserDxRight As Double = 0 'BOSS雷射向右位移量
        Private Const BossLaserSpeed As Double = 0.25 'BOSS雷射速度

        Private CanDamageCharacter As Boolean = True '是否可以攻擊玩家
        Private LastDamageCharacter As Long '上次攻擊玩家時間
        Private DamageTimeGap As Long = 1000 '每次攻擊的間距時間 (ms)

        Private MonsterLeft As Boolean '怪物偵測左
        Private MonsterRight As Boolean '怪物偵測右
        Private MonsterTop As Boolean '怪物偵測上
        Private MonsterBottom As Boolean '怪物偵測下

        Private MonsterAniIndex As Integer = 0 '怪物動畫
        Private MonsterAniTimeGap As Long = 250 '怪物動畫每張圖片的間隔時間 (ms)
        Private LastAnimation As Long = 0 '上次換圖片的時間

        Public BossShouldAttack As Boolean = True 'BOSS是否該攻擊
        Private BossAttackSoundDelay As Long = 250

        '與遊戲同步變數
        Public Sub SyncWith(ByRef Game As Game)
            With Game
                x = .x
                y = .y
                width = .width
                height = .height
                CameraX = .CameraX
                CameraY = .CameraY
                CharacterX = .CharacterX
                CharacterY = .CharacterY
                CharacterShootX = .CharacterShootX
                CharacterShootY = .CharacterShootY
                Map = .Map
                Map_Width = .Map_Width
                Map_Height = .Map_Height
                BulletDirection = .BulletDirection
                BulletX = .BulletX
                BulletY = .BulletY
                Pause = .Pause
            End With
        End Sub

        '怪物偵測
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

            MonsterLeft = (Map(WallDetectY0, WallDetectX0) > 0 Or Map(WallDetectY09, WallDetectX0) > 0 Or Map(WallDetectY18, WallDetectX0) > 0) And Math.Floor(MonsterX) < MonsterX And MonsterX < Math.Floor(MonsterX) + 1
            MonsterRight = (Map(WallDetectY0, WallDetectX08) > 0 Or Map(WallDetectY09, WallDetectX08) > 0 Or Map(WallDetectY18, WallDetectX08) > 0) And Math.Floor(MonsterX + MonsterWidth / 48) < MonsterX + MonsterWidth / 48 And MonsterX + MonsterWidth / 48 < Math.Ceiling(MonsterX + MonsterWidth / 48)
            MonsterTop = (Map(WallDetectY18, WallDetectX0) > 0 Or Map(WallDetectY18, WallDetectX08) > 0) And Math.Floor(MonsterY + MonsterHeight / 48) < MonsterY + MonsterHeight / 48 And MonsterY + MonsterHeight / 48 < Math.Floor(MonsterY + MonsterHeight / 48) + 1
            MonsterBottom = (Map(WallDetectY0, WallDetectX0) > 0 Or Map(WallDetectY0, WallDetectX08) > 0) And Math.Floor(MonsterY) < MonsterY And MonsterY < Math.Floor(MonsterY) + 1
        End Sub

        '怪物重設
        Public Sub MonsterReset(LId As Integer, RId As Integer, x As Double, y As Double, Harm As Double)
            LeftId = LId
            RightId = RId
            MonsterAppearanceUpdate(RId)
            MonsterHealth = MonsterMaxHealth
            MonsterX = x
            MonsterY = y
            MonsterHarm = Harm
            MonsterLaserDxLeft = 0
            MonsterLaserDxRight = 0
            MonsterHasNotDied = True
            Pause = False
            BossShouldAttack = True
        End Sub

        '更新怪物外觀，並套用怪物傷害值
        Private Sub MonsterAppearanceUpdate(id As Integer)
            Select Case id
                Case 0
                    MonsterImg = My.Resources.Game.SlimeL
                    MonsterWidth = 63
                    MonsterHeight = 84
                    MonsterMaxHealth = 100
                    Monster_Speed = 0.03
                    MonsterDetectArea = 3.5
                    DamageTimeGap = 1000
                Case 1
                    MonsterImg = My.Resources.Game.SlimeR
                    MonsterWidth = 63
                    MonsterHeight = 84
                    MonsterMaxHealth = 100
                    Monster_Speed = 0.03
                    MonsterDetectArea = 3.5
                    DamageTimeGap = 1000
                Case 2
                    MonsterImg = My.Resources.Game.AbyssClose
                    MonsterWidth = 65.3613
                    MonsterHeight = 82.7405
                    MonsterMaxHealth = 200
                    Monster_Speed = 0.03
                    MonsterDetectArea = 3.5
                    DamageTimeGap = 1000
                Case 3
                    MonsterImg = My.Resources.Game.AbyssOpen
                    MonsterWidth = 65.3613
                    MonsterHeight = 82.7405
                    MonsterMaxHealth = 200
                    Monster_Speed = 0.03
                    MonsterDetectArea = 3.5
                    DamageTimeGap = 1000
                Case 4
                    MonsterImg = My.Resources.Game.Boss
                    MonsterWidth = 115.1535
                    MonsterHeight = 205.513
                    MonsterMaxHealth = 250
                    Monster_Speed = 0.06
                    MonsterDetectArea = 3.5
                    DamageTimeGap = 50
                Case 5
                    MonsterImg = My.Resources.Game.Boss
                    MonsterWidth = 115.1535
                    MonsterHeight = 205.513
                    MonsterMaxHealth = 250
                    Monster_Speed = 0.06
                    MonsterDetectArea = 3.5
                    DamageTimeGap = 50
                Case 6
                    MonsterImg = My.Resources.Game.BossMusicCore
                    MonsterWidth = 42
                    MonsterHeight = 30
                    MonsterMaxHealth = 250
                    Monster_Speed = 0.06
                    MonsterDetectArea = 3.5
                    DamageTimeGap = 1000
                Case 7
                    MonsterImg = My.Resources.Game.BossMusicCore
                    MonsterWidth = 42
                    MonsterHeight = 30
                    MonsterMaxHealth = 250
                    Monster_Speed = 0.06
                    MonsterDetectArea = 3.5
                    DamageTimeGap = 1000
                Case 8
                    MonsterImg = My.Resources.Game.RegenHealth
                    MonsterWidth = 42
                    MonsterHeight = 60
                    MonsterMaxHealth = 99999999
                    Monster_Speed = 0
                    MonsterDetectArea = 1
                    DamageTimeGap = 1000
                Case 9
                    MonsterImg = My.Resources.Game.RegenHealth
                    MonsterWidth = 42
                    MonsterHeight = 60
                    MonsterMaxHealth = 99999999
                    Monster_Speed = 0
                    MonsterDetectArea = 1
                    DamageTimeGap = 1000
            End Select
        End Sub

        '畫怪物，並偵測子彈是否打到怪物
        Public Sub Draw(ByRef e As PaintEventArgs, ScaleRatio As Double, ByRef CanShoot As Boolean, ByRef CharacterHealth As Double, ByRef CharacterDamage As Double, ByRef CharacterShowDamage As Boolean, ByRef CharacterLastShowDamage As Long, CharacterHarmDamage As Double, volume As Integer, ByRef xp As Integer)
            CharacterBeenHit.settings.volume = volume
            Hit.settings.volume = volume
            AddScore.settings.volume = volume

            '如果怪物還活著
            If MonsterHealth > 0 Then
                If Not Pause Then '如果沒有暫停

                    '史萊姆
                    If RightId = 1 Then
                        '怪物擋右牆
                        If CharacterX > MonsterX + MonsterWidth / 48 / 2 And CharacterX - MonsterX < MonsterDetectArea Then '如果角色在史萊姆右邊
                            MonsterDirection = True '向右

                            MonsterX += Monster_Speed '向右
                            MonsterAppearanceUpdate(RightId) '更新外觀，攻擊傷害

                            MonsterDetect() '偵測牆
                            If MonsterRight Then '如果右邊有牆
                                MonsterX = Math.Floor(MonsterX) + (Math.Ceiling(MonsterWidth / 48) - MonsterWidth / 48 - 0.000001) '卡住怪物
                            End If
                        End If

                        '怪物擋左牆
                        If CharacterX < MonsterX + MonsterWidth / 48 / 2 And MonsterX - CharacterX < MonsterDetectArea Then '如果角色在史萊姆左邊
                            MonsterDirection = False '向左

                            MonsterX -= Monster_Speed '向左
                            MonsterAppearanceUpdate(LeftId) '更新外觀，攻擊傷害

                            MonsterDetect() '偵測牆
                            If MonsterLeft Then '如果左邊有牆
                                MonsterX = Math.Floor(MonsterX) + 1 '卡住怪物
                            End If
                        End If

                        '深淵怪物
                    ElseIf RightId = 3 Then
                        '怪物擋右牆
                        If CharacterX > MonsterX + MonsterWidth / 48 / 2 And CharacterX - MonsterX < MonsterDetectArea Then '如果角色在深淵怪物右邊
                            MonsterDirection = True '向右

                            '動畫
                            If Now.Ticks() / 10000 - LastAnimation > MonsterAniTimeGap Then '如果時間到要放下一張圖片
                                MonsterAniIndex += 1 '放下一張圖片
                                If MonsterAniIndex + LeftId > 3 Then '如果圖片放過頭
                                    MonsterAniIndex = 0 '圖片換為0
                                End If
                                MonsterAppearanceUpdate(MonsterAniIndex + LeftId) '更新外觀
                                LastAnimation = Now.Ticks() / 10000 '上次更新動畫時間 = 現在
                            End If

                            MonsterDetect() '偵測牆
                            If MonsterRight Then '如果右邊有牆
                                MonsterX = Math.Floor(MonsterX) + (Math.Ceiling(MonsterWidth / 48) - MonsterWidth / 48 - 0.000001) '卡住怪物
                            End If
                        End If

                        '怪物擋左牆
                        If CharacterX < MonsterX + MonsterWidth / 48 / 2 And MonsterX - CharacterX < MonsterDetectArea Then '如果角色在深淵怪物左邊
                            MonsterDirection = False '向左

                            If Now.Ticks() / 10000 - LastAnimation > MonsterAniTimeGap Then '如果時間到要放下一張圖片
                                MonsterAniIndex += 1 '放下一張圖片
                                If MonsterAniIndex + LeftId > 3 Then '如果圖片放過頭
                                    MonsterAniIndex = 0 '圖片換為0
                                End If
                                MonsterAppearanceUpdate(MonsterAniIndex + LeftId) '更新外觀
                                LastAnimation = Now.Ticks() / 10000 '上次更新動畫時間 = 現在
                            End If

                            MonsterDetect() '偵測牆
                            If MonsterLeft Then '如果左邊有牆
                                MonsterX = Math.Floor(MonsterX) + 1 '卡住怪物
                            End If
                        End If

                        'Boss
                    ElseIf RightId = 5 Then
                        '怪物擋右牆
                        If CharacterX > MonsterX + MonsterWidth / 48 / 2 And CharacterX - MonsterX < MonsterDetectArea Then '如果角色在Boss右邊
                            MonsterDirection = True '怪物向右

                            MonsterAppearanceUpdate(RightId) '更新外觀

                            MonsterDetect() '偵測牆
                            If MonsterRight Then '如果怪物右邊有牆
                                MonsterX = Math.Floor(MonsterX) + (Math.Ceiling(MonsterWidth / 48) - MonsterWidth / 48 - 0.000001) '卡住怪物
                            End If
                        End If

                        '怪物擋左牆
                        If CharacterX < MonsterX + MonsterWidth / 48 / 2 And MonsterX - CharacterX < MonsterDetectArea Then '如果角色在Boss左邊
                            MonsterDirection = False '怪物向左

                            MonsterAppearanceUpdate(LeftId) '更新外觀

                            MonsterDetect() '偵測牆
                            If MonsterLeft Then '如果怪物左邊有牆
                                MonsterX = Math.Floor(MonsterX) + 1 '卡住怪物
                            End If
                        End If

                        'Boss音樂核心
                    ElseIf RightId = 7 Then
                        '怪物擋右牆
                        If CharacterX > MonsterX + MonsterWidth / 48 / 2 And CharacterX - MonsterX < MonsterDetectArea Then '如果角色在Boss音樂核心右邊
                            MonsterDirection = True '怪物向右

                            MonsterAppearanceUpdate(RightId) '更新外觀

                            MonsterDetect() '偵測牆
                            If MonsterRight Then '如果怪物右邊有牆
                                MonsterX = Math.Floor(MonsterX) + (Math.Ceiling(MonsterWidth / 48) - MonsterWidth / 48 - 0.000001) '卡住怪物
                            End If
                        End If

                        '怪物擋左牆
                        If CharacterX < MonsterX + MonsterWidth / 48 / 2 And MonsterX - CharacterX < MonsterDetectArea Then '如果角色在Boss音樂核心左邊
                            MonsterDirection = False '怪物向左

                            MonsterAppearanceUpdate(LeftId) '更新外觀

                            MonsterDetect() '偵測牆
                            If MonsterLeft Then '如果怪物左邊有牆
                                MonsterX = Math.Floor(MonsterX) + 1 '卡住怪物
                            End If
                        End If

                        '回血包
                    ElseIf RightId = 9 Then
                        '怪物擋右牆
                        If CharacterX > MonsterX + MonsterWidth / 48 / 2 And CharacterX - MonsterX < MonsterDetectArea Then '如果角色在回血包右邊
                            MonsterDirection = True '怪物向右

                            MonsterAppearanceUpdate(RightId) '更新外觀

                            MonsterDetect() '偵測牆
                            If MonsterRight Then '如果怪物右邊有牆
                                MonsterX = Math.Floor(MonsterX) + (Math.Ceiling(MonsterWidth / 48) - MonsterWidth / 48 - 0.000001) '卡住怪物
                            End If
                        End If

                        '怪物擋左牆
                        If CharacterX < MonsterX + MonsterWidth / 48 / 2 And MonsterX - CharacterX < MonsterDetectArea Then '如果角色在回血包左邊
                            MonsterDirection = False '怪物向左

                            MonsterAppearanceUpdate(LeftId) '更新外觀

                            MonsterDetect() '偵測牆
                            If MonsterLeft Then '如果怪物左邊有牆
                                MonsterX = Math.Floor(MonsterX) + 1 '卡住怪物
                            End If
                        End If
                    End If
                End If

                '發射Boss雷射---------------------------------------------------------------------------------------------------------
                If RightId = 5 And BossShouldAttack And Not Pause Then '如果是Boss，且可發射雷射，且不暫停
                    MonsterLaserDxLeft += BossLaserSpeed '左雷射向左偏移雷射
                    '畫雷射
                    e.Graphics.DrawImage(My.Resources.Game.BossLaser, New RectangleF(x + MonsterX * 48 * ScaleRatio - 328.62 * ScaleRatio - MonsterLaserDxLeft * 48 * ScaleRatio + CameraX,
                                                                                             y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight * ScaleRatio + 65 * ScaleRatio,
                                                                                             328.62 * ScaleRatio,
                                                                                             48 * ScaleRatio))

                    If x + MonsterX * 48 * ScaleRatio - 328.62 * ScaleRatio - MonsterLaserDxLeft * 48 * ScaleRatio <= 0 Then '如果雷射撞牆
                        MonsterLaserDxLeft = 0 '雷射向左偏移 = 0
                    End If

                    MonsterLaserDxRight += BossLaserSpeed '右雷射向右偏移雷射
                    '畫雷射
                    e.Graphics.DrawImage(My.Resources.Game.BossLaser, New RectangleF(x + MonsterX * 48 * ScaleRatio + MonsterWidth * ScaleRatio + MonsterLaserDxRight * 48 * ScaleRatio + CameraX,
                                                                                             y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight * ScaleRatio + 65 * ScaleRatio,
                                                                                             328.62 * ScaleRatio,
                                                                                             48 * ScaleRatio))
                    If x + MonsterX * 48 * ScaleRatio - 328.62 * ScaleRatio + MonsterLaserDxRight * 48 * ScaleRatio >= x + Map_Width * 48 * ScaleRatio Then '如果雷射撞牆
                        MonsterLaserDxRight = 0 '雷射向右偏移 = 0
                    End If
                End If

                '怪物重力---------------------------------------------------------------------------------------------------------
                If Not Pause Then '如果沒有暫停
                    MonsterYv -= G '向下速度
                    MonsterY += MonsterYv * 5 '根據Y軸速度，移動怪物
                    MonsterDetect() '偵測牆
                    If MonsterTop Then '如果上方有牆
                        MonsterY = Math.Floor(MonsterY + MonsterHeight / 48) - MonsterHeight / 48 '卡住怪物
                    End If
                    MonsterDetect() '偵測牆
                    If MonsterBottom Then '如果下方有牆
                        MonsterY = Math.Floor(MonsterY) + 1 '卡住怪物
                        MonsterYv = 0 'Y軸速度 = 0
                    End If
                End If

                '畫怪物---------------------------------------------------------------------------------------------------------
                '設定怪物要畫的地方
                MonsterBox = New RectangleF(x + CameraX + MonsterX * 48 * ScaleRatio,
                                                    y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight * ScaleRatio,
                                                    MonsterWidth * ScaleRatio,
                                                    MonsterHeight * ScaleRatio)
                e.Graphics.DrawImage(MonsterImg, MonsterBox) '畫怪物

                '偵測子彈是否打到怪物---------------------------------------------------------------------------------------------------------
                If Not Pause And RightId <> 9 Then '如果沒有暫停，且不是回血包
                    If CanShoot = False Then '如果不能射(代表正在射)
                        If BulletDirection = False And CharacterShootX < MonsterX And '如果子彈向左
                               BulletX > MonsterX And '子彈在怪物左邊
                               ((MonsterY < BulletY And BulletY < MonsterY + MonsterHeight / 48) Or (MonsterY < BulletY + 0.5 And BulletY + 0.5 < MonsterY + MonsterHeight / 48)) Then '子彈Y座標有擦到怪物

                            Damage = CharacterHarmDamage '設定傷害數值
                            MonsterHealth -= Damage '造成傷害
                            LastShowDamage = Now.Ticks() / 10000 '更新上次顯示時間

                            DamageTextBulletX = BulletX '更新被打到的X座標

                            ShowDamage = True '顯示傷害

                            '播放音效
                            Hit.URL = My.Application.Info.DirectoryPath & "\Music\打到.mp3" '選擇路徑
                            Hit.settings.setMode("loop", False) '設定是否循環
                            Hit.controls.play() '播放

                            CanShoot = True '可以射(代表射到怪物)

                        ElseIf BulletDirection = True And CharacterShootX > MonsterX And '如果子彈向右
                                   BulletX < MonsterX + MonsterWidth / 48 And '子彈在怪物右邊
                                   ((MonsterY < BulletY And BulletY < MonsterY + MonsterHeight / 48) Or (MonsterY < BulletY + 0.5 And BulletY + 0.5 < MonsterY + MonsterHeight / 48)) Then '子彈Y座標有擦到怪物

                            Damage = CharacterHarmDamage '設定傷害數值
                            MonsterHealth -= Damage '造成傷害
                            LastShowDamage = Now.Ticks() / 10000 '更新上次顯示時間

                            DamageTextBulletX = BulletX '更新被打到的X座標

                            ShowDamage = True '顯示傷害

                            '播放音效
                            Hit.URL = My.Application.Info.DirectoryPath & "\Music\打到.mp3" '選擇路徑
                            Hit.settings.setMode("loop", False) '設定是否循環
                            Hit.controls.play() '播放

                            CanShoot = True '可以射(代表射到怪物)
                        End If
                    End If
                End If

                If RightId <> 9 Then '如果不是回血包
                    '畫怪物血量
                    e.Graphics.FillRectangle(Brushes.White, New RectangleF(x + CameraX + MonsterX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                                                   y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight * ScaleRatio - 10 * ScaleRatio,
                                                                                   MonsterWidth * ScaleRatio,
                                                                                   5 * ScaleRatio))
                    e.Graphics.FillRectangle(Brushes.Red, New RectangleF(x + CameraX + MonsterX * 48 * ScaleRatio + 5 * ScaleRatio,
                                                                                   y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight * ScaleRatio - 10 * ScaleRatio,
                                                                                   MonsterWidth * ScaleRatio * MonsterHealth / MonsterMaxHealth,
                                                                                   5 * ScaleRatio))
                End If

                '顯示傷害
                If ShowDamage And RightId <> 9 Then '如果要顯示傷害，且不是回血包
                    If Now.Ticks() / 10000 - LastShowDamage > ShowDamageTime Then '如果顯示時間超過設定時間
                        ShowDamage = False '停止顯示
                    Else
                        '畫傷害文字
                        DamageText.point.X = x + DamageTextBulletX * ScaleRatio * 48 + CameraX
                        DamageText.point.Y = y + CameraY + (height - Map_Height * 48 * ScaleRatio) + (Map_Height - MonsterY) * 48 * ScaleRatio - MonsterHeight / 2 * ScaleRatio
                        DamageText.Draw("-" & CStr(Damage), e, myfont, ScaleRatio)
                    End If
                End If

                If Not Pause Then
                    '傷害角色
                    If CanDamageCharacter And '如果可以攻擊角色
                       CharacterX - 0.2 < MonsterX + MonsterWidth / 48 / 2 And MonsterX + MonsterWidth / 48 / 2 < CharacterX + 1 And '角色X座標碰到怪物X座標
                       ((CharacterY < MonsterY + MonsterHeight / 48 / 2 And MonsterY + MonsterHeight / 48 / 2 < CharacterY + 1.8) Or ((CharacterY < MonsterY + 0.1 And MonsterY + 0.1 < CharacterY + 1.8))) Then '角色Y座標碰到怪物Y座標

                        CharacterDamage = MonsterHarm
                        CharacterHealth -= CharacterDamage '讓角色受傷
                        CharacterShowDamage = True '讓角色顯示傷害
                        CharacterLastShowDamage = Now.Ticks() / 10000 '更新角色上次顯示傷害時間
                        LastDamageCharacter = Now.Ticks() / 10000 '更新上次攻擊角色時間

                        If RightId = 9 Then '如果是回血包
                            MonsterHealth = 0 '讓回血包消失
                            '播放補血音效
                            CharacterBeenHit.URL = My.Application.Info.DirectoryPath & "\Music\補血.mp3" '選擇路徑
                            CharacterBeenHit.settings.setMode("loop", False) '設定是否循環
                            CharacterBeenHit.controls.play() '播放
                        Else
                            '播放被打音效
                            CharacterBeenHit.URL = My.Application.Info.DirectoryPath & "\Music\被打.mp3" '選擇路徑
                            CharacterBeenHit.settings.setMode("loop", False) '設定是否循環
                            CharacterBeenHit.controls.play() '播放
                        End If

                        CanDamageCharacter = False '不可以傷害角色
                    End If

                    'Boss雷射
                    If RightId = 5 And BossShouldAttack And CanDamageCharacter And '如果是Boss，且可以設射雷射，且可以傷害角色，且角色碰到雷射
                            (((MonsterX - MonsterLaserDxLeft - 328.62 / 48 < CharacterX - 0.2 And CharacterX - 0.2 < MonsterX - MonsterLaserDxLeft) Or (MonsterX - MonsterLaserDxLeft - 328.62 / 48 < CharacterX + 1 And CharacterX + 1 < MonsterX - MonsterLaserDxLeft)) Or
                            ((MonsterX + MonsterLaserDxRight < CharacterX - 0.2 And CharacterX - 0.2 < MonsterX + MonsterLaserDxRight + 328.62 / 48) Or (MonsterX + MonsterLaserDxRight < CharacterX + 1 And CharacterX + 1 < MonsterX + MonsterLaserDxRight + 328.62 / 48))) And
                            ((MonsterY + MonsterHeight / 48 - 65 / 48 - 1 < CharacterY + 1.8 And CharacterY + 1.8 < MonsterY + MonsterHeight / 48 - 65 / 48) Or (MonsterY + MonsterHeight / 48 - 65 / 48 - 1 < CharacterY + 0.1 And CharacterY + 0.1 < MonsterY + MonsterHeight / 48 - 65 / 48)) Then

                        CharacterDamage = MonsterHarm
                        CharacterHealth -= CharacterDamage '讓角色受傷
                        CharacterShowDamage = True '讓角色顯示傷害
                        CharacterLastShowDamage = Now.Ticks() / 10000 '更新角色上次顯示傷害時間

                        If Now.Ticks() - LastDamageCharacter >= BossAttackSoundDelay Then
                            '播放被打音效
                            CharacterBeenHit.URL = My.Application.Info.DirectoryPath & "\Music\被打.mp3" '選擇路徑
                            CharacterBeenHit.settings.setMode("loop", False) '設定是否循環
                            CharacterBeenHit.controls.play() '播放
                            LastDamageCharacter = Now.Ticks() / 10000 '更新上次攻擊角色時間
                        End If

                        CanDamageCharacter = False '不可以傷害角色
                    End If

                    If Now.Ticks() / 10000 - LastDamageCharacter > DamageTimeGap Then '如果距離上次攻擊角色的時間到
                        CanDamageCharacter = True '可以傷害角色
                    End If
                End If

                '怪物死掉
            ElseIf MonsterHealth <= 0 And MonsterHasNotDied Then
                MonsterHasNotDied = False

                '加分
                Select Case RightId
                    Case 1
                        xp += SlimeXp
                    Case 3
                        xp += AbyssXp
                    Case 5
                        xp += BossXp
                    Case 7
                        xp += BossMusicCoreXp
                End Select

                If RightId <> 5 And RightId <> 9 Then '如果不是Boss或回血包
                    '播放加分音效
                    AddScore.URL = My.Application.Info.DirectoryPath & "\Music\加分.mp3" '選擇路徑
                    AddScore.settings.setMode("loop", False) '設定是否循環
                    AddScore.controls.play() '播放
                End If
            End If
        End Sub
    End Class

    '快速調整設定區--------------------------------------------------------------------------------------------------------------------------------------------
    Dim State As String = "Start" '整個程式的起始點

    Const Version As String = "Release 1.1"
    Const Copyright As String = "III Studio 製作"

    Const DefaultWidth As Integer = 800 '預設的寬度
    Const DefaultHeight As Integer = 450 '預設的高度
    Const DefaultDebugPanelOn As Boolean = False '預設除錯面板開啟狀態

    Const OpeningDimSpeed As Double = 7.5 '開場動畫淡入淡出的速度
    Const OpeningGapSpeed As Double = 1000 '開場動畫淡入淡出的速度 (ms)
    Const OpeningSpeed As Integer = 2000 '每個開場動畫持續的時間 (ms)
    Const ShowDamageTime As Double = 250 '顯示傷害數值的時間 (ms)

    Const RegenHealthChance As Double = 50 '不出現回血包的機率 (%)

    Const SlimeXp As Integer = 30
    Const AbyssXp As Integer = 75
    Const BossXp As Integer = 300
    Const BossMusicCoreXp As Integer = 300

    '初始化Timer--------------------------------------------------------------------------------------------------------------------------------------------
    Dim ScreenRefresh As New Timer(1)

    '字體初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim myfont As New PrivateFontCollection
    Dim bytes As Byte()
    Dim ptr As IntPtr

    '螢幕縮放比例初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim MyWidth As Integer = DefaultWidth
    Dim MyHeight As Integer = DefaultHeight
    Dim ScaleRatio As Double = 1.0

    '鍵盤初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim Keyboard(256) As Boolean

    '滑鼠初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim Mouse As New Point(0, 0)
    Dim MousePressed As Boolean = False

    '音樂初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim BGM As New WMPLib.WindowsMediaPlayer
    Dim SoundEffect As New WMPLib.WindowsMediaPlayer
    Dim Ding As New WMPLib.WindowsMediaPlayer

    'DebugPanel 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim DebugPanelOn As Boolean = DefaultDebugPanelOn
    Dim DebugInfo As New MyTextBox With {.font = "Consolas", .font_size = 10, .color = Color.Green}

    '計算幀數的東東--------------------------------------------------------------------------------------------------------------------------------------------
    Dim lastUpdate As Long
    Dim FPS As Double
    Dim Frametime As Double

    '背景初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim BGisOn As Boolean = True '預設的背景是否顯示
    Dim BGColorA As Double = 0 '預設透明度 (0-255)
    Dim BGColors As Color() = {Color.FromArgb(35, 35, 35), Color.FromArgb(0, 0, 0), Color.FromArgb(35, 35, 35)}
    Dim BGColorPosition As Single() = {0.0F, 0.5F, 1.0F}
    Dim BGColorBlend As New ColorBlend
    Dim BGBrush As New LinearGradientBrush(New Point(0, 0), New Point(100, 500), Color.Black, Color.White)
    Dim BGAnimation As Single = 0

    '開場動畫計時器--------------------------------------------------------------------------------------------------------------------------------------------
    Dim OpeningStartTime As Long 'Ticks = 毫秒/10000

    'Loading 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim LoadingIndex As Integer = 0
    Dim LoadingShow As Boolean = False '是否要顯示Loading
    Dim LoadingSeq() As Bitmap = {My.Resources.Loading._0, My.Resources.Loading._1, My.Resources.Loading._2, My.Resources.Loading._3, My.Resources.Loading._4, My.Resources.Loading._5, My.Resources.Loading._6, My.Resources.Loading._7, My.Resources.Loading._8, My.Resources.Loading._9, My.Resources.Loading._10, My.Resources.Loading._11, My.Resources.Loading._12, My.Resources.Loading._13, My.Resources.Loading._14, My.Resources.Loading._15, My.Resources.Loading._16, My.Resources.Loading._17, My.Resources.Loading._18, My.Resources.Loading._19, My.Resources.Loading._20, My.Resources.Loading._21, My.Resources.Loading._22, My.Resources.Loading._23, My.Resources.Loading._24, My.Resources.Loading._25, My.Resources.Loading._26, My.Resources.Loading._27, My.Resources.Loading._28, My.Resources.Loading._29}
    Dim Loading As New MyPictureBox

    'StudioLogo 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim StudioLogo As New MyTextBox With {.font = "Monoton", .font_size = 50, .color = Color.White, .opacity = 0, .Align = "Center", .LineAlign = "Center"}

    'GameLogo 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim GameLogo As New MyTextBox With {.font = "Cubic11", .font_size = 50, .color = Color.White, .opacity = 0, .Align = "Center", .LineAlign = "Center"}
    Dim GameSubLogo As New MyTextBox With {.font = "Cubic11", .font_size = 30.9, .color = Color.FromArgb(157, 157, 157), .opacity = 0, .Align = "Center", .LineAlign = "Center"}

    'Menu 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim MenuCaption As New MyPictureBox With {.Image = My.Resources.Menu.Caption}
    Dim VersionInfo As New MyTextBox With {.font = "Cubic11", .font_size = 8.5, .color = Color.FromArgb(0, 157, 157, 157), .Align = "Left", .LineAlign = "Bottom"}
    Dim CopyrightInfo As New MyTextBox With {.font = "Cubic11", .font_size = 8.5, .color = Color.FromArgb(0, 157, 157, 157), .Align = "Right", .LineAlign = "Bottom"}

    Dim StartButton As New MyButton With {.Image = My.Resources.Menu.Start, .PressedImage = My.Resources.Menu.Start_Pressed, .Player = Ding}
    Dim HowToPlayButton As New MyButton With {.Image = My.Resources.Menu.HowToPlay, .PressedImage = My.Resources.Menu.HowToPlay_Pressed, .Player = Ding}
    Dim SettingButton As New MyButton With {.Image = My.Resources.Menu.Setting, .PressedImage = My.Resources.Menu.Setting_Pressed, .Player = Ding}
    Dim ExitButton As New MyButton With {.Image = My.Resources.Menu._Exit, .PressedImage = My.Resources.Menu.Exit_Pressed, .Player = Ding}

    'HowToPlay1 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim NextPageButton As New MyButton With {.Image = My.Resources.HowToPlay.NextPage, .PressedImage = My.Resources.HowToPlay.NextPage_Pressed, .Player = Ding}
    Dim HowToPlay_Text As New MyTextBox With {.font = "Cubic11", .font_size = 21.5, .color = Color.Black}
    Dim HowToPlay_Img As New MyPictureBox With {.Image = My.Resources.HowToPlay.HowToPlay1}
    Dim HowToPlay1_Map As Integer(,) = {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1},
                                        {1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1},
                                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}
    Dim DemoGame As New Game With {.Map = HowToPlay1_Map, .Map_Width = 16, .Map_Height = 8, .BG = My.Resources.Game.Day}

    'HowToPlay2 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim HowToPlay2_Map As Integer(,) = {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}
    Dim DemoMonster As New Monster With {.MonsterX = 9, .MonsterY = 4}

    'Setting 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim SettingText As New MyTextBox With {.font = "Cubic11", .font_size = 30.9, .color = Color.White}
    Dim MusicSlider As New Slider
    Dim SoundEffectSlider As New Slider
    Dim DoneSettingButton As New MyButton With {.Image = My.Resources.Setting.DoneSetting, .PressedImage = My.Resources.Setting.DoneSetting_Activated, .Player = Ding}

    'Intro 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim IntroStartTime As Long '開始顯示字幕的時間 (ms)
    Dim IntroText() As String = {"我曾踏過千山萬水", "行過無數市鎮", "與牧人一起唱著山歌", "和漁夫共同經歷風暴", "只為求得那", "不存於人世的音樂", "那是我年輕時的故事了", "呵呵，別看我現在這糟老頭樣", "我年輕時可是才華洋溢的提琴手呢！", "孩子們", "靠過來點", "今天老莫爺爺我啊", "要說的是我年輕時最不可思議的事", "那就是跟使用邪惡音樂", "攻擊村莊的魔王死戰", "嘿！誰說英雄一定要拿劍的", "坐好坐好，要開始囉！", "嗯咳！", "那，是一個陰暗的日子。。。。。"}
    Dim IntroTimeCodeStart As Integer() = {12000, 15040, 16704, 19104, 21696, 23392, 26656, 28736, 31360, 34592, 35488, 36864, 38784, 42432, 44150, 46478, 49166, 51086, 52270} '(ms)
    Dim IntroTimeCodeEnd As Integer() = {14816, 16640, 18912, 21632, 23232, 26112, 28608, 31104, 34016, 35296, 36512, 38432, 41920, 44088, 45710, 48878, 50510, 51534, 56750} '(ms)
    Dim OriginalBGMvolume As Integer
    Dim SkipButtonShow As Boolean

    '文字動畫的東西
    Dim TextSpeed As Double = 15
    Dim TextIndex As Integer = 0
    Dim TextAnimationIndex As Integer = 0

    '顯示文字的東西
    Dim ShowText As New MyTextBox With {.font = "Cubic11", .font_size = 30, .color = Color.White, .LineAlign = "Center"}
    Dim NowShowText As String
    '最後的變暗

    Dim DimScreen As Integer = 0
    '跳過按鈕

    Dim SkipButton As New MyButton With {.Image = My.Resources.Intro.Skip, .PressedImage = My.Resources.Intro.SkipActivated, .Player = Ding}

    'SelectLevel 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim SelectLevelText As New MyTextBox With {.font = "Cubic11", .font_size = 30.9, .color = Color.White}

    Dim Button11 As New MyButton With {.Image = My.Resources.SelectLevel._1_1, .PressedImage = My.Resources.SelectLevel._1_1A, .Player = Ding}
    Dim Button12 As New MyButton With {.Image = My.Resources.SelectLevel._1_2, .PressedImage = My.Resources.SelectLevel._1_2A, .Player = Ding}
    Dim Button13 As New MyButton With {.Image = My.Resources.SelectLevel._1_3, .PressedImage = My.Resources.SelectLevel._1_3A, .Player = Ding}
    Dim Button14 As New MyButton With {.Image = My.Resources.SelectLevel._1_4, .PressedImage = My.Resources.SelectLevel._1_4A, .Player = Ding}
    Dim LevelDoneButton As New MyButton With {.Image = My.Resources.SelectLevel.Done, .PressedImage = My.Resources.SelectLevel.DoneA, .Player = Ding}

    Dim LevelPreview As New MyPictureBox With {.Image = My.Resources.SelectLevel._1_1Preview}

    'Game 初始化--------------------------------------------------------------------------------------------------------------------------------------------
    Dim MainGame1_Map As Integer(,) = {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                      {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, 0, 3, 0, 0, 0, 0, 1},
                                      {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, 0, 3, 0, 0, 0, 0, 1},
                                      {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, 0, 3, 0, 0, 0, 0, 1},
                                      {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 3, 0, 0, 0, 0, 1},
                                      {1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 3, 0, 0, -4, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, 0, 3, 0, 0, 0, 0, 1},
                                      {1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 3, 0, 0, -4, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, 0, 3, 0, 0, 0, 0, 1},
                                      {1, 0, 0, 0, 1, 1, 1, 3, 2, 2, 2, 3, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 0, 0, 0, 0, 1},
                                      {1, 1, 0, 0, 0, 0, 0, -4, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -5, 1},
                                      {1, 1, 1, 0, 0, 0, 0, -4, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
                                      {1, 1, 1, 1, 0, 0, 0, -4, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1},
                                      {1, 0, 0, 0, 0, 0, 1, 3, 2, 2, 2, 3, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1},
                                      {1, 0, 0, 0, 0, 1, 1, 3, 0, 0, 0, 3, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1},
                                      {1, 0, 0, 0, 1, 1, 1, 3, 0, 0, 0, 3, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1},
                                      {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}

    Dim MainGame2_Map As Integer(,) = {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                       {1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, -5, 1},
                                       {1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                       {1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 3, 0, 0, 0, -4, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 3, 1, 0, 0, 0, 0, 3, 2, 0, 0, -4, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 3, 1, 1, 0, 0, 0, 3, 2, 2, 2, 2, 1, 1, 0, 0, 0, 0, 1},
                                       {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 1, 1, 1, 0, 0, 3, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1},
                                       {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 3, 1, 1, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 1, 1, 1, 2, 2, 0, 0, 1, 1, 0, 2, 2, 2, 3, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1},
                                       {1, 1, 1, 1, 3, 0, 0, 0, 0, 1, 0, 0, 3, 0, 3, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1},
                                       {1, 1, 1, 0, 3, 0, 0, 0, 0, 0, 0, 0, 3, 0, 3, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1},
                                       {1, 1, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 3, 0, 3, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1},
                                       {1, 1, 0, 0, 3, 0, 0, 0, 1, 0, 0, 0, 3, 0, 3, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1},
                                       {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}

    Dim MainGame3_Map As Integer(,) = {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                       {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 0, 0, 0, 2, 2, 2, 3, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1},
                                       {1, 1, 0, 0, 0, 0, 0, 3, 1, 1, 0, 0, 1, 2, 2, 2, 2, 2, 2, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1},
                                       {1, 1, 1, 0, 0, 0, 0, 3, 1, 1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 0, 0, 1},
                                       {1, 1, 1, 1, 0, 0, 0, 3, 1, 1, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 1, 0, 0, 1},
                                       {1, 0, 0, 0, 0, 0, 1, 3, 1, 1, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 1, 0, 1},
                                       {1, 0, 0, 0, 0, 1, 1, 3, 1, 2, 0, 0, 0, 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, -5, 1},
                                       {1, 0, 0, 0, 1, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 2, 2, 0, 0, 3, 0, 0, 0, 0, 2, 2, 2, 1},
                                       {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 3, 2, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 3, 2, 2, 0, 0, 0, 0, 0, 1},
                                       {1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 2, 2, 2, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 3, 2, 2, 2, 0, 0, 0, 0, 1},
                                       {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}

    Dim MainGame4_Map As Integer(,) = {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                                       {1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                                       {1, 1, 1, 1, 0, 0, 1, 3, 0, 0, 3, 0, 0, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 0, 0, 0, 1},
                                       {1, 0, 0, 0, 0, 0, 1, 3, 2, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 2, 1},
                                       {1, 0, 0, 0, 0, 1, 1, 3, 0, 0, 3, 2, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 2, 2, 1},
                                       {1, 0, 0, 0, 1, 1, 1, 3, 0, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 2, 2, 2, 1},
                                       {1, 1, 0, 0, 0, 0, 0, 3, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 2, 0, 0, 0, 0, 0, 1},
                                       {1, 1, 1, 0, 0, 0, 0, 3, 0, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 2, 2, 0, 0, 0, 0, 1},
                                       {1, 1, 1, 1, 0, 0, 0, 3, 0, 0, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 0, 0, 0, 1},
                                       {1, 0, 0, 0, 0, 0, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1},
                                       {1, 0, 0, 0, 0, 1, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 1},
                                       {1, 0, 0, 0, 1, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 1},
                                       {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}


    Dim MainGame As New Game With {.Map = MainGame1_Map, .Map_Width = 32, .Map_Height = 15, .BG = My.Resources.Game.Day}

    Dim Slime1 As New Monster
    Dim Slime2 As New Monster
    Dim Slime3 As New Monster
    Dim Abyssosque1 As New Monster
    Dim Abyssosque2 As New Monster
    Dim Abyssosque3 As New Monster
    Dim Abyssosque4 As New Monster
    Dim Abyssosque5 As New Monster
    Dim Abyssosque6 As New Monster
    Dim Abyssosque7 As New Monster
    Dim Abyssosque8 As New Monster

    Dim Boss As New Monster
    Dim BossMusicCore As New Monster

    Dim RegenHealth As New Monster
    Dim RegenHealth2 As New Monster

    Dim Xp As Integer = 0
    Dim XpText As New MyTextBox With {.font = "Cubic11", .font_size = 18.75, .color = Color.Green, .LineAlign = "Center", .Align = "Center"}
    Dim XpBG As New MyPictureBox With {.Image = My.Resources.Game.XPBG}

    Dim PauseButton As New MyButton With {.Image = My.Resources.Game.PauseButton, .PressedImage = My.Resources.Game.PauseButtonA, .Player = Ding}

    'GodMode--------------------------------------------------------------------------------------------------------------------------------------------------
    Dim GodMode As Boolean = False
    Dim GodModePanel As New MyTextBox With {.font = "Consolas", .font_size = 18, .color = Color.Green}

    'DeadMenu--------------------------------------------------------------------------------------------------------------------------------------------
    Dim ShowDeathMenu As Boolean = False
    Dim DiedMessageArray As String() = {"讓家族...蒙羞..了....", "末日審判見~", "見上帝吧！", "我們懷念你", "Rest In Peace"}
    Dim DiedMessage As String
    Dim DiedText As New MyTextBox With {.font = "Cubic11", .font_size = 30, .color = Color.White, .LineAlign = "Center", .Align = "Center"}
    Dim DiedXpText As New MyTextBox With {.font = "Cubic11", .font_size = 18.75, .color = Color.White, .LineAlign = "Center", .Align = "Center"}
    Dim ReplayButton As New MyButton With {.Image = My.Resources.DiedMenu.Replay, .PressedImage = My.Resources.DiedMenu.ReplayActivated, .Player = Ding}
    Dim EndButton As New MyButton With {.Image = My.Resources.DiedMenu._End, .PressedImage = My.Resources.DiedMenu.EndActivated, .Player = Ding}

    'Pause----------------------------------------------------------------------------------------------------------------------------------------------
    Dim Pause As New MyTextBox With {.font = "Cubic11", .font_size = 30, .color = Color.White, .LineAlign = "Center", .Align = "Center"}
    Dim ToMenuButton As New MyButton With {.Image = My.Resources.Game.ToMenu, .PressedImage = My.Resources.Game.ToMenuA, .Player = Ding}
    Dim ContinueButton As New MyButton With {.Image = My.Resources.Game._Continue, .PressedImage = My.Resources.Game.ContinueA, .Player = Ding}

    'EndGame----------------------------------------------------------------------------------------------------------------------------
    Dim EndGameStartTime As Long = 0
    Dim EndGameText As New MyTextBox With {.font = "Cubic11", .font_size = 30, .color = Color.White, .LineAlign = "Center", .Align = "Center"}
    Dim EndGameXpText As New MyTextBox With {.font = "Cubic11", .font_size = 18.75, .color = Color.White, .LineAlign = "Center", .Align = "Center"}

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        '畫背景
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

        '畫載入圈圈
        If LoadingShow Then '如果要顯示載入圈圈
            LoadingIndex += 1 '下一張圖片
            If LoadingIndex >= LoadingSeq.Length() - 1 Then '如果圖片放過頭
                LoadingIndex = 0 '回到第0張圖片
            End If

            '畫圈圈
            Loading.BoxSetting(MyWidth - 50 * ScaleRatio, MyHeight - 50 * ScaleRatio, 30 * ScaleRatio, 30 * ScaleRatio)
            Loading.Image = LoadingSeq(LoadingIndex)
            Loading.Draw(e)
        End If

        '依照狀態更新畫面
        Select Case State
            Case "Start"
                BGisOn = True

                GameLogo.opacity = 0
                GameLogo.AlignSetting("Center", "Center")
                GameSubLogo.opacity = 0
                GameSubLogo.AlignSetting("Center", "Center")
                OpeningStartTime = Now.Ticks()

                State = "StudioLogo"

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
                    State = "ToMenu"
                End If

                GameLogo.point.X = MyWidth / 2
                GameLogo.point.Y = MyHeight / 2 - 28 * ScaleRatio
                GameLogo.Draw("莫舒巴頓", e, myfont, ScaleRatio)

                GameSubLogo.point.X = MyWidth / 2
                GameSubLogo.point.Y = MyHeight / 2 + 37.5 * ScaleRatio
                GameSubLogo.Draw("Mosche Barton", e, myfont, ScaleRatio)

            Case "ToMenu"
                BGisOn = True
                State = "Menu"
                SoundEffect.controls.stop()
                PlaySound(BGM, "MenuBGM.wav", True)

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
                If StartButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "Intro"
                    IntroStartTime = Now.Ticks() / 10000
                    TextIndex = 0
                    TextAnimationIndex = 0
                    DimScreen = 0
                    BGM.controls.stop()
                End If

                HowToPlayButton.Box = New RectangleF(MyWidth - 289 * ScaleRatio, 170 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If HowToPlayButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "HowToPlay1"
                    HowToPlay_Img.Image = My.Resources.HowToPlay.HowToPlay1
                    BGisOn = False
                    DemoGame.CharacterReset(1.1, 1)
                    DemoGame.MapSetting(16, 8)
                    DemoGame.CameraReset()
                    DemoGame.CharacterHealthReset()
                    DemoGame.Map = HowToPlay1_Map

                    NextPageButton.Image = My.Resources.HowToPlay.NextPage
                    NextPageButton.PressedImage = My.Resources.HowToPlay.NextPage_Pressed
                End If

                SettingButton.Box = New RectangleF(MyWidth - 258 * ScaleRatio, 245 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If SettingButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "Setting"
                End If

                ExitButton.Box = New RectangleF(MyWidth - 238 * ScaleRatio, 319 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If ExitButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "Exit"
                End If


            Case "HowToPlay1"
                DemoGame.BoxSetting(MyWidth / 2 - 335 * ScaleRatio, MyHeight / 2 - 112 * ScaleRatio, 670 * ScaleRatio, 287 * ScaleRatio)
                DemoGame.DrawGame(e, ScaleRatio, Keyboard, Mouse, MousePressed, SoundEffect.settings.volume)

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
                If NextPageButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    HowToPlay_Img.Image = My.Resources.HowToPlay.HowToPlay2
                    State = "HowToPlay2"
                    DemoGame.CharacterReset(1.1, 1)
                    DemoGame.MapSetting(14, 8)
                    DemoGame.CameraReset()
                    DemoGame.CharacterHealthReset()
                    DemoGame.Map = HowToPlay2_Map

                    DemoMonster.MonsterReset(0, 1, 9, 1, 0)

                    NextPageButton.Image = My.Resources.Game.ToMenu
                    NextPageButton.PressedImage = My.Resources.Game.ToMenuA
                End If

            Case "HowToPlay2"
                DemoGame.BoxSetting(MyWidth / 2 - 335 * ScaleRatio, MyHeight / 2 - 112 * ScaleRatio, 670 * ScaleRatio, 287 * ScaleRatio)
                DemoGame.DrawGame(e, ScaleRatio, Keyboard, Mouse, MousePressed, SoundEffect.settings.volume)

                DemoMonster.SyncWith(DemoGame)
                DemoMonster.Draw(e, ScaleRatio, DemoGame.CanShoot, DemoGame.CharacterHealth, DemoGame.Damage, DemoGame.ShowDamage, DemoGame.LastShowDamage, DemoGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)

                HowToPlay_Img.BoxSetting(MyWidth / 2 - 702 / 2 * ScaleRatio, MyHeight / 2 - 382.811 / 2 * ScaleRatio, 702 * ScaleRatio, 382.811 * ScaleRatio)
                HowToPlay_Img.Draw(e)

                HowToPlay_Text.point.Y = MyHeight / 2 - 167 * ScaleRatio

                HowToPlay_Text.point.X = MyWidth / 2 - 330 * ScaleRatio
                HowToPlay_Text.Draw("按下滑鼠攻擊敵人，", e, myfont, ScaleRatio)
                HowToPlay_Text.point.X = MyWidth / 2 - 4 * ScaleRatio
                HowToPlay_Text.Draw("釋放技能", e, myfont, ScaleRatio)

                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(0, 0, MyWidth, MyHeight / 2 - 382.811 / 2 * ScaleRatio))
                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(0, MyHeight / 2 - 382.811 / 2 * ScaleRatio + 382.811 * ScaleRatio, MyWidth, MyHeight - (MyHeight / 2 - 382.811 / 2 * ScaleRatio + 382.811 * ScaleRatio)))
                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(0, MyHeight / 2 - 382.811 / 2 * ScaleRatio, MyWidth / 2 - 702 / 2 * ScaleRatio, 382.811 * ScaleRatio))
                e.Graphics.FillRectangle(Brushes.Black, New RectangleF(MyWidth / 2 - 702 / 2 * ScaleRatio + 702 * ScaleRatio, MyHeight / 2 - 382.811 / 2 * ScaleRatio, MyWidth / 2 - 702 / 2 * ScaleRatio, 382.811 * ScaleRatio))

                NextPageButton.Box = New RectangleF(MyWidth / 2 + 175 * ScaleRatio, MyHeight / 2 + 150 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If NextPageButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
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
                If DoneSettingButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "Menu"
                End If

            Case "Intro"
                Select Case Now.Ticks() / 10000 - IntroStartTime
                    Case 0 To 3000
                        LoadingShow = True
                        SkipButtonShow = True
                        OriginalBGMvolume = BGM.settings.volume
                        TextAnimationIndex = 0
                        NowShowText = ""

                    Case 3000 To 3250
                        PlaySound(BGM, "Bach Air on the G String.mp3", False)
                        LoadingShow = False

                    Case IntroTimeCodeStart(0) To IntroTimeCodeStart(0) + 250
                        PlaySound(SoundEffect, "開場配音.wav", False)

                    Case IntroTimeCodeEnd(IntroText.Length() - 1) To IntroTimeCodeEnd(IntroText.Length() - 1) + 2000
                        SkipButtonShow = False
                        If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                            DimScreen += 5
                            If DimScreen >= 255 Then
                                DimScreen = 255
                            End If
                        End If
                        BGM.settings.volume *= 0.95

                    Case IntroTimeCodeEnd(IntroText.Length() - 1) + 2000 To IntroTimeCodeEnd(IntroText.Length() - 1) + 2000 + 2000
                        State = "ToSelectLevel"
                        MainGame.level = 0
                        LevelPreview.Image = My.Resources.SelectLevel._1_1Preview
                        BGM.settings.volume = OriginalBGMvolume

                    Case IntroTimeCodeStart(TextIndex) To IntroTimeCodeEnd(TextIndex)
                        If TextAnimationIndex < IntroText(TextIndex).Length Then
                            If (Now.Ticks / 10000 - IntroStartTime) Mod TextSpeed < 30 Then
                                NowShowText += IntroText(TextIndex)(TextAnimationIndex)
                                TextAnimationIndex += 1
                            End If
                        ElseIf Now.Ticks() / 10000 - IntroStartTime >= IntroTimeCodeEnd(TextIndex) - 30 Then
                            NowShowText = ""
                            TextIndex += 1
                            If TextIndex >= IntroText.Length() - 1 Then
                                TextIndex = IntroText.Length() - 1
                            End If
                        End If

                    Case Else
                        TextAnimationIndex = 0
                        NowShowText = ""
                End Select
                If Not NowShowText = "" Then
                    ShowText.point.X = MyWidth / 2 - IntroText(TextIndex).Length() / 2 * ShowText.FontHeight(e, myfont, ScaleRatio) + 1.15 * IntroText(TextIndex).Length() * ScaleRatio
                    ShowText.point.Y = MyHeight / 2
                    ShowText.Draw(NowShowText, e, myfont, ScaleRatio)
                End If

                If SkipButtonShow Then
                    SkipButton.Box = New RectangleF(MyWidth - 104 * ScaleRatio, 20 * ScaleRatio, 81.695 * ScaleRatio, 21.4067 * ScaleRatio)
                    If SkipButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                        State = "ToSelectLevel"
                        MainGame.level = 0
                        LevelPreview.Image = My.Resources.SelectLevel._1_1Preview
                    End If
                End If

                e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(DimScreen, 0, 0, 0)), New Rectangle(0, 0, MyWidth, MyHeight))

            Case "ToSelectLevel"
                BGM.controls.stop()
                SoundEffect.controls.stop()
                LoadingShow = False

                SettingText.point.X = 50 * ScaleRatio
                SettingText.point.Y = 42 * ScaleRatio
                SettingText.Draw("選擇關卡  關卡 1-" & CStr(MainGame.level + 1), e, myfont, ScaleRatio)

                Button11.Box = New RectangleF(64 * ScaleRatio, 140 * ScaleRatio, 93.5 * ScaleRatio, 49 * ScaleRatio)
                If Button11.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    MainGame.level = 0
                    MainGame.Map = MainGame1_Map
                    LevelPreview.Image = My.Resources.SelectLevel._1_1Preview
                End If
                Button12.Box = New RectangleF(64 * ScaleRatio, 206 * ScaleRatio, 93.5 * ScaleRatio, 49 * ScaleRatio)
                If Button12.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    MainGame.level = 1
                    MainGame.Map = MainGame2_Map
                    LevelPreview.Image = My.Resources.SelectLevel._1_2Preview
                End If
                Button13.Box = New RectangleF(64 * ScaleRatio, 272 * ScaleRatio, 93.5 * ScaleRatio, 49 * ScaleRatio)
                If Button13.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    MainGame.level = 2
                    MainGame.Map = MainGame3_Map
                    LevelPreview.Image = My.Resources.SelectLevel._1_3Preview
                End If
                Button14.Box = New RectangleF(64 * ScaleRatio, 338 * ScaleRatio, 93.5 * ScaleRatio, 49 * ScaleRatio)
                If Button14.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    MainGame.level = 3
                    MainGame.Map = MainGame4_Map
                    LevelPreview.Image = My.Resources.SelectLevel._1_4Preview
                End If

                LevelPreview.BoxSetting(201 * ScaleRatio, 140 * ScaleRatio, 518 * ScaleRatio, 247 * ScaleRatio)
                LevelPreview.Draw(e)

                LevelDoneButton.Box = New RectangleF(566 * ScaleRatio, 362 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If LevelDoneButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "ToGame"
                End If

            Case "ToGame"
                State = "NextLevel"
                Xp = 0
                MainGame.Pause = False

                MainGame.CharacterHealthReset()

                PlaySound(BGM, "Mozart Reqium Dies Irae.mp3", True)
                SoundEffect.controls.stop()
                LoadingShow = False
                DimScreen = -1

            Case "NextLevel"
                State = "Game"

                Select Case MainGame.level
                    Case 0
                        MainGame.CameraReset()
                        MainGame.MapSetting(32, 15)
                        MainGame.Map = MainGame1_Map
                        MainGame.CharacterReset(1.1, 1)
                        Slime1.MonsterReset(0, 1, RandomInt(12, 23), 11, RandomInt(20, 30))
                        Slime2.MonsterReset(0, 1, RandomInt(12, 23), 8, RandomInt(20, 30))
                        Abyssosque1.MonsterReset(2, 3, 16.4, 1, 750)
                        Abyssosque2.MonsterReset(2, 3, 20.4, 1, 750)
                        Abyssosque3.MonsterReset(2, 3, 24.4, 1, 750)
                        RegenHealth.MonsterReset(8, 9, RandomInt(12, 23), 11, -RandomInt(20, 30))
                        If RandomInt(0, 100) < RegenHealthChance Then
                            RegenHealth.MonsterHealth = 0
                        End If
                        RegenHealth2.MonsterReset(8, 9, RandomInt(12, 23), 8, -RandomInt(20, 30))
                        If RandomInt(0, 100) < RegenHealthChance Then
                            RegenHealth2.MonsterHealth = 0
                        End If
                    Case 1
                        MainGame.CameraReset()
                        MainGame.MapSetting(32, 15)
                        MainGame.Map = MainGame2_Map
                        MainGame.CharacterReset(1.1, 6)
                        Slime1.MonsterReset(0, 1, 8.5, 6, RandomInt(20, 30))
                        Slime2.MonsterReset(0, 1, 13.5, 11, RandomInt(20, 30))
                        Slime3.MonsterReset(0, 1, 24.5, 4, RandomInt(20, 30))
                        Abyssosque1.MonsterReset(2, 3, 5.4, 1, 750)
                        Abyssosque2.MonsterReset(2, 3, 6.4, 1, 750)
                        Abyssosque3.MonsterReset(2, 3, 9.4, 1, 750)
                        Abyssosque4.MonsterReset(2, 3, 10.4, 1, 750)
                        Abyssosque5.MonsterReset(2, 3, 22.4, 1, 750)
                        Abyssosque6.MonsterReset(2, 3, 26.4, 1, 750)
                        RegenHealth.MonsterReset(8, 9, RandomInt(12, 14), 11, -RandomInt(20, 30))
                        If RandomInt(0, 100) < RegenHealthChance Then
                            RegenHealth.MonsterHealth = 0
                        End If
                        RegenHealth2.MonsterReset(8, 9, RandomInt(22, 26), 9, -RandomInt(20, 30))
                        If RandomInt(0, 100) < RegenHealthChance Then
                            RegenHealth2.MonsterHealth = 0
                        End If
                    Case 2
                        MainGame.CameraReset(-1530 * ScaleRatio + MyWidth)
                        MainGame.MapSetting(32, 15)
                        MainGame.Map = MainGame3_Map
                        MainGame.CharacterReset(30.1, 1)
                        Slime1.MonsterReset(0, 1, RandomInt(5, 8), 1, RandomInt(20, 30))
                        Slime2.MonsterReset(0, 1, RandomInt(5, 8), 11, RandomInt(20, 30))
                        Slime3.MonsterReset(0, 1, RandomInt(12, 19), 11, RandomInt(20, 30))
                        Abyssosque1.MonsterReset(2, 3, 13.4, 1, 750)
                        Abyssosque2.MonsterReset(2, 3, 16.4, 1, 750)
                        Abyssosque3.MonsterReset(2, 3, 17.4, 1, 750)
                        Abyssosque4.MonsterReset(2, 3, 20.4, 1, 750)
                        Abyssosque5.MonsterReset(2, 3, 21.4, 1, 750)
                        Abyssosque6.MonsterReset(2, 3, 10.4, 8, 750)
                        Abyssosque7.MonsterReset(2, 3, 20.4, 9, 750)
                        Abyssosque8.MonsterReset(2, 3, 24.4, 9, 750)
                        RegenHealth.MonsterReset(8, 9, RandomInt(4, 9), 11, -RandomInt(20, 30))
                        If RandomInt(0, 100) < RegenHealthChance Then
                            RegenHealth.MonsterHealth = 0
                        End If
                    Case 3
                        MainGame.Map(1, 21) = 4
                        MainGame.Map(3, 21) = 4

                        MainGame.CameraReset()
                        MainGame.MapSetting(33, 15)
                        MainGame.Map = MainGame4_Map
                        MainGame.CharacterReset(1.1, 1)

                        Boss.MonsterReset(4, 5, 18.1, 5, 6.5)
                        BossMusicCore.MonsterReset(6, 7, 21, 12, 20)

                        Slime1.MonsterReset(0, 1, RandomInt(15, 26), 1, RandomInt(20, 30))
                        Slime2.MonsterReset(0, 1, RandomInt(15, 26), 1, RandomInt(20, 30))
                        Abyssosque1.MonsterReset(2, 3, RandomInt(15, 26), 1, 750)
                        Abyssosque2.MonsterReset(2, 3, RandomInt(15, 26), 1, 750)
                        Abyssosque3.MonsterReset(2, 3, 17, 11, 750)
                        Abyssosque4.MonsterReset(2, 3, 19, 11, 750)
                        Abyssosque5.MonsterReset(2, 3, 23, 11, 750)
                        Abyssosque6.MonsterReset(2, 3, 25, 11, 750)
                        RegenHealth.MonsterReset(8, 9, RandomInt(9, 28), 1, -RandomInt(20, 30))
                        If RandomInt(0, 100) < RegenHealthChance Then
                            RegenHealth.MonsterHealth = 0
                        End If
                        RegenHealth2.MonsterReset(8, 9, RandomInt(13, 16), 11, -RandomInt(20, 30))
                        If RandomInt(0, 100) < RegenHealthChance Then
                            RegenHealth2.MonsterHealth = 0
                        End If
                End Select

            Case "Game"
                MainGame.BoxSetting(0, 0, MyWidth, MyHeight)
                MainGame.DrawGame(e, ScaleRatio, Keyboard, Mouse, MousePressed, SoundEffect.settings.volume)

                Select Case MainGame.level
                    Case 0
                        Slime1.SyncWith(MainGame)
                        Slime1.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Slime2.SyncWith(MainGame)
                        Slime2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque1.SyncWith(MainGame)
                        Abyssosque1.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque2.SyncWith(MainGame)
                        Abyssosque2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque3.SyncWith(MainGame)
                        Abyssosque3.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        RegenHealth.SyncWith(MainGame)
                        RegenHealth.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        RegenHealth2.SyncWith(MainGame)
                        RegenHealth2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)

                        If MainGame.CharacterX >= 30 And MainGame.CharacterY >= 6 Then
                            MainGame.level = 1
                            State = "NextLevel"
                        End If
                    Case 1
                        Slime1.SyncWith(MainGame)
                        Slime1.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Slime2.SyncWith(MainGame)
                        Slime2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Slime3.SyncWith(MainGame)
                        Slime3.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque1.SyncWith(MainGame)
                        Abyssosque1.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque2.SyncWith(MainGame)
                        Abyssosque2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque3.SyncWith(MainGame)
                        Abyssosque3.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque4.SyncWith(MainGame)
                        Abyssosque4.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque5.SyncWith(MainGame)
                        Abyssosque5.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque6.SyncWith(MainGame)
                        Abyssosque6.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        RegenHealth.SyncWith(MainGame)
                        RegenHealth.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        RegenHealth2.SyncWith(MainGame)
                        RegenHealth2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)

                        If MainGame.CharacterX >= 30 And MainGame.CharacterY >= 12 Then
                            MainGame.level = 2
                            State = "NextLevel"
                        End If
                    Case 2
                        Slime1.SyncWith(MainGame)
                        Slime1.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Slime2.SyncWith(MainGame)
                        Slime2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Slime3.SyncWith(MainGame)
                        Slime3.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque1.SyncWith(MainGame)
                        Abyssosque1.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque2.SyncWith(MainGame)
                        Abyssosque2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque3.SyncWith(MainGame)
                        Abyssosque3.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque4.SyncWith(MainGame)
                        Abyssosque4.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque5.SyncWith(MainGame)
                        Abyssosque5.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque6.SyncWith(MainGame)
                        Abyssosque6.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque7.SyncWith(MainGame)
                        Abyssosque7.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque8.SyncWith(MainGame)
                        Abyssosque8.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        RegenHealth.SyncWith(MainGame)
                        RegenHealth.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)

                        If MainGame.CharacterX >= 30 And MainGame.CharacterY >= 5 Then
                            MainGame.level = 3
                            State = "NextLevel"
                        End If

                    Case 3
                        Boss.SyncWith(MainGame)
                        Boss.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)

                        BossMusicCore.SyncWith(MainGame)
                        BossMusicCore.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)

                        Slime1.SyncWith(MainGame)
                        Slime1.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Slime2.SyncWith(MainGame)
                        Slime2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)

                        Abyssosque1.SyncWith(MainGame)
                        Abyssosque1.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque2.SyncWith(MainGame)
                        Abyssosque2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque3.SyncWith(MainGame)
                        Abyssosque3.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque4.SyncWith(MainGame)
                        Abyssosque4.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque5.SyncWith(MainGame)
                        Abyssosque5.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        Abyssosque6.SyncWith(MainGame)
                        Abyssosque6.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        RegenHealth.SyncWith(MainGame)
                        RegenHealth.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)
                        RegenHealth2.SyncWith(MainGame)
                        RegenHealth2.Draw(e, ScaleRatio, MainGame.CanShoot, MainGame.CharacterHealth, MainGame.Damage, MainGame.ShowDamage, MainGame.LastShowDamage, MainGame.CharacterHarmDamage, SoundEffect.settings.volume, Xp)

                        If BossMusicCore.MonsterHealth > 0 Then '如果音樂核心沒被打掉
                            '畫傳輸能量雷射
                            e.Graphics.DrawLine(New Pen(Color.Red, 3 * ScaleRatio),
                                                New PointF(BossMusicCore.MonsterX * 48 * ScaleRatio + MainGame.CameraX + BossMusicCore.MonsterWidth / 2 * ScaleRatio,
                                                           MainGame.CameraY + (MainGame.height - MainGame.Map_Height * 48 * ScaleRatio) + (MainGame.Map_Height - (BossMusicCore.MonsterY - 0.5)) * 48 * ScaleRatio - BossMusicCore.MonsterHeight * ScaleRatio),
                                                New PointF(Boss.MonsterX * 48 * ScaleRatio + MainGame.CameraX + Boss.MonsterWidth / 2 * ScaleRatio,
                                                           MainGame.CameraY + (MainGame.height - MainGame.Map_Height * 48 * ScaleRatio) + (MainGame.Map_Height - (Boss.MonsterY - 1.35)) * 48 * ScaleRatio - Boss.MonsterHeight * ScaleRatio))
                        Else
                            '解除柵欄
                            MainGame.Map(1, 21) = 0
                            MainGame.Map(3, 21) = 0
                            'Boss不可射雷射
                            Boss.BossShouldAttack = False
                        End If

                        '如果Boss死
                        If Boss.MonsterHealth <= 0 Then
                            If DimScreen = -1 Then '如果初次播放變暗動畫
                                DimScreen = 0 '重設變暗動畫
                                PlaySound(BGM, "勝利結束.mp3", False) '撥放結尾音效
                                EndGameStartTime = Now.Ticks() / 10000 '開始計時
                            End If

                            DimScreen += 5 '變暗
                            If DimScreen >= 255 Then '如果變到最暗
                                DimScreen = 255 '強制變成最暗
                                State = "EndGame"
                                BGisOn = True '顯示背景
                            Else
                                '顯示變暗動畫
                                e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(DimScreen, 0, 0, 0)), New Rectangle(0, 0, MyWidth, MyHeight))
                            End If
                        End If
                End Select

                '畫分數
                XpBG.Box = New RectangleF(0, MyHeight - 45.125 * ScaleRatio, 138 * ScaleRatio, 45.125 * ScaleRatio)
                XpBG.Draw(e)
                XpText.point.X = 138 / 2 * ScaleRatio
                XpText.point.Y = MyHeight - 45.125 / 2 * ScaleRatio + 2.5 * ScaleRatio
                XpText.Draw("分數 " & CStr(Xp), e, myfont, ScaleRatio)

                '如果角色死亡
                If MainGame.CharacterDied Then
                    If DimScreen = -1 Then '如果初次播放變暗動畫
                        DimScreen = 0 '重設變暗動畫
                        BGM.controls.stop() '停止撥放BGM
                        PlaySound(SoundEffect, "死掉.mp3", False) '撥放死掉音效
                    End If

                    DimScreen += 5 '變暗
                    If DimScreen >= 255 Then '如果變到最暗
                        DimScreen = 255 '強制變成最暗
                        State = "DiedMenu"
                        MainGame.CharacterHealthReset() '角色血量重設
                        BGisOn = True '顯示背景
                        DiedMessage = DiedMessageArray(RandomInt(0, DiedMessageArray.Length() - 1)) '隨機選擇要顯示的死亡訊息
                    End If

                    '畫背景
                    e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(DimScreen, 0, 0, 0)), New Rectangle(0, 0, MyWidth, MyHeight))
                End If

                '如果沒有暫停，且角色沒死
                If Not MainGame.Pause And Not MainGame.CharacterDied Then
                    '暫停按鈕設定外框
                    PauseButton.Box = New RectangleF(147 * ScaleRatio, MyHeight - 31 * ScaleRatio, 40.8475 * ScaleRatio, 21.4067 * ScaleRatio)
                    If PauseButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then '如果按下按鈕
                        BGM.controls.pause() 'BGM暫停
                        BGisOn = True '顯示背景
                        MainGame.Pause = True '暫停遊戲
                    End If
                End If

                '如果遊戲暫停，且角色沒死
                If MainGame.Pause And Not MainGame.CharacterDied Then
                    '畫半透明背景
                    e.Graphics.FillRectangle(New SolidBrush(Color.FromArgb(127, 0, 0, 0)), New Rectangle(0, 0, MyWidth, MyHeight))

                    '畫按鈕------------------------------------
                    ToMenuButton.Box = New RectangleF(MyWidth / 2 - 309 * ScaleRatio, MyHeight / 2 + 111 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                    If ToMenuButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                        State = "ToMenu"
                    End If

                    ContinueButton.Box = New RectangleF(MyWidth / 2 - 93 * ScaleRatio, MyHeight / 2 + 111 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                    If ContinueButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                        BGM.controls.play()
                        BGisOn = False
                        MainGame.Pause = False
                    End If

                    SettingButton.Box = New RectangleF(MyWidth / 2 + 123 * ScaleRatio, MyHeight / 2 + 111 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                    If SettingButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                        State = "InGameSetting"
                    End If

                    '顯示遊戲已暫停
                    Pause.point.X = MyWidth / 2
                    Pause.point.Y = MyHeight / 2 - 10 * ScaleRatio
                    Pause.Draw("遊戲已暫停", e, myfont, ScaleRatio)
                End If

            '在遊戲內的設定
            Case "InGameSetting"
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
                If DoneSettingButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "Game"
                End If

            '死亡介面
            Case "DiedMenu"
                EndButton.Box = New RectangleF(MyWidth / 2 - 210 * ScaleRatio, MyHeight / 2 + 111 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If EndButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "ToMenu"
                End If

                ReplayButton.Box = New RectangleF(MyWidth / 2 + 25 * ScaleRatio, MyHeight / 2 + 111 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If ReplayButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "ToGame"
                End If

                DiedText.point.X = MyWidth / 2
                DiedText.point.Y = MyHeight / 2 - 48 * ScaleRatio
                DiedText.Draw(DiedMessage, e, myfont, ScaleRatio)
                DiedXpText.point.X = MyWidth / 2
                DiedXpText.point.Y = MyHeight / 2 + 5 * ScaleRatio
                DiedXpText.Draw("分數 " & CStr(Xp), e, myfont, ScaleRatio)

            '完成遊戲
            Case "EndGame"
                If Now.Ticks() / 10000 - EndGameStartTime > 22000 Then
                    State = "ToMenu"
                End If

                EndGameText.point.X = MyWidth / 2
                EndGameText.point.Y = MyHeight / 2 - 48 * ScaleRatio
                EndGameText.Draw("恭喜你成功從魔王手中拯救了村莊", e, myfont, ScaleRatio)

                EndGameXpText.point.X = MyWidth / 2
                EndGameXpText.point.Y = MyHeight / 2 + 5 * ScaleRatio
                EndGameXpText.Draw("分數 " & CStr(Xp), e, myfont, ScaleRatio)

                ToMenuButton.Box = New RectangleF(MyWidth / 2 - 187 / 2 * ScaleRatio, MyHeight / 2 + 111 * ScaleRatio, 187 * ScaleRatio, 49 * ScaleRatio)
                If ToMenuButton.Draw(e, Mouse, MousePressed, SoundEffect.settings.volume, SomeButtonClick) = 3 Then
                    State = "ToMenu"
                End If

            '離開遊戲
            Case "Exit"
                WMPExit(BGM)
                WMPExit(SoundEffect)
                WMPExit(Ding)
                Me.Close()
        End Select

        '作弊模式開---------------------------------------------------------------------------------------------------------------------------------------------
        If GodMode Then
            MainGame.CharacterHealth = 100000000
            MainGame.CharacterATKDamage = 1000
            MainGame.CharacterSkillDamage = 1000
            GodModePanel.point.X = MyWidth
            GodModePanel.point.Y = 0
            GodModePanel.Align = "Right"
            GodModePanel.Draw("GodMode On", e, myfont, ScaleRatio)
        End If

        '除錯面板開-------------------------------------------------------------------------------------------------------------------------------------------
        If DebugPanelOn Then
            '計算FPS
            Frametime = Now.Ticks() / 10000 - lastUpdate
            FPS = 1 / (Frametime / 1000)
            DebugInfo.Draw("Mosche Barton " & Version & vbNewLine &
                           CStr(Math.Round(My.Computer.Info.TotalPhysicalMemory / 1000000 - My.Computer.Info.AvailablePhysicalMemory / 1000000)) & " / " & CStr(Math.Round(My.Computer.Info.TotalPhysicalMemory / 1000000)) & " MB" & vbNewLine &
                           CStr(MyWidth) & "x" & CStr(MyHeight) & " Scale by " & CStr(ScaleRatio) & vbNewLine & vbNewLine &
                           "FPS " & CStr(Math.Round(FPS, 2)) & vbNewLine &
                           "Frametime " & CStr(Math.Round(Frametime, 2)) & vbNewLine & vbNewLine &
                           "State " & State & vbNewLine & vbNewLine &
                           "BGM.volume = " & CStr(BGM.settings.volume) & vbNewLine &
                           "SE.volume = " & CStr(SoundEffect.settings.volume) & vbNewLine &
                           "Ding.volume = " & CStr(Ding.settings.volume) & vbNewLine & vbNewLine &
                           "SomeButtonClick = " & CStr(SomeButtonClick),
                           e, myfont, ScaleRatio)

            e.Graphics.DrawLine(New Pen(Color.Green), New PointF(0, MyHeight / 2), New PointF(MyWidth, MyHeight / 2))
            e.Graphics.DrawLine(New Pen(Color.Green), New PointF(MyWidth / 2, 0), New PointF(MyWidth / 2, MyHeight))
            lastUpdate = Now.Ticks() / 10000
        End If
    End Sub

    '更新螢幕畫面-------------------------------------------------------------------------------------------------------------------------------------
    Private Sub ScreenRefresh_Tick(sender As Object, e As EventArgs)
        Me.Invalidate()
    End Sub

    '程式載入----------------------------------------------------------------------------------------------------------------------------------------------
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Timer的東西
        ScreenRefresh.SynchronizingObject = Me '使Timer的事件依附在本體的執行緒
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

        '設定自訂字體
        DemoGame.myfont = myfont
        DemoMonster.myfont = myfont
        MainGame.myfont = myfont
        Slime1.myfont = myfont
        Slime2.myfont = myfont
        Slime3.myfont = myfont
        Abyssosque1.myfont = myfont
        Abyssosque2.myfont = myfont
        Abyssosque3.myfont = myfont
        Abyssosque4.myfont = myfont
        Abyssosque5.myfont = myfont
        Abyssosque6.myfont = myfont
        Abyssosque7.myfont = myfont
        Abyssosque8.myfont = myfont
        Boss.myfont = myfont
        BossMusicCore.myfont = myfont
        RegenHealth.myfont = myfont

        '調整遊戲預設音量
        BGM.settings.volume = 100
        SoundEffect.settings.volume = 100
    End Sub

    '處理視窗大小變化
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        MyWidth = Me.ClientSize.Width
        MyHeight = Me.ClientSize.Height
        ScaleRatio = (MyWidth / DefaultWidth + MyHeight / DefaultHeight) / 2
        If ScaleRatio > 2 Then
            ScaleRatio = 2
        End If
        Me.Invalidate()
    End Sub

    '鍵盤按下
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyValue <= 255 Then
            Keyboard(e.KeyValue) = True
        End If
    End Sub

    '鍵盤抬起
    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        If e.KeyValue <= 255 Then
            Keyboard(e.KeyValue) = False
        End If

        If e.KeyCode = Keys.F3 Then
            DebugPanelOn = Not DebugPanelOn
        End If

        If e.KeyCode = Keys.F7 Then
            GodMode = Not GodMode
            If Not GodMode Then
                MainGame.CharacterHealthReset()
                MainGame.CharacterATKReset()
            End If
        End If

        If e.KeyCode = Keys.Escape Then
            If State = "Game" And Not MainGame.CharacterDied Then
                MainGame.Pause = Not MainGame.Pause
                If MainGame.Pause Then
                    BGM.controls.pause()
                    BGisOn = True
                Else
                    BGM.controls.play()
                    BGisOn = False
                End If
            End If
        End If
    End Sub

    '滑鼠移動
    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        Mouse.X = e.X
        Mouse.Y = e.Y
    End Sub

    '滑鼠按下
    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        MousePressed = True
    End Sub

    '滑鼠抬起
    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        MousePressed = False
    End Sub
End Class
