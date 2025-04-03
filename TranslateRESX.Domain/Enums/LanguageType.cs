using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateRESX.Domain.Enums
{
    public enum LanguageType
    {
        [Description("Русский")]
        Russian,

        [Description("English")]
        English,

        [Description("Francais")]
        French,

        [Description("Espagnol")]
        Spanish
    }
}
