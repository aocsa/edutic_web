using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Web.Models
{
    [Serializable]
    public class ToastMessage
    {
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public ToastType ToastType { get; set; }
        public bool IsSticky { get; set; }
    }
}
