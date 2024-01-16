using System;
using System.Collections.Generic;
using System.IO;
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
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageListStudent.xaml
    /// </summary>
    public partial class PageListStudent : Page
    {
        public PageListStudent()
        {
            InitializeComponent();

            DtgListStudent.ItemsSource =
                    УчебнаяEntities.GetContext().Предприятия.ToList();

            // var listDisc =   lesuser28Entities.GetContext().Ученики_Кузьмина.
            //     Select(x => x.Предмет).Distinct().ToList();


            //CmbDiscipline.Items.Add("Все предметы");
            //foreach ( var item in listDisc ) 
            //{
            //    CmbDiscipline.Items.Add(item);
            //}
            CmbDiscipline.ItemsSource = УчебнаяEntities.GetContext().
                 Товары.ToList();
            CmbDiscipline.SelectedValuePath = "ID_товара";
            CmbDiscipline.DisplayMemberPath = "Название_товара";
        }

        private void CmbDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // string discipline = CmbDiscipline.SelectedValue.ToString();

            // if (discipline == "Все предметы")
            //     DtgListStudent.ItemsSource =
            //    lesuser28Entities.GetContext().Ученики_Кузьмина.ToList();
            // else
            //DtgListStudent.ItemsSource = 
            //     lesuser28Entities.GetContext().Ученики_Кузьмина.
            //     Where(x=>x.Предмет ==  discipline).ToList();
            int idDisc = int.Parse(CmbDiscipline.SelectedValue.ToString());
            DtgListStudent.ItemsSource =
                УчебнаяEntities.GetContext().Предприятия.
                Where(x => x.ID_предприятия == idDisc).ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TxtSearch.Text;
            DtgListStudent.ItemsSource =
                УчебнаяEntities.GetContext().Предприятия.
                Where(x => x.Название_предприятия.Contains(search)).ToList();
        }

        private void RbtnAsc_Click(object sender, RoutedEventArgs e)
        {
            //сортировка по возрастанию
            DtgListStudent.ItemsSource =
                УчебнаяEntities.GetContext().Предприятия.
                OrderBy(x => x.цена).ToList();
        }

        private void RbtnDesc_Click(object sender, RoutedEventArgs e)
        {
            //сортировка по убыванию
            DtgListStudent.ItemsSource =
                УчебнаяEntities.GetContext().Предприятия.
                OrderByDescending(x => x.цена).ToList();
        }

        private void BtnResults_Click(object sender, RoutedEventArgs e)
        {
            LstResults.Items.Clear();
            //подсчет агрегатных функций

            int count = УчебнаяEntities.GetContext().
                Предприятия.Count();

            double averageMark = (double)УчебнаяEntities.GetContext().
                Предприятия.Select(x => x.цена).Average();

            double minmark = (double)УчебнаяEntities.GetContext().
                            Предприятия.Select(x => x.цена).Min();

            double maxMark = (double)УчебнаяEntities.GetContext().
                Предприятия.Select(x => x.цена).Max();

            double sumMark = (double)УчебнаяEntities.GetContext().
                Предприятия.Select(x => x.цена).Sum();

            LstResults.Items.Add($"количество записей {count}");
            LstResults.Items.Add($"Средняя цена {averageMark}");
            LstResults.Items.Add($"Минимальная цена {minmark}");
            LstResults.Items.Add($"Максимальная цена {maxMark}");
            LstResults.Items.Add($"Общая ценность {sumMark}");

            string minMarkFIO = УчебнаяEntities.GetContext().
                Предприятия.First(x => x.цена == minmark).Название_предприятия.ToString();
            MessageBox.Show(minMarkFIO);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ClassFrame.frmObj.
                Navigate(new PageAddEdit(null));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // удаление нескольких строк
            var studentsForRemoving =
                DtgListStudent.SelectedItems.
                Cast<Предприятия>().ToList();

            if (MessageBox.Show
                ($"Удалить {studentsForRemoving.Count()} " +
                $"Предприятия?",
                "Внимание", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)

                try
                {
                    УчебнаяEntities.GetContext().
                        Предприятия.RemoveRange(studentsForRemoving);

                    УчебнаяEntities.GetContext().SaveChanges();

                    MessageBox.Show("Данные удалены");
                    DtgListStudent.ItemsSource =
                        УчебнаяEntities.GetContext().
                        Предприятия.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ClassFrame.frmObj.
               Navigate(new PageAddEdit((sender as Button ).DataContext as Предприятия));
        }

        private void BtnListView_Click(object sender, RoutedEventArgs e)
        {
            ClassFrame.frmObj.
            Navigate(new PageViewList());
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            //объект Excel
            var app = new Excel.Application();

            //книга 
            Excel.Workbook wb = app.Workbooks.Add();
            //лист
            Excel.Worksheet worksheet =
                app.Worksheets.Item[1];

            int indexRows = 1;
            //ячейка
            worksheet.Cells[1][indexRows] = "id предприятия";
            worksheet.Cells[2][indexRows] = "Название предприятия";
            worksheet.Cells[3][indexRows] = "Наименование_товара";
            worksheet.Cells[4][indexRows] = "цена";
            worksheet.Cells[5][indexRows] = "Объём";
            worksheet.Cells[6][indexRows] = "Себестоимость";

            //набор данных для вывода

            var listPR = УчебнаяEntities.
                GetContext().Предприятия.ToList();

            foreach (var Pred in listPR)
            {
                indexRows++;
                worksheet.Cells[1][indexRows] = indexRows - 1;
                worksheet.Cells[2][indexRows] = Pred.Название_предприятия;
                worksheet.Cells[3][indexRows] = Pred.Товары.Название_товара;
                worksheet.Cells[4][indexRows] = Pred.цена;
                worksheet.Cells[5][indexRows] = Pred.Объём;
                worksheet.Cells[6][indexRows] = Pred.Себестоимость;
            }
            //показать Excel
            app.Visible = true;
        }

        private void BtnExcelShablon_Click(object sender, RoutedEventArgs e)
        {
            //объект Excel
            var app = new Excel.Application();
            Excel.Workbook wb = app.Workbooks.Open($"" + $"{Directory.GetCurrentDirectory()}" + $"\\Шаблон.xlsx");
            Excel.Worksheet worksheet = (Excel.Worksheet)wb.Worksheets[1];

            //книга 
            //Excel.Workbook wb = app.Workbooks.Add();
            //лист
            //Excel.Worksheet worksheet =
            //app.Worksheets.Item[1];

            int indexRows = 1;
            //ячейка
            worksheet.Cells[1][indexRows] = "id предприятия";
            worksheet.Cells[2][indexRows] = "Название предприятия";
            worksheet.Cells[3][indexRows] = "Наименование_товара";
            worksheet.Cells[4][indexRows] = "цена";
            worksheet.Cells[5][indexRows] = "Объём";
            worksheet.Cells[6][indexRows] = "Себестоимость";

            //набор данных для вывода

            var listPR = УчебнаяEntities.
                GetContext().Предприятия.ToList();

            foreach (var Pred in listPR)
            {
                indexRows++;
                worksheet.Cells[1][indexRows] = indexRows - 1;
                worksheet.Cells[2][indexRows] = Pred.Название_предприятия;
                worksheet.Cells[3][indexRows] = Pred.Товары.Название_товара;
                worksheet.Cells[4][indexRows] = Pred.цена;
                worksheet.Cells[5][indexRows] = Pred.Объём;
                worksheet.Cells[6][indexRows] = Pred.Себестоимость;
            }
            Excel.Range proect = worksheet.Range[worksheet.Cells[1][indexRows + 1], worksheet.Cells[6][indexRows + 1]];
            proect.Merge();
            proect.Value = "Количество предприятий";
            proect.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            worksheet.Cells[7][indexRows + 1].Formula = "=Count(A2:A" + (indexRows + 1) + ")";

            proect.Font.Bold = worksheet.Cells[1][indexRows].Font.Bold = true;
            Excel.Range Rangebroders = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[7][indexRows + 1]];
            Rangebroders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
            Rangebroders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
            Rangebroders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
            Rangebroders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
            Rangebroders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
            Rangebroders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
            worksheet.Columns.AutoFit();
            app.Visible = true;
        }

        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            var allpred = УчебнаяEntities.GetContext().Предприятия.ToList();

            var application = new Word.Application();
            Word.Document document = application.Documents.Add();
            Word.Paragraph userParagraph = document.Paragraphs.Add();
            Word.Range userRange = userParagraph.Range;
            userRange.Text = "Karpov";

            userRange.InsertParagraphAfter();
            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table paymentsTable = document.Tables.Add(tableRange, allpred.Count + 1, 3);
            foreach (var book in allpred) 
            {
                paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle
                    = Word.WdLineStyle.wdLineStyleSingle;
                paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                Word.Range cellRange;
                cellRange = paymentsTable.Cell(1, 1).Range;
                cellRange.Text = "Название производства";
                cellRange = paymentsTable.Cell(1, 2).Range;
                cellRange.Text = "Название товара";
                cellRange = paymentsTable.Cell(1, 3).Range;
                cellRange.Text = "цена";
                paymentsTable.Rows[1].Range.Bold = 1;
                paymentsTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                for (int i = 0; i < allpred.Count; i++)
                {
                    var currentCategory = allpred[i];
                    cellRange = paymentsTable.Cell(i + 2, 1).Range;
                    cellRange.Text = currentCategory.Название_предприятия;
                    cellRange = paymentsTable.Cell(i + 2, 2).Range;
                    cellRange.Text = currentCategory.Товары.Название_товара.ToString();
                    cellRange = paymentsTable.Cell(i + 2, 3).Range;
                    cellRange.Text = currentCategory.цена.ToString();

                }
            }
            application.Visible = true;
        }
    }
}
