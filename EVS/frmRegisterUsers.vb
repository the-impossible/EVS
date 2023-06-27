Imports System.Transactions
Imports Neuro.Business

Public Class frmRegisterUsers

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        frmMain.Show()
        frmMain.Activate()

        Me.Close()

    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Me.txtPassword.Text = Nothing
        Me.txtUsername.Text = Nothing
        Me.cmbType.Text = Nothing

    End Sub

    Private Sub btnAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAll.Click
        frmAllUsers.Show()
        frmAllUsers.Activate()
        Me.Close()

    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If String.IsNullOrWhiteSpace(Me.txtPassword.Text) Then Me.txtPassword.Focus() : Return
        If String.IsNullOrWhiteSpace(Me.txtUsername.Text) Then Me.txtUsername.Focus() : Return

        Dim type As String = Me.cmbType.Text.Trim
        If type Is Nothing Then Me.cmbType.Focus() : Return

        If type <> "Admin" AndAlso type <> "verify" Then
            Me.cmbType.Focus() : Return
        End If

        Try
            Using scope As New TransactionScope(TransactionScopeOption.Required)

                StudentManager.InsertUsers(txtUsername.Text, txtPassword.Text, type)
                scope.Complete()

            End Using
            MsgBox("User registered successfully", MsgBoxStyle.Information, "Saved Successfully")
            btnReset_Click(sender, e)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error")
        End Try

    End Sub
End Class