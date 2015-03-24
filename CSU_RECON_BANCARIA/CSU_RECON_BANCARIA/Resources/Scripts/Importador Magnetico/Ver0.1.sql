
/****** Object:  StoredProcedure [dbo].[CSU_DROP_CDU]    Script Date: 08/21/2012 16:21:11 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CSU_DROP_CDU]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CSU_DROP_CDU]
GO

/****** Object:  StoredProcedure [dbo].[CSU_DROP_CDU]    Script Date: 08/21/2012 16:21:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CSU_DROP_CDU]
     @Table as VARCHAR(50),
     @Campo as VARCHAR(50) 
AS
BEGIN
      DECLARE @NomeConst      nvarchar (255)
      
      -- Por norma um campo tem duas contraints - Default e Foreign Key
      --1
      select distinct @NomeConst = obj2.name from sysobjects obj INNER JOIN syscolumns col on obj.id=col.id inner join sysconstraints const on const.id = obj.id and const.id = col.id and const.colid = col.colid inner join sysobjects obj2 on const.constid = obj2.id where obj2.xtype IN ('D','F') and obj.name=@Table and col.name IN (@campo) 
	  IF LEN(ISNULL(@NomeConst,'')) > 0 
		   EXEC STD_DROPConstraint  @Table, @NomeConst
      --2
      select distinct @NomeConst = obj2.name from sysobjects obj INNER JOIN syscolumns col on obj.id=col.id inner join sysconstraints const on const.id = obj.id and const.id = col.id and const.colid = col.colid inner join sysobjects obj2 on const.constid = obj2.id where obj2.xtype IN ('D','F') and obj.name=@Table and col.name IN (@campo) 
	  IF LEN(ISNULL(@NomeConst,'')) > 0 
		   EXEC STD_DROPConstraint  @Table, @NomeConst 
      
      EXEC STD_DROPColumn @Table, @Campo 
      
      DELETE FROM [dbo].[StdCamposVar]  WHERE ([Tabela]=@Table) AND ([Campo] IN ( @Campo ) )
END	

GO



/****** Object:  StoredProcedure [dbo].[CSU_CREATE_CDU]    Script Date: 08/21/2012 16:20:34 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CSU_CREATE_CDU]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CSU_CREATE_CDU]
GO

/****** Object:  StoredProcedure [dbo].[CSU_CREATE_CDU]    Script Date: 08/21/2012 16:20:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CSU_CREATE_CDU] 
      @Table as VARCHAR(50),
      @Campo VARCHAR(50), 
      @Tipo Varchar(20)='Varchar(20)', 
      @strDescricao as varchar(20)='', 
      @strNULL varchar(1024) ='NULL', 
      @Visivel BIT=1, 
      @ValorDefeito varchar(30)='', 
      @strQuery varchar(1000)=''
AS
BEGIN
      DECLARE @Icol        INT
      DECLARE @SQL            nvarchar (max)
      IF NOT EXISTS( SELECT col.ID 
            FROM sysobjects obj INNER JOIN syscolumns col ON (obj.ID = col.ID) 
            WHERE (obj.xtype='U') AND (obj.name=@Table))
      BEGIN
            SET @SQL='CREATE TABLE [' + @Table + '] ( [' + @Campo + '] ' + @Tipo + ' ' + @strNULL + ')'
            EXECUTE SP_EXECUTESQL @SQL
            INSERT INTO [dbo].[StdTabelasVar]([Tabela],[Apl]) VALUES (@Table, 'ERP')
      END
      IF NOT EXISTS( SELECT col.ID 
            FROM sysobjects obj INNER JOIN syscolumns col ON (obj.ID = col.ID) 
            WHERE (obj.xtype='U') AND (obj.name=@Table) AND (col.name IN (@Campo) ) )
      BEGIN
            SET @SQL='ALTER TABLE [' + @Table + '] ADD [' + @Campo + '] ' + @Tipo + ' ' + @strNULL
            EXECUTE SP_EXECUTESQL @SQL
      END
      SET @Visivel = 1
      DELETE FROM [dbo].[StdCamposVar]  WHERE ([Tabela]=@Table) AND 
            ([Campo] IN ( @Campo ))
      --
      SELECT @Icol = MAX(IsNull([Ordem],0)) FROM [StdCamposVar] WHERE ([Tabela]=@Table) 
      SET @Icol = IsNull(@Icol,0) + 1
      --
      INSERT INTO [dbo].[StdCamposVar]([Tabela],[Campo],[Descricao],[Texto],[Visivel],[Ordem],[Pagina],[ValorDefeito],[Query])
             VALUES(@Table, @Campo ,  @strDescricao, @strDescricao , @Visivel, @Icol, NULL, NULL, @strQuery)
      SET @Icol = @Icol + 1
END
GO

