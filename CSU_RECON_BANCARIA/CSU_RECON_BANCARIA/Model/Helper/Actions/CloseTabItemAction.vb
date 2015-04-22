
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Interactivity

Public Class CloseTabItemAction
    Inherits TriggerAction(Of DependencyObject)
    Protected Overrides Sub Invoke(parameter As Object)
        Me.TabControl.Items.Remove(Me.TabItem)
    End Sub

    Public Shared ReadOnly TabControlProperty As DependencyProperty = DependencyProperty.Register("TabControl", GetType(TabControl), GetType(CloseTabItemAction), New PropertyMetadata(Nothing))

    Public Property TabControl() As TabControl
        Get
            Return DirectCast(GetValue(TabControlProperty), TabControl)
        End Get
        Set(value As TabControl)
            SetValue(TabControlProperty, value)
        End Set
    End Property

    Public Shared ReadOnly TabItemProperty As DependencyProperty = DependencyProperty.Register("TabItem", GetType(TabItem), GetType(CloseTabItemAction), New PropertyMetadata(Nothing))

    Public Property TabItem() As TabItem
        Get
            Return DirectCast(GetValue(TabItemProperty), TabItem)
        End Get
        Set(value As TabItem)
            SetValue(TabItemProperty, value)
        End Set
    End Property
End Class