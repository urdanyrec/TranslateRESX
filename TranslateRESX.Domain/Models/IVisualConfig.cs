using Newtonsoft.Json;
using System.Xml.Serialization;
using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Domain.Models
{
    public interface IVisualConfig : IParametersModel, IConfig
    {
        [JsonIgnore]
        string DataPath { get; }
    }
}
