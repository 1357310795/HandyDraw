Imports System.Runtime.InteropServices

Public Module WindowsMonitorAPI
    ' Token: 0x06000004 RID: 4
    Public Declare Auto Function GetMonitorInfo Lib "user32.dll" (hmonitor As HandleRef, <[In]()> <Out()> info As WindowsMonitorAPI.MONITORINFOEX) As Boolean

    ' Token: 0x06000005 RID: 5
    Public Declare Function EnumDisplayMonitors Lib "user32.dll" (hdc As HandleRef, rcClip As WindowsMonitorAPI.COMRECT, lpfnEnum As WindowsMonitorAPI.MonitorEnumProc, dwData As IntPtr) As Boolean

    ' Token: 0x04000001 RID: 1
    Private Const User32 As String = "user32.dll"

    ' Token: 0x04000002 RID: 2
    Public NullHandleRef As HandleRef = New HandleRef(Nothing, IntPtr.Zero)

    ' Token: 0x02000008 RID: 8
    ' (Invoke) Token: 0x06000016 RID: 22
    Public Delegate Function MonitorEnumProc(monitor As IntPtr, hdc As IntPtr, lprcMonitor As IntPtr, lParam As IntPtr) As Boolean

    ' Token: 0x02000009 RID: 9
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto, Pack:=4)>
    Public Class MONITORINFOEX
        ' Token: 0x04000007 RID: 7
        Friend cbSize As Integer = Marshal.SizeOf(GetType(WindowsMonitorAPI.MONITORINFOEX))

        ' Token: 0x04000008 RID: 8
        Friend rcMonitor As WindowsMonitorAPI.RECT1 = Nothing

        ' Token: 0x04000009 RID: 9
        Friend rcWork As WindowsMonitorAPI.RECT1 = Nothing

        ' Token: 0x0400000A RID: 10
        Friend dwFlags As Integer = 0

        ' Token: 0x0400000B RID: 11
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)>
        Friend szDevice As Char() = New Char(31) {}
    End Class

    ' Token: 0x0200000A RID: 10
    Public Structure RECT1
        ' Token: 0x0600001A RID: 26 RVA: 0x000022B9 File Offset: 0x000004B9
        Public Sub New(r As System.Windows.Rect)
            Me.left = CInt(r.Left)
            Me.top = CInt(r.Top)
            Me.right = CInt(r.Right)
            Me.bottom = CInt(r.Bottom)
        End Sub

        ' Token: 0x0400000C RID: 12
        Public left As Integer

        ' Token: 0x0400000D RID: 13
        Public top As Integer

        ' Token: 0x0400000E RID: 14
        Public right As Integer

        ' Token: 0x0400000F RID: 15
        Public bottom As Integer
    End Structure

    ' Token: 0x0200000B RID: 11
    <StructLayout(LayoutKind.Sequential)>
    Public Class COMRECT
        ' Token: 0x04000010 RID: 16
        Public left As Integer

        ' Token: 0x04000011 RID: 17
        Public top As Integer

        ' Token: 0x04000012 RID: 18
        Public right As Integer

        ' Token: 0x04000013 RID: 19
        Public bottom As Integer
    End Class
End Module

Public Class ScreenHelper
    ' Token: 0x06000007 RID: 7 RVA: 0x000020A8 File Offset: 0x000002A8
    Public Shared Function GetScalingRatio() As Double
        Dim logicalHeight As Double = ScreenHelper.GetLogicalHeight()
        Dim actualHeight As Double = ScreenHelper.GetActualHeight()
        Dim flag As Boolean = logicalHeight > 0.0 AndAlso actualHeight > 0.0
        Dim result As Double
        If flag Then
            result = logicalHeight / actualHeight
        Else
            result = 1.0
        End If
        Return result
    End Function

    ' Token: 0x06000008 RID: 8 RVA: 0x000020F8 File Offset: 0x000002F8
    Private Shared Function GetActualHeight() As Double
        Return SystemParameters.PrimaryScreenHeight
    End Function

    ' Token: 0x06000009 RID: 9 RVA: 0x00002110 File Offset: 0x00000310
    Private Shared Function GetLogicalHeight() As Double
        Dim logicalHeight As Double = 0.0
        Dim proc As WindowsMonitorAPI.MonitorEnumProc = Function(m As IntPtr, h As IntPtr, lm As IntPtr, lp As IntPtr)
                                                            Dim info As WindowsMonitorAPI.MONITORINFOEX = New WindowsMonitorAPI.MONITORINFOEX()
                                                            WindowsMonitorAPI.GetMonitorInfo(New HandleRef(Nothing, m), info)
                                                            Dim flag As Boolean = (info.dwFlags And 1) <> 0
                                                            If flag Then
                                                                logicalHeight = CDbl((info.rcMonitor.bottom - info.rcMonitor.top))
                                                            End If
                                                            Return True
                                                        End Function
        WindowsMonitorAPI.EnumDisplayMonitors(WindowsMonitorAPI.NullHandleRef, Nothing, proc, IntPtr.Zero)
        Return logicalHeight
    End Function
End Class
