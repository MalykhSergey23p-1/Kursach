using System;

namespace АгентствоНедвижимости.Models
{
    public class Клиент
    {
        public int КодКлиента { get; set; }
        public string Фамилия { get; set; }
        public string Имя { get; set; }
        public string Отчество { get; set; }
        public string Телефон { get; set; }
        public string Email { get; set; }
        public DateTime ДатаРегистрации { get; set; }
    }
}