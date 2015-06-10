Public Class ExcelViewCrtl
    Private Sub dgExcell_LoadingRow(sender As Object, e As DataGridRowEventArgs) Handles dgExcell.LoadingRow
        e.Row.Header = (e.Row.GetIndex() + 1).ToString()
    End Sub
End Class
