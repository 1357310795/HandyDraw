Public Class SaveNoti1
    Inherits UserControl

    Public Savepath As String
    Public succeed As Boolean
    Public exp As Exception

    Public Sub New(issucceed As Boolean, path As String, ex As Exception)

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        Savepath = path
        succeed = issucceed
        exp = ex
        If succeed Then
            Transitioner.SelectedIndex = 1
            succeedText.Text = "文件已保存于 " + path
            succeedText.ToolTip = succeedText.Text
        Else
            Transitioner.SelectedIndex = 2
        End If
    End Sub
    Public Sub init(issucceed As Boolean, path As String, ex As Exception)
        Savepath = path
        succeed = issucceed
        exp = ex
        If succeed Then
            Transitioner.SelectedIndex = 1
            succeedText.Text = "文件已保存于 " + path
            succeedText.ToolTip = succeedText.Text
        Else
            Transitioner.SelectedIndex = 2
        End If
    End Sub

    Private Async Sub ButtonFail_Click(sender As Object, e As RoutedEventArgs)
        Await MaterialDesignThemes.Wpf.DialogHost.Show(New YesNoDialog(200, exp.Message), "MainDialogHost1")
    End Sub

    Private Sub ButtonOpen_Click(sender As Object, e As RoutedEventArgs)
        Process.Start(Savepath)
    End Sub

    Private Sub ButtonOpenDir_Click(sender As Object, e As RoutedEventArgs)
        Process.Start("explorer", New IO.FileInfo(Savepath).DirectoryName)
        'Process.Start("explorer", Savepath)
    End Sub

    Private Sub Close_MouseUp(sender As Object, e As MouseButtonEventArgs)
        closeme()
    End Sub

    Private Sub closeme()

        Dim sp As StackPanel = VisualTreeHelper.GetParent(Me)
        sp.Children.Remove(Me)
    End Sub
End Class
