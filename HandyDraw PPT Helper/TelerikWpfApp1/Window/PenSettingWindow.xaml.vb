Imports System.Globalization
Imports System.Windows.Ink

Public Class PenSettingWindow
    Inherits Window

    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        ColorTabControl.AddHandler(Button.ClickEvent, New RoutedEventHandler(AddressOf ColorButton_Click))
    End Sub

    Public drawer As DrawingAttributes

    Public Sub initdrawer(d As DrawingAttributes)
        drawer = d
        Dim mb1 As Binding = New Binding
        mb1.Source = drawer
        mb1.Path = New PropertyPath("Width")
        Dim mb2 As Binding = New Binding
        mb2.Source = drawer
        mb2.Path = New PropertyPath("Height")
        Dim mb As MultiBinding = New MultiBinding
        mb.Mode = BindingMode.TwoWay
        mb.Bindings.Add(mb1)
        mb.Bindings.Add(mb2)
        mb.Converter = New DisplayTwoDecPlaces
        StylusSizeSlider.SetBinding(Slider.ValueProperty, mb)

        Dim mbcolor As Binding = New Binding
        mbcolor.Source = drawer
        mbcolor.Path = New PropertyPath("Color")
        mbcolor.Converter = New ColorValueConverter
        ColorRectangle.SetBinding(Rectangle.FillProperty, mbcolor)
    End Sub
    'Public Property drawer As DrawingAttributes
    '    Get
    '        Return CType(GetValue(drawerProperty), DrawingAttributes)
    '    End Get
    '    Set(ByVal value As DrawingAttributes)
    '        SetValue(drawerProperty, value)
    '    End Set
    'End Property

    'Public Shared ReadOnly drawerProperty As DependencyProperty = DependencyProperty.Register("drawer", GetType(DrawingAttributes), GetType(Pen), New PropertyMetadata(Nothing, AddressOf OndrawerChanged))

    'Private Shared Sub OndrawerChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
    '    Console.WriteLine("OndrawerChanged")
    'End Sub

    Private Sub ColorButton_Click(sender As Object, e As RoutedEventArgs)
        Console.WriteLine("ColorButton_Click")
        drawer.Color = TryCast(TryCast(e.OriginalSource, Button).Background, SolidColorBrush).Color
        RemoveHandler Me.Deactivated, AddressOf Window_LostFocus
        Me.Close()
    End Sub

    Private Class DisplayTwoDecPlaces
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
            Return CType(values(0), Double)
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
            Dim a(1) As Object
            a(0) = value
            a(1) = value
            Return a
        End Function
    End Class
    Private Class ColorValueConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Dim c As Color = CType(value, Color)
            Dim b As SolidColorBrush = New SolidColorBrush(c)
            Return b
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function
    End Class

    Private Sub Window_LostFocus(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As EventArgs)
        Dim da1 = CubicBezierDoubleAnimation(TimeSpan.FromSeconds(0.3), 0, 1, "0,.71,.47,1")
        Dim da2 = CubicBezierDoubleAnimation(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.5), 0, 1, "0,.71,.47,1")
        Dim da3 = CubicBezierDoubleAnimation(TimeSpan.FromSeconds(0.4), TimeSpan.FromSeconds(0.7), 0, 1, "0,.71,.47,1")
        MyScaleTransform1.BeginAnimation(ScaleTransform.ScaleXProperty, da3)
        MyScaleTransform2.BeginAnimation(ScaleTransform.ScaleXProperty, da2)
        MyScaleTransform3.BeginAnimation(ScaleTransform.ScaleXProperty, da1)
        AddHandler da3.Completed, Sub()
                                      MyScaleTransform1.BeginAnimation(ScaleTransform.ScaleXProperty, Nothing)
                                      MyScaleTransform2.BeginAnimation(ScaleTransform.ScaleXProperty, Nothing)
                                      MyScaleTransform3.BeginAnimation(ScaleTransform.ScaleXProperty, Nothing)
                                  End Sub
    End Sub

    Private Sub Window_Closed(sender As Object, e As EventArgs)
        FlushMemory.Flush()
    End Sub
End Class
