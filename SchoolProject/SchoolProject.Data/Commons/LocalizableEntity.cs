using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Data.Commons
{
    public class LocalizableEntity
    {
        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string GetLocalized()
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;

            return culture.TwoLetterISOLanguageName.ToLower().Equals("ar")
                    ? NameAr : NameEn;
        }
    }
}

