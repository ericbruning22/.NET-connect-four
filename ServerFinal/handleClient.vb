'Eric Bruning
'April 29, 2017
'Project 2 Operating Systems (Final Project)\
'Connect Four Game

Imports System.Net.Sockets
Imports System.IO
Public Class handleClient

    'handle the events of getting messages from  clients and disconnecting clients
    Public Event getMessage(ByVal str As String)
    Public Event clientLogout(ByVal client As handleClient)
    Private sendMessage As StreamWriter
    Private listClient As TcpClient
    Dim playernum As Integer

    'assign client id
    Sub New(ByVal forClient As TcpClient)
        listClient = forClient
        listClient.GetStream.BeginRead(New Byte() {0}, 0, 0, AddressOf ReadAllClient, Nothing)
    End Sub

    'read client message
    Private Sub ReadAllClient()
        Try
            RaiseEvent getMessage(New StreamReader(listClient.GetStream).ReadLine)
            listClient.GetStream.BeginRead(New Byte() {0}, 0, 0, New AsyncCallback(AddressOf ReadAllClient), Nothing)
        Catch ex As Exception
            RaiseEvent clientLogout(Me)
        End Try
    End Sub

    'send message back to clients 
    Public Sub Send(ByVal Messsage As String)
        sendMessage = New StreamWriter(listClient.GetStream)
        sendMessage.WriteLine(Messsage)
        sendMessage.Flush()
    End Sub
End Class
