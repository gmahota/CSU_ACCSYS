CREATE VIEW [dbo].[View_Empresas]
AS
SELECT        dbo.Empresas.Codigo, dbo.Empresas.IDNome, dbo.Empresas.IDMorada, dbo.CategoriasEmpresas.Descricao AS Categoria, dbo.Empresas.IDTelefone, 
                         dbo.Empresas.IDIndicativoTelefone, dbo.Empresas.IDEmail, dbo.Empresas.IDLocalidade, dbo.Empresas.IFNIF
FROM            dbo.Empresas INNER JOIN
                         dbo.CategoriasEmpresas ON dbo.Empresas.Categoria = dbo.CategoriasEmpresas.Categoria
WHERE        (dbo.CategoriasEmpresas.Descricao = 'GRUPO MERIDIAN')

GO

create view [dbo].[View_Lista_Contactos_Pendentes] as

select c.Cliente, c.Nome, c.Fac_Mor, c.Fac_Local,c.NumContrib,c.Pais,c.Fac_Tel,c.Moeda, c.CDU_ContaRec,c.CDU_EnviaCobranca,
	cont.PrimeiroNome, cont.UltimoNome, cont.Titulo ,cont.Email,cont.EmailAssist, lce.tipoContacto, 
	sum (p.ValorPendente) as ValorPendente, sum (p.ValorTotal) as ValorTotal 
	from clientes c   
	inner join Pendentes p on tipoentidade = 'C' and Entidade = c.Cliente
	inner join LinhasContactoEntidades lce on lce.Entidade = c.Cliente and lce.TipoEntidade = 'C' 
	inner join Contactos cont on cont.Id = lce.IDContacto 
	group by c.Cliente, c.Nome, c.Fac_Mor, c.Fac_Local,c.NumContrib,c.Pais,c.Fac_Tel,c.Moeda, 
	c.CDU_ContaRec,c.CDU_EnviaCobranca,cont.PrimeiroNome, cont.UltimoNome, cont.Titulo ,cont.Email,cont.EmailAssist ,lce.tipoContacto


GO

CREATE VIEW [dbo].[View_Pendentes_Doc_Clientes]
AS
SELECT        Modulo, TipoEntidade, Entidade, TipoDoc, NumDoc, NumDocInt, DataDoc, DataVenc, ValorTotal, ValorPendente, Moeda, Cambio, NumAvisos, NumPrestacao, Serie, 
                         Conta
FROM            dbo.Pendentes
WHERE        (TipoEntidade = N'C')

GO