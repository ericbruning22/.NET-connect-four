'Eric Bruning
'April 29, 2017
'Project 2 Operating Systems (Final Project)\
'Connect Four Game

Imports System.Net
Imports System.IO
Imports System.Net.Sockets

Public Class Form1
    'initialize variables
    Dim client As TcpClient
    Dim serverStream As NetworkStream
    Dim sWriter As StreamWriter
    Dim readData As String
    Dim infiniteCounter As Integer
    Dim NIckFrefix As Integer = New Random().Next(1111, 9999)
    Dim player1turn As Boolean
    Dim player2turn As Boolean
    Dim player1 As Boolean = False
    Dim player2 As Boolean = False

    'initialize grid
    Public Box(9, 14) As PictureBox
    Public x_coord = 52
    Public y_coord = 130
    Public x_begin = 52
    Public y_begin = 130

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'set ip address and port when window opens
        TextBox1.Text = "localhost"
        TextBox2.Text = "80"
    End Sub

    'build grid for game
    Public Sub BuildMap()
        Dim i As Integer = 0
        For row = 0 To 9
            For column = 0 To 14
                drawBox(row, column)
            Next
        Next
    End Sub

    'draw each box 
    Public Sub drawBox(row As Integer, column As Integer)
        'use picture boxes to make grid and place in specific location to fit window
        Box(row, column) = New PictureBox
        Box(row, column).Size = New Point(50, 50)
        Box(row, column).BorderStyle = BorderStyle.FixedSingle
        Box(row, column).BackColor = Color.White

        'space out boxes
        If column = 14 Then
            Box(row, column).Location = New Point((x_begin), (y_coord))
            y_coord = y_coord + 50
            x_coord = 52
        Else
            Box(row, column).Location = New Point((x_coord), (y_coord))
            x_coord = x_coord + 50
        End If

        'add handler for clicking purposes
        AddHandler Box(row, column).Click, AddressOf Box_Click
        Me.Controls.Add(Box(row, column))
    End Sub

    'if a box is clicked
    Private Sub Box_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim pic As PictureBox = DirectCast(sender, PictureBox)

        'find out which box was clicked on
        For x = 0 To 9
            For y = 0 To 14
                'send click to server
                If sender Is Box(x, y) Then
                    send("Player " & NIckFrefix & " clicked box: " & x & " , " & y)
                    y = 14
                    x = 9
                End If
            Next
        Next
    End Sub

    'when send button is clicked, send message to server and clear message textbox
    'Private Sub Button2_Click(ByVal sender As System.Object,
    '   ByVal e As System.EventArgs) Handles Button2.Click
    '  send("Player " & NIckFrefix & " says : " & TextBox4.Text)
    ' TextBox4.Clear()
    'End Sub

    'if connect button is clicked
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'try to connect to server by using the ip and port number and if successful disable connect button and build grid
        Try
            client = New TcpClient(TextBox1.Text, CInt(TextBox2.Text))
            client.GetStream.BeginRead(New Byte() {0}, 0, 0, New AsyncCallback(AddressOf read), Nothing)
            Button1.Enabled = False
            BuildMap()
        Catch ex As Exception
            xUpdate("Can't connect to the server!")
        End Try
    End Sub

    'update message box
    Delegate Sub _xUpdate(ByVal str As String)
    Sub xUpdate(ByVal str As String)
        If InvokeRequired Then
            Invoke(New _xUpdate(AddressOf xUpdate), str)
        Else
            TextBox3.AppendText(str & vbNewLine)
        End If
    End Sub

    'read messages being sent to client from server
    Sub read(ByVal ar As IAsyncResult)
        Try
            'read message
            Dim hold As String = New StreamReader(client.GetStream).ReadLine

            'if player 1 joins
            If hold.Contains("Player 1 Joined") Then
                xUpdate(hold)
                player1 = True

                'tell server player 1's ID
                send("Player 1 ID: " & NIckFrefix)

                'if player 2 joins
            ElseIf hold.Contains("Player 2 Joined") Then
                xUpdate(hold)
                player2 = True

                'tell server player 2's ID
                send("Player 2 ID: " & NIckFrefix)

                'if a player clicks on a box and it is their turn
            ElseIf hold.Contains("box") Then

                'get client ID to know which picture to place in the box
                Dim sentence As String() = hold.Split(New Char() {" "c})
                Dim clientId As String = sentence(1)

                'change picture to a red dot if player 1 clicked a box
                If clientId.Equals("1") Then
                    xUpdate(hold)
                    Dim x As Integer = Convert.ToInt32(sentence(4))
                    Dim y As Integer = Convert.ToInt32(sentence(6))
                    Box(x, y).Image = Image.FromFile("E:/Final Project/Source Code/ClientFinal/Red.png")
                    Box(x, y).SizeMode = PictureBoxSizeMode.StretchImage
                    RemoveHandler Box(x, y).Click, AddressOf Me.Box_Click
                    xUpdate("Player 2's turn")

                    'change picutre to a red dot if player 2 clicked a box
                ElseIf clientId.Equals("2") Then
                    xUpdate(hold)
                    Dim x As Integer = Convert.ToInt32(sentence(4))
                    Dim y As Integer = Convert.ToInt32(sentence(6))
                    Box(x, y).Image = Image.FromFile("E:/Final Project/Source Code/ClientFinal/Black.png")
                    Box(x, y).SizeMode = PictureBoxSizeMode.StretchImage
                    RemoveHandler Box(x, y).Click, AddressOf Me.Box_Click
                    xUpdate("Player 1's turn")
                End If

                'if a player wins
            ElseIf hold.Contains("Wins") Then

                'get clientID of winner
                Dim sentence As String() = hold.Split(New Char() {" "c})
                Dim clientId As String = sentence(1)
                Dim replay As Integer

                'display popup window asking if they want to play again
                replay = MsgBox(hold & "  - Want to play again?", MsgBoxStyle.YesNo Or MsgBoxStyle.DefaultButton1, "The winner is:")

                'if player wants to play again
                If replay = 6 Then
                    TextBox3.Clear()
                    xUpdate("Wait for both players to connect")
                    clearBoard()
                    send("replay")
                    replay = 0

                    'if player does not want to play again, close window and disconnect from server
                Else
                    Me.Close()
                End If

                'if server sends any other message to client
            Else
                xUpdate(hold)
            End If
            client.GetStream.BeginRead(New Byte() {0}, 0, 0, AddressOf read, Nothing)

            'if server disconnects
        Catch ex As Exception
            xUpdate("You have disconnected from server")
            Exit Sub
        End Try
    End Sub

    'clear board to replay game
    Private Sub clearBoard()
        Dim i As Integer = 0
        For row = 0 To 9
            For column = 0 To 14
                'if box contains a red or black dot, remove the picture and add a handler to the box again
                If Not Box(row, column).Image Is Nothing Then
                    Box(row, column).Image = Nothing

                    AddHandler Box(row, column).Click, AddressOf Box_Click
                    Me.Controls.Add(Box(row, column))
                End If
            Next
        Next
    End Sub

    'send messages to server
    Private Sub send(ByVal str As String)
        Try
            sWriter = New StreamWriter(client.GetStream)
            sWriter.WriteLine(str)
            sWriter.Flush()

            'if you are not connected to server
        Catch ex As Exception
            xUpdate("You're not connected to the server")
        End Try
    End Sub
End Class
