Imports System.IO
Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Timers
Imports System.Windows.Interop
Imports Microsoft.Office.Interop
Imports TelerikWpfApp1.logcat.Log

Class Application
    Inherits System.Windows.Application
    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    Private Declare Function IsWindow Lib "user32" Alias "IsWindow" (ByVal hwnd As Int32) As Int32
    Private Declare Function GetWindowRect Lib "user32" Alias "GetWindowRect" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer
    Private Declare Function GetClientRect Lib "user32" Alias "GetClientRect" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer
    Private Declare Function MoveWindow Lib "user32" Alias "MoveWindow" (ByVal hwnd As IntPtr,
                                                                         ByVal x As Integer,
                                                                         ByVal y As Integer,
                                                                         ByVal nWidth As Integer,
                                                                         ByVal nHeight As Integer,
                                                                         ByVal bRepaint As Boolean) As Integer
    Private Declare Function SetForegroundWindow Lib "user32" Alias "SetForegroundWindow" (ByVal hwnd As Int32) As Int32
    Private Declare Function SetProcessDPIAware Lib "user32" Alias "SetProcessDPIAware" () As Boolean
    Public mw As MainWindow1
    Dim prepare_timer, close_timer As Timer
    Dim ppt_hwnd As Int32

    Protected Overrides Sub OnStartup(ByVal e As StartupEventArgs)
        MyBase.OnStartup(e)
        'Dim splashScreen = New SplashScreenWindow()
        'Me.MainWindow = splashScreen
        'splashScreen.Show()
        AddHandler DispatcherUnhandledException, AddressOf App_DispatcherUnhandledException
        'SetProcessDPIAware()

        prepare_timer = New Timer
        prepare_timer.Interval = 1000
        close_timer = New Timer
        close_timer.Interval = 200
        AddHandler prepare_timer.Elapsed, AddressOf prepare
        AddHandler close_timer.Elapsed, AddressOf close
        prepare_timer.Start()
    End Sub

    Private Sub prepare()
        'ppt_hwnd = FindWindow("PPTFrameClass", vbNullString)
        ppt_hwnd = FindWindow("screenClass", vbNullString)
        If ppt_hwnd <> 0 Then
            prepare_timer.Stop()
            Task.Factory.StartNew(Sub()
                                      Me.Dispatcher.Invoke(Sub()
                                                               mw = New MainWindow1()
                                                               mw.ppt_hwnd = ppt_hwnd
                                                               GetWindowRect(ppt_hwnd, mw.ppt_rect)
                                                               mw.ppt_obj = New PowerPoint.Application
                                                               mw.ppt_view = mw.ppt_obj.ActivePresentation.SlideShowWindow.View
                                                               Me.MainWindow = mw
                                                               MainWindow.Show()
                                                           End Sub)
                                  End Sub)
            Dim seewo = Process.GetProcessesByName("PPTService")
            If seewo.Length <> 0 Then
                For Each i In seewo
                    i.Kill()
                Next
            End If
            close_timer.Start()
        End If
    End Sub

    Private Sub close()
        If IsWindow(ppt_hwnd) = 0 Then
            close_timer.Stop()
            If mw IsNot Nothing Then
                Me.Dispatcher.Invoke(Sub()
                                         mw.Close()
                                         mw = Nothing
                                         Me.MainWindow = Nothing
                                         'Application.Current.Shutdown()
                                     End Sub)
                'Console.WriteLine("WINDOW_CLOSE")
            End If
            FlushMemory.Flush()
            If updatehelper.updateok Then
                Dim t1 = New DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                Dim downini = t1.Parent.FullName & "\tmp.ini"
                Dim localini = t1.Parent.FullName & "\version.ini"
                Dim rootpath = t1.Parent.FullName
                Logger.Instance.WriteLog("运行loader")
                Process.Start(rootpath & "\loader.exe")
                End
                Logger.Instance.WriteLog("什么？END之后还能执行？")
            End If
            prepare_timer.Start()
        End If

    End Sub


    Private Sub App_DispatcherUnhandledException(ByVal sender As Object, ByVal e As System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)
        MessageBox.Show("程序异常." & Environment.NewLine + e.Exception.Message)
        e.Handled = True
    End Sub
End Class
