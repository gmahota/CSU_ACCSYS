//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Empresas
    {
        public int Id { get; set; }
        public string CodEmpresa { get; set; }
        public string CodEmpresaPri { get; set; }
        public string NomeEmpresa { get; set; }
        public byte[] LogoTipo { get; set; }
        public string Conexao { get; set; }
        public Nullable<bool> EmpresaPrimavera { get; set; }
        public string TipoEmpresa { get; set; }
        public Nullable<bool> UseDefaultCredentials { get; set; }
        public string Credentials { get; set; }
        public Nullable<int> Port { get; set; }
        public Nullable<bool> EnableSsl { get; set; }
        public string Host { get; set; }
    }
}
