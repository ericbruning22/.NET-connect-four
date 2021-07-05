'Eric Bruning
'April 29, 2017
'Project 2 Operating Systems (Final Project)\
'Connect Four Game

Imports System.Threading
Imports System.Net
Imports System.Net.Sockets
Imports System.IO

Public Class Child

    'initalize variables
    Inherits System.Windows.Forms.Form
    Dim serverSocket As TcpListener
    Dim clientSocket As TcpClient
    Dim infiniteCounter As Integer
    Dim counter As Integer
    Dim clientList As New List(Of handleClient)
    Dim pReader As StreamReader
    Dim pClient As handleClient
    Private myThread As Thread
    Dim player1 As Boolean = False
    Dim player2 As Boolean = False
    Dim play1turn As Boolean = False
    Dim play2turn As Boolean = False
    Dim playnum1 As String
    Dim playnum2 As String
    Dim red As New List(Of String)
    Dim black As New List(Of String)
    Dim i As Integer = 0
    Dim j As Integer = 0
    Dim replay As Boolean = False
    Dim win As Boolean = False

    Private Sub Child_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'start server
        serverSocket = New TcpListener(Form1.Port)
        serverSocket.Start()

        'display message that server has started
        UpdateList("Chat Server Started on " & Form1.Port, True)

        'start accepting clients
        serverSocket.BeginAcceptTcpClient(New AsyncCallback(AddressOf AcceptClient), serverSocket)
    End Sub

    ' create a delegate
    Delegate Sub _cUpdate(ByVal str As String, ByVal relay As Boolean)

    'update the message box 
    Sub UpdateList(ByVal str As String, ByVal relay As Boolean)
        On Error Resume Next
        If InvokeRequired Then
            Invoke(New _cUpdate(AddressOf UpdateList), str, relay)
        Else
            TextBox1.AppendText(str & vbNewLine)
            ' if relay we will send a string
            If relay Then
                send(str)
            End If
        End If
    End Sub

    'notify client if they joined and if others have also
    Sub send(ByVal str As String)
        For x As Integer = 0 To clientList.Count - 1
            Try
                clientList(x).Send(str)
            Catch ex As Exception
                clientList.RemoveAt(x)
            End Try
        Next
    End Sub

    'accept clients to server add handler to know when clients send messages or disconnect from the server
    Sub AcceptClient(ByVal ar As IAsyncResult)
        pClient = New handleClient(serverSocket.EndAcceptTcpClient(ar))
        AddHandler(pClient.getMessage), AddressOf MessageReceived
        AddHandler(pClient.clientLogout), AddressOf ClientExited
        clientList.Add(pClient)

        'determine which player is trying to connect
        If player1 = False Then
            player1 = True
            UpdateList("Player 1 Joined!", True)
        ElseIf player2 = False Then
            player2 = True
            play1turn = True
            UpdateList("Player 2 Joined!", True)
            send("Player 1 goes first")
        Else
            UpdateList("New Client Joined", True)
        End If
        serverSocket.BeginAcceptTcpClient(New AsyncCallback(AddressOf AcceptClient), serverSocket)
    End Sub

    'update message box when client sends message to server
    Sub MessageReceived(ByVal str As String)
        If str.Contains("Player 1 ID:") Then
            playnum1 = str.Substring(13)
        ElseIf str.Contains("Player 2 ID:") Then
            playnum2 = str.Substring(13)
        ElseIf play1turn = False And play2turn = False Then
            send("Wait until both players are connected")
        ElseIf str.Contains("replay") Then
            'restart game
            If replay = False Then
                replay = True
                play1turn = True
                play2turn = False
                player1 = True
                i = 0
                j = 0
            Else
                replay = False
                player2 = True
                send("Player 1 goes first")
            End If
        Else
            Dim sentence As String() = str.Split(New Char() {" "c})
            Dim clientId As String = sentence(1)

            'if player 1 clicks a box
            If clientId.Equals(playnum1) And play1turn = True Then
                UpdateList("Player 1 clicked box: " & sentence(4) & " , " & sentence(6), True)
                red.Add(sentence(4) & sentence(6))

                win = False
                Dim wincount = 0

                While win = False
                    'check to see if player wins horizontally right
                    For z As Integer = 0 To red.Count - 1
                        If red(z).Chars(0).Equals(red(i).Chars(0)) Then
                            If Convert.ToInt32(red(z).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) + 1 Or Convert.ToInt32(red(z).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) + 2 Or Convert.ToInt32(red(z).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) + 3 Or Convert.ToInt32(red(z).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 1 Or Convert.ToInt32(red(z).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 2 Or Convert.ToInt32(red(z).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 3 Then
                                wincount = wincount + 1
                                If wincount = 3 Then
                                    UpdateList("Player 1 Wins!!!!", True)
                                    z = red.Count
                                    red.Clear()
                                    black.Clear()
                                    win = True
                                End If
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins horizontally left
                    For z As Integer = 0 To red.Count - 1
                        If red(z).Chars(0).Equals(red(i).Chars(0)) Then
                            If Convert.ToInt32(red(z).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 1 Or Convert.ToInt32(red(z).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 2 Or Convert.ToInt32(red(z).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 3 Then
                                wincount = wincount + 1
                                If wincount = 3 Then
                                    UpdateList("Player 1 Wins!!!!", True)
                                    z = red.Count
                                    red.Clear()
                                    black.Clear()
                                    win = True
                                End If
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins vertically down
                    For y As Integer = 0 To red.Count - 1
                        If red(y).Chars(1).Equals(red(i).Chars(1)) Then
                            If Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) + 1 Or Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) + 2 Or Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) + 3 Or Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 1 Or Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 2 Or Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 3 Then
                                wincount = wincount + 1
                                If wincount = 3 Then
                                    UpdateList("Player 1 Wins!!!!", True)
                                    y = red.Count
                                    red.Clear()
                                    black.Clear()
                                    win = True
                                End If
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins vertically up
                    For y As Integer = 0 To red.Count - 1
                        If red(y).Chars(1).Equals(red(i).Chars(1)) Then
                            If Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 1 Or Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 2 Or Convert.ToInt32(red(y).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 3 Then
                                wincount = wincount + 1
                                If wincount = 3 Then
                                    UpdateList("Player 1 Wins!!!!", True)
                                    y = red.Count
                                    red.Clear()
                                    black.Clear()
                                    win = True
                                End If
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins diagonally down left
                    For x As Integer = 0 To red.Count - 1
                        If (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) + 1 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 1) Or (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) + 2 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 2) Or (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) + 3 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 3) Then
                            wincount = wincount + 1
                            If wincount = 3 Then
                                UpdateList("Player 1 Wins!!!!", True)
                                x = red.Count
                                red.Clear()
                                black.Clear()
                                win = True
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins diagonally up right
                    For x As Integer = 0 To red.Count - 1
                        If (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 1 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) + 1) Or (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 2 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) + 2) Or (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 3 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) + 3) Then
                            wincount = wincount + 1
                            If wincount = 3 Then
                                UpdateList("Player 1 Wins!!!!", True)
                                x = red.Count
                                red.Clear()
                                black.Clear()
                                win = True
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins diagonally up left
                    For x As Integer = 0 To red.Count - 1
                        If (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 1 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 1) Or (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 2 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 2) Or (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) - 3 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) - 3) Then
                            wincount = wincount + 1
                            If wincount = 3 Then
                                UpdateList("Player 1 Wins!!!!", True)
                                x = red.Count
                                red.Clear()
                                black.Clear()
                                win = True
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins diagonally down right
                    For x As Integer = 0 To red.Count - 1
                        If (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) + 1 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) + 1) Or (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) + 2 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) + 2) Or (Convert.ToInt32(red(x).Chars(0)) = Convert.ToInt32(red(i).Chars(0)) + 3 And Convert.ToInt32(red(x).Chars(1)) = Convert.ToInt32(red(i).Chars(1)) + 3) Then
                            wincount = wincount + 1
                            If wincount = 3 Then
                                UpdateList("Player 1 Wins!!!!", True)
                                x = red.Count
                                red.Clear()
                                black.Clear()
                                win = True
                            End If
                        End If
                    Next

                    'if player doesnt win change turns
                    If win = False Then
                        i = i + 1
                        play2turn = True
                        play1turn = False
                        win = True
                    End If
                End While
                'if player 2 clicks a box
            ElseIf clientId.Equals(playnum2) And play2turn = True Then
                UpdateList("Player 2 clicked box: " & sentence(4) & " , " & sentence(6), True)
                black.Add(sentence(4) & sentence(6))

                Dim wincount = 0
                win = False
                While win = False
                    'check to see if player wins horizontally right
                    For z As Integer = 0 To black.Count - 1
                        If black(z).Chars(0).Equals(black(j).Chars(0)) Then
                            If Convert.ToInt32(black(z).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) + 1 Or Convert.ToInt32(black(z).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) + 2 Or Convert.ToInt32(black(z).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) + 3 Or Convert.ToInt32(black(z).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 1 Or Convert.ToInt32(black(z).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 2 Or Convert.ToInt32(black(z).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 3 Then
                                wincount = wincount + 1
                                If wincount = 3 Then
                                    UpdateList("Player 2 Wins!!!!", True)
                                    z = black.Count
                                    red.Clear()
                                    black.Clear()
                                    win = True
                                End If
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins horizontally left
                    For z As Integer = 0 To black.Count - 1
                        If black(z).Chars(0).Equals(black(j).Chars(0)) Then
                            If Convert.ToInt32(black(z).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 1 Or Convert.ToInt32(black(z).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 2 Or Convert.ToInt32(black(z).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 3 Then
                                wincount = wincount + 1
                                If wincount = 3 Then
                                    UpdateList("Player 2 Wins!!!!", True)
                                    z = black.Count
                                    red.Clear()
                                    black.Clear()
                                    win = True
                                End If
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins vertically down
                    For y As Integer = 0 To black.Count - 1
                        If black(y).Chars(1).Equals(black(j).Chars(1)) Then
                            If Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) + 1 Or Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) + 2 Or Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) + 3 Or Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 1 Or Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 2 Or Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 3 Then
                                wincount = wincount + 1
                                If wincount = 3 Then
                                    UpdateList("Player 2 Wins!!!!", True)
                                    y = black.Count
                                    red.Clear()
                                    black.Clear()
                                    win = True
                                End If
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins vertically up
                    For y As Integer = 0 To black.Count - 1
                        If black(y).Chars(1).Equals(black(j).Chars(1)) Then
                            If Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 1 Or Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 2 Or Convert.ToInt32(black(y).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 3 Then
                                wincount = wincount + 1
                                If wincount = 3 Then
                                    UpdateList("Player 2 Wins!!!!", True)
                                    y = black.Count
                                    red.Clear()
                                    black.Clear()
                                    win = True
                                End If
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins diagonally down left
                    For x As Integer = 0 To black.Count - 1
                        If (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) + 1 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 1) Or (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) + 2 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 2) Or (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) + 3 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 3) Then
                            wincount = wincount + 1
                            If wincount = 3 Then
                                UpdateList("Player 2 Wins!!!!", True)
                                x = black.Count
                                red.Clear()
                                black.Clear()
                                win = True
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins diagonally up right
                    For x As Integer = 0 To black.Count - 1
                        If (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 1 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) + 1) Or (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 2 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) + 2) Or (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 3 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) + 3) Then
                            wincount = wincount + 1
                            If wincount = 3 Then
                                UpdateList("Player 2 Wins!!!!", True)
                                x = black.Count
                                red.Clear()
                                black.Clear()
                                win = True
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins diagonally up left
                    For x As Integer = 0 To black.Count - 1
                        If (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 1 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 1) Or (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 2 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 2) Or (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) - 3 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) - 3) Then
                            wincount = wincount + 1
                            If wincount = 3 Then
                                UpdateList("Player 2 Wins!!!!", True)
                                x = black.Count
                                red.Clear()
                                black.Clear()
                                win = True
                            End If
                        End If
                    Next

                    wincount = 0
                    'check to see if player wins diagonally down right
                    For x As Integer = 0 To black.Count - 1
                        If (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) + 1 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) + 1) Or (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) + 2 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) + 2) Or (Convert.ToInt32(black(x).Chars(0)) = Convert.ToInt32(black(j).Chars(0)) + 3 And Convert.ToInt32(black(x).Chars(1)) = Convert.ToInt32(black(j).Chars(1)) + 3) Then
                            wincount = wincount + 1
                            If wincount = 3 Then
                                UpdateList("Player 2 Wins!!!!", True)
                                x = black.Count
                                red.Clear()
                                black.Clear()
                                win = True
                            End If
                        End If
                    Next

                    'if player doesnt win change turns
                    If win = False Then
                        j = j + 1
                        play2turn = False
                        play1turn = True
                        win = True
                    End If
                End While

            Else
                'if players click out of turn
                If clientId.Equals(playnum1) Then
                    send("Player 1 clicked out of turn")
                Else
                    send("Player 2 clicked out of turn")
                End If
            End If
        End If
    End Sub

    'remove client from server
    Sub ClientExited(ByVal client As handleClient)
        clientList.Remove(client)
        UpdateList("Player exited game", True)
    End Sub

    'close server window
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class