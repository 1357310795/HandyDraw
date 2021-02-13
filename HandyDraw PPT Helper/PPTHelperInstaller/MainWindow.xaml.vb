Imports System.IO
Imports System.Reflection
Imports Ionic.Zip
Imports Microsoft.Win32

Class MainWindow
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim t As New Threading.Thread(AddressOf main)
        t.Start()
    End Sub
    Public Shared Sub UnPack(ByVal PackPath As String, ByVal FolerPath As String)
        Dim zip As New ZipFile
        zip = ZipFile.Read(PackPath)
        'zip.Password = Psd  '注意密码一定要放在读取后
        zip.ExtractAll(FolerPath, ExtractExistingFileAction.OverwriteSilently)
        zip.Dispose()
    End Sub
    Private Sub main()
        Dim rootpath = New DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName
        Dim installpath = Environment.GetEnvironmentVariable("LocalAppData") + "\HandyDraw\"
        Try
            UnPack(rootpath & "\bin.zip", installpath)
            Me.Dispatcher.Invoke(New Action(Of String)(AddressOf undate), "解压成功")
            Dim s1 = "HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run"
            Dim s2 = "PPTService"
            Dim s3 = """" & installpath & "loader.exe" & """"
            Registry.SetValue(s1, s2, s3, RegistryValueKind.String)
            Me.Dispatcher.Invoke(New Action(Of String)(AddressOf undate), "设置开机启动成功")
            Process.Start(s3)
            Me.Dispatcher.Invoke(New Action(Of String)(AddressOf undate), "程序启动成功")
            Threading.Thread.Sleep(1000)
            End
        Catch ex As Exception
            Me.Dispatcher.Invoke(New Action(Of String)(AddressOf undate), "错误：" & ex.Message)
        End Try
    End Sub
    Public Sub undate(x As String)
        ListBox1.Items.Add(x)
    End Sub
End Class
