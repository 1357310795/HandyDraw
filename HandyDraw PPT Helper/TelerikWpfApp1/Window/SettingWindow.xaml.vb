Public Class SettingWindow
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        ToggleButton1.IsChecked = CType(GetKeyValue("main", "StartAnimation", "true", inipath), Boolean)
    End Sub

    Private Sub ToggleButton1_Unchecked(sender As Object, e As RoutedEventArgs) Handles ToggleButton1.Unchecked
        SetKeyValue("main", "StartAnimation", "false", inipath)
    End Sub

    Private Sub ToggleButton1_Checked(sender As Object, e As RoutedEventArgs) Handles ToggleButton1.Checked
        SetKeyValue("main", "StartAnimation", "true", inipath)
    End Sub
End Class
