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
    
    public partial class View_Lista_Contactos_Pendentes
    {
        public string Cliente { get; set; }
        public string Nome { get; set; }
        public string Fac_Mor { get; set; }
        public string Fac_Local { get; set; }
        public string NumContrib { get; set; }
        public string Pais { get; set; }
        public string Fac_Tel { get; set; }
        public string Moeda { get; set; }
        public string CDU_ContaRec { get; set; }
        public Nullable<bool> CDU_EnviaCobranca { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public string Titulo { get; set; }
        public string Email { get; set; }
        public string EmailAssist { get; set; }
        public string tipoContacto { get; set; }
        public Nullable<double> ValorPendente { get; set; }
        public Nullable<double> ValorTotal { get; set; }
    }
}