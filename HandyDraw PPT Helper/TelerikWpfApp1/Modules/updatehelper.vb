Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports Ionic.Zip
Imports TelerikWpfApp1.logcat.Log

Public Class updatehelper
    Public Shared f As FtpWeb
    Public Shared updateok As Boolean = False
    Public Shared ftpaddress = "ftp://192.168.2.104"
    Public Shared Sub updatemain()
        Logger.Instance.WriteLog("Update Start")
        Dim t1 = New DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
        Dim downini = t1.Parent.FullName & "\tmp.ini"
        Dim localini = t1.Parent.FullName & "\version.ini"
        Dim rootpath = t1.Parent.FullName
        Dim curver = GetKeyValue("main", "ver", "", localini)
        SetKeyValue("main", "lastver", curver, localini)
        Try
            f = New FtpWeb(ftpaddress, "user", "2003")
            f.Download(ftpaddress & "/HandyDraw/ver.ini", downini)
            Dim newestver = GetKeyValue("main", "ver", "", downini)
            Logger.Instance.WriteLog("Local:" & localini)
            Logger.Instance.WriteLog("Server:" & downini)
            If newestver = "" Or curver = "" Then
                Logger.Instance.WriteLog("Empty String")
                Exit Sub
            End If
            If curver = newestver Then
                Logger.Instance.WriteLog("无需更新")
                Return
            End If
            f.Download(ftpaddress & "/HandyDraw/" & newestver & ".zip", rootpath & "\" & newestver & ".zip")
            Console.WriteLine(rootpath & "\" & newestver & ".zip")
            UnPack(rootpath & "\" & newestver & ".zip", rootpath & "\" & newestver & "\")
            SetKeyValue("main", "ver", newestver, localini)
            updateok = True
            Logger.Instance.WriteLog("Update End")
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Logger.Instance.WriteException(ex)
        End Try
    End Sub
    Public Shared Sub UnPack(ByVal PackPath As String, ByVal FolerPath As String)
        Dim zip As New ZipFile
        zip = ZipFile.Read(PackPath)
        'zip.Password = Psd  '注意密码一定要放在读取后
        zip.ExtractAll(FolerPath, ExtractExistingFileAction.OverwriteSilently)
        zip.Dispose()
    End Sub
End Class
