using Caliburn.Micro;
using System.Collections.Generic;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.ViewModel
{
    public class LanguageViewModel : PropertyChangedBase, ILanguageModel
    {
        private int _id;
        public int Id 
        {
            get => _id;
            set
            {
                _id = value;
                NotifyOfPropertyChange();
            }
        }

        private string _languageName;
        public string LanguageName 
        {
            get => _languageName;
            set
            {
                _languageName = value;
                NotifyOfPropertyChange();
            }
        }

        private string _localizationSuffix;
        public string LocalizationSuffix 
        {
            get => _localizationSuffix;
            set
            {
                _localizationSuffix = value;
                NotifyOfPropertyChange();
            }
        }

        private string _languageCode;
        public string LanguageCode 
        {
            get => _languageCode;
            set
            {
                _languageCode = value;
                NotifyOfPropertyChange();
            } 
        }

        private bool _isDefault;
        public bool IsDefault 
        {
            get => _isDefault;
            set
            {
                _isDefault = value;
                NotifyOfPropertyChange();
            }
        }
        /*
        protected bool Equals(LanguageViewModel obj)
        {
            return Id == obj.Id &&
                   string.Equals(LanguageName, obj.LanguageName) &&
                   string.Equals(LocalizationSuffix, obj.LocalizationSuffix) &&
                   string.Equals(LanguageCode, obj.LanguageCode) &&
                   IsDefault == obj.IsDefault;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LanguageViewModel)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = -1875062899;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LanguageName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocalizationSuffix);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LanguageCode);
            hashCode = hashCode * -1521134295 + IsDefault.GetHashCode();
            return hashCode;
        }*/
    }
}
