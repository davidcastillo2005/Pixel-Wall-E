   M S B A M L   `   `   `  ����J  FViewPixelWall-E, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:ViewPixelWall_EViewPixelWall_E  5   	        ViewPixelWall_E.MainWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:ViewPixelWall_E     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      � .+�5
   
   �� $��0��5      $��1��5      $��5��5      $��10��5      
�������$��Center=�5      $��Stretch=�5       �� ResizeDirection$ Columns=�5      �� 5   
   $��2��5      $��0��5      $��Stretch=�5      
�������$��Center=�5      $
 Rows=�5      $��10��5      �� 5   
   $��0��5      $��2��5      
������� �� Orientation$ 
Horizontal=�5      .+�5       �� $��5q�5!      $��Center=�5"      $
��Zoom��5#      �� 5$      -   5        $��ZoomTextBox��5%      $��Center=�5&      $
��Left=�5'      $
��100%��5(       �� MaxLines$ 1��5)      $��35.23��5*      5+      5,      �� 5-   
   -   5        $��MainScrollViewer��51      $��1��5.      $��2��5/      $��4��50      $
��Auto=�52      $
��Auto=�53      
�������.��55      (� $��Center=�56      $��Center=�57       (� Stretch$ Uniform=�58      $	��100��59      .��5:      �� -   5        $��
MainCanvas��5;      $��Center=�5<      $��Center=�5=      
�������5?      5@      5A      � 5B   
   $��3��5C      $��0��5D      
�������.+�5F      �� .@�5G      �� $��Output��5H       �� 
IsSelected$
 True��5I      .��5J      �� -   5        $��OutputTextBox��5K      $��Consolas)�5L      $��14(�5M       �� 
IsReadOnly$
 true��5N      " �� VerticalScrollBarVisibility$ Visible=�5O      $ �� HorizontalScrollBarVisibility$
 Auto=�5P      
�������. �5R      /� 	 /� 
LineHeight$	 7��5S      .T�5T      J� .*�6(   .*�5U   "   �� $	�...��5V   %   5W      5X      5Y      5Z      �� 5[      -   5        $��ProblemsTab��5]      $��Problems��5\      .��5^      �� -   5        $��PloblemsTextBox��5_      $��Consolas)�5`      $��14(�5a      $
 true��5b      $ Visible=�5c      $
 Auto=�5d      
�������. �5f      /� $	 7��5g      .T�5h      J� .*�6(   .*�5i   "   �� $	�...��5j   %   5k      5l      5m      5n      5o      5p      � 5q   
   $��0��5r      $��0��5s      
�������.+�5u      o� $��0��5v      $��0��5w      $��Center=�5x      
�������.@�5z      m� $��_File��5{      .@�5|      m� -   5        $��NewMenuItem��5}      $
��_New��5~      
 m� Command$
 ApplicationCommands.New��5�      m� 5�      $��_Open��5�      $
 ApplicationCommands.Open��5�      m� 5�      $��_Save��5�      $
 ApplicationCommands.Save��5�      m� 5�      $��Save _As...��5�      $ 
 ApplicationCommands.SaveAs��5�      �� 5�      m� 5�      -   5         m�Name$ ExitMenuItem��5�      $��_Exit��5�      5�      5�      m� 5�      $��_Edit��5�      .@�5�      m� $��	_Deshacer��5�      $
 ApplicationCommands.Undo��5�      m� 5�      $��_Rehacer��5�      $
 ApplicationCommands.Redo��5�      �� 5�      m� 5�      $
��_Cut��5�      $
 ApplicationCommands.Cut��5�      m� 5�      $��_Copy��5�      $
 ApplicationCommands.Copy��5�      m� 5�      $��_Paste��5�      $
 ApplicationCommands.Paste��5�      �� 5�      m� 5�      $��Select _All��5�      $#
 ApplicationCommands.SelectAll��5�      5�      5�      5�      �� 5�   
   $��4��5�      $��0��5�      
�������.@�5�      �� .��5�      �� $��Ready��5�      5�      5�      �� 5�   
   -	   5        $��
SourceCode��5�      $��1��5�      $��0��5�       �� AcceptsReturn$
 True��5�       �� 
AcceptsTab$
 True��5�      $��14(�5�      $��Consolas)�5�      
�������$
 Auto=�5�      $
 Auto=�5�      $	��150��5�      . �5�      /� $	 7��5�      .T�5�      J� .*�6   .*�5�      �� $�Write here...��5�      .�6.   5�      5�      5�      5�       � ColumnDefinitions 5�   
   �� 5�      $
��auto �5�      $	��150��5�      �� 5�      $
��auto �5�      �� 5�      $��* �5�      $	��100��5�       � RowDefinitions 5�   
   �� 5�      $
��auto �5�      �� 5�      $��* �5�      $��20��5�      �� 5�      $
��auto �5�      �� 5�      $��65��5�      �� 5�      $
��auto �5�      5�      5�      