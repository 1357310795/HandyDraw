Imports System.Timers
Imports System.Windows.Ink
Imports System.Windows.Interop
Imports System.Windows.Media.Animation
Imports Microsoft.Office.Interop

Class MainWindow1
    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    Private Declare Function SetForegroundWindow Lib "user32" Alias "SetForegroundWindow" (ByVal hwnd As Int32) As Int32
    Private Declare Function GetWindowRect Lib "user32" Alias "GetWindowRect" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer
    Private Declare Function GetClientRect Lib "user32" Alias "GetClientRect" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer
    Private Declare Function MoveWindow Lib "user32" Alias "MoveWindow" (ByVal hwnd As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bRepaint As Boolean) As Integer

    Public pen, marker As DrawingAttributes
    Public pen1, marker1 As DrawingAttributes
    Public settingwindow As UserControl

    Public Edit_Mode As Edit_Mode_Enum
    Public App_Mode As App_Mode_Enum
    Private ci As InkCanvas
    Private animation_timer As Timer

    Public Sub New()
        pen = New DrawingAttributes With {
            .Color = Color.FromRgb(245, 63, 54),
            .Height = 4,
            .Width = 4,
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
        pen1 = New DrawingAttributes With {
            .Color = Colors.Black,
            .Height = 4,
            .Width = 4,
            .FitToCurve = True,
            .IsHighlighter = False,
            .StylusTip = StylusTip.Ellipse
        }

        InitializeComponent()
        InkCanvas1.EraserShape = New Ink.RectangleStylusShape(40, 60)
        ci = InkCanvas1
        App_Mode = App_Mode_Enum.PPT
        _currentCanvasStrokes = New Dictionary(Of Integer, Stroke)()
        _lasttimestamp = New Dictionary(Of Integer, Double)
        _lastpoint = New Dictionary(Of Integer, StylusPoint)
        AddHandler InkCanvas1.TouchDown, AddressOf OnTouchDown
        AddHandler InkCanvas1.TouchUp, AddressOf OnTouchUp
        AddHandler InkCanvas1.TouchMove, AddressOf OnTouchMove
        AddHandler InkCanvas1.PreviewMouseDown, AddressOf OnMouseDown
        AddHandler InkCanvas1.MouseUp, AddressOf OnMouseUp
        'AddHandler InkCanvas1.MouseLeave, AddressOf OnMouseUp


        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        '启动动画
        If GetKeyValue("main", "StartAnimation", "true", inipath) = "true" Then
            animation_timer = New Timer
            animation_timer.Interval = 100
            AddHandler animation_timer.Elapsed, AddressOf animation_timer_tick
            animation_timer.Start()
        End If

        MoveWindow(New WindowInteropHelper(Me).Handle,
                                   ppt_rect.Left,
                                    ppt_rect.Top,
                                    ppt_rect.Right - ppt_rect.Left,
                                    ppt_rect.Bottom - ppt_rect.Top,
                                    True)

        ReDim inks(GetTotalSlideCount() + 2)
        For i = 0 To GetTotalSlideCount() + 2
            inks(i) = New StrokeCollection
        Next
        updatepage(1)
        InkCanvas1.Strokes = inks(currentpage)
        'AddHandler InkCanvas1.Strokes.StrokesChanged, AddressOf StrokesChanged

        Dim PenColorBinding As Binding = New Binding
        PenColorBinding.Source = pen
        PenColorBinding.Path = New PropertyPath("Color")
        PenColorBinding.Converter = New ColorValueConverter
        PenColorTip.SetBinding(Shape.FillProperty, PenColorBinding)

        Dim MarkerColorBinding As Binding = New Binding
        MarkerColorBinding.Source = marker
        MarkerColorBinding.Path = New PropertyPath("Color")
        MarkerColorBinding.Converter = New ColorValueConverter
        MarkerColorTip.SetBinding(Shape.FillProperty, MarkerColorBinding)

        SetForegroundWindow(New WindowInteropHelper(Me).Handle)

        update_timer.Interval = 1000
        AddHandler update_timer.Elapsed, AddressOf update_timer_Tick
        update_timer.Start()

        Dim u As New Threading.Thread(AddressOf updatehelper.updatemain)
        u.Start()
    End Sub


#Region "listboxTools"
    Public Sub Set_Edit_Mode(e As Edit_Mode_Enum)
        If App_Mode = App_Mode_Enum.PPT Then
            Select Case e
                Case Edit_Mode_Enum.Cursor
                    InkCanvas1.EditingMode = InkCanvasEditingMode.None
                    CursorRadioButton.IsChecked = True
                    InkCanvas1.Background = TryCast(Application.Current.Resources("TrueTransparent"), Brush)
                Case Edit_Mode_Enum.Pen
                    InkCanvas1.EditingMode = InkCanvasEditingMode.None
                    InkCanvas1.DefaultDrawingAttributes = pen
                    PenRadioButton.IsChecked = True
                    InkCanvas1.Background = TryCast(Application.Current.Resources("FakeTransparent"), Brush)
                Case Edit_Mode_Enum.Selectt
                    InkCanvas1.EditingMode = InkCanvasEditingMode.Select
                    InkCanvas1.Background = TryCast(Application.Current.Resources("FakeTransparent"), Brush)
                    SelectRadioButton.IsChecked = True
                Case Edit_Mode_Enum.Marker
                    InkCanvas1.EditingMode = InkCanvasEditingMode.Ink
                    InkCanvas1.DefaultDrawingAttributes = marker
                    MarkerRadioButton.IsChecked = True
                    InkCanvas1.Background = TryCast(Application.Current.Resources("FakeTransparent"), Brush)
                Case Edit_Mode_Enum.Eraser
                    If InkCanvas1.EditingMode <> InkCanvasEditingMode.EraseByStroke And
                    InkCanvas1.EditingMode <> InkCanvasEditingMode.EraseByPoint Then
                        InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint
                    End If
                    EraserRadioButton.IsChecked = True
                    InkCanvas1.Background = TryCast(Application.Current.Resources("FakeTransparent"), Brush)
            End Select
            Edit_Mode = e
        ElseIf App_Mode = App_Mode_Enum.Board Then
            Select Case e
                Case Edit_Mode_Enum.Cursor
                    bv.InkCanvas1.EditingMode = InkCanvasEditingMode.None
                    CursorRadioButton.IsChecked = True
                Case Edit_Mode_Enum.Pen
                    bv.InkCanvas1.EditingMode = InkCanvasEditingMode.None
                    bv.InkCanvas1.DefaultDrawingAttributes = pen1
                    PenRadioButton.IsChecked = True
                Case Edit_Mode_Enum.Selectt
                    bv.InkCanvas1.EditingMode = InkCanvasEditingMode.Select
                    SelectRadioButton.IsChecked = True
                Case Edit_Mode_Enum.Marker
                    bv.InkCanvas1.EditingMode = InkCanvasEditingMode.Ink
                    bv.InkCanvas1.DefaultDrawingAttributes = marker
                    MarkerRadioButton.IsChecked = True
                Case Edit_Mode_Enum.Eraser
                    If bv.InkCanvas1.EditingMode <> InkCanvasEditingMode.EraseByStroke And
                        bv.InkCanvas1.EditingMode <> InkCanvasEditingMode.EraseByPoint Then
                        bv.InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint
                    End If
                    EraserRadioButton.IsChecked = True
            End Select
            Edit_Mode = e
            bv.Edit_Mode = e
        End If
    End Sub
    Private Sub Cursor_Selected(sender As Object, e As RoutedEventArgs)
        Set_Edit_Mode(Edit_Mode_Enum.Cursor)
    End Sub

    Private Sub Pen_Selected(sender As Object, e As RoutedEventArgs)
        Set_Edit_Mode(Edit_Mode_Enum.Pen)
    End Sub

    Private Sub Select_Selected(sender As Object, e As RoutedEventArgs)
        Set_Edit_Mode(Edit_Mode_Enum.Selectt)
    End Sub

    Private Sub Marker_Selected(sender As Object, e As RoutedEventArgs)
        Set_Edit_Mode(Edit_Mode_Enum.Marker)
    End Sub

    Private Sub Eraser_Selected(sender As Object, e As RoutedEventArgs)
        Set_Edit_Mode(Edit_Mode_Enum.Eraser)
    End Sub
    Private Sub Setting_Selected(sender As Object, e As RoutedEventArgs)
        TryCast(sender, RadioButton).IsChecked = False
        More.mypopup = MorePopup
        MorePopup.IsPopupOpen = True
    End Sub
    Private Sub ListBoxItem_MouseUp(sender As Object, e As MouseEventArgs)
        'ListboxClickItem.IsSelected = True
    End Sub
    Private Sub ListBoxItem_PreviewMouseUp(sender As Object, e As MouseEventArgs)
        If TryCast(sender, RadioButton).IsChecked Then
            Select Case TryCast(sender, RadioButton).Tag
                Case "Pen"
                    'PenSettingPopup.IsPopupOpen = True
                    'If App_Mode = App_Mode_Enum.PPT Then
                    '    PenSetting.initdrawer(pen)
                    'ElseIf App_Mode = App_Mode_Enum.Board Then
                    '    PenSetting.initdrawer(pen1)
                    'End If
                    'PenSetting.popup = PenSettingPopup

                    Dim w As New PenSettingWindow
                     If App_Mode = App_Mode_Enum.PPT Then
                        w.initdrawer(pen)
                    ElseIf App_Mode = App_Mode_Enum.Board Then
                        w.initdrawer(pen1)
                    End If
                    Dim rr = New RECT()
                    Dim s = ScreenHelper.GetScalingRatio()
                    w.Show()
                    w.Top = TryCast(sender, RadioButton).PointToScreen(New Point(0, 0)).Y / s - w.ActualHeight
                    w.Left = TryCast(sender, RadioButton).PointToScreen(New Point(0, 0)).X / s
                    w.Focus()
                    Exit Sub
                Case "Marker"
                    MarkerSettingPopup.IsPopupOpen = True
                    MarkerSetting.initdrawer(marker)
                    MarkerSetting.popup = MarkerSettingPopup
                Case "Eraser"
                    EraserSettingPopup.IsPopupOpen = True
                    EraserSetting.initdrawerandcanvas(ci, Me)
                Case "Cursor"
                    Exit Sub
            End Select
        End If
    End Sub

#End Region
#Region "PPTControl"
    Public ppt_obj As PowerPoint.Application
    Friend ppt_rect As RECT
    Public ppt_view As PowerPoint.SlideShowView
    Public ppt_hwnd As Int32
    Public currentpage As Int32
    Private inks As StrokeCollection()
    Dim update_timer As New Timer
    Public erroccured As Boolean
    Private Sub ppt_next()
        ppt_view.Next()
        updatepage(1)
        SetForegroundWindow(ppt_hwnd)
    End Sub
    Private Sub ppt_prev()
        ppt_view.Previous()
        updatepage(-1)
        SetForegroundWindow(ppt_hwnd)
    End Sub
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        If App_Mode = App_Mode_Enum.PPT Then
            ppt_prev()
        ElseIf App_Mode = App_Mode_Enum.Board Then
            bv.PrevPage()
            TextPage.Text = bv.getlabel
            If bv.n = bv.inks.Count - 1 Then
                PageControlNextIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add
                PageControlNextText.Text = "加页"
            Else
                PageControlNextIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.KeyboardArrowRight
                PageControlNextText.Text = "下一页"
            End If
        End If
    End Sub
    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        If App_Mode = App_Mode_Enum.PPT Then
            ppt_next()
        ElseIf App_Mode = App_Mode_Enum.Board Then
            If bv.n = bv.inks.Count - 1 Then
                bv.AddPage()
                PageControlNextIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add
                PageControlNextText.Text = "加页"
            Else
                bv.NextPage()
                If bv.n = bv.inks.Count - 1 Then
                    PageControlNextIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add
                    PageControlNextText.Text = "加页"
                Else
                    PageControlNextIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.KeyboardArrowRight
                    PageControlNextText.Text = "下一页"
                End If
            End If
            TextPage.Text = bv.getlabel
        End If
    End Sub
    Private Function GetTotalSlideCount() As Int32
        Return ppt_obj.ActivePresentation.Slides.Count
    End Function
    Private Sub Exit_presentation(sender As Object, e As RoutedEventArgs)
        update_timer.Stop()
        ppt_view.Exit()
    End Sub
    Private Sub Window_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        If e.Delta < 0 Then
            ppt_next()
        Else
            ppt_prev()
        End If
    End Sub
    Private Sub Window_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Left Or e.Key = Key.PageUp Or e.Key = Key.Up Then
            ppt_prev()
        ElseIf e.Key = Key.Right Or e.Key = Key.PageDown Or e.Key = Key.Down Then
            ppt_next()
        ElseIf e.Key = Key.Escape Then
            ppt_view.Exit()
        End If
    End Sub
    Private Sub updatepage(Optional isnext As Int32 = 0)
        Try
            Dim tmp, tmp1 As Int32
            tmp = ppt_view.CurrentShowPosition
            If tmp = 0 Then
                tmp1 = currentpage + isnext
            Else
                tmp1 = tmp
            End If
            If tmp1 <> currentpage Then
                currentpage = tmp1
                Me.Dispatcher.Invoke(Sub()
                                         TextPage.Text = currentpage & "/" & GetTotalSlideCount()
                                         InkCanvas1.Strokes = inks(currentpage)
                                         ClearHistory()
                                     End Sub)
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            If update_timer IsNot Nothing Then
                update_timer.Stop()
            End If

            Dim t As System.Threading.Thread = New System.Threading.Thread(AddressOf err)
            t.Start()
        End Try
    End Sub
    Private Sub update_timer_Tick(sender As Object, e As EventArgs)
        updatepage()
        'Try
        'Catch ex As Exception
        '    Console.WriteLine(ex.Message)
        '    TryCast(sender, Timer).Stop()
        '    Dim t As System.Threading.Thread = New System.Threading.Thread(AddressOf err)
        '    t.Start()
        'End Try
    End Sub
    Private Sub err()
        'Me.Dispatcher.Invoke(Async Sub()
        'If MainDialogHost1.IsOpen Then
        '    Exit Sub
        'End If
        'Dim res As String
        'res = Await MaterialDesignThemes.Wpf.DialogHost.Show(New YesNoDialog(300, "程序出现内部错误，是否继续运行？"), "MainDialogHost1")
        'Console.WriteLine(res)
        'If res = "OK" Then
        '    System.Threading.Thread.Sleep(1000)
        '    update_timer.Start()
        'Else
        '    Application.Current.Shutdown()
        'End If

        'End Sub)
        If erroccured Then Exit Sub
        erroccured = True
        System.Threading.Thread.Sleep(2000)
        If update_timer IsNot Nothing Then update_timer.Start()
    End Sub
#End Region
#Region "MultiTouch"
    Private Const ThreasholdNearbyDistance As Double = 0.01
    Private ReadOnly _currentCanvasStrokes As Dictionary(Of Integer, Stroke)
    Private ReadOnly _lasttimestamp As Dictionary(Of Integer, Double)
    Private ReadOnly _lastpoint As Dictionary(Of Integer, StylusPoint)
    Private _strokeHitTester As IncrementalStrokeHitTester
    Private _addingStroke As Stroke
    Private maxv As Double = 200
    Private Sub StrokeHit(sender As Object, argsHitTester As StrokeHitEventArgs)
        Dim eraseResults = argsHitTester.GetPointEraseResults()
        InkCanvas1.Strokes.Remove(argsHitTester.HitStroke)
        InkCanvas1.Strokes.Add(eraseResults)
    End Sub

#Disable Warning BC40005 ' 成员隐藏基类型中的可重写的方法
    Private Sub OnTouchDown(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        Console.WriteLine("OnTouchDown")
        Dim touchPoint = touchEventArgs.GetTouchPoint(Me)
        Dim point = touchPoint.Position

        If InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint Then
            If _strokeHitTester Is Nothing Then
                _strokeHitTester = InkCanvas1.Strokes.GetIncrementalStrokeHitTester(InkCanvas1.EraserShape)
                AddHandler _strokeHitTester.StrokeHit, AddressOf StrokeHit
            End If
            _strokeHitTester.AddPoint(point)
            Return
        End If

        If Edit_Mode = Edit_Mode_Enum.Pen Then
            _addingStroke = New Stroke(New StylusPointCollection(New List(Of StylusPoint) From {
                New StylusPoint(point.X, point.Y, 0.5)
            }), InkCanvas1.DefaultDrawingAttributes.Clone)

            If Not _currentCanvasStrokes.ContainsKey(touchPoint.TouchDevice.Id) Then
                _currentCanvasStrokes.Add(touchPoint.TouchDevice.Id, _addingStroke)
                InkCanvas1.Strokes.Add(_addingStroke)
                _lasttimestamp.Add(touchPoint.TouchDevice.Id, DateTime.Now.Ticks / 1000000D)
                _lastpoint.Add(touchPoint.TouchDevice.Id, _addingStroke.StylusPoints(0))
            End If
        End If
    End Sub

    Private Sub OnTouchUp(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        Console.WriteLine("OnTouchUp")

        If InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint Then
            _strokeHitTester = Nothing
            Return
        End If

        If Edit_Mode = Edit_Mode_Enum.Pen Then
            Dim touchPoint = touchEventArgs.GetTouchPoint(Me)
            Dim spc As StylusPointCollection = _currentCanvasStrokes(touchPoint.TouchDevice.Id).StylusPoints
            Console.WriteLine(spc.Count)
            If (spc.Count > 5) Then
                If (spc(spc.Count - 2).PressureFactor < 0.8) Then
                    For i = 1 To 1 Step -1
                        Dim t As StylusPoint = spc(spc.Count - i)
                        t.PressureFactor = 0.1F + (spc(spc.Count - 2).PressureFactor - 0.1F) * (i - 1) / 2
                        spc(spc.Count - i) = t
                    Next
                End If
            End If
            _currentCanvasStrokes.Remove(touchPoint.TouchDevice.Id)
            _lasttimestamp.Remove(touchPoint.TouchDevice.Id)
            _lastpoint.Remove(touchPoint.TouchDevice.Id)
        End If
    End Sub

    Private Sub OnTouchMove(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        'Console.WriteLine("OnTouchMove")
        Dim touchPoint = touchEventArgs.GetTouchPoint(Me)
        Dim point = touchPoint.Position

        If InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint Then
            If _strokeHitTester IsNot Nothing Then
                _strokeHitTester.AddPoint(point)
            End If
            Return
        End If

        If Edit_Mode = Edit_Mode_Enum.Pen Then
            If _currentCanvasStrokes.ContainsKey(touchPoint.TouchDevice.Id) Then
                Dim stroke = _currentCanvasStrokes(touchPoint.TouchDevice.Id)
                Dim nearbyPoint = IsNearbyPoint(stroke, point)

                If Not nearbyPoint Then
                    Dim sp As StylusPoint = New StylusPoint(point.X, point.Y)
                    Dim nowtime As Double = DateTime.Now.Ticks / 1000000D
                    Dim v = (point - _lastpoint(touchPoint.TouchDevice.Id).ToPoint).Length / (nowtime - _lasttimestamp(touchPoint.TouchDevice.Id))
                    'If (Double.IsNaN(v) Or v > maxv) Then
                    '    sp.PressureFactor = 0.2F
                    'Else
                    '    sp.PressureFactor = CType((0.8F - (0.6F / maxv) * v), Single)
                    'End If
                    sp.PressureFactor = CType(calc_pressure(v), Single)
                    stroke.StylusPoints.Add(sp)
                    _lastpoint(touchPoint.TouchDevice.Id) = sp
                    _lasttimestamp(touchPoint.TouchDevice.Id) = nowtime
                    'Application.Current.Resources("speed") = v.ToString()
                    'l.Add(v)
                    'If (Not Double.IsNaN(v) And Not Double.IsPositiveInfinity(v)) Then
                    '    Application.Current.Resources("speedint") = v
                    'End If
                End If
            End If
        End If
    End Sub

    Private Sub OnMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
        Console.WriteLine("OnMouseDown")
        If e.StylusDevice IsNot Nothing Then Return
        Dim point = e.GetPosition(InkCanvas1)
        If Edit_Mode = Edit_Mode_Enum.Pen Then
            InkCanvas1.EditingMode = InkCanvasEditingMode.Ink
            InkCanvas1.CaptureMouse()
        End If
    End Sub

    Private Sub OnMouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        Console.WriteLine("OnMouseUp")
        CompareStrokes()
        PushToHistory()
        If e.StylusDevice IsNot Nothing Then Return
        If Edit_Mode = Edit_Mode_Enum.Pen Then
            InkCanvas1.EditingMode = InkCanvasEditingMode.None
            InkCanvas1.ReleaseMouseCapture()
        End If
    End Sub


    Private Shared Function IsNearbyPoint(ByVal stroke As Stroke, ByVal point As Point) As Boolean
        Return stroke.StylusPoints.Any(Function(p) (Math.Abs(p.X - point.X) <= ThreasholdNearbyDistance) AndAlso (Math.Abs(p.Y - point.Y) <= ThreasholdNearbyDistance))
    End Function

    Private Function calc_pressure(v As Double) As Double
        Dim a = -1.7
        Dim b = 0.6
        Dim c = 80
        If v = Double.NaN Then Return 0.5
        Dim f = -(2 * b * Math.Atan(v / c + a)) / Math.PI + b
        Return f
    End Function
#End Region
#Region "Animation"
    Private Sub animation_timer_tick()
        animation_timer.Stop()
        Me.Dispatcher.Invoke(AddressOf startanimation)
    End Sub

    Private Sub startanimation()
        logogrid.Visibility = Visibility.Visible

        '开源免费 位移
        Dim doubleKeyFrame1 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame1.KeyTime = TimeSpan.FromSeconds(0.0)
        doubleKeyFrame1.Value = 40
        Dim splineDoubleKeyFrame As SplineDoubleKeyFrame = New SplineDoubleKeyFrame()
        splineDoubleKeyFrame.KeyTime = TimeSpan.FromSeconds(0.4)
        Dim controlPoint As Point = New Point(0, 0.25) 'cubic-bezier(0,.25,.36,1)
        Dim controlPoint2 As Point = New Point(0.36, 1)
        splineDoubleKeyFrame.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint, .ControlPoint2 = controlPoint2}
        splineDoubleKeyFrame.Value = 0
        Dim doubleKeyFramea As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFramea.KeyTime = TimeSpan.FromSeconds(0.7)
        doubleKeyFramea.Value = 0
        Dim doubleKeyFrame2 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame2.KeyTime = TimeSpan.FromSeconds(1)
        doubleKeyFrame2.Value = 40
        Dim logo2animation2 = New DoubleAnimationUsingKeyFrames
        logo2animation2.KeyFrames.Add(doubleKeyFrame1)
        logo2animation2.KeyFrames.Add(splineDoubleKeyFrame)
        logo2animation2.KeyFrames.Add(doubleKeyFramea)
        logo2animation2.KeyFrames.Add(doubleKeyFrame2)
        Dim x As New TranslateTransform
        logo2.RenderTransform = x

        '开源免费 透明度
        Dim doubleKeyFrame3 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame3.KeyTime = TimeSpan.FromSeconds(0.0)
        doubleKeyFrame3.Value = 0
        Dim splineDoubleKeyFrame2 As SplineDoubleKeyFrame = New SplineDoubleKeyFrame()
        splineDoubleKeyFrame2.KeyTime = TimeSpan.FromSeconds(0.4)
        Dim controlPoint3 As Point = New Point(0, 0.25)
        Dim controlPoint4 As Point = New Point(0.36, 1)
        splineDoubleKeyFrame2.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint3, .ControlPoint2 = controlPoint4}
        splineDoubleKeyFrame2.Value = 1
        Dim doubleKeyFrameb As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrameb.KeyTime = TimeSpan.FromSeconds(0.7)
        doubleKeyFrameb.Value = 1
        Dim doubleKeyFrame4 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame4.KeyTime = TimeSpan.FromSeconds(1)
        doubleKeyFrame4.Value = 0
        Dim logo2animation As New DoubleAnimationUsingKeyFrames
        logo2animation.KeyFrames.Add(doubleKeyFrame3)
        logo2animation.KeyFrames.Add(splineDoubleKeyFrame2)
        logo2animation.KeyFrames.Add(doubleKeyFrameb)
        logo2animation.KeyFrames.Add(doubleKeyFrame4)

        'HandyDrawPPTHelper 透明度
        Dim doubleKeyFrame5 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame5.KeyTime = TimeSpan.FromSeconds(0.0)
        doubleKeyFrame5.Value = 0
        Dim splineDoubleKeyFrame3 As SplineDoubleKeyFrame = New SplineDoubleKeyFrame()
        splineDoubleKeyFrame3.KeyTime = TimeSpan.FromSeconds(0.4)
        Dim controlPoint5 As Point = New Point(0, 0.48) 'cubic-bezier(0,.48,.8,.99)
        Dim controlPoint6 As Point = New Point(0.8, 0.99)
        splineDoubleKeyFrame3.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint5, .ControlPoint2 = controlPoint6}
        splineDoubleKeyFrame3.Value = 1
        Dim doubleKeyFramec As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFramec.KeyTime = TimeSpan.FromSeconds(0.7)
        doubleKeyFramec.Value = 1
        Dim doubleKeyFrame6 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame6.KeyTime = TimeSpan.FromSeconds(1)
        doubleKeyFrame6.Value = 0
        Dim logo1animation As New DoubleAnimationUsingKeyFrames
        logo1animation.KeyFrames.Add(doubleKeyFrame5)
        logo1animation.KeyFrames.Add(splineDoubleKeyFrame3)
        logo1animation.KeyFrames.Add(doubleKeyFramec)
        logo1animation.KeyFrames.Add(doubleKeyFrame6)

        '背景颜色
        Dim ColorKeyFrame7 As ColorKeyFrame = New LinearColorKeyFrame()
        ColorKeyFrame7.KeyTime = TimeSpan.FromSeconds(0.0)
        ColorKeyFrame7.Value = Color.FromArgb(0, 0, 0, 0)
        Dim splineColorKeyFrame4 As SplineColorKeyFrame = New SplineColorKeyFrame()
        splineColorKeyFrame4.KeyTime = TimeSpan.FromSeconds(0.5)
        Dim controlPoint7 As Point = New Point(0, 0.48) 'cubic-bezier(0,.48,.8,.99)
        Dim controlPoint8 As Point = New Point(0.8, 0.99)
        splineColorKeyFrame4.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint7, .ControlPoint2 = controlPoint8}
        splineColorKeyFrame4.Value = Color.FromArgb(150, 0, 0, 0)
        Dim ColorKeyFramed As ColorKeyFrame = New LinearColorKeyFrame()
        ColorKeyFramed.KeyTime = TimeSpan.FromSeconds(0.7)
        ColorKeyFramed.Value = Color.FromArgb(150, 0, 0, 0)
        Dim ColorKeyFrame8 As ColorKeyFrame = New LinearColorKeyFrame()
        ColorKeyFrame8.KeyTime = TimeSpan.FromSeconds(1)
        ColorKeyFrame8.Value = Color.FromArgb(0, 0, 0, 0)
        Dim backanimation As New ColorAnimationUsingKeyFrames
        backanimation.KeyFrames.Add(ColorKeyFrame7)
        backanimation.KeyFrames.Add(splineColorKeyFrame4)
        backanimation.KeyFrames.Add(ColorKeyFramed)
        backanimation.KeyFrames.Add(ColorKeyFrame8)

        '进度条 透明度
        Dim doubleKeyFrame7 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame7.KeyTime = TimeSpan.FromSeconds(0.6)
        doubleKeyFrame7.Value = 1
        Dim doubleKeyFramee As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFramee.KeyTime = TimeSpan.FromSeconds(0.7)
        doubleKeyFramee.Value = 1
        Dim doubleKeyFrame8 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame8.KeyTime = TimeSpan.FromSeconds(1)
        doubleKeyFrame8.Value = 0
        Dim progressanimation As New DoubleAnimationUsingKeyFrames
        progressanimation.KeyFrames.Add(doubleKeyFrame7)
        progressanimation.KeyFrames.Add(doubleKeyFramee)
        progressanimation.KeyFrames.Add(doubleKeyFrame8)

        '进度条 值
        Dim doubleKeyFrame11 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame11.KeyTime = TimeSpan.FromSeconds(0)
        doubleKeyFrame11.Value = 0
        Dim doubleKeyFrame12 As DoubleKeyFrame = New LinearDoubleKeyFrame()
        doubleKeyFrame12.KeyTime = TimeSpan.FromSeconds(1)
        doubleKeyFrame12.Value = 100
        Dim proganimation As New DoubleAnimationUsingKeyFrames
        proganimation.KeyFrames.Add(doubleKeyFrame11)
        proganimation.KeyFrames.Add(doubleKeyFrame12)

        AddHandler progressanimation.Completed, AddressOf animationend
        x.BeginAnimation(TranslateTransform.YProperty, logo2animation2)
        logo2.BeginAnimation(UIElement.OpacityProperty, logo2animation)
        logo1.BeginAnimation(UIElement.OpacityProperty, logo1animation)
        logogrid.Background.BeginAnimation(SolidColorBrush.ColorProperty, backanimation)
        loadprog.BeginAnimation(UIElement.OpacityProperty, progressanimation)
        loadprog.BeginAnimation(ProgressBar.ValueProperty, proganimation)
    End Sub

    Private Sub animationend()
        MainGrid.Children.Remove(logogrid)
        GC.Collect()
    End Sub
#End Region
#Region "History"
    Private ReadOnly _history As New Stack(Of StrokesHistoryNode)
    Private ReadOnly _redoHistory As New Stack(Of StrokesHistoryNode)
    Private _ignoreStrokesChange As Boolean
    Private strokeadded, strokeremoved As New StrokeCollection
    Private PreStrokes As New StrokeCollection

    Private Sub Undo()
        CompareStrokes()
        If strokeadded.Count <> 0 Or strokeremoved.Count <> 0 Then
            PushToHistory()
        End If
        If Not CanUndo() Then Return

        Dim last = Pop(_history)
        _ignoreStrokesChange = True

        InkCanvas1.Strokes.Add(last.StrokesRemoved)
        InkCanvas1.Strokes.Remove(last.StrokesAdded)
        PreStrokes.Add(last.StrokesRemoved)
        PreStrokes.Remove(last.StrokesAdded)

        _ignoreStrokesChange = False
        Push(_redoHistory, last)
    End Sub

    Private Sub Redo()
        CompareStrokes()
        If strokeadded.Count <> 0 Or strokeremoved.Count <> 0 Then
            PushToHistory()
            Return
        End If
        If Not CanRedo() Then Return
        Dim last = Pop(_redoHistory)
        _ignoreStrokesChange = True

        InkCanvas1.Strokes.Remove(last.StrokesRemoved)
        InkCanvas1.Strokes.Add(last.StrokesAdded)
        PreStrokes.Add(last.StrokesAdded)
        PreStrokes.Remove(last.StrokesRemoved)

        _ignoreStrokesChange = False
        Push(_history, last)
    End Sub

    Private Shared Sub Push(ByVal collection As Stack(Of StrokesHistoryNode), ByVal node As StrokesHistoryNode)
        collection.Push(node)
    End Sub

    Private Shared Function Pop(ByVal collection As Stack(Of StrokesHistoryNode)) As StrokesHistoryNode
        Return If(collection.Count = 0, Nothing, collection.Pop())
    End Function

    Private Function CanUndo() As Boolean
        Return _history.Count <> 0
    End Function

    Private Function CanRedo() As Boolean
        Return _redoHistory.Count <> 0
    End Function

    'Private Sub StrokesChanged(ByVal sender As Object, ByVal e As StrokeCollectionChangedEventArgs)
    '    If _ignoreStrokesChange Then Exit Sub
    '    For Each i In e.Added
    '        strokeadded.Add(i)
    '    Next
    '    For Each i In e.Removed
    '        strokeremoved.Add(i)
    '    Next
    '    'strokeadded = TryCast(strokeadded.Concat(e.Added), StrokeCollection)
    '    'strokeremoved = TryCast(strokeremoved.Concat(e.Removed), StrokeCollection)
    'End Sub
    Public Sub CompareStrokes()
        Dim t As New StrokeCollection
        For Each s In PreStrokes
            If Not InkCanvas1.Strokes.Contains(s) Then
                strokeremoved.Add(s)
                t.Add(s)
            End If
        Next
        For Each s In t
            PreStrokes.Remove(s)
        Next
        For Each s In InkCanvas1.Strokes
            If Not PreStrokes.Contains(s) Then
                strokeadded.Add(s)
                PreStrokes.Add(s)
            End If
        Next
    End Sub

    Private Sub PushToHistory()
        If strokeadded.Count = 0 And strokeremoved.Count = 0 Then Return
        Dim t As New StrokesHistoryNode()
        t.StrokesAdded = strokeadded
        t.StrokesRemoved = strokeremoved
        Push(_history, t)

        strokeadded = New StrokeCollection()
        strokeremoved = New StrokeCollection()
        ClearHistory(_redoHistory)
    End Sub

    Private Sub ClearHistory()
        ClearHistory(_history)
        ClearHistory(_redoHistory)
    End Sub

    Private Shared Sub ClearHistory(ByVal collection As Stack(Of StrokesHistoryNode))
        collection?.Clear()
    End Sub

    Public Sub Clear()
        InkCanvas1.Strokes.Clear()
        ClearHistory()
        FlushMemory.Flush()
    End Sub

    Private Sub AnimatedClear()
        Dim ani = New DoubleAnimation(0, New Duration(New TimeSpan(0, 0, 0, 0, 3)))
        AddHandler ani.Completed, AddressOf ClearAniComplete
        InkCanvas1.BeginAnimation(OpacityProperty, ani)
    End Sub

    Private Sub ClearAniComplete(ByVal sender As Object, ByVal e As EventArgs)
        Clear()
        InkCanvas1.BeginAnimation(OpacityProperty, New DoubleAnimation(1, New Duration(New TimeSpan(0, 0, 0, 0, 3))))
    End Sub

    Private Sub Redo_Selected(sender As Object, e As RoutedEventArgs)
        TryCast(sender, RadioButton).IsChecked = False
        If App_Mode = App_Mode_Enum.PPT Then
            Redo()
        ElseIf App_Mode = App_Mode_Enum.Board Then
            bv.Redo()
        End If

    End Sub

    Private Sub Undo_Selected(sender As Object, e As RoutedEventArgs)
        TryCast(sender, RadioButton).IsChecked = False
        If App_Mode = App_Mode_Enum.PPT Then
            Undo()
        ElseIf App_Mode = App_Mode_Enum.Board Then
            bv.Undo()
        End If
    End Sub
#End Region
    Private Sub Set_App_Mode(e As App_Mode_Enum)
        If e = App_Mode_Enum.Board Then
            App_Mode = e
            update_timer.Stop()
            ci = bv.InkCanvas1
            BoardGrid.Visibility = Visibility.Visible
            Dim da = CubicBezierDoubleAnimation(TimeSpan.FromSeconds(0.3), 0, MainGrid.ActualHeight, "0,.96,.8,1")
            BoardGrid.BeginAnimation(Grid.HeightProperty, da)
            TextPage.Text = bv.getlabel
            If bv.n = bv.inks.Count - 1 Then
                PageControlNextIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Add
                PageControlNextText.Text = "加页"
            Else
                PageControlNextIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.KeyboardArrowRight
                PageControlNextText.Text = "下一页"
            End If
            Set_Edit_Mode(Edit_Mode_Enum.Pen)
            ButtonBoardIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.KeyboardBackspace
            ButtonBoardText.Text = "返回"
            ButtonCameraIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WebCamera
            ButtonCameraText.Text = "视频展台"
        ElseIf e = App_Mode_Enum.ppt Then
            App_Mode = e
            ci = InkCanvas1
            Dim da = CubicBezierDoubleAnimation(TimeSpan.FromSeconds(0.3), MainGrid.ActualHeight, 0, "0,.96,.8,1")
            BoardGrid.BeginAnimation(Grid.HeightProperty, da)
            'BoardGrid.Visibility = Visibility.Collapsed
            TextPage.Text = currentpage & "/" & GetTotalSlideCount()
            update_timer.Start()
            PageControlNextIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.KeyboardArrowRight
            PageControlNextText.Text = "下一页"
            Set_Edit_Mode(Edit_Mode_Enum.Pen)
            ButtonBoardIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WebAsset
            ButtonBoardText.Text = "白板"
            ButtonCameraIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.WebCamera
            ButtonCameraText.Text = "视频展台"
        ElseIf e = App_Mode_Enum.Camera Then

        End If

    End Sub

    Private Sub Button_Board_Click(sender As Object, e As RoutedEventArgs)
        If App_Mode = App_Mode_Enum.PPT Then
            Set_App_Mode(App_Mode_Enum.Board)
        ElseIf App_Mode = App_Mode_Enum.Board Then
            Set_App_Mode(App_Mode_Enum.PPT)
        ElseIf App_Mode = App_Mode_Enum.Camera Then
            Set_App_Mode(App_Mode_Enum.Camera)
        End If
    End Sub

    Private Sub Button_Camera_Click(sender As Object, e As RoutedEventArgs)
        If App_Mode = App_Mode_Enum.PPT Then
            Set_App_Mode(App_Mode_Enum.Camera)
        ElseIf App_Mode = App_Mode_Enum.Board Then
            Set_App_Mode(App_Mode_Enum.Camera)
        ElseIf App_Mode = App_Mode_Enum.Camera Then
            Set_App_Mode(App_Mode_Enum.PPT)
        End If
    End Sub

    Private Class ColorValueConverter
        Implements IValueConverter
        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            Dim c As Color = CType(value, Color)
            Dim b As SolidColorBrush = New SolidColorBrush(c)
            Return b
        End Function
        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function
    End Class


    'Private Sub startnotianimation(c As Canvas, n As UserControl1)
    '    Dim doubleKeyFrame1 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame1.KeyTime = TimeSpan.FromSeconds(0.0)
    '    doubleKeyFrame1.Value = 0
    '    Dim splineDoubleKeyFrame As SplineDoubleKeyFrame = New SplineDoubleKeyFrame()
    '    splineDoubleKeyFrame.KeyTime = TimeSpan.FromSeconds(0.5)
    '    Dim controlPoint As Point = New Point(0.3, 1.01) 'cubic-bezier(.3,1.01,.64,1.19)
    '    Dim controlPoint2 As Point = New Point(0.64, 1.19)
    '    splineDoubleKeyFrame.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint, .ControlPoint2 = controlPoint2}
    '    splineDoubleKeyFrame.Value = n.Width
    '    Console.WriteLine(n.Width)
    '    Dim doubleKeyFramea As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFramea.KeyTime = TimeSpan.FromSeconds(2.5)
    '    doubleKeyFramea.Value = n.Width
    '    Dim doubleKeyFrame2 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame2.KeyTime = TimeSpan.FromSeconds(3)
    '    doubleKeyFrame2.Value = 0
    '    Dim a = New DoubleAnimationUsingKeyFrames
    '    a.KeyFrames.Add(doubleKeyFrame1)
    '    a.KeyFrames.Add(splineDoubleKeyFrame)
    '    a.KeyFrames.Add(doubleKeyFramea)
    '    a.KeyFrames.Add(doubleKeyFrame2)

    '    AddHandler a.Completed, AddressOf notianimationend
    '    c.BeginAnimation(Canvas.WidthProperty, a)
    'End Sub

    'Private Sub notianimationend(sender As Object, e As EventArgs)
    '    Dim a As DoubleAnimationUsingKeyFrames = TryCast(sender, DoubleAnimationUsingKeyFrames)
    '    a = Nothing
    '    NotiStackPanel.Children.Clear()
    'End Sub
    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        update_timer.Stop()
        update_timer = Nothing
    End Sub
End Class

