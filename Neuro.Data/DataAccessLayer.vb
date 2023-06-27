
'Imports System.Configuration

Public Class DataAccessLayer

#Region "Data Members"

    'Private Shared _connString As String
    Private Shared _procName As String
    Private Shared _query As String
    Private Shared _command As System.Data.SqlClient.SqlCommand
    Private Shared _connection As System.Data.SqlClient.SqlConnection
    Private Shared _dataAdapter As System.Data.SqlClient.SqlDataAdapter
    Private Shared _trans As System.Data.SqlClient.SqlTransaction
    Private Shared _userID, _password, _instanceName, _dbName As String
    Public Shared loggedInUser, loggedInUserType As String
#End Region

#Region "Properties"
    'Public Shared ReadOnly Property UserID As String
    '    Get
    '        Return clsDbConfig.userID
    '    End Get
    'End Property
    'Public Shared ReadOnly Property Password As String
    '    Get
    '        Return clsDbConfig.password
    '    End Get
    'End Property
    'Public Shared ReadOnly Property InstanceName As String
    '    Get
    '        Return clsDbConfig.instanceName
    '    End Get
    'End Property
    'Public Shared ReadOnly Property DbName As String
    '    Get
    '        Return clsDbConfig.dbName
    '    End Get
    'End Property

    Public Shared Property UserID As String
        Get
            Return _userID
        End Get
        Set(ByVal value As String)
            _userID = value
        End Set
    End Property
    Public Shared Property Password As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property
    Public Shared Property InstanceName As String
        Get
            Return _instanceName
        End Get
        Set(ByVal value As String)
            _instanceName = value
        End Set
    End Property
    Public Shared Property DbName As String
        Get
            Return _dbName
        End Get
        Set(ByVal value As String)
            _dbName = value
        End Set
    End Property

    Public Shared ReadOnly Property ConnectionString As String
        Get
            'Return ConfigurationManager.AppSettings("connString")
            Return "Data Source =" & InstanceName & ";Initial Catalog = " & DbName & ";User ID = " & UserID & ";Password = " & Password

        End Get
    End Property
    Public Shared Property Query As String
        Get
            Return _query
        End Get
        Set(ByVal value As String)
            _query = value
        End Set
    End Property
    Public Shared Property ProcedureName As String
        Get
            Return _procName
        End Get
        Set(ByVal value As String)
            _procName = value
        End Set
    End Property
#End Region

#Region "Methods"

#Region "Connections"
    Public Shared Function openConnection() As SqlClient.SqlConnection
        _connection = New SqlClient.SqlConnection(ConnectionString)
        _connection.Open()
        Return _connection
    End Function
    Public Shared Sub closeConnection()
        If Not _connection Is Nothing Then
            _connection.Close()
        End If
    End Sub
    Public Shared Sub disposeConnection()
        If Not _connection Is Nothing Then
            _connection.Dispose()
            _connection = Nothing
        End If
    End Sub
#End Region

#Region "DataAdapter"
    Public Shared Function createDataAdapter() As SqlClient.SqlDataAdapter
        _dataAdapter = New SqlClient.SqlDataAdapter(_command)
        Return _dataAdapter
    End Function
#End Region

#Region "Transactions"
    Public Shared Sub BeginTransaction()
        _trans = _connection.BeginTransaction
    End Sub
    Public Shared Sub commitTransaction()
        _trans.Commit()
    End Sub
    Public Shared Sub rollbackTransaction()
        _trans.Rollback()
    End Sub
#End Region

#Region "Commands"
    Public Shared Sub createProcedureCommand()

        _command = _connection.CreateCommand
        _command.CommandText = ProcedureName
        _command.CommandType = CommandType.StoredProcedure
        _command.Connection = _connection
        _command.Transaction = _trans
        _command.CommandTimeout = 1200

    End Sub
    Public Shared Sub createQueryCommand()

        _command = _connection.CreateCommand
        _command.CommandText = Query
        _command.CommandType = CommandType.Text
        _command.Connection = _connection
        _command.Transaction = _trans
        _command.CommandTimeout = 1200

    End Sub

#End Region

#Region "Parameters"
    'Public Shared Sub Parameters(ByVal name As String, ByVal type As SqlDbType, ByVal size As Integer, ByVal value As String)
    '    _command.Parameters.Add(name, type, size)
    '    _command.Parameters(name).Value = value
    'End Sub
    'Public Shared Sub Parameters(ByVal name As String, ByVal type As SqlDbType, ByVal value As Double)
    '    _command.Parameters.Add(name, type)
    '    _command.Parameters(name).Value = value
    'End Sub
    'Public Shared Sub Parameters(ByVal name As String, ByVal type As SqlDbType, ByVal value As Integer)
    '    _command.Parameters.Add(name, type)
    '    _command.Parameters(name).Value = value
    'End Sub

    Public Shared Sub Parameters(ByVal name As String, ByVal value As Object)
        _command.Parameters.AddWithValue(name, value)
    End Sub
    Public Shared Sub Parameters(name As String, dbType As SqlDbType)
        _command.Parameters.Add(name, dbType)
        _command.Parameters(name).Direction = ParameterDirection.Output
    End Sub
    Public Shared Function Parameters(retParam As String) As String
        Return _command.Parameters(retParam).Value.ToString()
    End Function

#End Region

#Region "Execute Commands"
    Public Shared Function ExecuteNonQuery() As Integer
        Return Val(_command.ExecuteNonQuery())
    End Function
    Public Shared Function ExecuteReader() As SqlClient.SqlDataReader
        Return _command.ExecuteReader
    End Function
    Public Shared Function ExecuteScaler() As Object
        Return _command.ExecuteScalar
    End Function
#End Region

#End Region

End Class
