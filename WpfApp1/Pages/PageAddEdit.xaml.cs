using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Classes;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAddEdit.xaml
    /// </summary>
    public partial class PageAddEdit : Page
    {
        private Предприятия pred = new Предприятия();
        public PageAddEdit(Предприятия predlocal)
        {
            InitializeComponent();

            Cmbtovar.ItemsSource =
                УчебнаяEntities.GetContext().Товары.ToList();
            Cmbtovar.SelectedValuePath = "ID_товара";
            Cmbtovar.DisplayMemberPath = "Название_товара";

            if (predlocal != null)
                pred = predlocal;
            //создаем контекст

            DataContext = pred;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (pred.ID_предприятия == 0)
                УчебнаяEntities.GetContext().
                    Предприятия.Add(pred); //добавить в контекст

            try
            {
                УчебнаяEntities.GetContext().SaveChanges();
                MessageBox.Show("Изменения успешно сохранены");
                ClassFrame.frmObj.Navigate(new PageListStudent());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
