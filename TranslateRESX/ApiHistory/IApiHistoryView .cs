using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.ApiHistory
{
    public interface IApiHistoryView
    {
        ApiKeyModel SelectedApiKey { get; set; }
    }
}
