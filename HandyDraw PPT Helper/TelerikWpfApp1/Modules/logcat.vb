Imports System
Imports System.IO
Imports System.Reflection
Namespace logcat.Log
    Public Enum LogType
        All
        Information
        Debug
        Success
        Failure
        Warning
        [Error]
    End Enum

    Public Class Logger
        Private Shared logLock As Object
        Private Shared _instance As Logger
        Private Shared logFileName As String

        Private Sub New()
        End Sub

        Public Shared ReadOnly Property Instance As Logger
            Get

                If _instance Is Nothing Then
                    _instance = New Logger()
                    logLock = New Object()
                    logFileName = Guid.NewGuid().ToString & ".log"
                End If

                Return _instance
            End Get
        End Property

        Public Sub WriteLog(ByVal logContent As String, ByVal Optional logType As LogType = LogType.Information, ByVal Optional fileName As String = Nothing)
            Try
                Dim basePath As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                basePath = "C:\Users\13573\Source\Repos\ppthelper11.28\TelerikWpfApp1\bin"

                If Not Directory.Exists(basePath & "\Log") Then
                    Directory.CreateDirectory(basePath & "\Log")
                End If

                Dim dataString As String = DateTime.Now.ToString("yyyy-MM-dd")

                If Not Directory.Exists(basePath & "\Log\" & dataString) Then
                    Directory.CreateDirectory(basePath & "\Log\" & dataString)
                End If

                Dim logText As String() = New String() {DateTime.Now.ToString("hh:mm:ss") & ": " & logType.ToString() & ": " & logContent}

                If Not String.IsNullOrEmpty(fileName) Then
                    fileName = fileName & "_" & logFileName
                Else
                    fileName = logFileName
                End If

                SyncLock logLock
                    File.AppendAllLines(basePath & "\Log\" & dataString & "\" & fileName, logText)
                End SyncLock

            Catch __unusedException1__ As Exception
            End Try
        End Sub

        Public Sub WriteException(ByVal exception As Exception, ByVal Optional specialText As String = Nothing)
            If exception IsNot Nothing Then
                Dim exceptionType As Type = exception.[GetType]()
                Dim text As String = String.Empty

                If Not String.IsNullOrEmpty(specialText) Then
                    text = text & specialText & Environment.NewLine
                End If

                text = "Exception: " & exceptionType.Name & Environment.NewLine
                text += "               " & "Message: " & exception.Message & Environment.NewLine
                text += "               " & "Source: " & exception.Source & Environment.NewLine
                text += "               " & "StackTrace: " & exception.StackTrace & Environment.NewLine
                WriteLog(text, LogType.[Error])
            End If
        End Sub
    End Class

End Namespace

