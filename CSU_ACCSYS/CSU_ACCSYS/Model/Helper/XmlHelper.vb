Imports System.IO

Imports System.Xml.Serialization
Imports System.Xml

Public Class XmlHelper
    Public filename As String
    Public instancia As Instancia

    Public Sub loadFolder()
        Try
            Dim doc As XmlDocument = New XmlDocument
            doc.Load(filename)

            Dim pastas As XmlNode = doc.SelectSingleNode("/instancias")

            For Each node As XmlNode In pastas
                instancia.empresa = node.Attributes.GetNamedItem("empresa").InnerText
                instancia.empresaSql = node.Attributes.GetNamedItem("empresaSql").InnerText
                instancia.instanciaSql = node.Attributes.GetNamedItem("instanciaSql").InnerText
                instancia.password = node.Attributes.GetNamedItem("password").InnerText
                instancia.passwordSql = node.Attributes.GetNamedItem("passwordSql").InnerText
                instancia.usuario = node.Attributes.GetNamedItem("usuario").InnerText
                instancia.usuarioSql = node.Attributes.GetNamedItem("usuarioSql").InnerText
            Next

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Erro ao carregar os dados da Instancia ")
        End Try



    End Sub

    Public Function daPasta(tipo As String) As String
        Try
            Dim doc As XmlDocument = New XmlDocument
            doc.Load(filename)

            Dim pastas As XmlNode = doc.SelectSingleNode("/instancias")

            For Each node As XmlNode In pastas
                If (tipo = node.Attributes.GetNamedItem("tipo").InnerText) Then
                    Return node.Attributes.GetNamedItem("folder").InnerText
                End If

            Next
        Catch ex As Exception
            MessageBox.Show("Pasta de configuração não encontrada: " + filename)
        End Try
        Return ""
    End Function

    Public Sub New()
        filename = DefaultData.Pasta_Config_Filename
        instancia = New Instancia
        loadFolder()
        ' My.Application.Info.DirectoryPath + "\Resources\Config.xml"
    End Sub
End Class

Friend Class DefaultData
    Public Shared Pasta_Config As String = "resources/Config.xml"
    Public Shared Pasta_Config_Filename As String = System.IO.
        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + "\Resources\Config.xml"
End Class