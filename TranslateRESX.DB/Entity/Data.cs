using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateRESX.DB.Entity
{
    [Serializable]
    public class Data : IEntity
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public string DictinaryKey { get; set; }

        public string ApiKey { get; set; }

        public int StatusCode { get; set; }

        public string RequestString { get; set; }

        public string AnswerString { get; set; }

        public string SourceLanguage { get; set; }

        public string DestinationLanguage { get; set; }

        public string SourcePhrase { get; set; }

        public string DestinationPhrase { get; set; }
    }
}
