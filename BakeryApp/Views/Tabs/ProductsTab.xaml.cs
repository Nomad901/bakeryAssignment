// Views/Tabs/ProductsTab.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BakeryApp.Data;
using BakeryApp.Models;
using BakeryApp.Views.Dialogs;

namespace BakeryApp.Views.Tabs
{
    public partial class ProductsTab : UserControl
    {
        private readonly ProductRepository _repo = new ProductRepository();
        private List<Product> _items = new List<Product>();

        public ProductsTab()
        {
            InitializeComponent();

            // Не обращаться к БД в режиме дизайнера Visual Studio
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            Loaded += (_, __) => LoadData();
        }

        // ─── Загрузка ────────────────────────────────────────────────────────

        private void LoadData()
        {
            try
            {
                _items = _repo.GetAll(txtSearch.Text.Trim());
                dgProducts.ItemsSource = null;
                dgProducts.ItemsSource = _items;
                lblCount.Text = $"Записей: {_items.Count}";
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // ─── Добавление ──────────────────────────────────────────────────────

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm(out string name, out string type, out decimal price, out string notes))
                return;

            try
            {
                var item = new Product
                {
                    Name      = name,
                    Type      = type,
                    SalePrice = price,
                    Notes     = notes
                };
                _repo.Add(item);
                ClearForm();
                LoadData();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        // ─── Редактирование ─────────────────────────────────────────────────

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgProducts.SelectedItem as Product;
            if (selected == null) return;

            var dlg = new ProductDialog(selected.Clone()) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    _repo.Update(dlg.Result);
                    LoadData();
                }
                catch (Exception ex) { ShowError(ex); }
            }
        }

        // ─── Удаление ────────────────────────────────────────────────────────

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgProducts.SelectedItem as Product;
            if (selected == null) return;

            var result = MessageBox.Show(
                $"Удалить продукт «{selected.Name}»?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try { _repo.Delete(selected.Id); LoadData(); }
                catch (Exception ex) { ShowError(ex); }
            }
        }

        // ─── UI-события ─────────────────────────────────────────────────────

        private void DgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool has = dgProducts.SelectedItem != null;
            btnEdit.IsEnabled   = has;
            btnDelete.IsEnabled = has;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();

        // ─── Вспомогательные ─────────────────────────────────────────────────

        private bool ValidateForm(out string name, out string type, out decimal price, out string notes)
        {
            name  = txtProdName.Text.Trim();
            type  = (cmbProdType.SelectedItem as ComboBoxItem)?.Content?.ToString()
                    ?? cmbProdType.Text.Trim();
            price = 0;
            notes = txtProdNotes.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название продукта.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtProdName.Focus(); return false;
            }
            if (string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Выберите или введите тип продукта.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbProdType.Focus(); return false;
            }
            if (!decimal.TryParse(txtProdPrice.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out price))
            {
                MessageBox.Show("Введите корректную цену.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtProdPrice.Focus(); return false;
            }
            return true;
        }

        private void ClearForm()
        {
            txtProdName.Text  = string.Empty;
            cmbProdType.Text  = string.Empty;
            cmbProdType.SelectedIndex = -1;
            txtProdPrice.Text = "0";
            txtProdNotes.Text = string.Empty;
        }

        private void ShowError(Exception ex) =>
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
