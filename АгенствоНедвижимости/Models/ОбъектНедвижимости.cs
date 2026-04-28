using System;

namespace АгентствоНедвижимости.Models
{
    public class ОбъектНедвижимости
    {
        public int КодОбъекта { get; set; }
        public string Адрес { get; set; }
        public string ТипНедвижимости { get; set; }
        public decimal Площадь { get; set; }
        public decimal Цена { get; set; }
        public int? КоличествоКомнат { get; set; }
        public string Местоположение { get; set; }
        public string Статус { get; set; }
        public DateTime ДатаДобавления { get; set; }
    }
}