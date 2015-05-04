Imports System.Windows
Imports System.Windows.Controls

Namespace CMS.Models.UserLogin.XamlHelpers

    Public Class PasswordBoxHelper
        Public Shared ReadOnly BoundPassword As DependencyProperty = _
                DependencyProperty.RegisterAttached("BoundPassword", GetType(String), GetType(PasswordBoxHelper), New FrameworkPropertyMetadata(String.Empty, AddressOf OnBoundPasswordChanged))

        Public Shared ReadOnly BindPassword As DependencyProperty = _
            DependencyProperty.RegisterAttached("BindPassword", GetType(Boolean), GetType(PasswordBoxHelper), New PropertyMetadata(False, AddressOf OnBindPasswordChanged))

        Private Shared ReadOnly UpdatingPassword As DependencyProperty = _
            DependencyProperty.RegisterAttached("UpdatingPassword", GetType(Boolean), GetType(PasswordBoxHelper))

        Private Shared Sub OnBoundPasswordChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            Dim box As PasswordBox = TryCast(d, PasswordBox)

            If d Is Nothing OrElse Not GetBindPassword(d) Then
                Return
            End If

            RemoveHandler box.PasswordChanged, AddressOf HandlePasswordChanged

            Dim newPassword As String = DirectCast(e.NewValue, String)

            If Not GetUpdatingPassword(box) Then
                box.Password = newPassword
            End If

            AddHandler box.PasswordChanged, AddressOf HandlePasswordChanged
        End Sub

        Private Shared Sub OnBindPasswordChanged(ByVal dp As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            Dim box As PasswordBox = TryCast(dp, PasswordBox)

            If box Is Nothing Then
                Return
            End If

            Dim wasBound As Boolean = CBool(e.OldValue)
            Dim needToBind As Boolean = CBool(e.NewValue)

            If wasBound Then
                RemoveHandler box.PasswordChanged, AddressOf HandlePasswordChanged
            End If

            If needToBind Then
                AddHandler box.PasswordChanged, AddressOf HandlePasswordChanged
            End If
        End Sub

        Private Shared Sub HandlePasswordChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim box As PasswordBox = TryCast(sender, PasswordBox)

            SetUpdatingPassword(box, True)
            SetBoundPassword(box, box.Password)
            SetUpdatingPassword(box, False)
        End Sub

        Public Shared Sub SetBindPassword(ByVal dp As DependencyObject, ByVal value As Boolean)
            dp.SetValue(BindPassword, value)
        End Sub

        Public Shared Function GetBindPassword(ByVal dp As DependencyObject) As Boolean
            Return CBool(dp.GetValue(BindPassword))
        End Function

        Public Shared Function GetBoundPassword(ByVal dp As DependencyObject) As String
            Return DirectCast(dp.GetValue(BoundPassword), String)
        End Function

        Public Shared Sub SetBoundPassword(ByVal dp As DependencyObject, ByVal value As String)
            dp.SetValue(BoundPassword, value)
        End Sub

        Private Shared Function GetUpdatingPassword(ByVal dp As DependencyObject) As Boolean
            Return CBool(dp.GetValue(UpdatingPassword))
        End Function

        Private Shared Sub SetUpdatingPassword(ByVal dp As DependencyObject, ByVal value As Boolean)
            dp.SetValue(UpdatingPassword, value)
        End Sub

    End Class
End Namespace