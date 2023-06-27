Imports Neuro.Business

Public Class frmModify

    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub frmModify_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'EVSDataSet.Students' table. You can move, or remove it, as needed.
        Me.StudentsTableAdapter.Fill(Me.EVSDataSet.Students)

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click

        frmMain.Show()
        frmMain.Activate()
        Me.Close()
    End Sub

    Private Sub btnDeleteStudent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteStudent.Click
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim i As Integer = DataGridView1.SelectedRows(0).Index
            If i >= 0 And i < DataGridView1.Rows.Count Then

                Dim rowIndex As Integer = DataGridView1.CurrentCell.RowIndex
                Dim columnIndex As Integer = DataGridView1.CurrentCell.ColumnIndex
                VerifyManager.deleteStudent(DataGridView1.Rows([rowIndex]).Cells([columnIndex]).Value.ToString())
                MsgBox("Student Deleted", MsgBoxStyle.Exclamation, "Info")
                DataGridView1.Rows.RemoveAt(i)
            End If
        End If
    End Sub
End Class