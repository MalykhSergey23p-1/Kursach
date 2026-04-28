using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using АгентствоНедвижимости.Models;

namespace АгентствоНедвижимости.DataAccess
{
    public class БДРабота
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["БазаДанных"].ConnectionString;

        public List<ОбъектНедвижимости> ПолучитьВсеОбъекты()
        {
            var список = new List<ОбъектНедвижимости>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM ОбъектыНедвижимости ORDER BY КодОбъекта";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // ИСПРАВЛЕНО: используем Convert.ToInt32 для nullable полей
                    int? количествоКомнат = null;
                    if (reader["КоличествоКомнат"] != DBNull.Value)
                        количествоКомнат = Convert.ToInt32(reader["КоличествоКомнат"]);

                    список.Add(new ОбъектНедвижимости
                    {
                        КодОбъекта = Convert.ToInt32(reader["КодОбъекта"]),
                        Адрес = reader["Адрес"].ToString(),
                        ТипНедвижимости = reader["ТипНедвижимости"].ToString(),
                        Площадь = Convert.ToDecimal(reader["Площадь"]),
                        Цена = Convert.ToDecimal(reader["Цена"]),
                        КоличествоКомнат = количествоКомнат,
                        Местоположение = reader["Местоположение"].ToString(),
                        Статус = reader["Статус"].ToString(),
                        ДатаДобавления = Convert.ToDateTime(reader["ДатаДобавления"])
                    });
                }
            }
            return список;
        }

        public List<ОбъектНедвижимости> ПодобратьОбъекты(string тип, decimal? минПлощадь, decimal? максЦена, string местоположение)
        {
            var результат = new List<ОбъектНедвижимости>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM ОбъектыНедвижимости 
                                WHERE Статус = 'Доступно' 
                                AND (@Тип IS NULL OR ТипНедвижимости = @Тип)
                                AND (@МинПлощадь IS NULL OR Площадь >= @МинПлощадь)
                                AND (@МаксЦена IS NULL OR Цена <= @МаксЦена)
                                AND (@Местоположение IS NULL OR Местоположение LIKE '%' + @Местоположение + '%')";
                SqlCommand cmd = new SqlCommand(query, conn);

                // ИСПРАВЛЕНО: явное приведение к object для передачи null
                cmd.Parameters.AddWithValue("@Тип", (object)тип ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@МинПлощадь", минПлощадь.HasValue ? (object)минПлощадь.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@МаксЦена", максЦена.HasValue ? (object)максЦена.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Местоположение", (object)местоположение ?? (object)DBNull.Value);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int? количествоКомнат = null;
                    if (reader["КоличествоКомнат"] != DBNull.Value)
                        количествоКомнат = Convert.ToInt32(reader["КоличествоКомнат"]);

                    результат.Add(new ОбъектНедвижимости
                    {
                        КодОбъекта = Convert.ToInt32(reader["КодОбъекта"]),
                        Адрес = reader["Адрес"].ToString(),
                        ТипНедвижимости = reader["ТипНедвижимости"].ToString(),
                        Площадь = Convert.ToDecimal(reader["Площадь"]),
                        Цена = Convert.ToDecimal(reader["Цена"]),
                        КоличествоКомнат = количествоКомнат,
                        Местоположение = reader["Местоположение"].ToString(),
                        Статус = reader["Статус"].ToString(),
                        ДатаДобавления = Convert.ToDateTime(reader["ДатаДобавления"])
                    });
                }
            }
            return результат;
        }

        public List<Клиент> ПолучитьВсехКлиентов()
        {
            var список = new List<Клиент>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Клиенты ORDER BY КодКлиента";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    список.Add(new Клиент
                    {
                        КодКлиента = Convert.ToInt32(reader["КодКлиента"]),
                        Фамилия = reader["Фамилия"].ToString(),
                        Имя = reader["Имя"].ToString(),
                        Отчество = reader["Отчество"] == DBNull.Value ? null : reader["Отчество"].ToString(),
                        Телефон = reader["Телефон"].ToString(),
                        Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                        ДатаРегистрации = Convert.ToDateTime(reader["ДатаРегистрации"])
                    });
                }
            }
            return список;
        }

        public void ДобавитьКлиента(Клиент клиент)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Клиенты (Фамилия, Имя, Отчество, Телефон, Email) 
                                VALUES (@Фамилия, @Имя, @Отчество, @Телефон, @Email)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Фамилия", клиент.Фамилия);
                cmd.Parameters.AddWithValue("@Имя", клиент.Имя);
                // ИСПРАВЛЕНО: проверка на null
                cmd.Parameters.AddWithValue("@Отчество", string.IsNullOrEmpty(клиент.Отчество) ? (object)DBNull.Value : (object)клиент.Отчество);
                cmd.Parameters.AddWithValue("@Телефон", клиент.Телефон);
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(клиент.Email) ? (object)DBNull.Value : (object)клиент.Email);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable ПолучитьОтчетПоПроданным()
        {
            DataTable таблица = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                                    П.КодПродажи,
                                    О.Адрес,
                                    О.ТипНедвижимости,
                                    О.Площадь,
                                    П.ЦенаПродажи,
                                    П.ДатаПродажи,
                                    К.Фамилия + ' ' + К.Имя AS Покупатель
                                FROM Продажи П
                                JOIN ОбъектыНедвижимости О ON П.КодОбъекта = О.КодОбъекта
                                JOIN Клиенты К ON П.КодКлиента = К.КодКлиента
                                ORDER BY П.ДатаПродажи DESC";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(таблица);
            }
            return таблица;
        }

        public List<Заявка> ПолучитьВсеЗаявки()
        {
            var список = new List<Заявка>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM ЗаявкиНаПокупку ORDER BY КодЗаявки";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // ИСПРАВЛЕНО: явная обработка nullable полей
                    decimal? минПлощадь = null;
                    if (reader["МинПлощадь"] != DBNull.Value)
                        минПлощадь = Convert.ToDecimal(reader["МинПлощадь"]);

                    decimal? максЦена = null;
                    if (reader["МаксЦена"] != DBNull.Value)
                        максЦена = Convert.ToDecimal(reader["МаксЦена"]);

                    список.Add(new Заявка
                    {
                        КодЗаявки = Convert.ToInt32(reader["КодЗаявки"]),
                        КодКлиента = Convert.ToInt32(reader["КодКлиента"]),
                        ЖелаемыйТип = reader["ЖелаемыйТип"].ToString(),
                        МинПлощадь = минПлощадь,
                        МаксЦена = максЦена,
                        ЖелаемоеМестоположение = reader["ЖелаемоеМестоположение"].ToString(),
                        СтатусЗаявки = reader["СтатусЗаявки"].ToString(),
                        ДатаСоздания = Convert.ToDateTime(reader["ДатаСоздания"])
                    });
                }
            }
            return список;
        }

        public void ОбновитьСтатусЗаявки(int кодЗаявки, string новыйСтатус)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE ЗаявкиНаПокупку SET СтатусЗаявки = @Статус WHERE КодЗаявки = @Код";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Статус", новыйСтатус);
                cmd.Parameters.AddWithValue("@Код", кодЗаявки);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}