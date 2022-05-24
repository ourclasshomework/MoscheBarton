<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意: 以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請勿使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.KeyboardScaning = New System.Windows.Forms.Timer(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LogoPanel = New System.Windows.Forms.Panel()
        Me.LogoTimer = New System.Windows.Forms.Timer(Me.components)
        Me.TeamLogoTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'KeyboardScaning
        '
        Me.KeyboardScaning.Interval = 1
        '
        'Panel1
        '
        Me.Panel1.Location = New System.Drawing.Point(-1, -1)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(0, 0)
        Me.Panel1.TabIndex = 2
        '
        'LogoPanel
        '
        Me.LogoPanel.BackColor = System.Drawing.Color.White
        Me.LogoPanel.Location = New System.Drawing.Point(2, 1)
        Me.LogoPanel.Name = "LogoPanel"
        Me.LogoPanel.Size = New System.Drawing.Size(1330, 830)
        Me.LogoPanel.TabIndex = 0
        '
        'LogoTimer
        '
        Me.LogoTimer.Interval = 20
        '
        'TeamLogoTimer
        '
        Me.TeamLogoTimer.Interval = 20
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1332, 833)
        Me.Controls.Add(Me.LogoPanel)
        Me.Controls.Add(Me.Panel1)
        Me.DoubleBuffered = True
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents KeyboardScaning As Timer
    Friend WithEvents Panel1 As Panel
    Friend WithEvents LogoPanel As Panel
    Friend WithEvents LogoTimer As Timer
    Friend WithEvents TeamLogoTimer As Timer
End Class
