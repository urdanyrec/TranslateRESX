using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateRESX.Core.Helpers
{
    public class TranslateTaskResult
    {
        public int StatusCode { get; set; }

        public string Request { get; set; }

        public string Answer { get; set; }

        public string TranslatedPhrase { get; set; }
    }
}
