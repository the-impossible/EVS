
Imports DAL = Neuro.Data.DataAccessLayer


Public Class LoginManager

    Public Shared Function getLoginDetailByUserNamePassword(ByVal userName As String, ByVal password As String) As DataSet
        Dim ds As DataSet = Nothing
        Try
            DAL.openConnection()
            DAL.ProcedureName = "mSP_GetUserLoginDetail"
            DAL.createProcedureCommand()
            DAL.Parameters("@UserName", userName)
            DAL.Parameters("@Password", password)
            Dim da As SqlClient.SqlDataAdapter = DAL.createDataAdapter
            ds = New DataSet
            da.Fill(ds)

        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
            DAL.closeConnection()
            DAL.disposeConnection()
        End Try
        Return ds
    End Function
End Class
