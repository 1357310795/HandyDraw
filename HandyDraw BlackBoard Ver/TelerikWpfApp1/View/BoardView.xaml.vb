Public Class BoardView
    Inherits UserControl
    Public scale As Double = 1
    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()
        InkCanvas1.EraserShape = New Ink.RectangleStylusShape(30, 50)
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub

End Class
