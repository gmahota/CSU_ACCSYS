Public Class Instancia
    Public instancia As Integer = 0

    Public empresa As String
    Public usuario As String
    Public password As String
    Public instanciaSql As String
    Public empresaSql As String
    Public usuarioSql As String
    Public passwordSql As String
    Public Shared pastaConfig As String = "PRIMAVERA\\SG800"

    Public Function daConnectionString()
        Return "Data Source=" + instanciaSql + ";Initial Catalog= " + empresaSql + ";User Id=" + usuarioSql + ";Password=" + passwordSql
    End Function
End Class
