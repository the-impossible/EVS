Imports DAL = Neuro.Data.DataAccessLayer
Public Class StudentManager
    Public Shared Function InsertStudent(ByVal regNo As String, ByVal name As String, ByVal phone As String, ByVal level As String, ByVal fingerPrint1 As String, ByVal fingerPrint2 As String, ByVal img As Byte()) As Double
        Try
            DAL.openConnection()
            DAL.BeginTransaction()
            DAL.ProcedureName = "msp_Insert_Student"
            DAL.createProcedureCommand()
            DAL.Parameters("@RegNo", regNo)
            DAL.Parameters("@Name", name)
            DAL.Parameters("@Phone", phone)
            DAL.Parameters("@Level", level)
            DAL.Parameters("@FingerPrint1", fingerPrint1)
            DAL.Parameters("@FingerPrint2", fingerPrint2)
            DAL.Parameters("@StudentImage", img)
            InsertStudent = DAL.ExecuteNonQuery()
            DAL.commitTransaction()

        Catch ex As Exception
            DAL.rollbackTransaction()
            Throw New Exception(ex.Message)
        Finally
            DAL.closeConnection()
            DAL.disposeConnection()
        End Try
        Return InsertStudent
    End Function
End Class
