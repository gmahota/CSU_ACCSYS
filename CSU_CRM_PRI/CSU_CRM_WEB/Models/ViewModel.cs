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
        public string data { get; set; }
        public double valor { get; set; }
        public string empresa { get; set; }
        public string cliente { get; set; }


        //public int Year { get; set; }
        //public int Value { get; set; }
    }
}

