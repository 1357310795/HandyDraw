Public Class AboutWindow
    Inherits Window

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub

    Private Sub Window_Closed(sender As Object, e As EventArgs)
        FlushMemory.Flush()
    End Sub

    Private Sub TextBlock_MouseDown(sender As Object, e As MouseButtonEventArgs)
        Clipboard.SetText("https://github.com/1357310795/HandyDraw")
        Snackbar1.MessageQueue.Enqueue(
                "链接已复制到剪切板",
                Nothing,
                Nothing,
                Nothing,
                False,
                True,
                TimeSpan.FromSeconds(1))
    End Sub

    Private Sub TextBlock_MouseDown_1(sender As Object, e As MouseButtonEventArgs)
        Clipboard.SetText("https://space.bilibili.com/171443036")
        Snackbar1.MessageQueue.Enqueue(
                "链接已复制到剪切板",
                Nothing,
                Nothing,
                Nothing,
                False,
                True,
                TimeSpan.FromSeconds(1))
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Me.Activate()
        Me.Focus()
        Dim a = CubicBezierDoubleAnimation(TimeSpan.FromSeconds(0.3), 0, 1, "0,1.25,.91,1.11")
        MyScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, a)
    End Sub
End Class
