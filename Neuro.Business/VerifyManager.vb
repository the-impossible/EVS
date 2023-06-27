Imports DAL = Neuro.Data.DataAccessLayer

Public Class VerifyManager

    Public Shared Function getStudentFingerPrintData() As DataSet
        Dim ds As New DataSet
        Try
            DAL.openConnection()
            DAL.BeginTransaction()
            DAL.ProcedureName = "msp_GetStudentFingerPrintData"
            DAL.createProcedureCommand()

            Dim da As SqlClient.SqlDataAdapter = DAL.createDataAdapter
            da.Fill(ds)
            DAL.commitTransaction()
            Return ds

        Catch ex As Exception
            DAL.rollbackTransaction()
            Throw New Exception(ex.Message)
        Finally
            DAL.closeConnection()
            DAL.disposeConnection()
        End Try
    End Function

    Public Shared Function deleteStudent(ByVal id As Double) As DataSet
        Dim ds As New DataSet
        Try
            DAL.openConnection()
            DAL.BeginTransaction()
            DAL.ProcedureName = "mSP_DeleteStudent"
            DAL.createProcedureCommand()
            DAL.Parameters("@ID", id)

            Dim da As SqlClient.SqlDataAdapter = DAL.createDataAdapter
            da.Fill(ds)
            DAL.commitTransaction()
            Return ds

        Catch ex As Exception
            DAL.rollbackTransaction()
            Throw New Exception(ex.Message)
        Finally
            DAL.closeConnection()
            DAL.disposeConnection()
        End Try
    End Function

    Public Shared Function getStudentInformationByID(ByVal id As Double) As DataSet
        Dim ds As New DataSet
        Try
            DAL.openConnection()
            DAL.BeginTransaction()
            DAL.ProcedureName = "mSP_GetStudentInformation"
            DAL.createProcedureCommand()
            DAL.Parameters("@ID", id)

            Dim da As SqlClient.SqlDataAdapter = DAL.createDataAdapter
            da.Fill(ds)
            DAL.commitTransaction()
            Return ds

        Catch ex As Exception
            DAL.rollbackTransaction()
            Throw New Exception(ex.Message)
        Finally
            DAL.closeConnection()
            DAL.disposeConnection()
        End Try
    End Function

End Class
