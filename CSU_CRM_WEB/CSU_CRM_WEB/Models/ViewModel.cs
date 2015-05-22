using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSU_CRM_WEB.Models
{
    public class ViewModel
    {
    }

    public class YearlyStat
    {
        public int Year { get; set; }
        public int Value { get; set; }
    }
    
    public class Pendentes
    {
        public string Modulo { get; set; }
        public string TipoEntidade { get; set; }
        public string Entidade { get; set; }
        public string TipoDoc { get; set; }
        public string NumDoc { get; set; }
        public int NumDocInt { get; set; }
        public Nullable<System.DateTime> DataDoc { get; set; }
        public Nullable<System.DateTime> DataVenc { get; set; }
        public Nullable<double> ValorTotal { get; set; }
        public Nullable<double> ValorPendente { get; set; }
        public string Moeda { get; set; }
        public Nullable<double> Cambio { get; set; }
        public Nullable<short> NumAvisos { get; set; }
        public short NumPrestacao { get; set; }
        public string Serie { get; set; }
        public string Conta { get; set; }
    }

    public class Documentos
    {
        public string modulo { get; set; }
        public string tipoEntidade { get; set; }
        public string entidade { get; set; }
        public string tipoDoc { get; set; }
        public string numDoc { get; set; }
        public int numDocInt { get; set; }
        public Nullable<System.DateTime> dataDoc { get; set; }
        public Nullable<System.DateTime> dataVenc { get; set; }
        public Nullable<double> valorTotal { get; set; }
        public Nullable<double> valorPendente { get; set; }
        public string moeda { get; set; }
        public Nullable<double> cambio { get; set; }
        public Nullable<short> numAvisos { get; set; }
        public short numPrestacao { get; set; }
        public string serie { get; set; }
        public string conta { get; set; }


        public Entidade Entidade { get; set; }
        public List<Documentos_Pendentes> Documentos_Pendentes { get; set; }
    }

    public class Documentos_Pendentes
    {
        public string modulo { get; set; }
        public string tipoEntidade { get; set; }
        public string entidade { get; set; }
        public string tipoDoc { get; set; }
        public string numDoc { get; set; }
        public int numDocInt { get; set; }
        public Nullable<System.DateTime> dataDoc { get; set; }
        public Nullable<System.DateTime> dataVenc { get; set; }
        public Nullable<double> valorTotal { get; set; }
        public Nullable<double> valorPendente { get; set; }
        public string moeda { get; set; }
        public Nullable<double> cambio { get; set; }
        public Nullable<short> numAvisos { get; set; }
        public short numPrestacao { get; set; }
        public string serie { get; set; }
        public string conta { get; set; }

        public Entidade Entidade { get; set; }
    }

    public class Entidade
    {
        public string tipoEntidade { get; set; }
        public string entidade { get; set; }
        public string Nome { get; set; }
        public string Fac_Mor { get; set; }
        public string Fac_Local { get; set; }
        public string NumContrib { get; set; }
        public string Pais { get; set; }
        public string Fac_Tel { get; set; }
        public string Moeda { get; set; }
        public string CDU_ContaRec { get; set; }
        public Nullable<bool> CDU_EnviaCobranca { get; set; }
        public Nullable<double> ValorPendente { get; set; }
        public Nullable<double> ValorTotal { get; set; }

        public List<Documentos_Pendentes> documentosPendentes { get; set; }
        public List<Contactos> contactos { get; set; }
    }

    public class Contactos
    {
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public string Titulo { get; set; }
        public string Email { get; set; }
        public string EmailAssist { get; set; }
        public string tipoContacto { get; set; }
    }
}

