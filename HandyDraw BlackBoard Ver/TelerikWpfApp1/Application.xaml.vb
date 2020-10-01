Imports System.Threading.Tasks

Class Application
    Inherits System.Windows.Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.
    Public mw As Window
    Protected Overrides Sub OnStartup(ByVal e As StartupEventArgs)
        MyBase.OnStartup(e)
        Dim splashScreen = New SplashScreenWindow()
        Me.MainWindow = splashScreen
        splashScreen.Show()
        Task.Factory.StartNew(Function()
                                  System.Threading.Thread.Sleep(1000)
                                  Me.Dispatcher.Invoke(Function()
                                                           mw = New MainWindow1()
                                                           Me.MainWindow = mw
                                                           MainWindow.Show()
                                                           splashScreen.Close()
                                                       End Function)
                              End Function)
    End Sub
End Class
