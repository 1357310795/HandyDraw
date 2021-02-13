Public Class YesNoDialog
    Public Sub New(ByVal w As Int32, ByVal content As String)

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        MainStackPanel.MaxWidth = w
        MainTextBlock.Text = content
    End Sub
End Class
