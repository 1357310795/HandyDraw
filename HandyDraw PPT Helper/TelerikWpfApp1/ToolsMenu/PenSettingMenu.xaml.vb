Imports System.Globalization
Imports System.Windows.Ink

Partial Public Class PenSettingMenu
    Inherits UserControl

    Public Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        ColorTabControl.AddHandler(Button.ClickEvent, New RoutedEventHandler(AddressOf ColorButton_Click))
    End Sub

    Public popup As MaterialDesignThemes.Wpf.PopupBox
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
        popup.IsPopupOpen = False
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
End Class


