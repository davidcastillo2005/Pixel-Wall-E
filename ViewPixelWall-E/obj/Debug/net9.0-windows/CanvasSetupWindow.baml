   M S B A M L   `   `   `  ����J  FViewPixelWall-E, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:ViewPixelWall_EViewPixelWall_E  5   	   '    !ViewPixelWall_E.CanvasSetupWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:ViewPixelWall_E     5   	     � Title$  CanvasSetupWindow��5   	   $	��200��5	   	   $	��400��5
   	    � 
ResizeMode$ NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5   	   .��5      � -   5        $��MainGrid��5   	   .+�5   
   �� -   5        $��OKButton��5      $��4��5      $��0��5      $��5q�5      $��Right=�5      $��Center=�5      $��75��5      $��25��5       �� 	IsDefault$
 True��5      $��OK��5      �� 5   
   -   5        $��CancelButton��5      $��4��5      $��1��5      $��5q�5       $
��Left=�5!      $��Center=�5"      $��75��5#      $��25��5$       �� IsCancel$
 True��5%      $��Cancel��5'      �� 5(   
   $��1��5)      $��0��5*      $��Right=�5+      $��Center=�5,      $��12(�5-      $
��Bold"�5.      $
��Wrap=�5/      $��Height��50      $��5,5,5,5q�51      �� 52   
   $��2��53      $��0��54      $��Right=�55      $��Center=�56      $��12(�57      $
��Bold"�58      $
��Wrap=�59      $��Width��5:      $��5,5,5,5q�5;      �� 5<   
   -   5        $��HeightTextBox��5=      $��1��5>      $��1��5?      $��5q�5@      $��Center=�5A      $
��Left=�5B      $��50��5C       �� MaxLines$ 1��5D      
�������5F      �� 5G   
   -   5        $��WidthTextBox��5H      $��2��5I      $��1��5J      $��5q�5K      
�������$��Center=�5M      $
��Left=�5N      $��50��5O      $ 1��5P      5Q      � 5R   
   -   5        $��PreviewGrid��5S      $��0��5T      $��3��5U      $��5��5V      
�������.+�5X      �� -   5        $��PreviewBorder��5Y      
���   �$��2q�5\      ��5Z      ��  �� ElementName$ MainGrid�� �� Path$ ActualHeight�._�5]      �� -   5        $��PreviewCanvas��5^      $��Center=�5_      $��Center=�5`      
�������5a      5b      5c       � ColumnDefinitions 5d   
   �� 5e      $��* �5f      �� 5g      $��* �5h      �� 5i      $
��auto �5j      	 � RowDefinitions	 5l   
   �� 5m      $��* �5n      �� 5o      $
��auto �5p      �� 5q      $
��auto �5r      �� 5s      $��* �5t      �� 5u      5w      5x      