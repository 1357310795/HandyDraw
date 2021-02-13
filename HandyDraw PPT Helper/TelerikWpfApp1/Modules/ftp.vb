Imports System.IO
Imports System.Net
Imports System.Text

Public Class FtpWeb
    Private ftpRemotePath As String
    Private ftpUserID As String
    Private ftpPassword As String
    Private ftpURI As String
    Private ftpServerIP As String

    Public Sub New(ByVal FtpServerIP1 As String, ByVal FtpUserID1 As String, ByVal FtpPassword1 As String)
        ftpServerIP = FtpServerIP1
        ftpUserID = FtpUserID1
        ftpPassword = FtpPassword1
        ftpURI = FtpServerIP1 & "/"
    End Sub

    Public Sub Upload(ByVal filename As String)
        Dim fileInf As FileInfo = New FileInfo(filename)
        Dim uri As String = ftpURI & fileInf.Name
        Dim reqFTP As FtpWebRequest
        reqFTP = CType(FtpWebRequest.Create(New Uri(uri)), FtpWebRequest)
        reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
        reqFTP.KeepAlive = False
        reqFTP.Method = WebRequestMethods.Ftp.UploadFile
        reqFTP.UseBinary = True
        reqFTP.ContentLength = fileInf.Length
        Dim buffLength As Integer = 2048
        Dim buff As Byte() = New Byte(buffLength - 1) {}
        Dim contentLen As Integer
        Dim fs As FileStream = fileInf.OpenRead()

        Try
            Dim strm As Stream = reqFTP.GetRequestStream()
            contentLen = fs.Read(buff, 0, buffLength)

            While contentLen <> 0
                strm.Write(buff, 0, contentLen)
                contentLen = fs.Read(buff, 0, buffLength)
            End While

            strm.Close()
            fs.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub Upload2(ByVal ftpuri As String, ByVal filename As String)
        Dim fileInf As FileInfo = New FileInfo(filename)
        Dim uri As String = ftpuri
        Dim reqFTP As FtpWebRequest
        reqFTP = CType(FtpWebRequest.Create(New Uri(uri)), FtpWebRequest)
        reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
        reqFTP.KeepAlive = False
        reqFTP.Method = WebRequestMethods.Ftp.UploadFile
        reqFTP.UseBinary = True
        reqFTP.ContentLength = fileInf.Length
        Dim buffLength As Integer = 2048
        Dim buff As Byte() = New Byte(buffLength - 1) {}
        Dim contentLen As Integer
        Dim fs As FileStream = fileInf.OpenRead()

        Try
            Dim strm As Stream = reqFTP.GetRequestStream()
            contentLen = fs.Read(buff, 0, buffLength)

            While contentLen <> 0
                strm.Write(buff, 0, contentLen)
                contentLen = fs.Read(buff, 0, buffLength)
            End While

            strm.Close()
            fs.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub Download(ByVal ftpUri As String, ByVal fileFullPath As String)
        Dim reqFTP As FtpWebRequest

        Try
            Dim outputStream As FileStream = New FileStream(fileFullPath, FileMode.Create)
            reqFTP = CType(FtpWebRequest.Create(New Uri(ftpUri)), FtpWebRequest)
            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile
            reqFTP.UseBinary = True
            reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
            Dim response As FtpWebResponse = CType(reqFTP.GetResponse(), FtpWebResponse)
            Dim ftpStream As Stream = response.GetResponseStream()
            Dim cl As Long = response.ContentLength
            Dim bufferSize As Integer = 2048
            Dim readCount As Integer
            Dim buffer As Byte() = New Byte(bufferSize - 1) {}
            readCount = ftpStream.Read(buffer, 0, bufferSize)

            While readCount > 0
                outputStream.Write(buffer, 0, readCount)
                readCount = ftpStream.Read(buffer, 0, bufferSize)
            End While

            ftpStream.Close()
            outputStream.Close()
            response.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub Delete(ByVal fileName As String)
        Try
            Dim uri As String = fileName
            Dim reqFTP As FtpWebRequest
            reqFTP = CType(FtpWebRequest.Create(New Uri(uri)), FtpWebRequest)
            reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
            reqFTP.KeepAlive = False
            reqFTP.Method = WebRequestMethods.Ftp.DeleteFile
            Dim result As String = String.Empty
            Dim response As FtpWebResponse = CType(reqFTP.GetResponse(), FtpWebResponse)
            Dim size As Long = response.ContentLength
            Dim datastream As Stream = response.GetResponseStream()
            Dim sr As StreamReader = New StreamReader(datastream)
            result = sr.ReadToEnd()
            sr.Close()
            datastream.Close()
            response.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub RemoveDirectory(ByVal folderName As String)
        Try
            Dim uri As String = ftpURI & folderName
            Dim reqFTP As FtpWebRequest
            reqFTP = CType(FtpWebRequest.Create(New Uri(uri)), FtpWebRequest)
            reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
            reqFTP.KeepAlive = False
            reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory
            Dim result As String = String.Empty
            Dim response As FtpWebResponse = CType(reqFTP.GetResponse(), FtpWebResponse)
            Dim size As Long = response.ContentLength
            Dim datastream As Stream = response.GetResponseStream()
            Dim sr As StreamReader = New StreamReader(datastream)
            result = sr.ReadToEnd()
            sr.Close()
            datastream.Close()
            response.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function GetFilesDetailList() As String()
        Dim downloadFiles As String()

        Try
            Dim result As StringBuilder = New StringBuilder()
            Dim ftp As FtpWebRequest
            ftp = CType(FtpWebRequest.Create(New Uri(ftpURI)), FtpWebRequest)
            ftp.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
            ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails
            Dim response As WebResponse = ftp.GetResponse()
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream(), Encoding.[Default])
            Dim line As String = reader.ReadLine()

            While line IsNot Nothing
                result.Append(line)
                result.Append(vbLf)
                line = reader.ReadLine()
            End While

            result.Remove(result.ToString().LastIndexOf(vbLf), 1)
            reader.Close()
            response.Close()
            Return result.ToString().Split(vbLf)
        Catch ex As Exception
            downloadFiles = Nothing
            Throw ex
        End Try
    End Function

    Public Function GetFileList(ByVal mask As String) As String()
        Dim downloadFiles As String()
        Dim result As StringBuilder = New StringBuilder()
        Dim reqFTP As FtpWebRequest

        Try
            reqFTP = CType(FtpWebRequest.Create(New Uri(ftpURI)), FtpWebRequest)
            reqFTP.UseBinary = True
            reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
            reqFTP.Method = WebRequestMethods.Ftp.ListDirectory
            Dim response As WebResponse = reqFTP.GetResponse()
            Dim reader As StreamReader = New StreamReader(response.GetResponseStream(), Encoding.[Default])
            Dim line As String = reader.ReadLine()

            While line IsNot Nothing

                If mask.Trim() <> String.Empty AndAlso mask.Trim() <> "*.*" Then
                    Dim mask_ As String = mask.Substring(0, mask.IndexOf("*"))

                    If line.Substring(0, mask_.Length) = mask_ Then
                        result.Append(line)
                        result.Append(vbLf)
                    End If
                Else
                    result.Append(line)
                    result.Append(vbLf)
                End If

                line = reader.ReadLine()
            End While

            result.Remove(result.ToString().LastIndexOf(vbLf), 1)
            reader.Close()
            response.Close()
            Return result.ToString().Split(vbLf)
        Catch ex As Exception
            downloadFiles = Nothing
            Throw ex
        End Try
    End Function

    Public Function GetDirectoryList() As String()
        Dim drectory As String() = GetFilesDetailList()
        Dim m As String = String.Empty

        For Each str As String In drectory
            Dim dirPos As Integer = str.IndexOf("<DIR>")

            If dirPos > 0 Then
                m += str.Substring(dirPos + 5).Trim() & vbLf
            ElseIf str.Trim().Substring(0, 1).ToUpper() = "D" Then
                Dim dir As String = str.Substring(54).Trim()

                If dir <> "." AndAlso dir <> ".." Then
                    m += dir & vbLf
                End If
            End If
        Next

        Dim n As Char() = New Char() {vbLf}
        Return m.Split(n)
    End Function

    Public Function DirectoryExist(ByVal RemoteDirectoryName As String) As Boolean
        Try
            Dim dirList As String() = GetDirectoryList()

            For Each str As String In dirList

                If str.Trim() = RemoteDirectoryName.Trim() Then
                    Return True
                End If
            Next

            Return False
        Catch
            Return False
        End Try
    End Function

    Public Function FileExist(ByVal RemoteFileName As String) As Boolean
        Dim fileList As String() = GetFileList("*.*")

        For Each str As String In fileList

            If str.Trim() = RemoteFileName.Trim() Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Sub MakeDir(ByVal dirName As String)
        Dim reqFTP As FtpWebRequest

        Try
            reqFTP = CType(FtpWebRequest.Create(New Uri(ftpURI & dirName)), FtpWebRequest)
            reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory
            reqFTP.UseBinary = True
            reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
            Dim response As FtpWebResponse = CType(reqFTP.GetResponse(), FtpWebResponse)
            Dim ftpStream As Stream = response.GetResponseStream()
            ftpStream.Close()
            response.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Function GetFileSize(ByVal filename As String) As Long
        Dim reqFTP As FtpWebRequest
        Dim fileSize As Long = 0

        Try
            reqFTP = CType(FtpWebRequest.Create(New Uri(ftpURI & filename)), FtpWebRequest)
            reqFTP.Method = WebRequestMethods.Ftp.GetFileSize
            reqFTP.UseBinary = True
            reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
            Dim response As FtpWebResponse = CType(reqFTP.GetResponse(), FtpWebResponse)
            Dim ftpStream As Stream = response.GetResponseStream()
            fileSize = response.ContentLength
            ftpStream.Close()
            response.Close()
        Catch ex As Exception
            Throw ex
        End Try

        Return fileSize
    End Function

    Public Sub ReName(ByVal currentFilename As String, ByVal newFilename As String)
        Dim reqFTP As FtpWebRequest

        Try
            reqFTP = CType(FtpWebRequest.Create(New Uri(ftpURI & currentFilename)), FtpWebRequest)
            reqFTP.Method = WebRequestMethods.Ftp.Rename
            reqFTP.RenameTo = newFilename
            reqFTP.UseBinary = True
            reqFTP.Credentials = New NetworkCredential(ftpUserID, ftpPassword)
            Dim response As FtpWebResponse = CType(reqFTP.GetResponse(), FtpWebResponse)
            Dim ftpStream As Stream = response.GetResponseStream()
            ftpStream.Close()
            response.Close()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub MovieFile(ByVal currentFilename As String, ByVal newDirectory As String)
        ReName(currentFilename, newDirectory)
    End Sub

    Public Sub GotoDirectory(ByVal DirectoryName As String, ByVal IsRoot As Boolean)
        If IsRoot Then
            ftpRemotePath = DirectoryName
        Else
            ftpRemotePath += DirectoryName & "/"
        End If

        ftpURI = "ftp://" & ftpServerIP & "/" & ftpRemotePath & "/"
    End Sub
End Class
