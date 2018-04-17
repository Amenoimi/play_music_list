Imports System.IO
Imports System.Text

Public Class Form1
    Public Const MM_MCINOTIFY As Integer = 953
    Dim adio As New List(Of String)
    Dim palyimg_mod As Integer

    ' 宣告 API 
    Private Declare Ansi Function mciSendStringA Lib "winmm.dll" Alias "mciSendStringA" (ByVal command As String, ByRef buffer As StringBuilder, ByVal bufferSize As Integer, ByVal hWndCallback As IntPtr) As Integer


    Private Declare Auto Function GetShortPathName Lib "kernel32" (
        ByVal longPath As String,
        ByVal shortPath As StringBuilder,
        ByVal shortBufferSize As Int32) As Int32

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = MM_MCINOTIFY Then
            ' The file is done playing, do whatever is necessary at this point
            'MsgBox("Done playing.")
            If (Button1.Text = "停止" And Button2.Text = "暫停") Then
                If (ListBox1.SelectedIndex < ListBox1.Items.Count - 1) Then
                    If (palyimg_mod = 0) Then
                        ListBox1.SelectedIndex = ListBox1.SelectedIndex + 1
                        PlayMediaFile(adio(ListBox1.SelectedIndex))
                    Else
                        Dim rnd As New Random
                        Dim s, d As Integer
                        d = ListBox1.Items.Count - 1
                        s = rnd.Next(0, d)
                        ListBox1.SelectedIndex = s
                        PlayMediaFile(adio(ListBox1.SelectedIndex))
                    End If
                ElseIf (ListBox1.SelectedIndex = ListBox1.Items.Count - 1) Then
                    Button1.Text = "播放"
                    Button2.Text = "暫停"
                End If



            End If

        End If
        MyBase.WndProc(m)
    End Sub

    '播放媒體檔案
    Private Function PlayMediaFile(ByVal mediaFile As String) As Boolean
        If File.Exists(mediaFile) Then
            mediaFile = ConvertToShortPathName(mediaFile)
            mciSendStringA("stop music", Nothing, 0, 0)
            mciSendStringA("close music", Nothing, 0, 0)
            mciSendStringA(String.Format("open {0} alias music", mediaFile), Nothing, 0, 0)
            PlayMediaFile = mciSendStringA("play music notify", Nothing, 0, Me.Handle.ToInt64()) = 0
        End If
    End Function


    '取得短路徑及檔名
    Private Function ConvertToShortPathName(fileName As String) As String
        Dim shortName As New StringBuilder(256)
        GetShortPathName(fileName, shortName, shortName.Capacity)
        Return shortName.ToString
    End Function

    ' 停止播放
    Private Function StopMedia() As Boolean
        StopMedia = mciSendStringA("stop music", Nothing, 0, 0) = 0
        mciSendStringA("close music", Nothing, 0, 0)
    End Function

    ' 暫停播放
    Private Function PauseMedia() As Boolean
        Return mciSendStringA("pause music", Nothing, 0, 0) = 0
    End Function

    ' 繼續播放
    Private Function ContinueMedia() As Boolean
        Return mciSendStringA("play music", Nothing, 0, 0) = 0
    End Function


    Private Sub ListBox1_DragEnter(sender As Object, e As DragEventArgs) Handles ListBox1.DragEnter
        'e.Effect = DragDropEffects.All

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If

    End Sub

    Private Sub ListBox1_DragDrop(sender As Object, e As DragEventArgs) Handles ListBox1.DragDrop

        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)

        For Each path In files
            'ReDim Preserve adio(i + 1)
            'adio(i - 1) = path.ToString
            Dim data As String() = path.Split("."c)
            If (data(data.Length - 1) = "wav" Or data(data.Length - 1) = "mp3" Or data(data.Length - 1) = "mid") Then
                adio.Add(path)
            End If


        Next
        ListBox1.Items.Clear()

        For Each path In adio
            Dim name As String() = path.Split("\"c)
            ListBox1.Items.Add(name(name.Length - 1))

        Next




    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.AllowDrop = True
        palyimg_mod = 0
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (Button1.Text = "播放" And ListBox1.SelectedIndex <> -1) Then

            PlayMediaFile(adio(ListBox1.SelectedIndex))

            Button1.Text = "停止"

        ElseIf (Button1.Text = "播放" And ListBox1.SelectedIndex = -1 And ListBox1.Items.Count > 0) Then
            ListBox1.SelectedIndex = 0
            PlayMediaFile(adio(ListBox1.SelectedIndex))
            Button1.Text = "停止"

        ElseIf (Button1.Text = "停止") Then
            StopMedia()
            Button1.Text = "播放"
        End If

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If (palyimg_mod = 0) Then
            Button5.Enabled = False
            Button4.Enabled = True

        Else
            Button5.Enabled = True
            Button4.Enabled = False
        End If
        For Each f In adio
            If (f = "") Then
                ccc()
            End If

        Next



    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If (Button2.Text = "繼續" And ListBox1.SelectedIndex <> -1) Then
            ContinueMedia()
            Button2.Text = "暫停"
        ElseIf (Button2.Text = "暫停") Then
            PauseMedia()
            Button2.Text = "繼續"
        End If


    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If (ListBox1.SelectedIndex <> -1) Then
            Dim tmp_adio As New List(Of String)
            For Each no_del In adio
                If (no_del <> adio(ListBox1.SelectedIndex)) Then
                    tmp_adio.Add(no_del)
                End If

            Next
            adio = tmp_adio
            ListBox1.Items.Clear()

            For Each path In adio
                Dim name As String() = path.Split("\"c)
                ListBox1.Items.Add(name(name.Length - 1))

            Next

        End If
    End Sub

    Sub ccc()
        Dim tmp_adio As New List(Of String)
        For Each no_del In adio
            If (no_del <> "") Then
                tmp_adio.Add(no_del)
            End If

        Next
        adio = tmp_adio
        ListBox1.Items.Clear()

        For Each path In adio
            Dim name As String() = path.Split("\"c)
            ListBox1.Items.Add(name(name.Length - 1))

        Next
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        palyimg_mod = 0
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        palyimg_mod = 1
    End Sub

    Sub w_txt(strTemp As String)
        Dim FileNum As Integer


        FileNum = FreeFile()
        FileOpen(FileNum, Application.StartupPath + "\tmp.txt", OpenMode.Output)


        PrintLine(FileNum, strTemp)

        FileClose(FileNum)
    End Sub
    Function r_txt()
        Dim FileNum As Integer
        Dim strTemp As String

        FileNum = FreeFile()
        FileOpen(FileNum, Application.StartupPath + "\tmp.txt", OpenMode.Input)

        Do Until EOF(FileNum)
            strTemp &= LineInput(FileNum) & vbNewLine
        Loop

        FileClose(FileNum)
        Return strTemp
    End Function
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim txt As String
        For Each a In adio
            txt += a + "###"
        Next

        w_txt(txt)
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click

        adio = New List(Of String)
        ListBox1.Items.Clear()

        Dim ts As String
        Dim tss As String()

        ts = r_txt()
        tss = ts.Split("###")


        For Each path In tss
            'ReDim Preserve adio(i + 1)
            'adio(i - 1) = path.ToString
            adio.Add(path)

        Next
        ListBox1.Items.Clear()

        For Each path In adio
            Dim name As String() = path.Split("\"c)
            ListBox1.Items.Add(name(name.Length - 1))

        Next



    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        adio = New List(Of String)
        ListBox1.Items.Clear()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If (ListBox1.SelectedIndex <> -1 And (ListBox1.SelectedIndex <> 0)) Then
            Dim g, g2 As String
            Dim p As Integer
            p = ListBox1.SelectedIndex
            g = ListBox1.Items(ListBox1.SelectedIndex)
            g2 = adio(ListBox1.SelectedIndex)
            ListBox1.Items(ListBox1.SelectedIndex) = ListBox1.Items(ListBox1.SelectedIndex - 1)
            adio(ListBox1.SelectedIndex) = adio(ListBox1.SelectedIndex - 1)
            ListBox1.Items(ListBox1.SelectedIndex - 1) = g
            adio(ListBox1.SelectedIndex - 1) = g2

            ListBox1.Items.Clear()

            For Each path In adio
                Dim name As String() = path.Split("\"c)
                ListBox1.Items.Add(name(name.Length - 1))

            Next
            ListBox1.SelectedIndex = p - 1
        End If
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If (ListBox1.SelectedIndex <> -1 And ListBox1.SelectedIndex <> (ListBox1.Items.Count - 1)) Then
            Dim g, g2 As String
            Dim p As Integer
            p = ListBox1.SelectedIndex
            g = ListBox1.Items(ListBox1.SelectedIndex)
            g2 = adio(ListBox1.SelectedIndex)
            ListBox1.Items(ListBox1.SelectedIndex) = ListBox1.Items(ListBox1.SelectedIndex + 1)
            adio(ListBox1.SelectedIndex) = adio(ListBox1.SelectedIndex + 1)
            ListBox1.Items(ListBox1.SelectedIndex + 1) = g
            adio(ListBox1.SelectedIndex + 1) = g2

            ListBox1.Items.Clear()

            For Each path In adio
                Dim name As String() = path.Split("\"c)
                ListBox1.Items.Add(name(name.Length - 1))

            Next
            ListBox1.SelectedIndex = p + 1
        End If
    End Sub
End Class
