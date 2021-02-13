Public Class FlushMemory

    Public Declare Function SetProcessWorkingSetSize Lib "kernel32" Alias "SetProcessWorkingSetSize" (ByVal proc As IntPtr, ByVal min As Integer, ByVal max As Integer) As Boolean

    Public Shared Sub Flush()
        GC.Collect()
        GC.WaitForPendingFinalizers()

        If Environment.OSVersion.Platform = PlatformID.Win32NT Then
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1)
        End If
    End Sub
End Class
