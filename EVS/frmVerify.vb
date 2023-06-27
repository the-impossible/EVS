Imports Neuro.Business
Imports DPUruNet
Imports DPUruNet.Constants
Imports System.IO

Public Class frmVerify

    Dim ds, ds1 As DataSet
    Dim dsFingerPrint As DataSet
    Dim userIDlist1 As List(Of Integer) = New List(Of Integer)
    Dim userIDlist2 As List(Of Integer) = New List(Of Integer)
    Dim fmdListFP1 As List(Of Fmd) = New List(Of Fmd)
    Dim fmdListFP2 As List(Of Fmd) = New List(Of Fmd)
    Dim fmds As New List(Of Fmd)
    Dim fpReader As DPUruNet.Reader

    Private Sub frmVerify_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            dsFingerPrint = New DataSet
            dsFingerPrint = VerifyManager.getStudentFingerPrintData

        Catch ex As Exception
            MsgBox(ex.Message)
            Return
        End Try

        Try
            For Each dr As DataRow In dsFingerPrint.Tables(0).Rows
                If Not IsDBNull(dr("FingerPrint1")) Then
                    Dim printXML As String = dr("FingerPrint1")
                    fmdListFP1.Add(Fmd.DeserializeXml(printXML))
                    userIDlist1.Add(dr("ID"))
                End If
                If Not IsDBNull(dr("FingerPrint2")) Then
                    Dim printXML As String = dr("FingerPrint2")
                    fmdListFP2.Add(Fmd.DeserializeXml(printXML))
                    userIDlist2.Add(dr("ID"))
                End If
            Next
        Catch ex As Exception
        End Try

        Indentify()

    End Sub

    Private Sub Indentify()


        fpReader = DPUruNet.ReaderCollection.GetReaders().Item(0)

        If (fpReader Is Nothing) Then
            MsgBox("Could not enumerate sensor. Please verify a DigitalPersona sensor is connected and try again.", MsgBoxStyle.Exclamation, "Device not Found!")
            BeginInvoke(Function()
                            Me.Close()
                            Return True
                        End Function)
            Return
        End If

        Dim result As Constants.ResultCode = ResultCode.DP_DEVICE_FAILURE

        result = fpReader.Open(CapturePriority.DP_PRIORITY_COOPERATIVE)

        If result <> ResultCode.DP_SUCCESS Then
            BeginInvoke(Function()
                            MsgBox("Failed to open device. Please verify a DigitalPersona sensor is connected or is not in use by another application.", MsgBoxStyle.Exclamation, "Faild to open device")
                            Me.Close()
                            Return True
                        End Function)
            Return
        End If

        Try
            GetStatus(fpReader)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error!")
            Return
        End Try

        fmds.Clear()

        Dim captureCallBack As Reader.CaptureCallback
        captureCallBack = New Reader.CaptureCallback(AddressOf onCapture)
        AddHandler fpReader.On_Captured, captureCallBack



        result = fpReader.CaptureAsync(Formats.Fid.ANSI, CaptureProcessing.DP_IMG_PROC_DEFAULT, 500)

        If result <> ResultCode.DP_SUCCESS Then
            MsgBox("Error starting capture")
            Return
        End If

    End Sub

    Private Sub onCapture(ByVal captureResult As CaptureResult)

        If captureResult.ResultCode <> ResultCode.DP_SUCCESS Then
            BeginInvoke(Function()
                            lblInfo.Text = "Capture failed. " & captureResult.ResultCode.ToString()
                            Me.Close()
                            Return True
                        End Function)
            Return
        End If

        If captureResult.Quality = CaptureQuality.DP_QUALITY_CANCELED Then
            BeginInvoke(Function()
                            lblInfo.Text = "Capture cancelled. " & captureResult.ResultCode.ToString()
                            Return True
                        End Function)
            Return
        End If

        If captureResult.Quality = CaptureQuality.DP_QUALITY_NO_FINGER Or captureResult.Quality = CaptureQuality.DP_QUALITY_TIMED_OUT Then
            BeginInvoke(Function()
                            lblInfo.Text = "Capture timed out. " & captureResult.ResultCode.ToString()
                            Return True
                        End Function)
            Return
        End If

        If captureResult.Quality = CaptureQuality.DP_QUALITY_FAKE_FINGER Then
            BeginInvoke(Function()
                            lblInfo.Text = "Capture failed due to fake finger detection. " & vbCrLf & "try again." & captureResult.ResultCode.ToString()
                            Return True
                        End Function)
            Return
        End If

        Dim resultConversion As DataResult(Of Fmd) = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Formats.Fmd.DP_VERIFICATION)

        If resultConversion.ResultCode <> ResultCode.DP_SUCCESS Then
            BeginInvoke(Function()
                            lblInfo.Text = "Could not create fmd." & captureResult.ResultCode.ToString()
                            Return True
                        End Function)
            Return
        End If

        BeginInvoke(Function()
                        Me.txtLevel.Text = Nothing
                        Me.txtPhone.Text = Nothing
                        Me.txtStudName.Text = Nothing
                        Me.txtRegNo.Text = Nothing
                        Me.studImage.Image = Nothing
                        Me.lblInfo.Text = "Awaiting Fingerprint...."
                        Return True
                    End Function)

        Dim idResult As IdentifyResult

        If (fmdListFP1.Count > 0) Then

            idResult = Comparison.Identify(resultConversion.Data, 0, fmdListFP1, &H7FFFFFFF / 100000, 4)

            If idResult.ResultCode <> ResultCode.DP_SUCCESS Then
                BeginInvoke(Function()
                                lblInfo.Text = "Unable to Identify, scan different. finger"
                                Return True
                            End Function)
                Return
            End If

            If (idResult.Indexes.Length >= 1) Then

                BeginInvoke(Function()
                                Try
                                    ds1 = New DataSet
                                    ds1 = VerifyManager.getStudentInformationByID(userIDlist1(idResult.Indexes(0)(0)))

                                    Try
                                        Dim b() As Byte = ds1.Tables(0)(0)("StudentImage")
                                        Me.studImage.Image = Image.FromStream(New MemoryStream(b))
                                    Catch ex As Exception
                                    End Try


                                    Me.txtRegNo.Text = ds1.Tables(0)(0)("RegNo")
                                    Me.txtStudName.Text = ds1.Tables(0)(0)("Name")
                                    Me.txtPhone.Text = ds1.Tables(0)(0)("Phone")
                                    Me.txtLevel.Text = ds1.Tables(0)(0)("Level")
                                    Me.lblInfo.Text = "Identified"


                                Catch ex As Exception
                                    lblInfo.Text = "Can not Identified"
                                End Try

                                Return True

                            End Function)

                Return

            End If

        End If


        If (fmdListFP2.Count > 0) Then
            idResult = Comparison.Identify(resultConversion.Data, 0, fmdListFP2, &H7FFFFFFF / 100000, 4)

            If idResult.ResultCode <> ResultCode.DP_SUCCESS Then
                BeginInvoke(Function()
                                lblInfo.Text = "Unable to Identify, scan different. finger"
                                Return True
                            End Function)
                Return
            End If

            If (idResult.Indexes.Length >= 1) Then

                BeginInvoke(Function()
                                Try
                                    ds1 = New DataSet
                                    ds1 = VerifyManager.getStudentInformationByID(userIDlist2(idResult.Indexes(0)(0)))

                                    Try
                                        Dim b() As Byte = ds1.Tables(0)(0)("StudentImage")
                                        Me.studImage.Image = Image.FromStream(New MemoryStream(b))
                                    Catch ex As Exception
                                    End Try


                                    Me.txtRegNo.Text = ds1.Tables(0)(0)("RegNo")
                                    Me.txtStudName.Text = ds1.Tables(0)(0)("Name")
                                    Me.txtPhone.Text = ds1.Tables(0)(0)("Phone")
                                    Me.txtLevel.Text = ds1.Tables(0)(0)("Level")
                                    Me.lblInfo.Text = "Identified"

                                Catch ex As Exception
                                    lblInfo.Text = "Can not Identified"
                                End Try

                                Return True

                            End Function)

                Return

            End If

        End If


    End Sub

    Private Sub GetStatus(ByVal currentReader As DPUruNet.Reader)
        Dim result = currentReader.GetStatus

        If result <> ResultCode.DP_SUCCESS Then
            Throw New Exception(result.ToString())
        End If

        If currentReader.Status.Status = ReaderStatuses.DP_STATUS_BUSY Then
            Threading.Thread.Sleep(50)
        ElseIf currentReader.Status.Status = ReaderStatuses.DP_STATUS_NEED_CALIBRATION Then
            currentReader.Calibrate()
        ElseIf currentReader.Status.Status <> ReaderStatuses.DP_STATUS_READY Then
            Throw New Exception("Reader Status - " & currentReader.Status.Status.ToString())
        End If

    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        Me.txtLevel.Text = Nothing
        Me.txtPhone.Text = Nothing
        Me.txtStudName.Text = Nothing
        Me.txtRegNo.Text = Nothing
        Me.studImage.Image = Nothing
        Me.lblInfo.Text = "Awaiting Fingerprint...."

    End Sub


    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Try
            fpReader.CancelCapture()
            fpReader.Dispose()
        Catch ex As Exception
        End Try

        Me.Close()

    End Sub
End Class