Imports System.Globalization
Imports System.Windows.Ink
Imports MaterialDesignThemes.Wpf

Public Class MoreMenu
    Inherits UserControl
    Public inkcanvas1 As InkCanvas
    Public mypopup As PopupBox
    Public Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Application.Current.MainWindow.Close()
    End Sub

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        Throw New Exception("Test!")
    End Sub

    Private Sub Button_Click_3(sender As Object, e As RoutedEventArgs)
        mypopup.IsPopupOpen = False
        Dim t As New SettingWindow
        t.Show()
    End Sub

    Private Sub Button_Click_4(sender As Object, e As RoutedEventArgs)
        mypopup.IsPopupOpen = False
        Dim t As New AboutWindow
        t.Show()
        t.Activate()
    End Sub


End Class
