<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.lblMessage1 = New System.Windows.Forms.Label()
        Me.lblMessage2 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.cbGB_COMPort = New System.Windows.Forms.ComboBox()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.btnGBDisconnect = New System.Windows.Forms.Button()
        Me.btnGBConnect = New System.Windows.Forms.Button()
        Me.lblGBConnectionStatus = New System.Windows.Forms.Label()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.rbAlgorithm = New System.Windows.Forms.RadioButton()
        Me.rbArtificialIntelligence = New System.Windows.Forms.RadioButton()
        Me.btn0Games = New System.Windows.Forms.Button()
        Me.btn75Games = New System.Windows.Forms.Button()
        Me.btn150Games = New System.Windows.Forms.Button()
        Me.btnCustomSave = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblLearningOps = New System.Windows.Forms.Label()
        Me.btnCustomLoad = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblMatrixStatus = New System.Windows.Forms.Label()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblMessage1
        '
        Me.lblMessage1.BackColor = System.Drawing.Color.White
        Me.lblMessage1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMessage1.Location = New System.Drawing.Point(221, 15)
        Me.lblMessage1.Name = "lblMessage1"
        Me.lblMessage1.Size = New System.Drawing.Size(137, 19)
        Me.lblMessage1.TabIndex = 19
        Me.lblMessage1.Text = "READY"
        '
        'lblMessage2
        '
        Me.lblMessage2.BackColor = System.Drawing.Color.White
        Me.lblMessage2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMessage2.Location = New System.Drawing.Point(221, 36)
        Me.lblMessage2.Name = "lblMessage2"
        Me.lblMessage2.Size = New System.Drawing.Size(137, 19)
        Me.lblMessage2.TabIndex = 23
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.cbGB_COMPort)
        Me.GroupBox2.Controls.Add(Me.Label36)
        Me.GroupBox2.Controls.Add(Me.btnGBDisconnect)
        Me.GroupBox2.Controls.Add(Me.btnGBConnect)
        Me.GroupBox2.Controls.Add(Me.lblGBConnectionStatus)
        Me.GroupBox2.Controls.Add(Me.Label38)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(189, 103)
        Me.GroupBox2.TabIndex = 95
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Game Board Comm"
        '
        'cbGB_COMPort
        '
        Me.cbGB_COMPort.FormattingEnabled = True
        Me.cbGB_COMPort.Location = New System.Drawing.Point(68, 13)
        Me.cbGB_COMPort.Name = "cbGB_COMPort"
        Me.cbGB_COMPort.Size = New System.Drawing.Size(60, 21)
        Me.cbGB_COMPort.TabIndex = 70
        Me.cbGB_COMPort.Text = "COM1"
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.Location = New System.Drawing.Point(5, 17)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(56, 13)
        Me.Label36.TabIndex = 69
        Me.Label36.Text = "COM Port:"
        '
        'btnGBDisconnect
        '
        Me.btnGBDisconnect.BackColor = System.Drawing.SystemColors.Control
        Me.btnGBDisconnect.Enabled = False
        Me.btnGBDisconnect.Location = New System.Drawing.Point(99, 60)
        Me.btnGBDisconnect.Name = "btnGBDisconnect"
        Me.btnGBDisconnect.Size = New System.Drawing.Size(74, 34)
        Me.btnGBDisconnect.TabIndex = 68
        Me.btnGBDisconnect.Text = "Disconnect"
        Me.btnGBDisconnect.UseVisualStyleBackColor = False
        '
        'btnGBConnect
        '
        Me.btnGBConnect.BackColor = System.Drawing.SystemColors.Control
        Me.btnGBConnect.Location = New System.Drawing.Point(16, 60)
        Me.btnGBConnect.Name = "btnGBConnect"
        Me.btnGBConnect.Size = New System.Drawing.Size(74, 34)
        Me.btnGBConnect.TabIndex = 67
        Me.btnGBConnect.Text = "Connect"
        Me.btnGBConnect.UseVisualStyleBackColor = False
        '
        'lblGBConnectionStatus
        '
        Me.lblGBConnectionStatus.AutoSize = True
        Me.lblGBConnectionStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblGBConnectionStatus.Location = New System.Drawing.Point(95, 41)
        Me.lblGBConnectionStatus.Name = "lblGBConnectionStatus"
        Me.lblGBConnectionStatus.Size = New System.Drawing.Size(80, 12)
        Me.lblGBConnectionStatus.TabIndex = 66
        Me.lblGBConnectionStatus.Text = "Not Connected"
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label38.Location = New System.Drawing.Point(5, 41)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(72, 12)
        Me.Label38.TabIndex = 65
        Me.Label38.Text = "Connect Status:"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rbAlgorithm)
        Me.GroupBox1.Controls.Add(Me.rbArtificialIntelligence)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 133)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(189, 86)
        Me.GroupBox1.TabIndex = 96
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Game Mode"
        '
        'rbAlgorithm
        '
        Me.rbAlgorithm.AutoSize = True
        Me.rbAlgorithm.Location = New System.Drawing.Point(18, 52)
        Me.rbAlgorithm.Name = "rbAlgorithm"
        Me.rbAlgorithm.Size = New System.Drawing.Size(94, 17)
        Me.rbAlgorithm.TabIndex = 1
        Me.rbAlgorithm.Text = "XOR Algorithm"
        Me.rbAlgorithm.UseVisualStyleBackColor = True
        '
        'rbArtificialIntelligence
        '
        Me.rbArtificialIntelligence.AutoSize = True
        Me.rbArtificialIntelligence.Checked = True
        Me.rbArtificialIntelligence.Location = New System.Drawing.Point(18, 29)
        Me.rbArtificialIntelligence.Name = "rbArtificialIntelligence"
        Me.rbArtificialIntelligence.Size = New System.Drawing.Size(118, 17)
        Me.rbArtificialIntelligence.TabIndex = 0
        Me.rbArtificialIntelligence.TabStop = True
        Me.rbArtificialIntelligence.Text = "Artificial Intelligence"
        Me.rbArtificialIntelligence.UseVisualStyleBackColor = True
        '
        'btn0Games
        '
        Me.btn0Games.Location = New System.Drawing.Point(224, 166)
        Me.btn0Games.Name = "btn0Games"
        Me.btn0Games.Size = New System.Drawing.Size(134, 29)
        Me.btn0Games.TabIndex = 97
        Me.btn0Games.Text = "0 Games Played"
        Me.btn0Games.UseVisualStyleBackColor = True
        '
        'btn75Games
        '
        Me.btn75Games.Location = New System.Drawing.Point(224, 201)
        Me.btn75Games.Name = "btn75Games"
        Me.btn75Games.Size = New System.Drawing.Size(134, 29)
        Me.btn75Games.TabIndex = 98
        Me.btn75Games.Text = "75 Games Played"
        Me.btn75Games.UseVisualStyleBackColor = True
        '
        'btn150Games
        '
        Me.btn150Games.Location = New System.Drawing.Point(224, 236)
        Me.btn150Games.Name = "btn150Games"
        Me.btn150Games.Size = New System.Drawing.Size(134, 29)
        Me.btn150Games.TabIndex = 99
        Me.btn150Games.Text = "150 Games Played"
        Me.btn150Games.UseVisualStyleBackColor = True
        '
        'btnCustomSave
        '
        Me.btnCustomSave.Location = New System.Drawing.Point(224, 86)
        Me.btnCustomSave.Name = "btnCustomSave"
        Me.btnCustomSave.Size = New System.Drawing.Size(134, 29)
        Me.btnCustomSave.TabIndex = 100
        Me.btnCustomSave.Text = "Save Custom Matrix"
        Me.btnCustomSave.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(211, 66)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(116, 13)
        Me.Label1.TabIndex = 101
        Me.Label1.Text = "Learning Opportunities:"
        '
        'lblLearningOps
        '
        Me.lblLearningOps.AutoSize = True
        Me.lblLearningOps.Location = New System.Drawing.Point(332, 66)
        Me.lblLearningOps.Name = "lblLearningOps"
        Me.lblLearningOps.Size = New System.Drawing.Size(13, 13)
        Me.lblLearningOps.TabIndex = 102
        Me.lblLearningOps.Text = "0"
        '
        'btnCustomLoad
        '
        Me.btnCustomLoad.Location = New System.Drawing.Point(224, 121)
        Me.btnCustomLoad.Name = "btnCustomLoad"
        Me.btnCustomLoad.Size = New System.Drawing.Size(134, 29)
        Me.btnCustomLoad.TabIndex = 103
        Me.btnCustomLoad.Text = "Load Custom Matrix"
        Me.btnCustomLoad.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(17, 236)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(38, 13)
        Me.Label2.TabIndex = 71
        Me.Label2.Text = "Matrix:"
        '
        'lblMatrixStatus
        '
        Me.lblMatrixStatus.AutoSize = True
        Me.lblMatrixStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMatrixStatus.Location = New System.Drawing.Point(60, 236)
        Me.lblMatrixStatus.Name = "lblMatrixStatus"
        Me.lblMatrixStatus.Size = New System.Drawing.Size(98, 13)
        Me.lblMatrixStatus.TabIndex = 71
        Me.lblMatrixStatus.Text = "Matrix 0 Loaded"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(376, 275)
        Me.Controls.Add(Me.lblMatrixStatus)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnCustomLoad)
        Me.Controls.Add(Me.lblLearningOps)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnCustomSave)
        Me.Controls.Add(Me.btn150Games)
        Me.Controls.Add(Me.btn75Games)
        Me.Controls.Add(Me.btn0Games)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.lblMessage2)
        Me.Controls.Add(Me.lblMessage1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "The Game of NIM"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblMessage1 As System.Windows.Forms.Label
    Friend WithEvents lblMessage2 As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents cbGB_COMPort As System.Windows.Forms.ComboBox
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents btnGBDisconnect As System.Windows.Forms.Button
    Friend WithEvents btnGBConnect As System.Windows.Forms.Button
    Friend WithEvents lblGBConnectionStatus As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents rbAlgorithm As System.Windows.Forms.RadioButton
    Friend WithEvents rbArtificialIntelligence As System.Windows.Forms.RadioButton
    Friend WithEvents btn0Games As Button
    Friend WithEvents btn75Games As Button
    Friend WithEvents btn150Games As Button
    Friend WithEvents btnCustomSave As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents lblLearningOps As Label
    Friend WithEvents btnCustomLoad As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents lblMatrixStatus As Label
End Class
