﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSU_CRM_WEB.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PRIACCEntities : DbContext
    {
        public PRIACCEntities()
            : base("name=PRIACCEntities")
        {
        }

        
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<View_Lista_Contactos_Pendentes> View_Lista_Contactos_Pendentes { get; set; }
        public virtual DbSet<View_Bancos_Cobrancas> View_Bancos_Cobrancas { get; set; }
        public virtual DbSet<View_Pendentes_Doc_Clientes> View_Pendentes_Doc_Clientes { get; set; }
    }
}
