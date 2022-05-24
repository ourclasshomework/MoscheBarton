Imports System.Drawing.Imaging
Public Class Form1
    Dim KeyPressed(256) As Boolean

    '材質載入
    Dim LogoResource As String = "C:\Users\ourcl\OneDrive\資訊專題報告\Game\Game\Resources\Logo.png" '遊戲Logo
    Dim TeamResource As String = "C:\Users\ourcl\OneDrive\資訊專題報告\Game\Game\Resources\Team.png" '團隊Logo
    Dim CharacterResource As String = "C:\Users\ourcl\OneDrive\資訊專題報告\Game\Game\Resources\Character.png" '角色

    '角色初始化
    Dim CharacterRotation As Single = 0 '角色預設角度
    Dim CharacterScale As Single = 0.8 '角色預設縮放
    Dim CharacterPosition As Point = New Point(150, 200) '角色預設位置
    Dim CharacterImage As Bitmap = Image.FromFile(CharacterResource) '載入角色圖片
    Dim CharacterBox As Rectangle = New Rectangle(CharacterPosition.X, CharacterPosition.Y, CharacterImage.Width, CharacterImage.Height) '角色繪圖位置

    'Logo初始化
    Dim Logo As Bitmap = Image.FromFile(LogoResource)
    Dim LogoMatrixItems As Single()() = {New Single() {1, 0, 0, 0, 0}, New Single() {0, 1, 0, 0, 0}, New Single() {0, 0, 1, 0, 0}, New Single() {0, 0, 0, 0, 0}, New Single() {0, 0, 0, 0, 1}}
    Dim LogoMatrix As ColorMatrix = New ColorMatrix(LogoMatrixItems)
    Dim LogoBox As New Rectangle(0, 0, Logo.Width, Logo.Height)
    Dim LogoAttr As New ImageAttributes()
    Dim LogoScale As Single = 0.5

    '繪圖
    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint
        With e.Graphics
            .TranslateTransform(CharacterBox.Left + CharacterImage.Width * CharacterScale / 2, CharacterBox.Top + CharacterImage.Height * CharacterScale / 2) '將參考點設定置角色中心
            .ScaleTransform(CharacterScale, CharacterScale) '縮放角色
            .RotateTransform(CharacterRotation) '旋轉角色
            .DrawImage(CharacterImage, New Point(-CharacterImage.Width / 2, -CharacterImage.Height / 2)) '畫出角色
            .CopyFromScreen(New Point(0, 0), New Point(0, 0), New Size(1, 1)) '將螢幕上 (0, 0) 到 (1, 1) 的像素複製到 (0, 0) ，雖然意義不明，但可以減少角色閃爍
        End With
    End Sub

    '鍵盤檢測
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown '按鍵壓下檢測，處理當偵測到有按鍵按下
        KeyPressed(e.KeyValue) = True '紀錄按下的按鍵
        KeyboardScaning.Enabled = True '啟動 Timer
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp '按鍵抬起檢測，處理當偵測到有按鍵放開
        KeyPressed(e.KeyValue) = False '取消紀錄已經按下的按鍵
        If KeyPressed(Keys.W) + KeyPressed(Keys.A) + KeyPressed(Keys.S) + KeyPressed(Keys.D) = 0 Then '如果WASD都沒按，就停止 Timer
            KeyboardScaning.Enabled = False
        End If
    End Sub

    Private Sub KeyboardScaning_Tick(sender As Object, e As EventArgs) Handles KeyboardScaning.Tick '鍵盤更新
        If KeyPressed(Keys.W) Then '偵測W是否按下
            CharacterBox.Y -= 1
            CharacterRotation = 0
        End If
        If KeyPressed(Keys.A) Then '偵測A是否按下
            CharacterBox.X -= 1
            CharacterRotation = 270
        End If
        If KeyPressed(Keys.S) Then '偵測S是否按下
            CharacterBox.Y += 1
            CharacterRotation = 180
        End If
        If KeyPressed(Keys.D) Then '偵測D是否按下
            CharacterBox.X += 1
            CharacterRotation = 90
        End If

        Panel1.Invalidate() '更新畫面
    End Sub



    '初始化
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For i = 0 To KeyPressed.Length() - 1
            KeyPressed(i) = False
        Next
    End Sub



    Private Sub LogoPanel_Paint(sender As Object, e As PaintEventArgs) Handles LogoPanel.Paint
        LogoAttr.SetColorMatrix(LogoMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
        e.Graphics.TranslateTransform(e.Graphics.VisibleClipBounds.Width / 2 - Logo.Width * LogoScale / 2, e.Graphics.VisibleClipBounds.Height / 2 - Logo.Height * LogoScale / 2)
        e.Graphics.ScaleTransform(LogoScale, LogoScale)
        e.Graphics.DrawImage(Logo, LogoBox, 0, 0, Logo.Width, Logo.Height, GraphicsUnit.Pixel, LogoAttr)
        e.Graphics.CopyFromScreen(New Point(0, 0), New Point(1, 1), New Size(1, 1))
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Panel1.Visible = False
        Panel1.Size = New Size(0, 0)
        Panel1.Location = New Point(-1, -1)
        LogoTimer.Enabled = True
        LogoPanel.Invalidate()
    End Sub

    Private Sub Logo_Tick(sender As Object, e As EventArgs) Handles LogoTimer.Tick
        If LogoMatrix.Matrix33 >= 1 Then
            LogoMatrix.Matrix33 = 0
            'LogoScale = 0.5
            Logo = Image.FromFile(TeamResource)
            LogoBox = New Rectangle(0, 0, Logo.Width, Logo.Height)
            TeamLogoTimer.Enabled = True
            LogoTimer.Enabled = False
        Else
            LogoMatrix.Matrix33 += 0.01
            LogoScale += 0.001
        End If

        LogoPanel.Invalidate()
    End Sub

    Private Sub TeamLogoTimer_Tick(sender As Object, e As EventArgs) Handles TeamLogoTimer.Tick
        If LogoMatrix.Matrix33 >= 1 Then
            Panel1.Visible = True
            CharacterRotation = 0
            CharacterPosition = New Point(150, 200)
            Panel1.Size = New Size(Me.Width, Me.Height)
            Panel1.Location = New Point(0, 0)
            LogoPanel.Visible = False
            LogoPanel.Dispose()

            TeamLogoTimer.Enabled = False
        Else
            LogoMatrix.Matrix33 += 0.01
            LogoScale += 0.001
        End If

        LogoPanel.Invalidate()
    End Sub
End Class
