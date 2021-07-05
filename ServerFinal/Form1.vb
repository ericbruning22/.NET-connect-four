'Eric Bruning
'April 29, 2017
'Project 2 Operating Systems (Final Project)\
'Connect Four Game

Public Class Form1
    Public Port As String

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'get port number from textbox
        Port = TextBox1.Text

        'open child window and close previous window
        Child.Show()
        Me.Close()
    End Sub

    'if cancel button is clicked
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    'set port number in textbox when window opens
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = "80"
    End Sub
End Class
