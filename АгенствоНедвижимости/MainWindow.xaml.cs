using System;
using System.Windows;
using System.Windows.Controls;
using АгентствоНедвижимости.DataAccess;
using АгентствоНедвижимости.Models;

namespace АгентствоНедвижимости
{
    public partial class MainWindow : Window
    {
        private БДРабота бд = new БДРабота();
        private int выбраннаяЗаявкаId = 0;

        public MainWindow()
        {
            InitializeComponent();
            ЗагрузитьДанные();
        }

        private void ЗагрузитьДанные()
        {
            dgОбъекты.ItemsSource = бд.ПолучитьВсеОбъекты();
            dgКлиенты.ItemsSource = бд.ПолучитьВсехКлиентов();
            dgЗаявки.ItemsSource = бд.ПолучитьВсеЗаявки();
        }

        private void btnОбновитьОбъекты_Click(object sender, RoutedEventArgs e)
        {
            dgОбъекты.ItemsSource = бд.ПолучитьВсеОбъекты();
        }

        private void btnПодобрать_Click(object sender, RoutedEventArgs e)
        {
            string тип = (cmbТип.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (тип == "Любой") тип = null;

            decimal? минПлощадь = null;
            if (!string.IsNullOrEmpty(txtМинПлощадь.Text))
                минПлощадь = Convert.ToDecimal(txtМинПлощадь.Text);

            decimal? максЦена = null;
            if (!string.IsNullOrEmpty(txtМаксЦена.Text))
                максЦена = Convert.ToDecimal(txtМаксЦена.Text);

            string местоположение = string.IsNullOrEmpty(txtМестоположение.Text) ? null : txtМестоположение.Text;

            dgПодбор.ItemsSource = бд.ПодобратьОбъекты(тип, минПлощадь, максЦена, местоположение);
        }

        private void btnДобавитьКлиента_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtФамилия.Text) || string.IsNullOrEmpty(txtИмя.Text) || string.IsNullOrEmpty(txtТелефон.Text))
            {
                MessageBox.Show("Заполните фамилию, имя и телефон!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Клиент новый = new Клиент
            {
                Фамилия = txtФамилия.Text,
                Имя = txtИмя.Text,
                Телефон = txtТелефон.Text,
                Отчество = null,
                Email = null
            };
            бд.ДобавитьКлиента(новый);
            MessageBox.Show("Клиент добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            txtФамилия.Clear();
            txtИмя.Clear();
            txtТелефон.Clear();
            dgКлиенты.ItemsSource = бд.ПолучитьВсехКлиентов();
        }

        private void btnСформироватьОтчет_Click(object sender, RoutedEventArgs e)
        {
            dgОтчет.ItemsSource = бд.ПолучитьОтчетПоПроданным().DefaultView;
        }

        private void btnОбновитьЗаявки_Click(object sender, RoutedEventArgs e)
        {
            dgЗаявки.ItemsSource = бд.ПолучитьВсеЗаявки();
        }

        private void dgЗаявки_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgЗаявки.SelectedItem is Заявка заявка)
            {
                выбраннаяЗаявкаId = заявка.КодЗаявки;
            }
        }

        private void btnИзменитьСтатус_Click(object sender, RoutedEventArgs e)
        {
            if (выбраннаяЗаявкаId == 0)
            {
                MessageBox.Show("Выберите заявку!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string новыйСтатус = (cmbНовыйСтатус.SelectedItem as ComboBoxItem)?.Content.ToString();
            бд.ОбновитьСтатусЗаявки(выбраннаяЗаявкаId, новыйСтатус);
            MessageBox.Show("Статус заявки обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            dgЗаявки.ItemsSource = бд.ПолучитьВсеЗаявки();
            выбраннаяЗаявкаId = 0;
        }
    }
}