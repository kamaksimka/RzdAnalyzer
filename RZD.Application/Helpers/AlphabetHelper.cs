using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Helpers
{
    public static class AlphabetHelper
    {
        public static readonly ReadOnlyCollection<string> RussianAlphabet = new ReadOnlyCollection<string>(
            new string[]
            {
                "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я"
            });
    }
}
