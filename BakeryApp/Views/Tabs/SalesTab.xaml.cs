// Views/Tabs/SalesTab.xaml.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BakeryApp.Data;
using BakeryApp.Models;
using BakeryApp.Views.Dialogs;

namespace BakeryApp.Views.Tabs
{
    public partial class SalesTab : UserControl
    {
        private readonly SaleRepository    _repo     = new SaleRepository();
        private readonly ProductRepository _prodRepo = new ProductRepository();
        private List<SaleRecord>           _items    = new List<SaleRecord>();

        public SalesTab()
        {
            InitializeComponent();

            // Не обращаться к БД в режиме дизайнера Visual Studio
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            dpSaleDate.SelectedDate = DateTime.Today;
            Loaded += (_, __) => { LoadProducts(); LoadData(); };
        }

        // ─── Загрузка продуктов в ComboBox ──────────────────────────────────

        private void LoadProducts()
        {
            try
            {
                var names = _prodRepo.GetNames();
                cmbSaleProduct.ItemsSource = names;

                // При выборе продукта подставляем справочную цену
                cmbSaleProduct.SelectionChanged += (s, e) =>
                {
                    if (cmbSaleProduct.SelectedItem is string name)
                    {
                        var products = _prodRepo.GetAll();
                        var p = products.FirstOrDefault(x => x.Name == name);
                        if (p != null)
                            txtSalePrice.Text = p.SalePrice.ToString("N2",
                                CultureInfo.InvariantCulture);
                    }
                };
            }
            catch { }
        }

        // ─── Загрузка данных ─────────────────────────────────────────────────

        private void LoadData()
        {
            try
            {
                DateTime? from = dpSaleFrom.SelectedDate;
                DateTime? to   = dpSaleTo.SelectedDate;
                string search  = txtSaleSearch.Text.Trim();

                _items = _repo.GetAll(from, to, search);
                dgSales.ItemsSource = null;
                dgSales.ItemsSource = _items;
                lblSaleCount.Text   = $"Записей: {_items.Count}";

                decimal total = _items.Sum(x => x.TotalAmount);
                lblTotal.Text = $"{total:N2} ₽";
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // ─── Добавление ──────────────────────────────────────────────────────

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm(out DateTime date, out string product,
                    out decimal qty, out decimal unitPrice, out decimal total, out string buyer))
                return;

            try
            {
                var rec = new SaleRecord
                {
                    RecordDate  = date,
                    ProductName = product,
                    Quantity    = qty,
                    UnitPrice   = unitPrice,
                    TotalAmount = total,
                    Buyer       = buyer
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
            var selected = dgSales.SelectedItem as SaleRecord;
            if (selected == null) return;

            var dlg = new SaleDialog(selected.Clone(), _prodRepo.GetNames())
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
            var selected = dgSales.SelectedItem as SaleRecord;
            if (selected == null) return;

            var res = MessageBox.Show(
                $"Удалить запись о продаже «{selected.ProductName}» от {selected.RecordDate:dd.MM.yyyy}?",
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
            dpSaleFrom.SelectedDate = null;
            dpSaleTo.SelectedDate   = null;
            txtSaleSearch.Text      = string.Empty;
            LoadData();
        }

        // ─── UI-события ─────────────────────────────────────────────────────

        private void DgSales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool has = dgSales.SelectedItem != null;
            btnEdit.IsEnabled   = has;
            btnDelete.IsEnabled = has;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();

        /// <summary>Пересчёт поля «Итого» при изменении количества или цены.</summary>
        private void CalcTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtSaleQty.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal q) &&
                decimal.TryParse(txtSalePrice.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal p))
            {
                txtSaleTotal.Text = (q * p).ToString("N2", CultureInfo.InvariantCulture);
            }
        }

        // ─── Валидация формы ─────────────────────────────────────────────────

        private bool ValidateForm(out DateTime date, out string product,
            out decimal qty, out decimal unitPrice, out decimal total, out string buyer)
        {
            date      = DateTime.Today;
            product   = string.Empty;
            qty       = 0; unitPrice = 0; total = 0;
            buyer     = txtSaleBuyer.Text.Trim();

            if (dpSaleDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpSaleDate.Focus(); return false;
            }
            date = dpSaleDate.SelectedDate.Value;

            product = cmbSaleProduct.Text.Trim();
            if (string.IsNullOrEmpty(product))
            {
                MessageBox.Show("Введите или выберите продукцию.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSaleProduct.Focus(); return false;
            }
            if (!decimal.TryParse(txtSaleQty.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out qty) || qty <= 0)
            {
                MessageBox.Show("Введите корректное количество (больше нуля).", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtSaleQty.Focus(); return false;
            }
            if (!decimal.TryParse(txtSalePrice.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Введите корректную цену за единицу.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtSalePrice.Focus(); return false;
            }
            total = qty * unitPrice;
            return true;
        }

        private void ClearForm()
        {
            dpSaleDate.SelectedDate    = DateTime.Today;
            cmbSaleProduct.Text        = string.Empty;
            cmbSaleProduct.SelectedIndex = -1;
            txtSaleQty.Text            = "1";
            txtSalePrice.Text          = "0";
            txtSaleTotal.Text          = "0";
            txtSaleBuyer.Text          = string.Empty;
        }

        private void ShowError(Exception ex) =>
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
