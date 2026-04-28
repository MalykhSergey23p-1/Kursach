using System;

namespace АгентствоНедвижимости.Models
{
    public class Заявка
    {
        public int КодЗаявки { get; set; }
        public int КодКлиента { get; set; }
        public string ЖелаемыйТип { get; set; }
        public decimal? МинПлощадь { get; set; }
        public decimal? МаксЦена { get; set; }
        public string ЖелаемоеМестоположение { get; set; }
        public string СтатусЗаявки { get; set; }
        public DateTime ДатаСоздания { get; set; }
    }
}