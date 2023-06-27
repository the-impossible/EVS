Imports DPUruNet
Imports DPUruNet.Constants
Imports Neuro.Business
Imports System.IO
Imports System.Transactions

Public Class frmStudent
    'Finger print related
    Dim ds As DataSet
    Dim btnContEnroll As Boolean = False
    Dim fpReader As DPUruNet.Reader
    Dim fmdLeftIndex As Fmd 'temporarily store finger data
    Dim fmdRightIndex As Fmd 'temporarily store finger data

    Dim fmds As New List(Of Fmd)
    Dim count As Integer = 4
    Dim pressCount As Integer = 0
    Dim chkReference As System.Windows.Forms.CheckBox = Nothing

    Private Sub frmStudent_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        btnNew_Click(sender, e)
        CheckForIllegalCrossThreadCalls = False

    End Sub

    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        Me.txtReg.Text = Nothing
        Me.txtName.Text = Nothing
        Me.txtPhone.Text = Nothing
        Me.cmbLevel.Text = Nothing
        Me.chkPrimary.Checked = False
        Me.chkPrimary.Enabled = True
        Me.chkSecondary.Checked = False
        Me.chkSecondary.Enabled = False
        Me.studImage.Image = Nothing
        clearPrintLabels()
        Me.txtReg.Focus()

        Try
            fpReader.CancelCapture()
            fpReader.Dispose()
        Catch ex As Exception
        End Try

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Try
            fpReader.CancelCapture()
            fpReader.Dispose()
        Catch ex As Exception
        End Try

        frmMain.Show()
        frmMain.Activate()
        Me.Close()


    End Sub

    Private Sub clearPrintLabels()
        Me.lbl1.BackColor = Nothing
        Me.lbl2.BackColor = Nothing
        Me.lbl3.BackColor = Nothing
        Me.lbl4.BackColor = Nothing
        Me.lblInfo.Text = "Ready"
    End Sub

    Private Sub Enroll()
        If (chkPrimary.Enabled <> True And chkPrimary.Checked <> True) Then
            chkReference = chkPrimary
        Else
            chkReference = chkSecondary
        End If


        fpReader = DPUruNet.ReaderCollection.GetReaders().Item(0)
        If (fpReader Is Nothing) Then
            MsgBox("Could not enumerate sensor, Please verify a digital personal sensor is connected and try again!", MsgBoxStyle.Exclamation, "Device not found!")
            BeginInvoke(Function()
                            If chkPrimary.Checked = False Then
                                chkPrimary.Enabled = True
                            End If
                            If chkSecondary.Checked = False Then
                                chkSecondary.Enabled = True
                            End If
                            Return True
                        End Function)
            Return
        End If

        Dim result As Constants.ResultCode = ResultCode.DP_DEVICE_FAILURE

        result = fpReader.Open(CapturePriority.DP_PRIORITY_COOPERATIVE)


        If result <> ResultCode.DP_SUCCESS Then
            BeginInvoke(Function()
                            MsgBox("Failed to open device, please verify a digitalpersonal sensor is connected or is not in use by another application.", MsgBoxStyle.Exclamation, "Failed to open device")
                            If chkPrimary.Checked = False Then
                                chkPrimary.Enabled = True
                            End If
                            If chkSecondary.Checked = False Then
                                chkSecondary.Enabled = True
                            End If
                            Return True
                        End Function)
            Return
        End If

        Try
            GetStatus(fpReader)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error")
            Return
        End Try

        fmds.Clear()
        count = 4
        pressCount = 0


        Dim captureCallBack As Reader.CaptureCallback
        captureCallBack = New Reader.CaptureCallback(AddressOf onCapture)
        AddHandler fpReader.On_Captured, captureCallBack

        result = fpReader.CaptureAsync(Formats.Fid.ANSI, CaptureProcessing.DP_IMG_PROC_DEFAULT, 500)

        If result <> ResultCode.DP_SUCCESS Then
            MsgBox("Error Initiating Capture")
            Return
        End If


        BeginInvoke(Function()
                        lblInfo.Text = "Place finger on the sensor at least 4 times"
                        Return True
                    End Function)
    End Sub

    Private Sub onCapture(ByVal captureResult As CaptureResult)

        If captureResult.ResultCode <> ResultCode.DP_SUCCESS Then
            BeginInvoke(Function()
                            lblInfo.Text = "Capture failed. " & captureResult.ResultCode.ToString()
                            If chkPrimary.Checked = False Then
                                chkPrimary.Enabled = True
                            End If
                            If chkSecondary.Checked = False Then
                                chkSecondary.Enabled = True
                            End If
                            Return True
                        End Function)
            Return

        End If

        If captureResult.Quality = CaptureQuality.DP_QUALITY_CANCELED Then
            BeginInvoke(Function()
                            lblInfo.Text = "Capture cancelled. " & captureResult.ResultCode.ToString()
                            If chkPrimary.Checked = False Then
                                chkPrimary.Enabled = True
                            End If
                            If chkSecondary.Checked = False Then
                                chkSecondary.Enabled = True
                            End If
                            Return True
                        End Function)
            Return
        End If

        If captureResult.Quality = CaptureQuality.DP_QUALITY_NO_FINGER Or captureResult.Quality = CaptureQuality.DP_QUALITY_TIMED_OUT Then
            BeginInvoke(Function()
                            lblInfo.Text = "Capture time out. " & captureResult.ResultCode.ToString()
                            If chkPrimary.Checked = False Then
                                chkPrimary.Enabled = True
                            End If
                            If chkSecondary.Checked = False Then
                                chkSecondary.Enabled = True
                            End If
                            Return True
                        End Function)
            Return
        End If

        If captureResult.Quality = CaptureQuality.DP_QUALITY_FAKE_FINGER Then
            BeginInvoke(Function()
                            lblInfo.Text = "Capture failed due to fake finger detection. " & vbCrLf & "Try again.!" & captureResult.ResultCode.ToString()
                            Return True
                        End Function)
            Return
        End If

        pressCount += 1

        Dim pIndex As Integer = 4 - count

        Try

            If pIndex = 0 Then
                lbl1.BackColor = Color.DarkGreen
            ElseIf pIndex = 1 Then
                lbl2.BackColor = Color.DarkGreen
            ElseIf pIndex = 2 Then
                lbl3.BackColor = Color.DarkGreen
            ElseIf pIndex = 3 Then
                lbl4.BackColor = Color.DarkGreen
            End If

        Catch ex As Exception

        End Try

        Dim resultConversion As DataResult(Of Fmd) = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Formats.Fmd.DP_PRE_REGISTRATION)

        If resultConversion.ResultCode <> ResultCode.DP_SUCCESS Then
            BeginInvoke(Function()
                            lblInfo.Text = "Could not create fmd." & captureResult.ResultCode.ToString()
                            Return True
                        End Function)
            Return
        End If

        fmds.Add(resultConversion.Data)

        count -= 1

        If count = 0 Then

            Dim resultEnrollment As DataResult(Of Fmd) = Enrollment.CreateEnrollmentFmd(Formats.Fmd.DP_REGISTRATION, fmds)

            If resultEnrollment.ResultCode = ResultCode.DP_SUCCESS Then
                BeginInvoke(Function()
                                lblInfo.Text = "Finger enrolled."
                                chkReference.Checked = True

                                If chkReference.Name = "chkPrimary" Then
                                    fmdRightIndex = resultEnrollment.Data
                                    fpReader.CancelCapture()
                                    fpReader.Dispose()
                                    If (chkSecondary.Checked <> True) Then
                                        chkSecondary.Enabled = True
                                    End If
                                Else
                                    fmdLeftIndex = resultEnrollment.Data
                                    If chkPrimary.Checked <> True Then
                                        chkPrimary.Enabled = True
                                    End If
                                    fpReader.CancelCapture()
                                    fpReader.Dispose()
                                End If

                                Return True
                            End Function)
                Return

            ElseIf resultEnrollment.ResultCode = ResultCode.DP_FAILURE Then

                BeginInvoke(Function()
                                lblInfo.Text = "Could not enroll finger." & vbCrLf & "Please try a different finger"
                                Return True
                            End Function)

                fmds.Clear()
                count = 4
                Return

            End If
        End If

        If count = 0 Then
            count += 1
        End If

        If pressCount = 5 Then
            BeginInvoke(Function()
                            MsgBox("Failed to enroll finger. Try registering a different finger.", MsgBoxStyle.Exclamation, "Failed to enroll")

                            If chkPrimary.Checked = False Then
                                chkPrimary.Enabled = True
                            End If
                            If chkSecondary.Checked = False Then
                                chkSecondary.Enabled = True
                            End If
                            If chkReference.Name = "chkPrimary" Then
                                chkSecondary.Enabled = False
                                chkSecondary.Checked = False
                            End If

                            clearPrintLabels()
                            fpReader.CancelCapture()
                            fpReader.Dispose()

                            pressCount = 0
                            count = 4

                            Return True
                        End Function)
            Return

        End If

        BeginInvoke(Function()
                        lblInfo.Text = "Finger Print Captured " & vbCrLf & count & " more times."

                        Return True

                    End Function)

    End Sub

    Private Sub GetStatus(ByVal currenReader As DPUruNet.Reader)
        Dim result = currenReader.GetStatus

        If result <> ResultCode.DP_SUCCESS Then
            Console.WriteLine(result.ToString())
            Throw New Exception(result.ToString())
        End If

        If currenReader.Status.Status = ReaderStatuses.DP_STATUS_BUSY Then
            Threading.Thread.Sleep(50)
        ElseIf currenReader.Status.Status = ReaderStatuses.DP_STATUS_NEED_CALIBRATION Then
            currenReader.Calibrate()
        ElseIf currenReader.Status.Status <> ReaderStatuses.DP_STATUS_READY Then
            Throw New Exception("Reader Status - " & currenReader.Status.Status.ToString())

        End If


    End Sub

    Private Sub chkPrimary_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPrimary.CheckedChanged

        If Not Me.chkPrimary.Checked Or chkPrimary.Enabled = False Then
            Return
        End If
        clearPrintLabels()

        chkPrimary.Checked = False
        chkPrimary.Enabled = False
        chkSecondary.Enabled = False
        btnContEnroll = True

        Enroll()

    End Sub

    Private Sub chkSecondary_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkSecondary.CheckedChanged
        If Not chkSecondary.Checked Or chkSecondary.Enabled = False Then
            Return
        End If

        clearPrintLabels()

        chkSecondary.Checked = False
        chkSecondary.Enabled = False
        chkPrimary.Enabled = False
        btnContEnroll = True
        Enroll()
    End Sub

    Dim delg As Image.GetThumbnailImageAbort

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If String.IsNullOrWhiteSpace(Me.txtName.Text) Then Me.txtName.Focus() : Return
        If String.IsNullOrWhiteSpace(Me.txtPhone.Text) Then Me.txtPhone.Focus() : Return
        If String.IsNullOrWhiteSpace(Me.txtReg.Text) Then Me.txtReg.Focus() : Return

        Dim level As String = Me.cmbLevel.Text
        If level Is Nothing Then Me.cmbLevel.Focus() : Return



        If level <> "100 Level" AndAlso level <> "200 Level" AndAlso level <> "300 Level" AndAlso level <> "400 Level" Then
            Me.cmbLevel.Focus() : Return
        End If

        If fmdLeftIndex Is Nothing Or fmdRightIndex Is Nothing Then
            MsgBox("Could not saved. Please provide finger information.", MsgBoxStyle.Exclamation, "No finger data captured")
            Return
        End If

        If Not studImage.Image Is Nothing Then
            studImage.Image = studImage.Image.GetThumbnailImage(200, 250, delg, IntPtr.Zero)
        End If

        Try
            Using scope As New TransactionScope(TransactionScopeOption.Required)

                StudentManager.InsertStudent(txtReg.Text.ToUpper(), txtName.Text, txtPhone.Text, cmbLevel.Text, Fmd.SerializeXml(fmdLeftIndex), Fmd.SerializeXml(fmdRightIndex), imageToByte(studImage.Image))
                scope.Complete()

            End Using
            MsgBox("Student enrolled successfully", MsgBoxStyle.Information, "Saved Successfully")
            btnNew_Click(sender, e)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error")
        End Try

    End Sub

    Private Function imageToByte(ByVal img As Image) As Byte()
        If img Is Nothing Then
            Return Nothing
        End If
        Dim ms = New MemoryStream()
        img.Save(ms, Imaging.ImageFormat.Jpeg)
        Return ms.ToArray()
    End Function

    Private Sub btnLoadImg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadImg.Click

        Dim opf As New OpenFileDialog
        opf.Filter = "Choose Image (*.jpg;*.png)|*.jpg;*.png"

        If opf.ShowDialog = DialogResult.OK Then
            studImage.Image = Image.FromFile(opf.FileName)
        End If

    End Sub

    Private Sub btnVerify_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        frmVerify.Show()
        frmVerify.Activate()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        frmModify.Show()
        frmModify.Activate()
    End Sub
End Class