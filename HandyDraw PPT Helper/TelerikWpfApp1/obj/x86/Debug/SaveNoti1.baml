   M S B A M L   `   `   `  ����  TelerikWpfApp1    TelerikWpfApp1.SaveNoti1   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      V RMaterialDesignThemes.Wpf, Version=3.1.3.1513, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      
������ $��98��5   8   $	��279��6D   $��5q�6S   .��5      #  MaterialDesignThemes.Wpf.Card      UniformCornerRadius$  5Q�6   +  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth3=�62   .��5	   
   � .+�5
      7  1MaterialDesignThemes.Wpf.Transitions.Transitioner  -   5        $��Transitioner��5
   *    �� SelectedIndex$ 1��6>   .@�5      ��  �� Orientation$ Vertical=�6   $��Center=�64   .+�5      �  %  !MaterialDesignCircularProgressBar#��[  6"    � Value$ 0Q�5   "   $��40��6,    � IsIndeterminate . $��Center=�5   "   $��40��6=   $��10q�6I   �� 5      $��正在保存��6    $��20(�6,   $��Center=�6:   5      � 5       � RowDefinitions 5      �� 5      $
��Auto �6(   �� 5      �� 5      $
��Auto �6(   �� 5      $
��Auto �6(   .+�5      �� $ 
Horizontal=�6!   $��0��6:   $
��Left=�5   !   $	��Top=�6<   $��5q�5   !   .+�5      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$! CheckboxMarkedCircleOutline=�62   $��32��6U   $��32��6`   ���	#FF00A0246l   �� 5      $��保存成功��6$   $��20(�60   $��Center=�6>   $��5,0,5,0q�6Y   5        5      -   5        $ WindowClose=�5   .   $��32��6A   $��32��6L   $��0��6X    � Cursor$
 Hand��6e   $	��Top=�5   .   $��Right=�6F   $��5q�5    .   ���	#FFFF4D4D
�������6h   �� 5!      -   5        	 ��Name$	 succeedText��5!       $��文件已保存于……��65   $��15(�6E   $��1��6S   $��	10,0,10,5q�6`   �� 5"      $��0,1,0,0q�6   $��0q�67   $��2��6B     MaterialDesignDivider#���  5#      � 5$      $��3��6   
 � ColumnDefinitions
 5%      �� 5&      $��1* �6/   �� 5'      $
��Auto �6/   �� 5(      $��1* �6/   .+�5*      �� -   5          MaterialDesignFlatButton#��[ 5*   !   $��30��5+   !   $��15(�6-   $��0��6;   .��5,   :   打开文件5.      �� 5/      $��1,0,0,0q�6!   $��0q�6;   $��1��6F   #���  50      �� 51      -   5        #��[ 51   !   $��30��52   !   $��15(�6-   $��2��6;   .��53   =   打开文件夹55      56      57      � 58       59      �� 5:      $
��Auto �6(   �� 5;      �� 5<      $
��Auto �6(   �� 5=      $
��Auto �6(   .+�5?      �� $ 
Horizontal=�6!   $��0��6:   $
��Left=�5@   !   $	��Top=�6<   $��5q�5A   !   .+�5B        $ WarningDecagramOutline=�62   $��32��6P   $��32��6[   
���  ��6g   �� 5C      $��保存失败��6$   $��20(�60   $��Center=�6>   $��5,0,0,0q�6Y   5D        5E      -   5        $ WindowClose=�5E   .   $��32��6A   $��32��6L   $��0��6X   $
 Hand��6e   $	��Top=�5F   .   $��Right=�6F   $��5q�5G   .   ���	#FFFF4D4D6Q   �� 5H      $��文件未保存。��6    $��15(�6.   $��1��6<   $��10,0,0,5q�6I   �� 5I      $��0,1,0,0q�6   $��0q�67   $��2��6B   #���  5J      �� 5K      -   5        #��[ 5K      $��30��5L   !   $��15(�6-   $��3��6;   .��5M   6   查看失败原因5O      5P      5Q      5R      5S      5T      