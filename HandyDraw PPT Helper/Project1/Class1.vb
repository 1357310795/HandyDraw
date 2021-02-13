Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices

Public Module m
    Public baseURL = New DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName
    Public inipath As String = baseURL & "\version.ini"
    Public Sub Main()
        'MsgBox(inipath)
        Dim ver = GetKeyValue("main", "ver", "", inipath)
        If ver = "" Then
            MsgBox("无法定位到程序！")
            Exit Sub
        End If
        Try
            Process.Start(baseURL & "\" & ver & "\TelerikWpfApp1.exe")
        Catch ex As Exception
            Try
                Dim lastver = GetKeyValue("main", "lastver", "", inipath)
                Process.Start(baseURL & "\" & lastver & "\TelerikWpfApp1.exe")

            Catch ex1 As Exception
                MsgBox(ex.Message)
            End Try

        End Try

    End Sub

    <DllImport("kernel32")>
    Public Function WritePrivateProfileString(ByVal section As String, ByVal key As String, ByVal val As String, ByVal filePath As String) As Long
    End Function
    <DllImport("kernel32")>
    Public Function WritePrivateProfileString(ByVal section As String, ByVal val As String, ByVal filePath As String) As Long
    End Function

    Public Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (
        ByVal lpApplicationName As String,
        ByVal lpKeyName As String,
        ByVal lpDefault As String,
        ByVal lpReturnedString As String,
        ByVal nSize As Integer,
        ByVal lpFileName As String) As Integer
    Public Function GetKeyValue(ByVal sectionName As String,
                                 ByVal keyName As String,
                                ByVal defaultText As String,
                                 ByVal filename As String) As String
        Dim Rvalue As Integer
        Dim BufferSize As Integer
        BufferSize = 255
        Dim keyValue As String
        keyValue = Space(BufferSize)
        Rvalue = GetPrivateProfileString(sectionName, keyName, "", keyValue, BufferSize, filename)
        If Rvalue = 0 Then
            keyValue = defaultText
        Else
            keyValue = GetIniValue(keyValue)
        End If
        Return keyValue
    End Function
    Public Function GetIniValue(ByVal msg As String) As String
        Dim PosChr0 As Integer
        PosChr0 = msg.IndexOf(Chr(0))
        If PosChr0 <> -1 Then msg = msg.Substring(0, PosChr0)
        Return msg
    End Function
    Public Function SetKeyValue(ByVal Section As String, ByVal Key As String, ByVal Value As String, ByVal iniFilePath As String) As Boolean
        Dim pat = Path.GetDirectoryName(iniFilePath)

        If Directory.Exists(pat) = False Then
            Directory.CreateDirectory(pat)
        End If

        If File.Exists(iniFilePath) = False Then
            File.Create(iniFilePath).Close()
        End If

        Dim OpStation As Long = WritePrivateProfileString(Section, Key, Value, iniFilePath)

        If OpStation = 0 Then
            Return False
        Else
            Return True
        End If
    End Function
End Module

