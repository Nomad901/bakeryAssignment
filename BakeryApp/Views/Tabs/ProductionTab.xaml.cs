// Views/Tabs/ProductionTab.xaml.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BakeryApp.Data;
using BakeryApp.Models;
using BakeryApp.Views.Dialogs;

namespace BakeryApp.Views.Tabs
{
    public partial class ProductionTab : UserControl
    {
        private readonly ProductionRepository _repo    = new ProductionRepository();
        private readonly ProductRepository    _prodRepo = new ProductRepository();
        private List<ProductionRecord>        _items   = new List<ProductionRecord>();

        public ProductionTab()
        {
            InitializeComponent();

            // Не обращаться к БД в режиме дизайнера Visual Studio
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            dpPrDate.SelectedDate = DateTime.Today;
            Loaded += (_, __) => { LoadProducts(); LoadData(); };
        }

        // ─── Загрузка продуктов в ComboBox ──────────────────────────────────

        private void LoadProducts()
        {
            try
            {
                var names = _prodRepo.GetNames();
                cmbPrProduct.ItemsSource = names;
            }
            catch { /* не блокируем — ввод вручную тоже допустим */ }
        }

        // ─── Загрузка данных ─────────────────────────────────────────────────

        private void LoadData()
        {
            try
            {
                DateTime? from = dpPrFrom.SelectedDate;
                DateTime? to   = dpPrTo.SelectedDate;
                string search  = txtPrSearch.Text.Trim();

                _items = _repo.GetAll(from, to, search);
                dgProduction.ItemsSource = null;
                dgProduction.ItemsSource = _items;
                lblPrCount.Text = $"Записей: {_items.Count}";
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // ─── Добавление ──────────────────────────────────────────────────────

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm(out DateTime date, out string product, out decimal qty, out string comment))
                return;

            try
            {
                var rec = new ProductionRecord
                {
                    RecordDate  = date,
                    ProductName = product,
                    Quantity    = qty,
                    Comment     = comment
                };
                _repo.Add(rec);
                ClearForm();
                LoadData();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // ─── Редактирование ─────────────────────────────────────────────────

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgProduction.SelectedItem as ProductionRecord;
            if (selected == null) return;

            var dlg = new ProductionDialog(selected.Clone(), _prodRepo.GetNames())
            {
                Owner = Window.GetWindow(this)
            };
            if (dlg.ShowDialog() == true)
            {
                try { _repo.Update(dlg.Result); LoadData(); }
                catch (Exception ex) { ShowError(ex); }
            }
        }

        // ─── Удаление ────────────────────────────────────────────────────────

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgProduction.SelectedItem as ProductionRecord;
            if (selected == null) return;

            var res = MessageBox.Show(
                $"Удалить запись от {selected.RecordDate:dd.MM.yyyy} — «{selected.ProductName}»?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.Yes)
            {
                try { _repo.Delete(selected.Id); LoadData(); }
                catch (Exception ex) { ShowError(ex); }
            }
        }

        // ─── Фильтрация ─────────────────────────────────────────────────────

        private void BtnFilter_Click(object sender, RoutedEventArgs e) => LoadData();

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            dpPrFrom.SelectedDate = null;
            dpPrTo.SelectedDate   = null;
            txtPrSearch.Text      = string.Empty;
            LoadData();
        }

        // ─── UI-события ─────────────────────────────────────────────────────

        private void DgProduction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool has = dgProduction.SelectedItem != null;
            btnEdit.IsEnabled   = has;
            btnDelete.IsEnabled = has;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();

        // ─── Вспомогательные ─────────────────────────────────────────────────

        private bool ValidateForm(out DateTime date, out string product, out decimal qty, out string comment)
        {
            date    = DateTime.Today;
            product = string.Empty;
            qty     = 0;
            comment = txtPrComment.Text.Trim();

            if (dpPrDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpPrDate.Focus(); return false;
            }
            date = dpPrDate.SelectedDate.Value;

            product = cmbPrProduct.Text.Trim();
            if (string.IsNullOrEmpty(product))
            {
                MessageBox.Show("Выберите или введите название продукции.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbPrProduct.Focus(); return false;
            }

            if (!decimal.TryParse(txtPrQty.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out qty) || qty <= 0)
            {
                MessageBox.Show("Введите корректное количество (больше нуля).", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrQty.Focus(); return false;
            }
            return true;
        }

        private void ClearForm()
        {
            dpPrDate.SelectedDate = DateTime.Today;
            cmbPrProduct.Text     = string.Empty;
            cmbPrProduct.SelectedIndex = -1;
            txtPrQty.Text         = "1";
            txtPrComment.Text     = string.Empty;
        }

        private void ShowError(Exception ex) =>
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
