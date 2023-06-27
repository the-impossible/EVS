Public Class clsDbConfiguration
    Sub New()
        'Neuro.Data.clsDbConfig.instanceName = My.Settings.InstanceName
        'Neuro.Data.clsDbConfig.dbName = My.Settings.DbName
        'Neuro.Data.clsDbConfig.userID = My.Settings.UserID
        'Neuro.Data.clsDbConfig.password = My.Settings.Password

        Neuro.Data.DataAccessLayer.InstanceName = My.Settings.InstanceName
        Neuro.Data.DataAccessLayer.DbName = My.Settings.DbName
        Neuro.Data.DataAccessLayer.UserID = My.Settings.UserID
        Neuro.Data.DataAccessLayer.Password = My.Settings.Password

        checkDBConnection()

    End Sub

    Private Sub checkDBConnection()
        Try
            Neuro.Data.DataAccessLayer.openConnection()
            'MsgBox("Connection Open Successfully")
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            Neuro.Data.DataAccessLayer.closeConnection()
        End Try

    End Sub
End Class
