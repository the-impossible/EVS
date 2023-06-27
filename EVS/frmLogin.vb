Imports Neuro.Business
Public Class frmLogin
    Dim ds As DataSet


    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        End
    End Sub

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If String.IsNullOrWhiteSpace(Me.txtUsername.Text) Then Me.txtUsername.Focus() : Exit Sub
        If String.IsNullOrWhiteSpace(Me.txtPassword.Text) Then Me.txtPassword.Focus() : Exit Sub

        Try
            ds = LoginManager.getLoginDetailByUserNamePassword(Me.txtUsername.Text.Trim, Me.txtPassword.Text)
            If ds.Tables(0).Rows.Count > 0 Then
                Neuro.Data.DataAccessLayer.loggedInUser = ds.Tables(0).Rows(0)("UserName")
                Neuro.Data.DataAccessLayer.loggedInUserType = ds.Tables(0).Rows(0)("UserType")

                If Neuro.Data.DataAccessLayer.loggedInUserType.Trim = "Admin" Then
                    frmMain.Show()
                    frmMain.Activate()
                    Me.Close()
                Else
                    frmMainInvigilator.Show()
                    frmMainInvigilator.Activate()
                    Me.Close()
                End If
            Else
                MsgBox("Invalid login credentials", MsgBoxStyle.Exclamation, "Invalid")
                Me.txtUsername.Focus()


            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class