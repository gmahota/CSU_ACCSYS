create view View_ImportadorFormatoMagnetico as
SELECT MOV.TipoEntidade, MOV.Entidade, MOV.NIBOrigem, MOV.ContaOrigem, MOV.IDExportacaoPS2, MOV.NIBTerceiro,MOV.NUMContaTerceiro,MOV.BancoTerceiro, MOV.NomeTerceiro, SUM(MOV.Valor) AS Valor 
FROM (SELECT MB.TipoEntidade, MB.Entidade, CB.NIB As NIBOrigem, CB.Conta As ContaOrigem,MB.IDExportacaoPS2,
             CASE WHEN MB.NIBExportaPS2 IS NOT NULL THEN MB.NIBExportaPS2 ELSE CASE WHEN MB.TipoEntidade = 'U' THEN
				 NULL ELSE (SELECT Top 1 NIB FROM ContasBancariasTerc WHERE TipoEntidade = MB.TipoEntidade and Entidade = MB.Entidade 
				 ORDER BY [DEFAULT] DESC) END END AS NIBTerceiro,
			CASE WHEN MB.TipoEntidade = 'U' THEN NULL 
				ELSE (SELECT Top 1 NUMConta FROM ContasBancariasTerc WHERE TipoEntidade = MB.TipoEntidade and Entidade = MB.Entidade 
				 ORDER BY [DEFAULT] DESC) END AS NUMContaTerceiro,
			CASE WHEN MB.TipoEntidade = 'U' THEN NULL 
				ELSE (SELECT Top 1 Banco FROM ContasBancariasTerc WHERE TipoEntidade = MB.TipoEntidade and Entidade = MB.Entidade 
				 ORDER BY [DEFAULT] DESC) END AS BancoTerceiro,
             CASE WHEN C.Nome IS NOT NULL AND MB.TipoEntidade = 'C' THEN C.Nome
                  ELSE CASE WHEN F.Nome IS NOT NULL AND MB.TipoEntidade = 'F' THEN F.Nome
                            ELSE CASE WHEN FU.Nome IS NOT NULL AND MB.TipoEntidade = 'U' THEN  FU.Nome
                                      ELSE CASE WHEN OT.Nome IS NOT NULL THEN OT.Nome 
                                                ELSE '' END
                                      END
                            END
                  END AS NomeTerceiro, (Case When MB.TipoMov ='D' Then -1 Else -1 * -1 End) * MB.Valor as Valor

             FROM MovimentosBancos           MB 
             INNER JOIN DocumentosBancos     DB ON DB.Movim     = MB.Movim 
             INNER JOIN ContasBancarias      CB ON MB.Conta     = CB.Conta 
             LEFT JOIN Clientes         C ON C.Cliente    = MB.Entidade  
LEFT JOIN LinhasTesouraria               On LinhasTesouraria.IDMovimentosBancos  = MB.ID
LEFT JOIN  CabecTesouraria               On LinhasTesouraria.IdCabecTesouraria   = CabecTesouraria.Id
LEFT JOIN  RastreabilidadeEstornos ED    ON ED.IdDocDestino                      = CabecTesouraria.Id
LEFT JOIN  RastreabilidadeEstornos EO    ON EO.IdDocOrigem                       = CabecTesouraria.Id
             LEFT JOIN Fornecedores     F ON F.Fornecedor = MB.Entidade  
             LEFT JOIN OutrosTerceiros OT ON OT.Terceiro  = MB.Entidade  
             LEFT JOIN Funcionarios    FU ON FU.Codigo    = MB.Entidade  

             WHERE MB.Entidade IS NOT NULL AND MB.Entidade<>'' AND  ( ED.ID IS NULL AND EO.ID IS NULL)  --AND IDExportacaoPS2 ='31'
     ) AS MOV
GROUP BY MOV.TipoEntidade, MOV.Entidade, MOV.NIBOrigem, MOV.ContaOrigem, MOV.IDExportacaoPS2, MOV.NIBTerceiro, MOV.NomeTerceiro,MOV.NUMContaTerceiro,MOV.BancoTerceiro

go



/****** Object:  View [dbo].[View_ImportadorFormatoMagnetico]    Script Date: 23-03-2015 19:31:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER view [dbo].[View_ImportadorFormatoMagnetico] as
SELECT MOV.TipoEntidade, MOV.Entidade, MOV.NIBOrigem, MOV.ContaOrigem, MOV.IDExportacaoPS2, MOV.NIBTerceiro,MOV.NUMContaTerceiro,MOV.BancoTerceiro, MOV.NomeTerceiro, SUM(MOV.Valor) AS Valor,
MOV.Email
FROM (SELECT MB.TipoEntidade, MB.Entidade, CB.NIB As NIBOrigem, CB.Conta As ContaOrigem,MB.IDExportacaoPS2,
             CASE WHEN MB.NIBExportaPS2 IS NOT NULL THEN MB.NIBExportaPS2 ELSE CASE WHEN MB.TipoEntidade = 'U' THEN
				 NULL ELSE (SELECT Top 1 NIB FROM ContasBancariasTerc WHERE TipoEntidade = MB.TipoEntidade and Entidade = MB.Entidade 
				 ORDER BY [DEFAULT] DESC) END END AS NIBTerceiro,
			CASE WHEN MB.TipoEntidade = 'U' THEN NULL 
				ELSE (SELECT Top 1 NUMConta FROM ContasBancariasTerc WHERE TipoEntidade = MB.TipoEntidade and Entidade = MB.Entidade 
				 ORDER BY [DEFAULT] DESC) END AS NUMContaTerceiro,
			CASE WHEN MB.TipoEntidade = 'U' THEN NULL 
				ELSE (SELECT Top 1 Banco FROM ContasBancariasTerc WHERE TipoEntidade = MB.TipoEntidade and Entidade = MB.Entidade 
				 ORDER BY [DEFAULT] DESC) END AS BancoTerceiro,
             CASE WHEN C.Nome IS NOT NULL AND MB.TipoEntidade = 'C' THEN C.Nome
                  ELSE CASE WHEN F.Nome IS NOT NULL AND MB.TipoEntidade = 'F' THEN F.Nome
                            ELSE CASE WHEN FU.Nome IS NOT NULL AND MB.TipoEntidade = 'U' THEN  FU.Nome
                                      ELSE CASE WHEN OT.Nome IS NOT NULL THEN OT.Nome 
                                                ELSE '' END
                                      END
                            END
                  END AS NomeTerceiro, (Case When MB.TipoMov ='D' Then -1 Else -1 * -1 End) * MB.Valor as Valor,
			case 
				when MB.TipoEntidade = 'C' then C.Cdu_Email 
				when MB.TipoEntidade = 'F' then F.Cdu_Email
				else OT.Cdu_Email end as Email,
			case 
				when MB.TipoEntidade = 'C' then C.CDU_DESTINATION_SORT_CODE 
				when MB.TipoEntidade = 'F' then F.CDU_DESTINATION_SORT_CODE
				else OT.CDU_DESTINATION_SORT_CODE end as CDU_DESTINATION_SORT_CODE

				
             FROM MovimentosBancos           MB 
             INNER JOIN DocumentosBancos     DB ON DB.Movim     = MB.Movim 
             INNER JOIN ContasBancarias      CB ON MB.Conta     = CB.Conta 
             LEFT JOIN Clientes         C ON C.Cliente    = MB.Entidade  
LEFT JOIN LinhasTesouraria               On LinhasTesouraria.IDMovimentosBancos  = MB.ID
LEFT JOIN  CabecTesouraria               On LinhasTesouraria.IdCabecTesouraria   = CabecTesouraria.Id
LEFT JOIN  RastreabilidadeEstornos ED    ON ED.IdDocDestino                      = CabecTesouraria.Id
LEFT JOIN  RastreabilidadeEstornos EO    ON EO.IdDocOrigem                       = CabecTesouraria.Id
             LEFT JOIN Fornecedores     F ON F.Fornecedor = MB.Entidade  
             LEFT JOIN OutrosTerceiros OT ON OT.Terceiro  = MB.Entidade  
             LEFT JOIN Funcionarios    FU ON FU.Codigo    = MB.Entidade  
             WHERE MB.Entidade IS NOT NULL AND MB.Entidade<>'' AND  ( ED.ID IS NULL AND EO.ID IS NULL)  --AND IDExportacaoPS2 ='31'
     ) AS MOV
GROUP BY MOV.Email,MOV.TipoEntidade, MOV.Entidade, MOV.NIBOrigem, MOV.ContaOrigem, MOV.IDExportacaoPS2, MOV.NIBTerceiro, MOV.NomeTerceiro,MOV.NUMContaTerceiro,MOV.BancoTerceiro
GO



ALTER view [dbo].[View_ImportadorFormatoMagnetico] as
SELECT MOV.TipoEntidade, MOV.Entidade, MOV.NIBOrigem, MOV.ContaOrigem, MOV.IDExportacaoPS2, MOV.NIBTerceiro,MOV.NUMContaTerceiro,MOV.BancoTerceiro, MOV.NomeTerceiro, SUM(MOV.Valor) AS Valor,
MOV.Email
FROM (SELECT MB.TipoEntidade, MB.Entidade, CB.NIB As NIBOrigem, CB.Conta As ContaOrigem,MB.IDExportacaoPS2,
             CASE WHEN MB.NIBExportaPS2 IS NOT NULL THEN MB.NIBExportaPS2 ELSE CASE WHEN MB.TipoEntidade = 'U' THEN
				 NULL ELSE (SELECT Top 1 NIB FROM ContasBancariasTerc WHERE TipoEntidade = MB.TipoEntidade and Entidade = MB.Entidade 
				 ORDER BY [DEFAULT] DESC) END END AS NIBTerceiro,
			CASE WHEN MB.TipoEntidade = 'U' THEN NULL 
				ELSE (SELECT Top 1 NUMConta FROM ContasBancariasTerc WHERE TipoEntidade = MB.TipoEntidade and Entidade = MB.Entidade 
				 ORDER BY [DEFAULT] DESC) END AS NUMContaTerceiro,
			CASE WHEN MB.TipoEntidade = 'U' THEN NULL 
				ELSE (SELECT Top 1 Banco FROM ContasBancariasTerc WHERE TipoEntidade = MB.TipoEntidade and Entidade = MB.Entidade 
				 ORDER BY [DEFAULT] DESC) END AS BancoTerceiro,
             CASE WHEN MB.TipoEntidade = 'C' THEN 
				  case when C.Descricao is not null then
					C.Descricao
				  else 
						C.Nome
				  end 
			 ELSE CASE WHEN MB.TipoEntidade = 'F' THEN
				case when F.Descricao is not null then
					F.Descricao
				  else 
						F.Nome
				  end 
             ELSE CASE WHEN FU.Nome IS NOT NULL AND MB.TipoEntidade = 'U' THEN  
					FU.Nome
			 ELSE CASE WHEN OT.Nome IS NOT NULL THEN 
				case when OT.Descricao is not null then
					OT.Descricao
				  else 
						OT.Nome
				  end 
			 
                                                ELSE '' END
                                      END
                            END
                  END AS NomeTerceiro,
				  
			(Case When MB.TipoMov ='D' Then -1 Else -1 * -1 End) * MB.Valor as Valor,
			
				case 
					when MB.TipoEntidade = 'C' then C.Cdu_Email 
					when MB.TipoEntidade = 'F' then F.Cdu_Email
					else OT.Cdu_Email end as Email,
				case 
					when MB.TipoEntidade = 'C' then C.CDU_DESTINATION_SORT_CODE 
					when MB.TipoEntidade = 'F' then F.CDU_DESTINATION_SORT_CODE
					else OT.CDU_DESTINATION_SORT_CODE end as CDU_DESTINATION_SORT_CODE

				
             FROM MovimentosBancos           MB 
             INNER JOIN DocumentosBancos     DB ON DB.Movim     = MB.Movim 
             INNER JOIN ContasBancarias      CB ON MB.Conta     = CB.Conta 
             LEFT JOIN Clientes         C ON C.Cliente    = MB.Entidade  
LEFT JOIN LinhasTesouraria               On LinhasTesouraria.IDMovimentosBancos  = MB.ID
LEFT JOIN  CabecTesouraria               On LinhasTesouraria.IdCabecTesouraria   = CabecTesouraria.Id
LEFT JOIN  RastreabilidadeEstornos ED    ON ED.IdDocDestino                      = CabecTesouraria.Id
LEFT JOIN  RastreabilidadeEstornos EO    ON EO.IdDocOrigem                       = CabecTesouraria.Id
             LEFT JOIN Fornecedores     F ON F.Fornecedor = MB.Entidade  
             LEFT JOIN OutrosTerceiros OT ON OT.Terceiro  = MB.Entidade  
             LEFT JOIN Funcionarios    FU ON FU.Codigo    = MB.Entidade  
             WHERE MB.Entidade IS NOT NULL AND MB.Entidade<>'' AND  ( ED.ID IS NULL AND EO.ID IS NULL)  --AND IDExportacaoPS2 ='31'
     ) AS MOV
GROUP BY MOV.Email,MOV.TipoEntidade, MOV.Entidade, MOV.NIBOrigem, MOV.ContaOrigem, MOV.IDExportacaoPS2, MOV.NIBTerceiro, MOV.NomeTerceiro,MOV.NUMContaTerceiro,MOV.BancoTerceiro

GO