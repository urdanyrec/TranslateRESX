using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslateRESX.DB.Entity;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.Converters
{
    public static class DataToApiModelConverter
    {
        public static BindableCollection<ApiKeyModel> Convert(IEnumerable<Data> data)
        {
            BindableCollection<ApiKeyModel> result = new BindableCollection<ApiKeyModel>();
            foreach (Data item in data)
            {
                var apiModel = new ApiKeyModel();
                apiModel.ApiKey = item.ApiKey;
                apiModel.Service = item.Service;
                result.Add(apiModel);
            }

            return result;
        }
    }
}
