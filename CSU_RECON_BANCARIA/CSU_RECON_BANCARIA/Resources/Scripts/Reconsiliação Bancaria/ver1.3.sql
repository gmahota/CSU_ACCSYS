
create view View_Nao_Rec_Bancos as 
select ceb.datafinal,leb.DataValor,leb.movimento,leb.natureza,leb.numero,leb.obs,leb.valorMov,leb.moedamov from LinhasExtractoBancario leb
inner join CabecExtractoBancario ceb on ceb.id = leb.idCabecExtractoBancario
where leb.id not in (select IdLinhasExtractoBancario from movimentosbancos where ReconciliadoPorExtracto =1)
go