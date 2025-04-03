using System;

namespace TranslateRESX.Domain.Models
{
    public interface IConfig
    {
        bool Read();

        bool Write();
    }
}
