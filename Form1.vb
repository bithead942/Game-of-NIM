''' <summary>
''' Codes of possible moves
''' 
''' Remaining Game Play
''' 1    Take 1 from A
''' 2    Take 2 from A
''' 3    Take 3 from A
''' 4    Take 1 from B
''' 5    Take 2 from B
''' 6    Take 3 from B
''' 7    Take 4 from B
''' 8    Take 5 from B
''' 9    Take 1 from C
''' 10   Take 2 from C
''' 11   Take 3 from C
''' 12   Take 4 from C
''' 13   Take 5 from C
''' 14   Take 6 from C
''' 15   Take 7 from C
''' 
''' Including the selection of who goes first, there are a max of 15 possible moves
''' 
''' Arduino Pin Assignment
''' 15 LED pins, 15 LED Button pins, 2 TX/RX for LCD, 2 TX/RX for PC
''' 1 Commit Button, 1 Commit LED, 1 Clear Button, 1 Clear LED, 1 New Game Button, 1 New Game LED
''' 
''' Digital Pin     Function
''' =============================================
''' 0               Serial RX (PC)
''' 1               Serial TX (PC)
''' 2               Commit Button
''' 3               Commit LED
''' 4               Clear Button
''' 5               Clear LED
''' 6               New Game Button
''' 7               Button A1
''' 8               Button A2
''' 9               Button A3
''' 10              Button B1
''' 11              Button B2
''' 12              Button B3
''' 13              Button B4
''' 14              Button B5
''' 15              Button C1
''' 16              Button C2
''' 17              Button C3
''' 18              Serial RX (LCD)
''' 19              Serial TX (LCD)
''' 20              Button C4
''' 21              Button C5
''' 22              Button C6
''' 23              Button C7
''' 24              LED A1 Red
''' 25              LED A1 Blue
''' 26              LED A2 Red
''' 27              LED A2 Blue
''' 28              LED A3 Red
''' 29              LED A3 Blue
''' 30              LED B1 Red
''' 31              LED B1 Blue
''' 32              LED B2 Red
''' 33              LED B2 Blue
''' 34              LED B3 Red
''' 35              LED B3 Blue
''' 36              LED B4 Red
''' 37              LED B4 Blue
''' 38              LED B5 Red
''' 39              LED B5 Blue
''' 40              LED C1 Red
''' 41              LED C1 Blue
''' 42              LED C2 Red
''' 43              LED C2 Blue
''' 44              LED C3 Red
''' 45              LED C3 Blue
''' 46              LED C4 Red
''' 47              LED C4 Blue
''' 48              LED C5 Red
''' 49              LED C5 Blue
''' 50              LED C6 Red
''' 51              LED C6 Blue
''' 52              LED C7 Red
''' 53              LED C7 Blue
''' 
''' 
''' </summary>
''' <remarks></remarks>


Public Class frmMain
    Dim iActivePlayer As Integer   'who's turn is it?
    Dim iMatrix1(14, 12, 10, 8, 6, 4, 2, 0) As Byte
    Dim iMatrix2(13, 11, 9, 7, 5, 3, 1) As Byte
    Dim iMove(15, 1) As Byte           'A list of moves made in the game (value 1 to 15 above)(0 for move value, 1 for index value)
    Dim iAvailableMoves(14) As Integer 'A list of the available moves listed above (0 - 14)
    Dim iRemaining(2) As Byte       'How many remain in each row
    Dim iMoveNo As Byte             'How many turns have players taken total
    Dim strSelectRow As Char        'A, B or C for start of range
    Dim iFirstSelectNum As Byte     'Number for start of range
    Dim iSecondSelectNum As Byte    'Number for end of range
    Public WithEvents _GBarduino As Arduino_Net2
    Dim A1Enabled, A2Enabled, A3Enabled As Boolean
    Dim B1Enabled, B2Enabled, B3Enabled, B4Enabled, B5Enabled As Boolean
    Dim C1Enabled, C2Enabled, C3Enabled, C4Enabled, C5Enabled, C6Enabled, C7Enabled As Boolean
    Dim CommitEnabled, ClearEnabled As Boolean
    Const iDelay = 40
    Private Delegate Sub UpdateMessageDelegate(ByVal TB As Label, ByVal txt As String)


    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim port As String
        Dim ports As String() = SerialPort.GetPortNames()
        Dim badChars As Char() = New Char() {"c"}

        Me.Show()

        For Each port In Ports
            ' .NET Framework has a bug where COM ports are
            ' sometimes appended with a 'c' characeter!
            If port.IndexOfAny(badChars) <> -1 Then
                cbGB_COMPort.Items.Add(port.Substring(0, port.Length - 1))
            Else
                cbGB_COMPort.Items.Add(port)
            End If
        Next

        If cbGB_COMPort.Items.Count = 0 Then
            cbGB_COMPort.Text = ""
        Else
            cbGB_COMPort.Text = cbGB_COMPort.Items(cbGB_COMPort.Items.Count - 1).ToString
        End If

        LoadMatrix(0)
        StartGBControl()
        If lblGBConnectionStatus.Text = "Connected" Then
            NewGame()
        End If
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        'SaveMatrix()
    End Sub

#Region "Game Board Control"
    Public Sub StartGBControl()
        lblGBConnectionStatus.Text = "Connecting"
        Try
            _GBarduino = New Arduino_Net2(cbGB_COMPort.Text)
            _GBarduino.ComPort = cbGB_COMPort.Text
            _GBarduino.BaudRate = 9600
            If Not _GBarduino.StartCommunication() Then
                Throw New System.Exception("Failed to Connect")
            End If

            _GBarduino.Flush()
            VCConfigPorts()

            btnGBConnect.Enabled = False
            btnGBDisconnect.Enabled = True
            cbGB_COMPort.Enabled = False

            lblGBConnectionStatus.Text = "Connected"

        Catch ex As Exception
            'MsgBox("Connect Error to Game Board")
            _GBarduino.StopCommunication()

            lblGBConnectionStatus.Text = "Not Connected"
            btnGBConnect.Enabled = True
            btnGBDisconnect.Enabled = False
            cbGB_COMPort.Enabled = True
            lblMessage1.Text = ""
            lblMessage2.Text = ""
            Exit Sub
        End Try
    End Sub

    Private Sub VCConfigPorts()
        'Set up Game Board Control Pins

        If _GBarduino.ComIsOpen Then
            For i = 0 To 0
                'Commit Button
                _GBarduino.EnableDigitalPort(2, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(2, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(2, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'Commit LED
                _GBarduino.SetDigitalDirection(3, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'Clear Button
                _GBarduino.EnableDigitalPort(4, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(4, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(4, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'Clear LED
                _GBarduino.SetDigitalDirection(5, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'New Game Button
                _GBarduino.EnableDigitalPort(6, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(6, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(6, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'A1 Button
                _GBarduino.EnableDigitalPort(7, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(7, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(7, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'A2 Button
                _GBarduino.EnableDigitalPort(8, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(8, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(8, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'A3 Button
                _GBarduino.EnableDigitalPort(9, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(9, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(9, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'B1 Button
                _GBarduino.EnableDigitalPort(10, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(10, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(10, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'B2 Button
                _GBarduino.EnableDigitalPort(11, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(11, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(11, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'B3 Button
                _GBarduino.EnableDigitalPort(12, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(12, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(12, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'B4 Button
                _GBarduino.EnableDigitalPort(13, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(13, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(13, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'B5 Button
                _GBarduino.EnableDigitalPort(14, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(14, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(14, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'C1 Button
                _GBarduino.EnableDigitalPort(15, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(15, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(15, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'C2 Button
                _GBarduino.EnableDigitalPort(16, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(16, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(16, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'C3 Button
                _GBarduino.EnableDigitalPort(17, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(17, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(17, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'C4 Button
                _GBarduino.EnableDigitalPort(20, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(20, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(20, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'C5 Button
                _GBarduino.EnableDigitalPort(21, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(21, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(21, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'C6 Button
                _GBarduino.EnableDigitalPort(22, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(22, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(22, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'C7 Button
                _GBarduino.EnableDigitalPort(23, True)
                Thread.Sleep(iDelay)
                _GBarduino.EnableDigitalTrigger(23, True)
                Thread.Sleep(iDelay)
                _GBarduino.SetDigitalDirection(23, Arduino_Net2.DigitalDirection.Input)
                Thread.Sleep(iDelay)

                'A1 LED Red
                _GBarduino.SetDigitalDirection(24, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'A1 LED Blue
                _GBarduino.SetDigitalDirection(25, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'A2 LED Red
                _GBarduino.SetDigitalDirection(26, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'A2 LED Blue
                _GBarduino.SetDigitalDirection(27, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'A3 LED Red
                _GBarduino.SetDigitalDirection(28, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'A3 LED Blue
                _GBarduino.SetDigitalDirection(29, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B1 LED Red
                _GBarduino.SetDigitalDirection(30, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B1 LED Blue
                _GBarduino.SetDigitalDirection(31, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B2 LED Red
                _GBarduino.SetDigitalDirection(32, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B2 LED Blue
                _GBarduino.SetDigitalDirection(33, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B3 LED Red
                _GBarduino.SetDigitalDirection(34, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B3 LED Blue
                _GBarduino.SetDigitalDirection(35, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B4 LED Red
                _GBarduino.SetDigitalDirection(36, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B4 LED Blue
                _GBarduino.SetDigitalDirection(37, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B5 LED Red
                _GBarduino.SetDigitalDirection(38, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'B5 LED Blue
                _GBarduino.SetDigitalDirection(39, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C1 LED Red
                _GBarduino.SetDigitalDirection(40, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C1 LED Blue
                _GBarduino.SetDigitalDirection(41, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C2 LED Red
                _GBarduino.SetDigitalDirection(42, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C2 LED Blue
                _GBarduino.SetDigitalDirection(43, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C3 LED Red
                _GBarduino.SetDigitalDirection(44, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C3 LED Blue
                _GBarduino.SetDigitalDirection(45, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C4 LED Red
                _GBarduino.SetDigitalDirection(46, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C4 LED Blue
                _GBarduino.SetDigitalDirection(47, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C5 LED Red
                _GBarduino.SetDigitalDirection(48, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C5 LED Blue
                _GBarduino.SetDigitalDirection(49, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C6 LED Red
                _GBarduino.SetDigitalDirection(50, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C6 LED Blue
                _GBarduino.SetDigitalDirection(51, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C7 LED Red
                _GBarduino.SetDigitalDirection(52, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)

                'C7 LED Blue
                _GBarduino.SetDigitalDirection(53, Arduino_Net2.DigitalDirection.DigitalOutput)
                Thread.Sleep(iDelay)
            Next i
        Else
            'MsgBox("Connect Error to Game Board")
            _GBarduino.StopCommunication()
            lblGBConnectionStatus.Text = "Not Connected"
            btnGBConnect.Enabled = True
            btnGBDisconnect.Enabled = False
            cbGB_COMPort.Enabled = True
            lblMessage1.Text = ""
            lblMessage2.Text = ""
        End If

    End Sub

    Private Sub _GBarduino_DigitalDataReceived(ByVal DPortNr As Integer, ByVal Value As Integer) Handles _GBarduino.DigitalDataReceived
        Select Case DPortNr
            Case 2
                If Value = 49 And CommitEnabled Then
                    'Commit Button Released
                    CommitMove()
                End If
            Case 4
                If Value = 49 And ClearEnabled Then
                    'Clear Button Released
                    ResetItems(strSelectRow, iFirstSelectNum, iSecondSelectNum)
                    strSelectRow = ""
                    iFirstSelectNum = 0
                    iSecondSelectNum = 0
                    UpdateMessage(lblMessage2, "Select Range    ")
                    CommitEnabled = False
                    _GBarduino.SetDigitalValue(3, 0)
                    ClearEnabled = False
                    _GBarduino.SetDigitalValue(5, 0)
                    EnableLEDButtons()
                End If
            Case 6
                If Value = 49 Then
                    'New Game Button Released
                    NewGame()
                End If
            Case 7
                If Value = 49 And A1Enabled Then
                    'A1 Button Released
                    strSelectRow = "A"
                    iFirstSelectNum = 1
                    iSecondSelectNum = 1
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 8
                If Value = 49 And A2Enabled Then
                    'A2 Button Released
                    strSelectRow = "A"
                    If iRemaining(0) = 3 Then
                        iFirstSelectNum = 1
                    Else
                        iFirstSelectNum = 2
                    End If

                    iSecondSelectNum = 2
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 9
                If Value = 49 And A3Enabled Then
                    'A3 Button Released
                    strSelectRow = "A"
                    If iRemaining(0) = 3 Then
                        iFirstSelectNum = 1
                    ElseIf iRemaining(0) = 2 Then
                        iFirstSelectNum = 2
                    Else
                        iFirstSelectNum = 3
                    End If

                    iSecondSelectNum = 3
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 10
                If Value = 49 And B1Enabled Then
                    'B1 Button Released
                    strSelectRow = "B"
                    iFirstSelectNum = 1
                    iSecondSelectNum = 1
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 11
                If Value = 49 And B2Enabled Then
                    'B2 Button Released
                    strSelectRow = "B"
                    If iRemaining(1) = 5 Then
                        iFirstSelectNum = 1
                    Else
                        iFirstSelectNum = 2
                    End If

                    iSecondSelectNum = 2
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 12
                If Value = 49 And B3Enabled Then
                    'B3 Button Released
                    strSelectRow = "B"
                    If iRemaining(1) = 5 Then
                        iFirstSelectNum = 1
                    ElseIf iRemaining(1) = 4 Then
                        iFirstSelectNum = 2
                    Else
                        iFirstSelectNum = 3
                    End If

                    iSecondSelectNum = 3
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 13
                If Value = 49 And B4Enabled Then
                    'B4 Button Released
                    strSelectRow = "B"
                    If iRemaining(1) = 5 Then
                        iFirstSelectNum = 1
                    ElseIf iRemaining(1) = 4 Then
                        iFirstSelectNum = 2
                    ElseIf iRemaining(1) = 3 Then
                        iFirstSelectNum = 3
                    Else
                        iFirstSelectNum = 4
                    End If

                    iSecondSelectNum = 4
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 14
                If Value = 49 And B5Enabled Then
                    'B5 Button Released
                    strSelectRow = "B"
                    If iRemaining(1) = 5 Then
                        iFirstSelectNum = 1
                    ElseIf iRemaining(1) = 4 Then
                        iFirstSelectNum = 2
                    ElseIf iRemaining(1) = 3 Then
                        iFirstSelectNum = 3
                    ElseIf iRemaining(1) = 4 Then
                        iFirstSelectNum = 4
                    Else
                        iFirstSelectNum = 5
                    End If

                    iSecondSelectNum = 5
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 15
                If Value = 49 And C1Enabled Then
                    'C1 Button Released
                    strSelectRow = "C"
                    iFirstSelectNum = 1
                    iSecondSelectNum = 1
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 16
                If Value = 49 And C2Enabled Then
                    'C2 Button Released
                    strSelectRow = "C"
                    If iRemaining(2) = 7 Then
                        iFirstSelectNum = 1
                    Else
                        iFirstSelectNum = 2
                    End If

                    iSecondSelectNum = 2
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 17
                If Value = 49 And C3Enabled Then
                    'C3 Button Released
                    strSelectRow = "C"
                    If iRemaining(2) = 7 Then
                        iFirstSelectNum = 1
                    ElseIf iRemaining(2) = 6 Then
                        iFirstSelectNum = 2
                    Else
                        iFirstSelectNum = 3
                    End If

                    iSecondSelectNum = 3
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            'Port 18, 19 reserved for COM2 (LCD)
            Case 20
                If Value = 49 And C4Enabled Then
                    'C4 Button Released
                    strSelectRow = "C"
                    If iRemaining(2) = 7 Then
                        iFirstSelectNum = 1
                    ElseIf iRemaining(2) = 6 Then
                        iFirstSelectNum = 2
                    ElseIf iRemaining(2) = 5 Then
                        iFirstSelectNum = 3
                    Else
                        iFirstSelectNum = 4
                    End If

                    iSecondSelectNum = 4
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 21
                If Value = 49 And C5Enabled Then
                    'C5 Button Released
                    strSelectRow = "C"
                    If iRemaining(2) = 7 Then
                        iFirstSelectNum = 1
                    ElseIf iRemaining(2) = 6 Then
                        iFirstSelectNum = 2
                    ElseIf iRemaining(2) = 5 Then
                        iFirstSelectNum = 3
                    ElseIf iRemaining(2) = 4 Then
                        iFirstSelectNum = 4
                    Else
                        iFirstSelectNum = 5
                    End If

                    iSecondSelectNum = 5
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 22
                If Value = 49 And C6Enabled Then
                    'C6 Button Released
                    strSelectRow = "C"
                    If iRemaining(2) = 7 Then
                        iFirstSelectNum = 1
                    ElseIf iRemaining(2) = 6 Then
                        iFirstSelectNum = 2
                    ElseIf iRemaining(2) = 5 Then
                        iFirstSelectNum = 3
                    ElseIf iRemaining(2) = 4 Then
                        iFirstSelectNum = 4
                    ElseIf iRemaining(2) = 3 Then
                        iFirstSelectNum = 5
                    Else
                        iFirstSelectNum = 6
                    End If

                    iSecondSelectNum = 6
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
            Case 23
                If Value = 49 And C7Enabled Then
                    'C7 Button Released
                    strSelectRow = "C"
                    If iRemaining(2) = 7 Then
                        iFirstSelectNum = 1
                    ElseIf iRemaining(2) = 6 Then
                        iFirstSelectNum = 2
                    ElseIf iRemaining(2) = 5 Then
                        iFirstSelectNum = 3
                    ElseIf iRemaining(2) = 4 Then
                        iFirstSelectNum = 4
                    ElseIf iRemaining(2) = 3 Then
                        iFirstSelectNum = 5
                    ElseIf iRemaining(2) = 2 Then
                        iFirstSelectNum = 6
                    Else
                        iFirstSelectNum = 7
                    End If

                    iSecondSelectNum = 7
                    SelectItems()
                    UpdateMessage(lblMessage2, "COMMIT or CLEAR ")
                    CommitEnabled = True
                    _GBarduino.SetDigitalValue(3, 1)
                    ClearEnabled = True
                    _GBarduino.SetDigitalValue(5, 1)
                    DisableLEDButtons()
                End If
        End Select
    End Sub

#End Region

#Region "Game Logic"

    Private Sub FlashLEDs()
        Dim iRed, iBlue As Integer

        For i = 0 To 3
            If i = 0 Then
                iRed = 1
                iBlue = 0
            End If
            If i = 1 Then
                iRed = 1
                iBlue = 1
            End If
            If i = 2 Then
                iRed = 0
                iBlue = 1
            End If
            If i = 3 Then
                iRed = 0
                iBlue = 0
            End If

            _GBarduino.SetDigitalValue(24, iRed)
            _GBarduino.SetDigitalValue(25, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(26, iRed)
            _GBarduino.SetDigitalValue(27, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(28, iRed)
            _GBarduino.SetDigitalValue(29, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(30, iRed)
            _GBarduino.SetDigitalValue(31, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(32, iRed)
            _GBarduino.SetDigitalValue(33, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(34, iRed)
            _GBarduino.SetDigitalValue(35, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(36, iRed)
            _GBarduino.SetDigitalValue(37, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(38, iRed)
            _GBarduino.SetDigitalValue(39, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(40, iRed)
            _GBarduino.SetDigitalValue(41, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(42, iRed)
            _GBarduino.SetDigitalValue(43, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(44, iRed)
            _GBarduino.SetDigitalValue(45, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(46, iRed)
            _GBarduino.SetDigitalValue(47, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(48, iRed)
            _GBarduino.SetDigitalValue(49, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(50, iRed)
            _GBarduino.SetDigitalValue(51, iBlue)
            Thread.Sleep(iDelay)
            _GBarduino.SetDigitalValue(52, iRed)
            _GBarduino.SetDigitalValue(53, iBlue)
        Next i

    End Sub

    Private Sub NewGame()
        Dim i As Integer

        'Splash Screen Message
        _GBarduino.SetMessage(1, "The Game of NIM ")
        _GBarduino.SetMessage(2, "                ")

        'Set up board
        A1Enabled = True
        A2Enabled = True
        A3Enabled = True
        B1Enabled = True
        B2Enabled = True
        B3Enabled = True
        B4Enabled = True
        B5Enabled = True
        C1Enabled = True
        C2Enabled = True
        C3Enabled = True
        C4Enabled = True
        C5Enabled = True
        C6Enabled = True
        C7Enabled = True

        _GBarduino.SetDigitalValue(24, 1)
        _GBarduino.SetDigitalValue(25, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(26, 1)
        _GBarduino.SetDigitalValue(27, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(28, 1)
        _GBarduino.SetDigitalValue(29, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(30, 1)
        _GBarduino.SetDigitalValue(31, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(32, 1)
        _GBarduino.SetDigitalValue(33, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(34, 1)
        _GBarduino.SetDigitalValue(35, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(36, 1)
        _GBarduino.SetDigitalValue(37, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(38, 1)
        _GBarduino.SetDigitalValue(39, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(40, 1)
        _GBarduino.SetDigitalValue(41, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(42, 1)
        _GBarduino.SetDigitalValue(43, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(44, 1)
        _GBarduino.SetDigitalValue(45, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(46, 1)
        _GBarduino.SetDigitalValue(47, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(48, 1)
        _GBarduino.SetDigitalValue(49, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(50, 1)
        _GBarduino.SetDigitalValue(51, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(52, 1)
        _GBarduino.SetDigitalValue(53, 0)

        For i = 0 To 15
            iMove(i, 0) = 0   'List of moves
            iMove(i, 1) = 0   'List of Matirx indexes
        Next
        For i = 0 To 14
            iAvailableMoves(i) = 1
        Next
        iRemaining(0) = 3
        iRemaining(1) = 5
        iRemaining(2) = 7

        'Select player to start
        If CheckMatrix2() Then
            UpdateMessage(lblMessage1, "Computer's Turn*")
            iMove(0, 0) = 1
            iActivePlayer = 1
        ElseIf CheckMatrix1() Then
            UpdateMessage(lblMessage1, "Player 2's Turn*")
            iMove(0, 0) = 2
            iActivePlayer = 2
        Else
            iActivePlayer = CInt(Math.Floor(2 * Rnd(DatePart(DateInterval.Second, Now)))) + 1
            If iActivePlayer = 1 Then
                UpdateMessage(lblMessage1, "Computer's Turn ")
                iMove(0, 0) = 1
            Else
                iActivePlayer = 2
                UpdateMessage(lblMessage1, "Player 2's Turn ")
                iMove(0, 0) = 2
            End If
        End If
        UpdateMessage(lblMessage2, "Select Range    ")
        iMoveNo = 1
        strSelectRow = ""
        iFirstSelectNum = 0
        iSecondSelectNum = 0
        CommitEnabled = False
        _GBarduino.SetDigitalValue(3, 0)
        ClearEnabled = False
        _GBarduino.SetDigitalValue(5, 0)
        _GBarduino.Flush()
        If iActivePlayer = 1 Then
            MyMove()
        End If
    End Sub

    Private Function CheckForWinner() As Boolean
        Dim bWinner As Boolean = False
        If iRemaining(0) = 0 And iRemaining(1) = 0 And iRemaining(2) = 1 Then
            bWinner = True
        End If
        If iRemaining(0) = 0 And iRemaining(1) = 1 And iRemaining(2) = 0 Then
            bWinner = True
        End If
        If iRemaining(0) = 1 And iRemaining(1) = 0 And iRemaining(2) = 0 Then
            bWinner = True
        End If

        If bWinner Then
            If iActivePlayer = 1 Then
                UpdateMessage(lblMessage1, "YEAH, I WIN!!   ")
                UpdateMessage(lblMessage2, "GAME OVER       ")
            Else
                UpdateMessage(lblMessage1, "Player 2 wins :(")
                UpdateMessage(lblMessage2, "GAME OVER       ")
                RememberLastMove()
            End If
            CommitEnabled = False
            _GBarduino.SetDigitalValue(3, 0)
            ClearEnabled = False
            _GBarduino.SetDigitalValue(5, 0)

            DisableLEDButtons()
            FlashLEDs()
        End If

        Return bWinner
    End Function

    Private Sub RememberLastMove()


        'Assume last move lost.  Make a note to never do that move again.
        If rbArtificialIntelligence.Checked Then
            If iMove(0, 0) = 1 Then  'Computer Went First
                If iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), iMove(11, 1), iMove(13, 1), iMove(15, 1)) = "0" Then
                    iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), iMove(11, 1), iMove(13, 1), iMove(15, 1)) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), iMove(11, 1), iMove(13, 1), 0) = "0" Then
                    iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), iMove(11, 1), iMove(13, 1), 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), iMove(11, 1), 0, 0) = "0" Then
                    iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), iMove(11, 1), 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), 0, 0, 0) = "0" Then
                    iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), 0, 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), 0, 0, 0, 0) = "0" Then
                    iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), 0, 0, 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), 0, 0, 0, 0, 0) = "0" Then
                    iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), 0, 0, 0, 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix1(iMove(1, 1), iMove(3, 1), 0, 0, 0, 0, 0, 0) = "0" Then
                    iMatrix1(iMove(1, 1), iMove(3, 1), 0, 0, 0, 0, 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix1(iMove(1, 1), 0, 0, 0, 0, 0, 0, 0) = "0" Then
                    iMatrix1(iMove(1, 1), 0, 0, 0, 0, 0, 0, 0) = "1"
                End If
            Else 'Player 2 went first
                If iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), iMove(10, 1), iMove(12, 1), iMove(14, 1)) = "0" Then
                    iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), iMove(10, 1), iMove(12, 1), iMove(14, 1)) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), iMove(10, 1), iMove(12, 1), 0) = "0" Then
                    iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), iMove(10, 1), iMove(12, 1), 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), iMove(10, 1), 0, 0) = "0" Then
                    iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), iMove(10, 1), 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), 0, 0, 0) = "0" Then
                    iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), 0, 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), 0, 0, 0, 0) = "0" Then
                    iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), 0, 0, 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix2(iMove(2, 1), iMove(4, 1), 0, 0, 0, 0, 0) = "0" Then
                    iMatrix2(iMove(2, 1), iMove(4, 1), 0, 0, 0, 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                ElseIf iMatrix2(iMove(2, 1), 0, 0, 0, 0, 0, 0) = "0" Then
                    iMatrix2(iMove(2, 1), 0, 0, 0, 0, 0, 0) = "1"
                    UpdateMessage(lblMessage2, "GAME OVER *     ")
                End If
            End If
            lblLearningOps.Text = CStr(CInt(lblLearningOps.Text) + 1)
            Me.Update()
            'SaveMatrix()
        End If


    End Sub

    Private Sub CommitMove()
        Dim iRemoved As Byte
        Dim bLegalMove As Boolean = True
        Dim iSelectedMove As Integer = 0

        iRemoved = iSecondSelectNum - iFirstSelectNum + 1
        Select Case strSelectRow
            Case "A"
                iSelectedMove = 0 + iRemoved - 1
            Case "B"
                iSelectedMove = 3 + iRemoved - 1
            Case "C"
                iSelectedMove = 8 + iRemoved - 1
        End Select
        If iAvailableMoves(iSelectedMove) = 1 Then
            If iRemaining(0) = 0 And iRemaining(1) = 0 And iRemaining(2) = iRemoved Then  'Check special case at end of game when player tries to take too many
                bLegalMove = False
            ElseIf iRemaining(0) = 0 And iRemaining(1) = iRemoved And iRemaining(2) = 0 Then
                bLegalMove = False
            ElseIf iRemaining(0) = iRemoved And iRemaining(1) = 0 And iRemaining(2) = 0 Then
                bLegalMove = False
            Else
                bLegalMove = True
            End If
        Else
            bLegalMove = False
        End If

        If bLegalMove Then
            ClearItems()
            Select Case strSelectRow
                Case "A"
                    iRemaining(0) = iRemaining(0) - iRemoved
                    iMove(iMoveNo, 0) = iRemoved
                Case "B"
                    iRemaining(1) = iRemaining(1) - iRemoved
                    iMove(iMoveNo, 0) = iRemoved + 3
                Case "C"
                    iRemaining(2) = iRemaining(2) - iRemoved
                    iMove(iMoveNo, 0) = iRemoved + 8
            End Select

            If Not CheckForWinner() Then
                UpdateAvailableMoves()
                iMoveNo = iMoveNo + 1
                strSelectRow = ""
                iFirstSelectNum = 0
                iSecondSelectNum = 0
                If iActivePlayer = 2 Then
                    UpdateMessage(lblMessage1, "Computer's Turn ")
                    iActivePlayer = 1
                    Thread.Sleep(500)
                    _GBarduino.Flush()
                    MyMove()
                Else
                    UpdateMessage(lblMessage1, "Player 2's Turn ")
                    UpdateMessage(lblMessage2, "Select Range    ")
                    CommitEnabled = False
                    _GBarduino.SetDigitalValue(3, 0)
                    ClearEnabled = False
                    _GBarduino.SetDigitalValue(5, 0)
                    EnableLEDButtons()
                    iActivePlayer = 2
                End If
                'Else
                '    MsgBox("Moves:  " & iMove(0, 0).ToString & ", " & iMove(1, 0).ToString & ", " & iMove(2, 0).ToString & ", " & iMove(3, 0).ToString & ", " & _
                '                            iMove(4, 0).ToString & ", " & iMove(5, 0).ToString & ", " & iMove(6, 0).ToString & ", " & iMove(7, 0).ToString & ", " & _
                '                            iMove(8, 0).ToString & ", " & iMove(9, 0).ToString & ", " & iMove(10, 0).ToString & ", " & iMove(11, 0).ToString & ", " & _
                '                            iMove(12, 0).ToString & ", " & iMove(13, 0).ToString & ", " & iMove(14, 0).ToString)
            End If
        Else
            UpdateMessage(lblMessage2, "Error, Try Again")
            ResetItems(strSelectRow, iFirstSelectNum, iSecondSelectNum)
            strSelectRow = ""
            iFirstSelectNum = 0
            iSecondSelectNum = 0
            CommitEnabled = False
            _GBarduino.SetDigitalValue(3, 0)
            ClearEnabled = False
            _GBarduino.SetDigitalValue(5, 0)
        End If
        _GBarduino.Flush()
    End Sub

    Private Sub MyMove()
        Dim iSelectedMove As Integer = 0

        DisableLEDButtons()
        If rbAlgorithm.Checked Then
            GetNextMove_Algorythm(iSelectedMove, iMoveNo)
        Else
            GetNextMove_AI(iSelectedMove, iMoveNo)
        End If


        Select Case iSelectedMove
            Case 0 To 2
                strSelectRow = "A"
                iFirstSelectNum = 4 - iRemaining(0)
                Select Case iSelectedMove
                    Case 0
                        iSecondSelectNum = iFirstSelectNum
                    Case 1
                        iSecondSelectNum = iFirstSelectNum + 1
                    Case 2
                        iSecondSelectNum = iFirstSelectNum + 2
                End Select
            Case 3 To 7
                strSelectRow = "B"
                iFirstSelectNum = 6 - iRemaining(1)
                Select Case iSelectedMove
                    Case 3
                        iSecondSelectNum = iFirstSelectNum
                    Case 4
                        iSecondSelectNum = iFirstSelectNum + 1
                    Case 5
                        iSecondSelectNum = iFirstSelectNum + 2
                    Case 6
                        iSecondSelectNum = iFirstSelectNum + 3
                    Case 7
                        iSecondSelectNum = iFirstSelectNum + 4
                End Select
            Case 8 To 14
                strSelectRow = "C"
                iFirstSelectNum = 8 - iRemaining(2)
                Select Case iSelectedMove
                    Case 8
                        iSecondSelectNum = iFirstSelectNum
                    Case 9
                        iSecondSelectNum = iFirstSelectNum + 1
                    Case 10
                        iSecondSelectNum = iFirstSelectNum + 2
                    Case 11
                        iSecondSelectNum = iFirstSelectNum + 3
                    Case 12
                        iSecondSelectNum = iFirstSelectNum + 4
                    Case 13
                        iSecondSelectNum = iFirstSelectNum + 5
                    Case 14
                        iSecondSelectNum = iFirstSelectNum + 6
                End Select
        End Select

        SelectItems()
        UpdateMessage(lblMessage2, "Press COMMIT    ")
        CommitEnabled = True
        _GBarduino.SetDigitalValue(3, 1)
        ClearEnabled = False
        _GBarduino.SetDigitalValue(5, 0)

    End Sub

    Private Function ConvertToSelectedMove(ByVal iNumRemove As Integer, ByRef strRow As String) As Integer
        Select Case strRow
            Case "A"
                Return iNumRemove - 1
            Case "B"
                Return iNumRemove + 2
            Case "C"
                Return iNumRemove + 7
            Case Else
                Return 0
        End Select
    End Function

    Private Function FlipBit(ByVal strVal As String, ByVal iPlace As Integer) As String
        Dim CharArray(4) As String

        CharArray(1) = Mid(strVal, 3, 1)
        CharArray(2) = Mid(strVal, 2, 1)
        CharArray(3) = Mid(strVal, 1, 1)

        If CharArray(1) = "" And CharArray(2) = "" Then
            iPlace = iPlace + 2
        ElseIf CharArray(1) = "" Then
            iPlace = iPlace + 1
        End If

        If CharArray(iPlace) = "0" Then
            CharArray(iPlace) = "1"
        Else
            CharArray(iPlace) = "0"
        End If

        Return CharArray(3) + CharArray(2) + CharArray(1)

    End Function

    Private Function iMid(ByVal strVal As String, ByVal iPlace As Integer) As String
        Dim CharArray(4) As String

        CharArray(1) = Mid(strVal, 3, 1)
        CharArray(2) = Mid(strVal, 2, 1)
        CharArray(3) = Mid(strVal, 1, 1)

        If CharArray(1) = "" And CharArray(2) = "" Then
            iPlace = iPlace + 2
        ElseIf CharArray(1) = "" Then
            iPlace = iPlace + 1
        End If

        If iPlace = 1 Then
            Return CharArray(1)
        ElseIf iPlace = 2 Then
            Return CharArray(2)
        ElseIf iPlace = 3 Then
            Return CharArray(3)
        Else
            Return ""
        End If

    End Function

    Private Sub GetNextMove_Algorythm(ByRef iSelectedMove As Integer, ByVal iMoveNo As Integer)
        Dim iXORResult, iTargetRowVal, iNumRemove As Integer
        Dim strXORResult, strRowAResult, strRowBResult, strRowCResult As String


        '    'Check for winning move
        'if one row empty and one row has only one, take all remaining from third row
        If iRemaining(0) = 1 And iRemaining(1) = 0 Then
            iSelectedMove = iRemaining(2) + 7
        ElseIf iRemaining(0) = 1 And iRemaining(2) = 0 Then
            iSelectedMove = iRemaining(1) + 2
        ElseIf iRemaining(1) = 1 And iRemaining(0) = 0 Then
            iSelectedMove = iRemaining(2) + 7
        ElseIf iRemaining(1) = 1 And iRemaining(2) = 0 Then
            iSelectedMove = iRemaining(0) - 1
        ElseIf iRemaining(2) = 1 And iRemaining(0) = 0 Then
            iSelectedMove = iRemaining(1) + 2
        ElseIf iRemaining(2) = 1 And iRemaining(1) = 0 Then
            iSelectedMove = iRemaining(0) - 1
            'If two rows empty, take all remaining - 1 
        ElseIf iRemaining(0) = 0 And iRemaining(1) = 0 Then
            iSelectedMove = iRemaining(2) + 7 - 1
        ElseIf iRemaining(0) = 0 And iRemaining(2) = 0 Then
            iSelectedMove = iRemaining(1) + 2 - 1
        ElseIf iRemaining(1) = 0 And iRemaining(2) = 0 Then
            iSelectedMove = iRemaining(0) - 1 - 1
            'Try to get into a state where there is 1 left in each row
        ElseIf iRemaining(0) = 1 And iRemaining(1) = 1 And iRemaining(2) > 1 Then
            iSelectedMove = iRemaining(2) + 7 - 1
        ElseIf iRemaining(0) = 1 And iRemaining(1) > 1 And iRemaining(2) = 1 Then
            iSelectedMove = iRemaining(1) + 2 - 1
        ElseIf iRemaining(0) > 1 And iRemaining(1) = 1 And iRemaining(2) = 1 Then
            iSelectedMove = iRemaining(0) - 1 - 1
        Else
            iXORResult = iRemaining(0) Xor iRemaining(1) Xor iRemaining(2)
            strXORResult = IntToBin(iXORResult)

            strRowAResult = IntToBin(iRemaining(0))
            strRowBResult = IntToBin(iRemaining(1))
            strRowCResult = IntToBin(iRemaining(2))


            Select Case strXORResult
                Case "0"
                    'No good move to make, so make first avaialble legal move
                    For i = 0 To 14
                        If iAvailableMoves(i) = 1 Then
                            iSelectedMove = i
                            Exit For
                        End If
                    Next
                Case "1"
                    If iMid(strRowAResult, 1) = "1" Then
                        strRowAResult = FlipBit(strRowAResult, 1)
                        iTargetRowVal = BinToInt(strRowAResult)
                        iNumRemove = iRemaining(0) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "A")
                    ElseIf iMid(strRowBResult, 1) = "1" Then
                        strRowBResult = FlipBit(strRowBResult, 1)
                        iTargetRowVal = BinToInt(strRowBResult)
                        iNumRemove = iRemaining(1) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "B")
                    ElseIf iMid(strRowCResult, 1) = "1" Then
                        strRowCResult = FlipBit(strRowCResult, 1)
                        iTargetRowVal = BinToInt(strRowCResult)
                        iNumRemove = iRemaining(2) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "C")
                    Else
                        'No good move to make, so make first avaialble legal move
                        For i = 0 To 14
                            If iAvailableMoves(i) = 1 Then
                                iSelectedMove = i
                                Exit For
                            End If
                        Next
                    End If
                Case "10"
                    If iMid(strRowAResult, 2) = "1" Then
                        strRowAResult = FlipBit(strRowAResult, 2)
                        iTargetRowVal = BinToInt(strRowAResult)
                        iNumRemove = iRemaining(0) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "A")
                    ElseIf iMid(strRowBResult, 2) = "1" Then
                        strRowBResult = FlipBit(strRowBResult, 2)
                        iTargetRowVal = BinToInt(strRowBResult)
                        iNumRemove = iRemaining(1) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "B")
                    ElseIf iMid(strRowCResult, 2) = "1" Then
                        strRowCResult = FlipBit(strRowCResult, 2)
                        iTargetRowVal = BinToInt(strRowCResult)
                        iNumRemove = iRemaining(2) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "C")
                    Else
                        'No good move to make, so make first avaialble legal move
                        For i = 0 To 14
                            If iAvailableMoves(i) = 1 Then
                                iSelectedMove = i
                                Exit For
                            End If
                        Next
                    End If
                Case "11"
                    'If iMid(strRowAResult, 1) = "1" Or iMid(strRowAResult, 2) = "1" Then
                    If iMid(strRowAResult, 2) = "1" Then
                        strRowAResult = FlipBit(strRowAResult, 1)
                        strRowAResult = FlipBit(strRowAResult, 2)
                        iTargetRowVal = BinToInt(strRowAResult)
                        iNumRemove = iRemaining(0) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "A")
                        'ElseIf iMid(strRowBResult, 1) = "1" Or iMid(strRowBResult, 2) = "1" Then
                    ElseIf iMid(strRowBResult, 2) = "1" Then
                        strRowBResult = FlipBit(strRowBResult, 1)
                        strRowBResult = FlipBit(strRowBResult, 2)
                        iTargetRowVal = BinToInt(strRowBResult)
                        iNumRemove = iRemaining(1) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "B")
                        'ElseIf iMid(strRowCResult, 1) = "1" Or iMid(strRowCResult, 2) = "1" Then
                    ElseIf iMid(strRowCResult, 2) = "1" Then
                        strRowCResult = FlipBit(strRowCResult, 1)
                        strRowCResult = FlipBit(strRowCResult, 2)
                        iTargetRowVal = BinToInt(strRowCResult)
                        iNumRemove = iRemaining(2) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "C")
                    Else
                        'No good move to make, so make first avaialble legal move
                        For i = 0 To 14
                            If iAvailableMoves(i) = 1 Then
                                iSelectedMove = i
                                Exit For
                            End If
                        Next
                    End If
                Case "100"
                    If iMid(strRowBResult, 3) = "1" Then
                        strRowBResult = FlipBit(strRowBResult, 3)
                        iTargetRowVal = BinToInt(strRowBResult)
                        iNumRemove = iRemaining(1) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "B")
                    ElseIf iMid(strRowCResult, 3) = "1" Then
                        strRowCResult = FlipBit(strRowCResult, 3)
                        iTargetRowVal = BinToInt(strRowCResult)
                        iNumRemove = iRemaining(2) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "C")
                    Else
                        'No good move to make, so make first avaialble legal move
                        For i = 0 To 14
                            If iAvailableMoves(i) = 1 Then
                                iSelectedMove = i
                                Exit For
                            End If
                        Next
                    End If
                Case "101"
                    'If iMid(strRowBResult, 1) = "1" Or iMid(strRowBResult, 3) = "1" Then
                    If iMid(strRowBResult, 3) = "1" Then
                        strRowBResult = FlipBit(strRowBResult, 1)
                        strRowBResult = FlipBit(strRowBResult, 3)
                        iTargetRowVal = BinToInt(strRowBResult)
                        iNumRemove = iRemaining(1) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "B")
                        'ElseIf iMid(strRowCResult, 1) = "1" Or iMid(strRowCResult, 3) = "1" Then
                    ElseIf iMid(strRowCResult, 3) = "1" Then
                        strRowCResult = FlipBit(strRowCResult, 1)
                        strRowCResult = FlipBit(strRowCResult, 3)
                        iTargetRowVal = BinToInt(strRowCResult)
                        iNumRemove = iRemaining(2) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "C")
                    Else
                        'No good move to make, so make first avaialble legal move
                        For i = 0 To 14
                            If iAvailableMoves(i) = 1 Then
                                iSelectedMove = i
                                Exit For
                            End If
                        Next
                    End If
                Case "110"
                    'If iMid(strRowBResult, 2) = "1" Or iMid(strRowBResult, 3) = "1" Then
                    If iMid(strRowBResult, 3) = "1" Then
                        strRowBResult = FlipBit(strRowBResult, 2)
                        strRowBResult = FlipBit(strRowBResult, 3)
                        iTargetRowVal = BinToInt(strRowBResult)
                        iNumRemove = iRemaining(1) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "B")
                        'ElseIf iMid(strRowCResult, 2) = "1" Or iMid(strRowCResult, 3) = "1" Then
                    ElseIf iMid(strRowCResult, 3) = "1" Then
                        strRowCResult = FlipBit(strRowCResult, 2)
                        strRowCResult = FlipBit(strRowCResult, 3)
                        iTargetRowVal = BinToInt(strRowCResult)
                        iNumRemove = iRemaining(2) - iTargetRowVal
                        iSelectedMove = ConvertToSelectedMove(iNumRemove, "C")
                    Else
                        'No good move to make, so make first avaialble legal move
                        For i = 0 To 14
                            If iAvailableMoves(i) = 1 Then
                                iSelectedMove = i
                                Exit For
                            End If
                        Next
                    End If
                Case "111"
                    If iRemaining(2) = 7 Then
                        iSelectedMove = 14
                    Else
                        'No good move to make, so make first avaialble legal move
                        For i = 0 To 14
                            If iAvailableMoves(i) = 1 Then
                                iSelectedMove = i
                                Exit For
                            End If
                        Next
                    End If

            End Select
        End If


    End Sub

    Private Sub GetNextMove_AI(ByRef iSelectedMove As Integer, ByVal iMoveNo As Integer)
        Dim bFoundMove As Boolean = False
        Dim iMatrixIndex As Integer = 1

        Select Case iMoveNo
            Case 1
                Try
                    'Look for a good move
                    While iMatrix1(iMatrixIndex, 0, 0, 0, 0, 0, 0, 0) = "1" And iMatrixIndex <= 14
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 14 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 2
                Try
                    'Look for a good move
                    While iMatrix2(iMatrixIndex, 0, 0, 0, 0, 0, 0) = "1" And iMatrixIndex <= 13
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 13 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try
            Case 3
                Try
                    'Look for a good move
                    While iMatrix1(iMove(1, 1), iMatrixIndex, 0, 0, 0, 0, 0, 0) = "1" And iMatrixIndex <= 12
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 12 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 4
                Try
                    'Look for a good move
                    While iMatrix2(iMove(2, 1), iMatrixIndex, 0, 0, 0, 0, 0) = "1" And iMatrixIndex <= 11
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 11 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 5
                Try
                    'Look for a good move
                    While iMatrix1(iMove(1, 1), iMove(3, 1), iMatrixIndex, 0, 0, 0, 0, 0) = "1" And iMatrixIndex <= 10
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 10 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 6
                Try
                    'Look for a good move
                    While iMatrix2(iMove(2, 1), iMove(4, 1), iMatrixIndex, 0, 0, 0, 0) = "1" And iMatrixIndex <= 9
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 9 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 7
                Try
                    'Look for a good move
                    While iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMatrixIndex, 0, 0, 0, 0) = "1" And iMatrixIndex <= 8
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 8 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 8
                Try
                    'Look for a good move
                    While iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMatrixIndex, 0, 0, 0) = "1" And iMatrixIndex <= 7
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 7 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 9
                Try
                    'Look for a good move
                    While iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMatrixIndex, 0, 0, 0) = "1" And iMatrixIndex <= 6
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 6 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 10
                Try
                    'Look for a good move
                    While iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), iMatrixIndex, 0, 0) = "1" And iMatrixIndex <= 5
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 5 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 11
                Try
                    'Look for a good move
                    While iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), iMatrixIndex, 0, 0) = "1" And iMatrixIndex <= 4
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 4 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 12
                Try
                    'Look for a good move
                    While iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), iMove(10, 1), iMatrixIndex, 0) = "1" And iMatrixIndex <= 3
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 3 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 13
                Try
                    'Look for a good move
                    While iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), iMove(11, 1), iMatrixIndex, 0) = "1" And iMatrixIndex <= 2
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 2 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 14
                Try
                    'Look for a good move
                    While iMatrix2(iMove(2, 1), iMove(4, 1), iMove(6, 1), iMove(8, 1), iMove(10, 1), iMove(12, 1), iMatrixIndex) = "1" And iMatrixIndex <= 1
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 1 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try

            Case 15
                Try
                    'Look for a good move
                    While iMatrix1(iMove(1, 1), iMove(3, 1), iMove(5, 1), iMove(7, 1), iMove(9, 1), iMove(11, 1), iMove(13, 1), iMatrixIndex) = "1" And iMatrixIndex <= 0
                        iMatrixIndex = iMatrixIndex + 1
                    End While
                    If iMatrixIndex <= 0 Then  'There is a good move
                        For i = 0 To iMatrixIndex - 1  'Use the Matrix offset to find the assocaited available move in this scenario
                            While iAvailableMoves(iSelectedMove) = 0
                                iSelectedMove = iSelectedMove + 1
                            End While
                            iSelectedMove = iSelectedMove + 1
                        Next
                        iSelectedMove = iSelectedMove - 1 'Adjust for final increment
                        iMove(iMoveNo, 1) = iMatrixIndex
                    Else  'there are no good moves - select the first available move
                        While iAvailableMoves(iSelectedMove) = 0
                            iSelectedMove = iSelectedMove + 1
                        End While
                        iMove(iMoveNo, 1) = 1
                    End If
                Catch ex As Exception
                    iSelectedMove = 0
                    While iAvailableMoves(iSelectedMove) = 0
                        iSelectedMove = iSelectedMove + 1
                    End While
                    iMove(iMoveNo, 1) = 1
                End Try
        End Select
    End Sub


    Function IntToBin(ByVal IntegerNumber As Long) As String
        Dim IntNum, TempValue As Integer
        Dim BinValue As String = ""

        IntNum = IntegerNumber
        Do
            'Use the Mod operator to get the current binary digit from the
            'Integer number
            TempValue = IntNum Mod 2
            BinValue = CStr(TempValue) + BinValue

            'Divide the current number by 2 and get the integer result
            IntNum = IntNum \ 2
        Loop Until IntNum = 0

        Return BinValue

    End Function

    Function BinToInt(ByVal BinaryNumber As String)
        Dim Length, TempValue As Integer

        'Get the length of the binary string
        Length = Len(BinaryNumber)

        'Convert each binary digit to its corresponding integer value
        'and add the value to the previous sum
        'The string is parsed from the right (LSB - Least Significant Bit)
        'to the left (MSB - Most Significant Bit)
        For x = 1 To Length
            TempValue = TempValue + Val(Mid(BinaryNumber, Length - x + 1, 1)) * 2 ^ (x - 1)
        Next

        Return TempValue

    End Function


    Private Sub UpdateMessage(ByVal TB As Label, ByVal txt As String)
        If TB.InvokeRequired Then
            TB.Invoke(New UpdateMessageDelegate(AddressOf UpdateMessage), New Object() {TB, txt})
        Else
            TB.Text = txt
            If TB.Name = "lblMessage1" Then
                _GBarduino.SetMessage(1, txt)
            ElseIf TB.Name = "lblMessage2" Then
                _GBarduino.SetMessage(2, txt)
            End If
        End If
    End Sub

    Private Sub UpdateAvailableMoves()
        Dim i As Integer
        'Evaluate last move
        'Make Changes to AvailabeMoves() matrix
        'Change 1's to 0's if the move is no longer possible

        Select Case strSelectRow
            Case "A"
                Select Case iRemaining(0)
                    Case 2
                        iAvailableMoves(2) = 0
                    Case 1
                        iAvailableMoves(2) = 0
                        iAvailableMoves(1) = 0
                    Case 0
                        iAvailableMoves(2) = 0
                        iAvailableMoves(1) = 0
                        iAvailableMoves(0) = 0
                End Select
            Case "B"
                Select Case iRemaining(1)
                    Case 4
                        iAvailableMoves(7) = 0
                    Case 3
                        iAvailableMoves(7) = 0
                        iAvailableMoves(6) = 0
                    Case 2
                        iAvailableMoves(7) = 0
                        iAvailableMoves(6) = 0
                        iAvailableMoves(5) = 0
                    Case 1
                        iAvailableMoves(7) = 0
                        iAvailableMoves(6) = 0
                        iAvailableMoves(5) = 0
                        iAvailableMoves(4) = 0
                    Case 0
                        iAvailableMoves(7) = 0
                        iAvailableMoves(6) = 0
                        iAvailableMoves(5) = 0
                        iAvailableMoves(4) = 0
                        iAvailableMoves(3) = 0
                End Select
            Case "C"
                Select Case iRemaining(2)
                    Case 6
                        iAvailableMoves(14) = 0
                    Case 5
                        iAvailableMoves(14) = 0
                        iAvailableMoves(13) = 0
                    Case 4
                        iAvailableMoves(14) = 0
                        iAvailableMoves(13) = 0
                        iAvailableMoves(12) = 0
                    Case 3
                        iAvailableMoves(14) = 0
                        iAvailableMoves(13) = 0
                        iAvailableMoves(12) = 0
                        iAvailableMoves(11) = 0
                    Case 2
                        iAvailableMoves(14) = 0
                        iAvailableMoves(13) = 0
                        iAvailableMoves(12) = 0
                        iAvailableMoves(11) = 0
                        iAvailableMoves(10) = 0
                    Case 1
                        iAvailableMoves(14) = 0
                        iAvailableMoves(13) = 0
                        iAvailableMoves(12) = 0
                        iAvailableMoves(11) = 0
                        iAvailableMoves(10) = 0
                        iAvailableMoves(9) = 0
                    Case 0
                        iAvailableMoves(14) = 0
                        iAvailableMoves(13) = 0
                        iAvailableMoves(12) = 0
                        iAvailableMoves(11) = 0
                        iAvailableMoves(10) = 0
                        iAvailableMoves(9) = 0
                        iAvailableMoves(8) = 0
                End Select
        End Select

        'Adjust for special end-of-game case
        If iRemaining(0) = 0 And iRemaining(1) = 0 Then
            For i = 14 To 8 Step -1
                If iAvailableMoves(i) = 1 Then
                    iAvailableMoves(i) = 0
                    Exit For
                End If
            Next
        End If

        If iRemaining(0) = 0 And iRemaining(2) = 0 Then
            For i = 7 To 3 Step -1
                If iAvailableMoves(i) = 1 Then
                    iAvailableMoves(i) = 0
                    Exit For
                End If
            Next
        End If

        If iRemaining(1) = 0 And iRemaining(2) = 0 Then
            For i = 2 To 0 Step -1
                If iAvailableMoves(i) = 1 Then
                    iAvailableMoves(i) = 0
                    Exit For
                End If
            Next
        End If

    End Sub
#End Region

#Region "Buttons"
    Private Sub rbArtificialIntelligence_CheckedChanged(sender As Object, e As EventArgs) Handles rbArtificialIntelligence.CheckedChanged
        If rbArtificialIntelligence.Checked Then
            rbAlgorithm.Checked = False
            btn0Games.Enabled = True
            btn75Games.Enabled = True
            btn150Games.Enabled = True
            btnCustomLoad.Enabled = True
            btnCustomSave.Enabled = True
            Label1.Visible = True
            lblLearningOps.Visible = True
            lblMatrixStatus.Text = "AI Mode"
            'NewGame()
        End If
    End Sub

    Private Sub btn0Games_Click(sender As Object, e As EventArgs) Handles btn0Games.Click
        LoadMatrix(0)
        'lblLearningOps.Text = "0"

        If lblGBConnectionStatus.Text = "Connected" Then
            NewGame()
        End If
        lblMatrixStatus.Text = "Matrix 0 Loaded"
    End Sub

    Private Sub btn75Games_Click(sender As Object, e As EventArgs) Handles btn75Games.Click
        LoadMatrix(75)
        'lblLearningOps.Text = "75"

        If lblGBConnectionStatus.Text = "Connected" Then
            NewGame()
        End If
        lblMatrixStatus.Text = "Matrix 75 Loaded"
    End Sub

    Private Sub btn150Games_Click(sender As Object, e As EventArgs) Handles btn150Games.Click
        LoadMatrix(150)
        'lblLearningOps.Text = "150"

        If lblGBConnectionStatus.Text = "Connected" Then
            NewGame()
        End If
        lblMatrixStatus.Text = "Matrix 150 Loaded"
    End Sub

    Private Sub btnCustomLoad_Click(sender As Object, e As EventArgs) Handles btnCustomLoad.Click
        LoadMatrix(300)
        'lblLearningOps.Text = "0"
    End Sub

    Private Sub btnCustomSave_Click(sender As Object, e As EventArgs) Handles btnCustomSave.Click
        SaveMatrix()
    End Sub




    Private Sub rbAlgorythm_CheckedChanged(sender As Object, e As EventArgs) Handles rbAlgorithm.CheckedChanged
        If rbAlgorithm.Checked Then
            rbArtificialIntelligence.Checked = False
            btn0Games.Enabled = False
            btn75Games.Enabled = False
            btn150Games.Enabled = False
            btnCustomSave.Enabled = False
            btnCustomLoad.Enabled = False
            Label1.Visible = False
            lblLearningOps.Visible = False
            lblMatrixStatus.Text = "XOR Algorithm Mode"
            NewGame()
        End If
    End Sub

    Private Sub btnVCConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGBConnect.Click
        lblGBConnectionStatus.Text = "Connecting"

        btnGBConnect.Enabled = False
        btnGBDisconnect.Enabled = True
        cbGB_COMPort.Enabled = False

        If cbGB_COMPort.Text = "" Then
            'MsgBox("Specify COM Port")
            lblGBConnectionStatus.Text = "Not Connected"
            Exit Sub
        End If

        LoadMatrix(0)

        'Start monitoring Arduino
        StartGBControl()
        If lblGBConnectionStatus.Text = "Connected" Then
            NewGame()
        End If

    End Sub

    Private Sub btnVCDisconnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGBDisconnect.Click
        btnGBConnect.Enabled = True
        btnGBDisconnect.Enabled = False
        cbGB_COMPort.Enabled = True
        _GBarduino.StopCommunication()

        lblGBConnectionStatus.Text = "Not Connected"

    End Sub

    Private Sub EnableLEDButtons()

        Select Case iRemaining(0)
            Case 3
                A1Enabled = True
                A2Enabled = True
                A3Enabled = True
            Case 2
                A1Enabled = False
                A2Enabled = True
                A3Enabled = True
            Case 1
                A1Enabled = False
                A2Enabled = False
                A3Enabled = True
            Case Else
                A1Enabled = False
                A2Enabled = False
                A3Enabled = False
        End Select
        Select Case iRemaining(1)
            Case 5
                B1Enabled = True
                B2Enabled = True
                B3Enabled = True
                B4Enabled = True
                B5Enabled = True
            Case 4
                B1Enabled = False
                B2Enabled = True
                B3Enabled = True
                B4Enabled = True
                B5Enabled = True
            Case 3
                B1Enabled = False
                B2Enabled = False
                B3Enabled = True
                B4Enabled = True
                B5Enabled = True
            Case 2
                B1Enabled = False
                B2Enabled = False
                B3Enabled = False
                B4Enabled = True
                B5Enabled = True
            Case 1
                B1Enabled = False
                B2Enabled = False
                B3Enabled = False
                B4Enabled = False
                B5Enabled = True
            Case Else
                B1Enabled = False
                B2Enabled = False
                B3Enabled = False
                B4Enabled = False
                B5Enabled = False
        End Select

        Select Case iRemaining(2)
            Case 7
                C1Enabled = True
                C2Enabled = True
                C3Enabled = True
                C4Enabled = True
                C5Enabled = True
                C6Enabled = True
                C7Enabled = True
            Case 6
                C1Enabled = False
                C2Enabled = True
                C3Enabled = True
                C4Enabled = True
                C5Enabled = True
                C6Enabled = True
                C7Enabled = True
            Case 5
                C1Enabled = False
                C2Enabled = False
                C3Enabled = True
                C4Enabled = True
                C5Enabled = True
                C6Enabled = True
                C7Enabled = True
            Case 4
                C1Enabled = False
                C2Enabled = False
                C3Enabled = False
                C4Enabled = True
                C5Enabled = True
                C6Enabled = True
                C7Enabled = True
            Case 3
                C1Enabled = False
                C2Enabled = False
                C3Enabled = False
                C4Enabled = False
                C5Enabled = True
                C6Enabled = True
                C7Enabled = True
            Case 2
                C1Enabled = False
                C2Enabled = False
                C3Enabled = False
                C4Enabled = False
                C5Enabled = False
                C6Enabled = True
                C7Enabled = True
            Case 1
                C1Enabled = False
                C2Enabled = False
                C3Enabled = False
                C4Enabled = False
                C5Enabled = False
                C6Enabled = False
                C7Enabled = True
            Case Else
                C1Enabled = False
                C2Enabled = False
                C3Enabled = False
                C4Enabled = False
                C5Enabled = False
                C6Enabled = False
                C7Enabled = False
        End Select

    End Sub

    Private Sub DisableLEDButtons()
        A1Enabled = False
        A2Enabled = False
        A3Enabled = False
        B1Enabled = False
        B2Enabled = False
        B3Enabled = False
        B4Enabled = False
        B5Enabled = False
        C1Enabled = False
        C2Enabled = False
        C3Enabled = False
        C4Enabled = False
        C5Enabled = False
        C6Enabled = False
        C7Enabled = False
    End Sub

#End Region

#Region "Adjust LEDs"
    Private Sub SelectItems()
        If iSecondSelectNum < iFirstSelectNum And iSecondSelectNum <> 0 Then
            Dim x As Integer
            x = iSecondSelectNum
            iSecondSelectNum = iFirstSelectNum
            iFirstSelectNum = x
        End If

        If strSelectRow = "A" Then
            iFirstSelectNum = 3 - iRemaining(0) + 1
            Select Case iFirstSelectNum
                Case 1
                    _GBarduino.SetDigitalValue(24, 0)
                    _GBarduino.SetDigitalValue(25, 1)
                Case 2
                    _GBarduino.SetDigitalValue(26, 0)
                    _GBarduino.SetDigitalValue(27, 1)
                Case 3
                    _GBarduino.SetDigitalValue(28, 0)
                    _GBarduino.SetDigitalValue(29, 1)
            End Select
        End If
        If strSelectRow = "B" Then
            iFirstSelectNum = 5 - iRemaining(1) + 1
            Select Case iFirstSelectNum
                Case 1
                    _GBarduino.SetDigitalValue(30, 0)
                    _GBarduino.SetDigitalValue(31, 1)
                Case 2
                    _GBarduino.SetDigitalValue(32, 0)
                    _GBarduino.SetDigitalValue(33, 1)
                Case 3
                    _GBarduino.SetDigitalValue(34, 0)
                    _GBarduino.SetDigitalValue(35, 1)
                Case 4
                    _GBarduino.SetDigitalValue(36, 0)
                    _GBarduino.SetDigitalValue(37, 1)
                Case 5
                    _GBarduino.SetDigitalValue(38, 0)
                    _GBarduino.SetDigitalValue(39, 1)
            End Select
        End If
        If strSelectRow = "C" Then
            iFirstSelectNum = 7 - iRemaining(2) + 1
            Select Case iFirstSelectNum
                Case 1
                    _GBarduino.SetDigitalValue(40, 0)
                    _GBarduino.SetDigitalValue(41, 1)
                Case 2
                    _GBarduino.SetDigitalValue(42, 0)
                    _GBarduino.SetDigitalValue(43, 1)
                Case 3
                    _GBarduino.SetDigitalValue(44, 0)
                    _GBarduino.SetDigitalValue(45, 1)
                Case 4
                    _GBarduino.SetDigitalValue(46, 0)
                    _GBarduino.SetDigitalValue(47, 1)
                Case 5
                    _GBarduino.SetDigitalValue(48, 0)
                    _GBarduino.SetDigitalValue(49, 1)
                Case 6
                    _GBarduino.SetDigitalValue(50, 0)
                    _GBarduino.SetDigitalValue(51, 1)
                Case 7
                    _GBarduino.SetDigitalValue(52, 0)
                    _GBarduino.SetDigitalValue(53, 1)
            End Select
        End If

        If iSecondSelectNum > 0 Then
            If strSelectRow = "A" Then
                Select Case iSecondSelectNum
                    Case 1
                        _GBarduino.SetDigitalValue(24, 0)
                        _GBarduino.SetDigitalValue(25, 1)
                    Case 2
                        _GBarduino.SetDigitalValue(26, 0)
                        _GBarduino.SetDigitalValue(27, 1)
                    Case 3
                        _GBarduino.SetDigitalValue(28, 0)
                        _GBarduino.SetDigitalValue(29, 1)
                        If iFirstSelectNum = 1 Then  'Turn on A2
                            _GBarduino.SetDigitalValue(26, 0)
                            _GBarduino.SetDigitalValue(27, 1)
                        End If
                End Select
            End If
            If strSelectRow = "B" Then
                Select Case iSecondSelectNum
                    Case 1
                        _GBarduino.SetDigitalValue(30, 0)
                        _GBarduino.SetDigitalValue(31, 1)
                    Case 2
                        _GBarduino.SetDigitalValue(32, 0)
                        _GBarduino.SetDigitalValue(33, 1)
                    Case 3
                        _GBarduino.SetDigitalValue(34, 0)
                        _GBarduino.SetDigitalValue(35, 1)
                        If iFirstSelectNum = 1 Then  'Turn on B2
                            _GBarduino.SetDigitalValue(32, 0)
                            _GBarduino.SetDigitalValue(33, 1)
                        End If
                    Case 4
                        _GBarduino.SetDigitalValue(36, 0)
                        _GBarduino.SetDigitalValue(37, 1)
                        If iFirstSelectNum = 1 Then  'Turn on B2 and B3
                            _GBarduino.SetDigitalValue(32, 0)
                            _GBarduino.SetDigitalValue(33, 1)
                            _GBarduino.SetDigitalValue(34, 0)
                            _GBarduino.SetDigitalValue(35, 1)
                        End If
                        If iFirstSelectNum = 2 Then  'Turn on B3
                            _GBarduino.SetDigitalValue(34, 0)
                            _GBarduino.SetDigitalValue(35, 1)
                        End If
                    Case 5
                        _GBarduino.SetDigitalValue(38, 0)
                        _GBarduino.SetDigitalValue(39, 1)
                        If iFirstSelectNum = 1 Then  'Turn on B2, B3, B4
                            _GBarduino.SetDigitalValue(32, 0)
                            _GBarduino.SetDigitalValue(33, 1)
                            _GBarduino.SetDigitalValue(34, 0)
                            _GBarduino.SetDigitalValue(35, 1)
                            _GBarduino.SetDigitalValue(36, 0)
                            _GBarduino.SetDigitalValue(37, 1)
                        End If
                        If iFirstSelectNum = 2 Then  'Turn on B3, B4
                            _GBarduino.SetDigitalValue(34, 0)
                            _GBarduino.SetDigitalValue(35, 1)
                            _GBarduino.SetDigitalValue(36, 0)
                            _GBarduino.SetDigitalValue(37, 1)
                        End If
                        If iFirstSelectNum = 3 Then  'Turn on B4
                            _GBarduino.SetDigitalValue(36, 0)
                            _GBarduino.SetDigitalValue(37, 1)
                        End If
                End Select
            End If
            If strSelectRow = "C" Then
                Select Case iSecondSelectNum
                    Case 1
                        _GBarduino.SetDigitalValue(40, 0)
                        _GBarduino.SetDigitalValue(41, 1)
                    Case 2
                        _GBarduino.SetDigitalValue(42, 0)
                        _GBarduino.SetDigitalValue(43, 1)
                    Case 3
                        _GBarduino.SetDigitalValue(44, 0)
                        _GBarduino.SetDigitalValue(45, 1)
                        If iFirstSelectNum = 1 Then  'Turn on C2
                            _GBarduino.SetDigitalValue(42, 0)
                            _GBarduino.SetDigitalValue(43, 1)
                        End If
                    Case 4
                        _GBarduino.SetDigitalValue(46, 0)
                        _GBarduino.SetDigitalValue(47, 1)
                        If iFirstSelectNum = 1 Then  'Turn on C2, C3
                            _GBarduino.SetDigitalValue(42, 0)
                            _GBarduino.SetDigitalValue(43, 1)
                            _GBarduino.SetDigitalValue(44, 0)
                            _GBarduino.SetDigitalValue(45, 1)
                        End If
                        If iFirstSelectNum = 2 Then 'Turn on C3
                            _GBarduino.SetDigitalValue(44, 0)
                            _GBarduino.SetDigitalValue(45, 1)
                        End If
                    Case 5
                        _GBarduino.SetDigitalValue(48, 0)
                        _GBarduino.SetDigitalValue(49, 1)
                        If iFirstSelectNum = 1 Then  'Turn on C2, C3, C4
                            _GBarduino.SetDigitalValue(42, 0)
                            _GBarduino.SetDigitalValue(43, 1)
                            _GBarduino.SetDigitalValue(44, 0)
                            _GBarduino.SetDigitalValue(45, 1)
                            _GBarduino.SetDigitalValue(46, 0)
                            _GBarduino.SetDigitalValue(47, 1)
                        End If
                        If iFirstSelectNum = 2 Then  'Turn on C3, C4
                            _GBarduino.SetDigitalValue(44, 0)
                            _GBarduino.SetDigitalValue(45, 1)
                            _GBarduino.SetDigitalValue(46, 0)
                            _GBarduino.SetDigitalValue(47, 1)
                        End If
                        If iFirstSelectNum = 3 Then  'Turn on C4
                            _GBarduino.SetDigitalValue(46, 0)
                            _GBarduino.SetDigitalValue(47, 1)
                        End If
                    Case 6
                        _GBarduino.SetDigitalValue(50, 0)
                        _GBarduino.SetDigitalValue(51, 1)
                        If iFirstSelectNum = 1 Then  'Turn on C2, C3, C4, C5
                            _GBarduino.SetDigitalValue(42, 0)
                            _GBarduino.SetDigitalValue(43, 1)
                            _GBarduino.SetDigitalValue(44, 0)
                            _GBarduino.SetDigitalValue(45, 1)
                            _GBarduino.SetDigitalValue(46, 0)
                            _GBarduino.SetDigitalValue(47, 1)
                            _GBarduino.SetDigitalValue(48, 0)
                            _GBarduino.SetDigitalValue(49, 1)
                        End If
                        If iFirstSelectNum = 2 Then  'Turn on C3, C4, C5
                            _GBarduino.SetDigitalValue(44, 0)
                            _GBarduino.SetDigitalValue(45, 1)
                            _GBarduino.SetDigitalValue(46, 0)
                            _GBarduino.SetDigitalValue(47, 1)
                            _GBarduino.SetDigitalValue(48, 0)
                            _GBarduino.SetDigitalValue(49, 1)
                        End If
                        If iFirstSelectNum = 3 Then  'Turn on C4, C5
                            _GBarduino.SetDigitalValue(46, 0)
                            _GBarduino.SetDigitalValue(47, 1)
                            _GBarduino.SetDigitalValue(48, 0)
                            _GBarduino.SetDigitalValue(49, 1)
                        End If
                        If iFirstSelectNum = 4 Then  'Turn on C5
                            _GBarduino.SetDigitalValue(48, 0)
                            _GBarduino.SetDigitalValue(49, 1)
                        End If
                    Case 7
                        _GBarduino.SetDigitalValue(52, 0)
                        _GBarduino.SetDigitalValue(53, 1)
                        If iFirstSelectNum = 1 Then  'Turn on C2, C3, C4, C5, C6
                            _GBarduino.SetDigitalValue(42, 0)
                            _GBarduino.SetDigitalValue(43, 1)
                            _GBarduino.SetDigitalValue(44, 0)
                            _GBarduino.SetDigitalValue(45, 1)
                            _GBarduino.SetDigitalValue(46, 0)
                            _GBarduino.SetDigitalValue(47, 1)
                            _GBarduino.SetDigitalValue(48, 0)
                            _GBarduino.SetDigitalValue(49, 1)
                            _GBarduino.SetDigitalValue(50, 0)
                            _GBarduino.SetDigitalValue(51, 1)
                        End If
                        If iFirstSelectNum = 2 Then  'Turn on C3, C4, C5, C6
                            _GBarduino.SetDigitalValue(44, 0)
                            _GBarduino.SetDigitalValue(45, 1)
                            _GBarduino.SetDigitalValue(46, 0)
                            _GBarduino.SetDigitalValue(47, 1)
                            _GBarduino.SetDigitalValue(48, 0)
                            _GBarduino.SetDigitalValue(49, 1)
                            _GBarduino.SetDigitalValue(50, 0)
                            _GBarduino.SetDigitalValue(51, 1)
                        End If
                        If iFirstSelectNum = 3 Then  'Turn on C4, C5, C6
                            _GBarduino.SetDigitalValue(46, 0)
                            _GBarduino.SetDigitalValue(47, 1)
                            _GBarduino.SetDigitalValue(48, 0)
                            _GBarduino.SetDigitalValue(49, 1)
                            _GBarduino.SetDigitalValue(50, 0)
                            _GBarduino.SetDigitalValue(51, 1)
                        End If
                        If iFirstSelectNum = 4 Then  'Turn on C5, C6
                            _GBarduino.SetDigitalValue(48, 0)
                            _GBarduino.SetDigitalValue(49, 1)
                            _GBarduino.SetDigitalValue(50, 0)
                            _GBarduino.SetDigitalValue(51, 1)
                        End If
                        If iFirstSelectNum = 5 Then  'Turn on C6
                            _GBarduino.SetDigitalValue(50, 0)
                            _GBarduino.SetDigitalValue(51, 1)
                        End If
                End Select
            End If
        End If

    End Sub

    Private Sub ClearItems()
        _GBarduino.SetDigitalValue(25, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(27, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(29, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(31, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(33, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(35, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(37, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(39, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(41, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(43, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(45, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(47, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(49, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(51, 0)
        Thread.Sleep(iDelay)
        _GBarduino.SetDigitalValue(53, 0)


    End Sub

    Private Sub ResetItems(ByVal strRow As String, ByVal iStart As Integer, ByVal iEnd As Integer)
        Select Case strRow
            Case "A"
                ' 24              LED A1 Red
                ' 25              LED A1 Blue
                ' 26              LED A2 Red
                ' 27              LED A2 Blue
                ' 28              LED A3 Red
                ' 29              LED A3 Blue
                If iStart = 1 And iEnd = 1 Then
                    _GBarduino.SetDigitalValue(25, 0)
                    _GBarduino.SetDigitalValue(24, 1)
                End If
                If iStart = 2 And iEnd = 2 Then
                    _GBarduino.SetDigitalValue(27, 0)
                    _GBarduino.SetDigitalValue(26, 1)
                End If
                If iStart = 3 And iEnd = 3 Then
                    _GBarduino.SetDigitalValue(29, 0)
                    _GBarduino.SetDigitalValue(28, 1)
                End If
                If iStart = 1 And iEnd = 2 Then
                    _GBarduino.SetDigitalValue(25, 0)
                    _GBarduino.SetDigitalValue(24, 1)
                    _GBarduino.SetDigitalValue(27, 0)
                    _GBarduino.SetDigitalValue(26, 1)
                End If
                If iStart = 1 And iEnd = 3 Then
                    _GBarduino.SetDigitalValue(25, 0)
                    _GBarduino.SetDigitalValue(24, 1)
                    _GBarduino.SetDigitalValue(27, 0)
                    _GBarduino.SetDigitalValue(26, 1)
                    _GBarduino.SetDigitalValue(29, 0)
                    _GBarduino.SetDigitalValue(28, 1)
                End If
                If iStart = 2 And iEnd = 3 Then
                    _GBarduino.SetDigitalValue(27, 0)
                    _GBarduino.SetDigitalValue(26, 1)
                    _GBarduino.SetDigitalValue(29, 0)
                    _GBarduino.SetDigitalValue(28, 1)
                End If
            Case "B"
                ' 30              LED B1 Red
                ' 31              LED B1 Blue
                ' 32              LED B2 Red
                ' 33              LED B2 Blue
                ' 34              LED B3 Red
                ' 35              LED B3 Blue
                ' 36              LED B4 Red
                ' 37              LED B4 Blue
                ' 38              LED B5 Red
                ' 39              LED B5 Blue
                If iStart = 1 And iEnd = 1 Then
                    _GBarduino.SetDigitalValue(31, 0)
                    _GBarduino.SetDigitalValue(30, 1)
                End If
                If iStart = 2 And iEnd = 2 Then
                    _GBarduino.SetDigitalValue(33, 0)
                    _GBarduino.SetDigitalValue(32, 1)
                End If
                If iStart = 3 And iEnd = 3 Then
                    _GBarduino.SetDigitalValue(35, 0)
                    _GBarduino.SetDigitalValue(34, 1)
                End If
                If iStart = 4 And iEnd = 4 Then
                    _GBarduino.SetDigitalValue(37, 0)
                    _GBarduino.SetDigitalValue(36, 1)
                End If
                If iStart = 5 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(39, 0)
                    _GBarduino.SetDigitalValue(38, 1)
                End If
                If iStart = 1 And iEnd = 2 Then
                    _GBarduino.SetDigitalValue(31, 0)
                    _GBarduino.SetDigitalValue(30, 1)
                    _GBarduino.SetDigitalValue(33, 0)
                    _GBarduino.SetDigitalValue(32, 1)
                End If
                If iStart = 1 And iEnd = 3 Then
                    _GBarduino.SetDigitalValue(31, 0)
                    _GBarduino.SetDigitalValue(30, 1)
                    _GBarduino.SetDigitalValue(33, 0)
                    _GBarduino.SetDigitalValue(32, 1)
                    _GBarduino.SetDigitalValue(35, 0)
                    _GBarduino.SetDigitalValue(34, 1)
                End If
                If iStart = 1 And iEnd = 4 Then
                    _GBarduino.SetDigitalValue(31, 0)
                    _GBarduino.SetDigitalValue(30, 1)
                    _GBarduino.SetDigitalValue(33, 0)
                    _GBarduino.SetDigitalValue(32, 1)
                    _GBarduino.SetDigitalValue(35, 0)
                    _GBarduino.SetDigitalValue(34, 1)
                    _GBarduino.SetDigitalValue(37, 0)
                    _GBarduino.SetDigitalValue(36, 1)
                End If
                If iStart = 1 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(31, 0)
                    _GBarduino.SetDigitalValue(30, 1)
                    _GBarduino.SetDigitalValue(33, 0)
                    _GBarduino.SetDigitalValue(32, 1)
                    _GBarduino.SetDigitalValue(35, 0)
                    _GBarduino.SetDigitalValue(34, 1)
                    _GBarduino.SetDigitalValue(37, 0)
                    _GBarduino.SetDigitalValue(36, 1)
                    _GBarduino.SetDigitalValue(39, 0)
                    _GBarduino.SetDigitalValue(38, 1)
                End If
                If iStart = 2 And iEnd = 3 Then
                    _GBarduino.SetDigitalValue(33, 0)
                    _GBarduino.SetDigitalValue(32, 1)
                    _GBarduino.SetDigitalValue(35, 0)
                    _GBarduino.SetDigitalValue(34, 1)
                End If
                If iStart = 2 And iEnd = 4 Then
                    _GBarduino.SetDigitalValue(33, 0)
                    _GBarduino.SetDigitalValue(32, 1)
                    _GBarduino.SetDigitalValue(35, 0)
                    _GBarduino.SetDigitalValue(34, 1)
                    _GBarduino.SetDigitalValue(37, 0)
                    _GBarduino.SetDigitalValue(36, 1)
                End If
                If iStart = 2 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(33, 0)
                    _GBarduino.SetDigitalValue(32, 1)
                    _GBarduino.SetDigitalValue(35, 0)
                    _GBarduino.SetDigitalValue(34, 1)
                    _GBarduino.SetDigitalValue(37, 0)
                    _GBarduino.SetDigitalValue(36, 1)
                    _GBarduino.SetDigitalValue(39, 0)
                    _GBarduino.SetDigitalValue(38, 1)
                End If
                If iStart = 3 And iEnd = 4 Then
                    _GBarduino.SetDigitalValue(35, 0)
                    _GBarduino.SetDigitalValue(34, 1)
                    _GBarduino.SetDigitalValue(37, 0)
                    _GBarduino.SetDigitalValue(36, 1)
                End If
                If iStart = 3 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(35, 0)
                    _GBarduino.SetDigitalValue(34, 1)
                    _GBarduino.SetDigitalValue(37, 0)
                    _GBarduino.SetDigitalValue(36, 1)
                    _GBarduino.SetDigitalValue(39, 0)
                    _GBarduino.SetDigitalValue(38, 1)

                End If
                If iStart = 4 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(37, 0)
                    _GBarduino.SetDigitalValue(36, 1)
                    _GBarduino.SetDigitalValue(39, 0)
                    _GBarduino.SetDigitalValue(38, 1)

                End If
            Case "C"
                ' 40              LED C1 Red
                ' 41              LED C1 Blue
                ' 42              LED C2 Red
                ' 43              LED C2 Blue
                ' 44              LED C3 Red
                ' 45              LED C3 Blue
                ' 46              LED C4 Red
                ' 47              LED C4 Blue
                ' 48              LED C5 Red
                ' 49              LED C5 Blue
                ' 50              LED C6 Red
                ' 51              LED C6 Blue
                ' 52              LED C7 Red
                ' 53              LED C7 Blue

                If iStart = 1 And iEnd = 1 Then
                    _GBarduino.SetDigitalValue(41, 0)
                    _GBarduino.SetDigitalValue(40, 1)
                End If
                If iStart = 2 And iEnd = 2 Then
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                End If
                If iStart = 3 And iEnd = 3 Then
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                End If
                If iStart = 4 And iEnd = 4 Then
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                End If
                If iStart = 5 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                End If
                If iStart = 6 And iEnd = 6 Then
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                End If
                If iStart = 7 And iEnd = 7 Then
                    _GBarduino.SetDigitalValue(53, 0)
                    _GBarduino.SetDigitalValue(52, 1)
                End If
                If iStart = 1 And iEnd = 2 Then
                    _GBarduino.SetDigitalValue(41, 0)
                    _GBarduino.SetDigitalValue(40, 1)
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                End If
                If iStart = 1 And iEnd = 3 Then
                    _GBarduino.SetDigitalValue(41, 0)
                    _GBarduino.SetDigitalValue(40, 1)
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                End If
                If iStart = 1 And iEnd = 4 Then
                    _GBarduino.SetDigitalValue(41, 0)
                    _GBarduino.SetDigitalValue(40, 1)
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                End If
                If iStart = 1 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(41, 0)
                    _GBarduino.SetDigitalValue(40, 1)
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                End If
                If iStart = 1 And iEnd = 6 Then
                    _GBarduino.SetDigitalValue(41, 0)
                    _GBarduino.SetDigitalValue(40, 1)
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                End If
                If iStart = 1 And iEnd = 7 Then
                    _GBarduino.SetDigitalValue(41, 0)
                    _GBarduino.SetDigitalValue(40, 1)
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                    _GBarduino.SetDigitalValue(53, 0)
                    _GBarduino.SetDigitalValue(52, 1)
                End If
                If iStart = 2 And iEnd = 3 Then
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                End If
                If iStart = 2 And iEnd = 4 Then
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                End If
                If iStart = 2 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                End If
                If iStart = 2 And iEnd = 6 Then
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                End If
                If iStart = 2 And iEnd = 7 Then
                    _GBarduino.SetDigitalValue(43, 0)
                    _GBarduino.SetDigitalValue(42, 1)
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                    _GBarduino.SetDigitalValue(53, 0)
                    _GBarduino.SetDigitalValue(52, 1)
                End If
                If iStart = 3 And iEnd = 4 Then
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                End If
                If iStart = 3 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                End If
                If iStart = 3 And iEnd = 6 Then
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                End If
                If iStart = 3 And iEnd = 7 Then
                    _GBarduino.SetDigitalValue(45, 0)
                    _GBarduino.SetDigitalValue(44, 1)
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                    _GBarduino.SetDigitalValue(53, 0)
                    _GBarduino.SetDigitalValue(52, 1)
                End If
                If iStart = 4 And iEnd = 5 Then
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                End If
                If iStart = 4 And iEnd = 6 Then
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                End If
                If iStart = 4 And iEnd = 7 Then
                    _GBarduino.SetDigitalValue(47, 0)
                    _GBarduino.SetDigitalValue(46, 1)
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                    _GBarduino.SetDigitalValue(53, 0)
                    _GBarduino.SetDigitalValue(52, 1)
                End If
                If iStart = 5 And iEnd = 6 Then
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                End If
                If iStart = 5 And iEnd = 7 Then
                    _GBarduino.SetDigitalValue(49, 0)
                    _GBarduino.SetDigitalValue(48, 1)
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                    _GBarduino.SetDigitalValue(53, 0)
                    _GBarduino.SetDigitalValue(52, 1)
                End If
                If iStart = 6 And iEnd = 7 Then
                    _GBarduino.SetDigitalValue(51, 0)
                    _GBarduino.SetDigitalValue(50, 1)
                    _GBarduino.SetDigitalValue(53, 0)
                    _GBarduino.SetDigitalValue(52, 1)
                End If
        End Select


    End Sub

#End Region

#Region "Matrix"
    Private Sub LoadMatrix(iMatrix As Integer)
        'If nothing to load, then initialize
        Dim fs1, fs2 As FileStream

        Try
            Select Case iMatrix
                Case 0
                    fs1 = New FileStream("Matrixa_0.dat", FileMode.Open)
                    fs2 = New FileStream("Matrixb_0.dat", FileMode.Open)
                    lblMatrixStatus.Text = "Matrix 0 Loaded"
                Case 75
                    fs1 = New FileStream("Matrixa_75.dat", FileMode.Open)
                    fs2 = New FileStream("Matrixb_75.dat", FileMode.Open)
                    lblMatrixStatus.Text = "Matrix 75 Loaded"
                Case 150
                    fs1 = New FileStream("Matrixa_150.dat", FileMode.Open)
                    fs2 = New FileStream("Matrixb_150.dat", FileMode.Open)
                    lblMatrixStatus.Text = "Matrix 150 Loaded"
                Case 300
                    fs1 = New FileStream("Matrixa_Custom.dat", FileMode.Open)
                    fs2 = New FileStream("Matrixb_Custom.dat", FileMode.Open)
                    lblMatrixStatus.Text = "Custom Matrix Loaded"
                Case Else
                    fs1 = New FileStream("Matrixa_0.dat", FileMode.Open)
                    fs2 = New FileStream("Matrixb_0.dat", FileMode.Open)
                    lblMatrixStatus.Text = "Matrix 0 Loaded"
            End Select

            Dim formatter As New BinaryFormatter

            iMatrix1 = formatter.Deserialize(fs1)
            iMatrix2 = formatter.Deserialize(fs2)

            fs1.Close()
            fs2.Close()

            lblLearningOps.Text = CountLearningOps.ToString

        Catch ex As Exception
            Console.WriteLine("Failed to deserialize. Reason: " & ex.Message)
            InitializeMatrix()
            lblMatrixStatus.Text = "Error:  Matrix 0 Loaded"
            'Throw
        End Try

    End Sub

    Private Sub SaveMatrix()
        Dim fs1, fs2 As FileStream

        Try
            fs1 = New FileStream("Matrixa_Custom.dat", FileMode.Create)
            fs2 = New FileStream("Matrixb_Custom.dat", FileMode.Create)
            Dim formatter As New BinaryFormatter
            formatter.Serialize(fs1, iMatrix1)
            formatter.Serialize(fs2, iMatrix2)
            fs1.Close()
            fs2.Close()
        Catch ex As Exception
            Console.WriteLine("failed to serialize. reason: " & ex.Message)
            'throw
        End Try
    End Sub

    Private Sub InitializeMatrix()
        Dim a, b, c, d, e, f, g, h As Integer

        If Not File.Exists("Matrix1a.dat") Then
            For a = 0 To 14
                For b = 0 To 12
                    For c = 0 To 10
                        For d = 0 To 8
                            For e = 0 To 6
                                For f = 0 To 4
                                    For g = 0 To 2
                                        For h = 0 To 0
                                            iMatrix1(a, b, c, d, e, f, g, h) = CByte("0")
                                        Next
                                    Next
                                Next
                            Next
                        Next
                    Next
                Next
            Next
        End If
        If Not File.Exists("Matrix1b.dat") Then
            For a = 0 To 13
                For b = 0 To 11
                    For c = 0 To 9
                        For d = 0 To 7
                            For e = 0 To 5
                                For f = 0 To 3
                                    For g = 0 To 1
                                        iMatrix2(a, b, c, d, e, f, g) = CByte("0")
                                    Next
                                Next
                            Next
                        Next
                    Next
                Next
            Next
        End If
        'SaveMatrix()
    End Sub

    Private Function CheckMatrix1() As Boolean
        Dim a As Integer
        'b, c, d, e, f, g As Integer

        'If all first moves are bad, then return True
        For a = 1 To 14
            'For b = 1 To 12
            '    For c = 1 To 10
            '        For d = 1 To 8
            '            For e = 1 To 6
            '                For f = 1 To 4
            '                    For g = 1 To 2
            If iMatrix1(a, 0, 0, 0, 0, 0, 0, 0) = "0" Then
                Return (False)
            End If
        Next
        '                    Next
        '                Next
        '            Next
        '        Next
        '    Next
        'Next

        Return (True)

    End Function

    Private Function CountLearningOps() As Integer
        Dim a, b, c, d, e, f, g, h As Integer
        'Dim i, j, k, l, m As Integer
        Dim iCounter As Integer = 0


        For a = 0 To 14
                For b = 0 To 12
                    For c = 0 To 10
                        For d = 0 To 8
                            For e = 0 To 6
                                For f = 0 To 4
                                    For g = 0 To 2
                                        For h = 0 To 0
                                        If iMatrix1(a, b, c, d, e, f, g, h) = CByte("1") Then
                                            iCounter = iCounter + 1
                                        End If
                                    Next
                                    Next
                                Next
                            Next
                        Next
                    Next
                Next
            Next


            For a = 0 To 13
                For b = 0 To 11
                    For c = 0 To 9
                        For d = 0 To 7
                            For e = 0 To 5
                                For f = 0 To 3
                                    For g = 0 To 1
                                    If iMatrix2(a, b, c, d, e, f, g) = CByte("1") Then
                                        iCounter = iCounter + 1
                                    End If
                                Next
                                Next
                            Next
                        Next
                    Next
                Next
            Next


        Return (iCounter)

    End Function
    Private Function CheckMatrix2() As Boolean
        Dim a As Integer
        ', b, c, d, e, f As Integer

        'If all first moves are bad, then return True
        For a = 1 To 13
            'For b = 1 To 11
            '    For c = 1 To 9
            '        For d = 1 To 7
            '            For e = 1 To 5
            '                For f = 1 To 3
            If iMatrix2(a, 0, 0, 0, 0, 0, 0) = "0" Then
                Return (False)
            End If
        Next
        '            Next
        '            Next
        '        Next
        '    Next
        'Next
        Return (True)

    End Function
#End Region

End Class
