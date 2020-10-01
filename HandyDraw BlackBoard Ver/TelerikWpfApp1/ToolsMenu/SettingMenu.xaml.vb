Imports System.Globalization
Imports System.Windows.Ink

Public Class SettingMenu
    Inherits UserControl
    Public Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        If Application.Current.MainWindow.WindowStyle = WindowStyle.SingleBorderWindow Then
            check1.IsChecked = True
        End If
        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub

    Private Sub check1_Checked(sender As Object, e As RoutedEventArgs) Handles check1.Checked
        Application.Current.MainWindow.WindowStyle = WindowStyle.SingleBorderWindow
    End Sub

    Private Sub check1_Unchecked(sender As Object, e As RoutedEventArgs) Handles check1.Unchecked
        Application.Current.MainWindow.WindowStyle = WindowStyle.None
    End Sub
End Class
