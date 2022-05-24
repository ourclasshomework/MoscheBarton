Imports System.Timers
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Runtime.InteropServices

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

        Public Sub AlignSetting(ByVal alignment As String, ByVal linealignment As String)
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
                    DrawFont = New Font(font, font_size * ScaleRatio)
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
        Dim Box As RectangleF
        Dim Image As Bitmap
        Dim isActivated As Boolean = False
        Dim isPressed As Boolean = False

        Public Sub BoxSetting(x1 As Single, y1 As Single, x2 As Single, y2 As Single, ScaleRatio As Double)
            Box.X = x1 * ScaleRatio
            Box.Y = y1 * ScaleRatio
            Box.Width = Math.Abs(x2 - x1) * ScaleRatio
            Box.Height = Math.Abs(y2 - y1) * ScaleRatio
        End Sub

        Public Sub Draw(e As PaintEventArgs)
            e.Graphics.DrawImage(Image, Box)
        End Sub
    End Class

    '快速調整設定區
    Const DefaultWidth As Integer = 800 '預設的寬度
    Const DefaultHeight As Integer = 450 '預設的高度
    Const OpeningDimSpeed As Double = 7.5 '開場動畫淡入淡出的速度
    Const OpeningGapSpeed As Double = 1000 '開場動畫淡入淡出的速度 (ms)
    Const OpeningSpeed As Integer = 2000 '每個開場動畫持續的時間 (ms)
    Dim State As String = "Start"

    '初始化Timer
    Dim ScreenRefresh As New Timer(1)
    Dim DebugInfoRefresh As New Timer(100)

    '字體初始化
    Dim myfont As New PrivateFontCollection
    Dim bytes As Byte()
    Dim ptr As IntPtr

    '螢幕縮放比例初始化
    Dim MyWidth As Integer
    Dim MyHeight As Integer
    Dim ScaleRatio As Double = 1.0

    '鍵盤初始化
    Dim Keyboard(256) As Boolean

    'DebugPanel 初始化
    Dim DebugPanelOn As Boolean = True
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


    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        If BGisOn Then
            '繪製背景
            BGBrush = New LinearGradientBrush(New Point(0, 0), New Point(0, MyHeight), Color.Black, Color.Black) '更新背景筆刷位置
            BGColorBlend.Colors = BGColors '更新背景漸層筆刷顏色
            BGColorBlend.Positions = BGColorPosition '更新背景漸層筆刷的顏色的位置
            BGBrush.InterpolationColors = BGColorBlend '更新背景成多色漸層
            BGBrush.RotateTransform(33 + BGAnimation) '旋轉背景
            e.Graphics.FillRectangle(BGBrush, New RectangleF(0, 0, MyWidth, MyHeight)) '繪製背景
        End If

        Select Case State
            Case "StudioLogo", "StudioLogoDim"
                StudioLogo.point.X = MyWidth / 2
                StudioLogo.point.Y = MyHeight / 2
                StudioLogo.Draw("III Studio", e, myfont, ScaleRatio)

            Case "GameLogo", "GameLogoDim"
                GameLogo.point.X = MyWidth / 2
                GameLogo.point.Y = MyHeight / 2 - 15 * ScaleRatio
                GameLogo.Draw("莫舒巴頓", e, myfont, ScaleRatio)

                GameSubLogo.point.X = MyWidth / 2
                GameSubLogo.point.Y = MyHeight / 2 + 45 * ScaleRatio
                GameSubLogo.Draw("Mosche Barton", e, myfont, ScaleRatio)
        End Select

        If DebugPanelOn Then
            DebugInfoRefresh.Enabled = True
            DebugInfo.Draw("FPS " & CStr(Math.Round(FPS, 2)) & vbNewLine &
                           "Frametime " & CStr(Math.Round(Frametime, 2)) & vbNewLine &
                           "State " & State,
                           e, myfont, ScaleRatio)

            e.Graphics.DrawLine(New Pen(Color.Yellow), New PointF(0, MyHeight / 2), New PointF(MyWidth, MyHeight / 2))
            e.Graphics.DrawLine(New Pen(Color.Yellow), New PointF(MyWidth / 2, 0), New PointF(MyWidth / 2, MyHeight))

            lastUpdate = Now.Ticks() / 10000
        Else
            DebugInfoRefresh.Enabled = False
        End If
    End Sub

    Private Sub ScreenRefresh_Tick(sender As Object, e As EventArgs)
        If BGisOn Then
            BGAnimation += 0.075
            If BGAnimation >= 360 Then
                BGAnimation = 0
            End If
        End If

        Select Case State
            Case "Start" '啟動計時器
                BGisOn = True
                State = "StudioLogo"
                OpeningStartTime = Now.Ticks()

            Case "StudioLogo" 'StudioLogo 淡入，並顯示 OpeningSpeed 毫秒
                StudioLogo.opacity += OpeningDimSpeed
                If (Now.Ticks() - OpeningStartTime) / 10000 >= OpeningSpeed Then
                    State = "StudioLogoDim"
                    OpeningStartTime = Now.Ticks()
                End If

            Case "StudioLogoDim" 'StudioLogo 淡出，並等待 OpeningGapSpeed 毫秒
                StudioLogo.opacity -= OpeningDimSpeed
                If (Now.Ticks() - OpeningStartTime) / 10000 >= OpeningGapSpeed Then
                    State = "GameLogo"
                    OpeningStartTime = Now.Ticks()
                End If

            Case "GameLogo" 'GameLogo 淡入，並顯示 OpeningSpeed 毫秒
                GameLogo.opacity += OpeningDimSpeed
                GameSubLogo.opacity += OpeningDimSpeed
                If (Now.Ticks() - OpeningStartTime) / 10000 >= OpeningSpeed Then
                    State = "GameLogoDim"
                    OpeningStartTime = Now.Ticks()
                End If

            Case "GameLogoDim" 'GameLogo 淡出，並等待 OpeningGapSpeed 毫秒
                GameLogo.opacity -= OpeningDimSpeed
                GameSubLogo.opacity -= OpeningDimSpeed
                If (Now.Ticks() - OpeningStartTime) / 10000 >= OpeningGapSpeed Then
                    State = "Start"
                    OpeningStartTime = Now.Ticks()
                End If
        End Select

        Me.Invalidate()
    End Sub

    Private Sub DebugInfoRefresh_Tick(sender As Object, e As EventArgs)
        Frametime = Now.Ticks() / 10000 - lastUpdate
        FPS = 1 / (Frametime / 1000)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ScreenRefresh.SynchronizingObject = Me '使Timer的事件依附在本體的執行續
        AddHandler ScreenRefresh.Elapsed, AddressOf ScreenRefresh_Tick '分派事件到ScreenRefresh_Tick
        ScreenRefresh.AutoReset = True 'Timer的自動重新計時開啟
        ScreenRefresh.Enabled = True 'Timer啟動

        DebugInfoRefresh.SynchronizingObject = Me
        AddHandler DebugInfoRefresh.Elapsed, AddressOf DebugInfoRefresh_Tick
        DebugInfoRefresh.AutoReset = True
        DebugInfoRefresh.Enabled = False

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
End Class
