Imports System.Threading.Tasks
Imports System.Windows.Ink
Imports System.Windows.Media.Animation

Class MainWindow1

    Public pen As DrawingAttributes
    Public marker As DrawingAttributes
    Public eraser As DrawingAttributes
    Public settingwindow As UserControl
    Private now_state As Now_state_enum
    Private Save_leftclicked As Boolean
    Private s As List(Of StrokeCollection)
    Private n As Int32
    Enum Now_state_enum As Integer
        Cursor = 1
        Pen = 2
        Marker = 4
        Eraser = 8
    End Enum

    Public Sub New()
        pen = New DrawingAttributes With {
            .Color = Colors.White,
            .Height = 3,
            .Width = 3,
            .FitToCurve = True,
            .IsHighlighter = False,
            .StylusTip = StylusTip.Ellipse
        }
        marker = New DrawingAttributes With {
            .Color = Colors.Yellow,
            .Height = 25,
            .Width = 10,
            .FitToCurve = False,
            .IsHighlighter = True,
            .StylusTip = StylusTip.Rectangle
        }
        eraser = New DrawingAttributes With {
            .Color = Colors.White,
            .Height = 25,
            .Width = 25,
            .FitToCurve = False,
            .IsHighlighter = True,
            .StylusTip = StylusTip.Ellipse
        }
        s = New List(Of StrokeCollection)
        s.Add(New StrokeCollection())
        n = 0
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        save_queue = New List(Of save_task)
    End Sub

#Region "listboxTools"
    Private Sub Cursor_Selected(sender As Object, e As RoutedEventArgs)
        cv.InkCanvas1.EditingMode = InkCanvasEditingMode.None
        now_state = Now_state_enum.Cursor
    End Sub

    Private Sub Pen_Selected(sender As Object, e As RoutedEventArgs)
        cv.InkCanvas1.EditingMode = InkCanvasEditingMode.Ink
        cv.InkCanvas1.DefaultDrawingAttributes = pen
        now_state = Now_state_enum.Pen
    End Sub

    Private Sub Marker_Selected(sender As Object, e As RoutedEventArgs)
        cv.InkCanvas1.EditingMode = InkCanvasEditingMode.Ink
        cv.InkCanvas1.DefaultDrawingAttributes = marker
        now_state = Now_state_enum.Marker
    End Sub

    Private Sub Eraser_Selected(sender As Object, e As RoutedEventArgs)
        If cv.InkCanvas1.EditingMode <> InkCanvasEditingMode.EraseByStroke And
                cv.InkCanvas1.EditingMode <> InkCanvasEditingMode.EraseByPoint Then
            cv.InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint
        End If
        now_state = Now_state_enum.Eraser
    End Sub

    Private Sub Save_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If Not Save_leftclicked Then
            SaveSettingPopup.IsPopupOpen = True
        End If
    End Sub

    Private Sub Save_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.RightButton = MouseButtonState.Pressed Then
            'SaveSettingPopup.IsPopupOpen = True
            Save_leftclicked = False
        Else
            Save_leftclicked = True
        End If
    End Sub

    Private Async Sub Save_checked(sender As Object, e As RoutedEventArgs)
        Console.WriteLine("Save_checked")
        If Save_leftclicked Then
            Await create_save_task()
            TryCast(sender, RadioButton).IsChecked = False
        Else
            SaveSettingPopup.IsPopupOpen = True
        End If
    End Sub

    Private Sub Setting_Selected(sender As Object, e As RoutedEventArgs)
        TryCast(sender, RadioButton).IsChecked = False
        SettingPopup.IsPopupOpen = True
    End Sub

    Private Sub ListBoxItem_MouseUp(sender As Object, e As MouseEventArgs)
        'ListboxClickItem.IsSelected = True
    End Sub
    Private Sub ListBoxItem_PreviewMouseUp(sender As Object, e As MouseEventArgs)
        If TryCast(sender, RadioButton).IsChecked Then
            Select Case TryCast(sender, RadioButton).Tag
                Case "Pen"
                    'settingwindow = New PenSetting(pen)
                    PenSettingPopup.IsPopupOpen = True
                    PenSetting.initdrawer(pen)
                    Exit Sub
                Case "Marker"
                    MarkerSettingPopup.IsPopupOpen = True
                    MarkerSetting.initdrawer(marker)
                Case "Eraser"
                    EraserSettingPopup.IsPopupOpen = True
                    EraserSetting.initdrawerandcanvas(cv.InkCanvas1, eraser)
                Case "Cursor"
                    Exit Sub
            End Select
        End If
    End Sub

#End Region
    Private Async Function create_save_task() As Task
        If CType(GetKeyValue("QuickSave", "enabled", "false", inipath), Boolean) Then
            Dim path = GetKeyValue("QuickSave", "LastSavePath", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), inipath)
            Dim i = GetKeyValue("QuickSave", "Size_rb_Checked_index", 0, inipath)
            Dim a = GetKeyValue("QuickSave", "Save_content_rb_Checked_index", 2, inipath)
            Dim w0 = cv.Grid1.ActualWidth
            Dim h0 = cv.Grid1.ActualHeight
            Dim warr = {w0, Int(w0 * cv.scale), Int(w0 * cv.scale / 2), 3840, 2160 / h0 * w0, 1920, 1080 / h0 * w0}
            Dim harr = {h0, Int(h0 * cv.scale), Int(w0 * cv.scale / 2), 3840 / w0 * h0, 2160, 1920 / w0 * h0, 1080}
            save_queue.Add(New save_task() With {.a = a, .h = harr(i), .w = warr(i), .path = path})
            Save_snapshot(save_queue.Item(0))
            save_queue.RemoveAt(0)
        Else
            Dim res As String
            Dim s As SaveDialog = New SaveDialog
            s.init(cv.Grid1.ActualWidth, cv.Grid1.ActualHeight, cv.scale, False)
            res = Await MaterialDesignThemes.Wpf.DialogHost.Show(s, "MainDialogHost1")
            Console.WriteLine(res)
            If res = "OK" Then
                Save_snapshot(save_queue.Item(0))
                save_queue.RemoveAt(0)
            End If
        End If
    End Function

    Private Sub Save_snapshot(ByVal t As save_task)
        Dim a = t.a
        Dim w = t.w
        Dim h = t.h
        Dim path = t.path
        Dim fullpath = path & xiegang(path) & getsnapshotname()
        Try
            Select Case a
                Case 0
                    RenderVisual(cv.InkCanvas1, fullpath, w, h)
                Case 1
                    RenderVisual(cv.MyBackControl, fullpath, w, h)
                Case 2
                    RenderVisual(cv.Grid1, fullpath, w, h)
            End Select

            Dim sn As New SaveNoti1(True, fullpath, Nothing)
            NotiStackPanel.Children.Add(sn)

        Catch ex As Exception
            Dim sn As New SaveNoti1(False, Nothing, ex)
            NotiStackPanel.Children.Add(sn)
        End Try
    End Sub

    Private Async Sub ButtonExit_Click(sender As Object, e As RoutedEventArgs)
        Dim res As String
        res = Await MaterialDesignThemes.Wpf.DialogHost.Show(New YesNoDialog(300, "确定退出？"), "MainDialogHost1")
        Console.WriteLine(res)
        If res = "OK" Then
            Application.Current.Shutdown()
        End If
    End Sub

    Private Sub ButtonMini_Click(sender As Object, e As RoutedEventArgs)
        Me.WindowState = WindowState.Minimized
    End Sub

    Private Sub ButtonAdd_Click(sender As Object, e As RoutedEventArgs)
        s(n) = cv.InkCanvas1.Strokes
        s.Add(New StrokeCollection)
        n = s.Count - 1
        cv.InkCanvas1.Strokes = s(n)
        TextPage.Text = CStr(n + 1) + "/" + CStr(s.Count)
    End Sub

    Private Sub ButtonPre_Click(sender As Object, e As RoutedEventArgs)
        If n = 0 Then
            Exit Sub
        End If
        s(n) = cv.InkCanvas1.Strokes
        cv.InkCanvas1.Strokes = s(n)
        n = n - 1
        cv.InkCanvas1.Strokes = s(n)
        TextPage.Text = CStr(n + 1) + "/" + CStr(s.Count)
    End Sub

    Private Sub ButtonNext_Click(sender As Object, e As RoutedEventArgs)
        If n = s.Count - 1 Then
            Exit Sub
        End If
        s(n) = cv.InkCanvas1.Strokes
        cv.InkCanvas1.Strokes = s(n)
        n = n + 1
        cv.InkCanvas1.Strokes = s(n)
        TextPage.Text = CStr(n + 1) + "/" + CStr(s.Count)
    End Sub
End Class
