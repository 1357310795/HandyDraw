Imports System.IO
Imports System.Threading
Imports MaterialDesignExtensions.Controls

Public Class SaveDialog
    Public Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub
    Public path As String = ""
    Public w() As Integer
    Public h() As Integer
    Public c() As String = {"原始大小", "缩放后大小", "缩放后大小的一半",
        "宽度为3840", "高度为2160", "宽度为1920", "高度为1080"}
    Public Size_rb_Checked_index As Integer
    Public Save_content_rb_Checked_index As Integer
    Public sd As Boolean
    Public r() As RadioButton
    'Public mw As MainWindow
    Dim dialogArgs1 As OpenDirectoryDialogArguments = New OpenDirectoryDialogArguments() With {
            .Width = Double.NaN,
            .Height = Double.NaN,
            .CreateNewDirectoryEnabled = True,
            .CurrentDirectory = GetKeyValue("main", "LastSavePath", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), inipath)
        }

    Public Sub init(ByVal w0 As Integer, h0 As Integer, scale As Double, setdefault As Boolean)
        r = {RadioButton1, RadioButton2, RadioButton3, RadioButton4,
        RadioButton5, RadioButton6, RadioButton7}
        w = {w0, Int(w0 * scale), Int(w0 * scale / 2), 3840, 2160 / h0 * w0, 1920, 1080 / h0 * w0}
        h = {h0, Int(h0 * scale), Int(w0 * scale / 2), 3840 / w0 * h0, 2160, 1920 / w0 * h0, 1080}
        For i = 0 To 6
            r(i).Content = c(i) + "（当前值：" + CStr(w(i)) + "×" + CStr(h(i)) + "）"
        Next
        If setdefault Then
            path = GetKeyValue("QuickSave", "LastSavePath", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), inipath)
            Size_rb_Checked_index = GetKeyValue("QuickSave", "Size_rb_Checked_index", 0, inipath)
            Save_content_rb_Checked_index = GetKeyValue("QuickSave", "Save_content_rb_Checked_index", 2, inipath)
        Else
            path = GetKeyValue("main", "LastSavePath", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), inipath)
            Size_rb_Checked_index = GetKeyValue("main", "Size_rb_Checked_index", 0, inipath)
            Save_content_rb_Checked_index = GetKeyValue("main", "Save_content_rb_Checked_index", 2, inipath)
        End If

        pathlabel.Text = path
        r(Size_rb_Checked_index).IsChecked = True
        Select Case Save_content_rb_Checked_index
            Case 0
                rb1.IsChecked = True
            Case 1
                rb2.IsChecked = True
            Case 2
                rb3.IsChecked = True
        End Select
        sd = setdefault
    End Sub

    Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
        'Transitioner.SelectedIndex = 1
        Dim result1 As OpenDirectoryDialogResult = Await OpenDirectoryDialog.ShowDialogAsync("MainDialogHost2", dialogArgs1)
        Console.WriteLine(result1)
        If result1.Confirmed Then
            path = result1.Directory
            pathlabel.Text = path
            dialogArgs1.CurrentDirectory = path
            SetKeyValue("main", "LastSavePath", path, inipath)
        End If
    End Sub

    Public Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        'Dim sample0_Thread As New Thread(AddressOf do_save)
        'sample0_Thread.Start()
        If sd Then
            SetKeyValue("QuickSave", "Save_content_rb_Checked_index", Save_content_rb_Checked_index, inipath)
            SetKeyValue("QuickSave", "Size_rb_Checked_index", Size_rb_Checked_index, inipath)
            SetKeyValue("QuickSave", "path", path, inipath)
        Else
            do_save()
        End If

    End Sub

    Public Sub do_save()
        Dim a As Integer, i As Integer
        a = Save_content_rb_Checked_index
        i = Size_rb_Checked_index
        SetKeyValue("main", "Save_content_rb_Checked_index", a, inipath)
        SetKeyValue("main", "Size_rb_Checked_index", i, inipath)
        save_queue.Add(New save_task() With {.a = a, .h = h(i), .w = w(i), .path = path})
        'mw.Save_snapshot(a, w(i), h(i), path)
        'mw.Dispatcher.BeginInvoke(New Action(Sub()
        '                                         mw.Save_snapshot(a, w(i), h(i), path)
        '                                     End Sub))
    End Sub

    Private Sub RadioButton_Checked(sender As Object, e As RoutedEventArgs)
        Size_rb_Checked_index = CInt(TryCast(sender, RadioButton).Tag)
    End Sub

    Private Sub Save_content_rb_Checked(sender As Object, e As RoutedEventArgs)
        Save_content_rb_Checked_index = CInt(TryCast(sender, RadioButton).Tag)
    End Sub
End Class
