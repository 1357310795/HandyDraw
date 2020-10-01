Imports System.Globalization
Imports System.Windows.Ink

Public Class SaveSettingMenu
    Inherits UserControl
    Public Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        Quickmodebutton.IsChecked = CType(GetKeyValue("QuickSave", "enabled", "false", inipath), Boolean)
    End Sub

    Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim res As String
        Dim s As SaveDialog = New SaveDialog
        s.init(1080, 1080, 1, True)
        res = Await MaterialDesignThemes.Wpf.DialogHost.Show(s, "MainDialogHost1")
    End Sub

    Private Sub Quickmodebutton_Checked(sender As Object, e As RoutedEventArgs) Handles Quickmodebutton.Checked
        SetKeyValue("QuickSave", "enabled", "true", inipath)
        e.Handled = True
    End Sub

    Private Sub Quickmodebutton_Unchecked(sender As Object, e As RoutedEventArgs) Handles Quickmodebutton.Unchecked
        SetKeyValue("QuickSave", "enabled", "false", inipath)
        e.Handled = True
    End Sub
End Class
