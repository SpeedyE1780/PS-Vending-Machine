   M S B A M L   `   `   `  ����B  >Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:ProjectProject  5          Project.Sign_Up   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      'localclr-namespace:Project     5        � 	Resources  5	       �� 
TargetType �� Property �� Value��%  '��       '�� n     �� 5
   
   " ��6   .�5      ��  � ��
 �����6+   �� 5       � ��$ 30(�6)   5      �� 5   
   " ��6   .�5      ��  � ��$ 30(�6)   �� 5       � ��$ 10q�6'   �� 5       � ��$ 40��6'   5      5      .��5      � ���#030130 � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5!   
   ��  �� Orientation$ Vertical=�6   $��0��6,   $��0��69   .+�5"      �� $��PERSONAL INFORMATION��6   �� 5#      $��
First Name��6   �� 5$      $��	Last Name��6   �� 5%      $��Email��6   �� 5&      $��Username��6   �� 5'      $��Password��6   �� 5(      $��Confirm Password��6   5)      �� 5+   
   $ Vertical=�6   $��0��6,   $��1��69   .+�5,      �� $��PAYMENT INFORMATION��6   �� 5-      |�.   �� 
IsEditable .  �� 
IsReadOnly . 	 �� Text$	 PAYMENT OPTION��6M   $��20(�6c   $��10q�6q   $��40��6}   .@�5.      �� $
��VISA��6   �� 5/      $��Master Card��6   �� 50      $��American Express��6   51      �� 52      $��Card Holder First Name��6   �� 53      $��Card Holder Last Name��6   �� 54      $ ��Card Holder Billing Adress��6   �� 55      $��Card Number��6   �� 57      $ 
Horizontal=�6   .+�59      �� $��Expiry Date��6   $��20(�6.   $��Center=�6<   (  "System.Windows.Controls.DatePicker  5:      
   SelectedDate$
 
2000-12-31X�6      SelectedDateFormat$ Short=�67   $	��200��6R   $��40��6^   $��30(�6j   �� 5<      $	��CVV��6   5=      5>      �� 5@   
   $��0,25,0,0q�6   $��Sign UP��6#   $��1��65   $��0��6B   $��2��6R   $��40(�6f   $��70��6t   $	��200��6�   ���#002566
�������6�   5B      5C      