// Views/Dialogs/ProductionDialog.xaml.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using BakeryApp.Models;

namespace BakeryApp.Views.Dialogs
{
    public partial class ProductionDialog : Window
    {
        public ProductionRecord Result { get; private set; }
        private readonly ProductionRecord _original;

        public ProductionDialog(ProductionRecord item, List<string> productNames)
        {
            InitializeComponent();
            _original             = item;
            dpDate.SelectedDate   = item.RecordDate;
            cmbProduct.ItemsSource = productNames;
            cmbProduct.Text       = item.ProductName;
            txtQty.Text           = item.Quantity.ToString("G", CultureInfo.InvariantCulture);
            txtComment.Text       = item.Comment;
            Loaded += (_, __) => cmbProduct.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpDate.Focus(); return;
            }
            string product = cmbProduct.Text.Trim();
            if (string.IsNullOrEmpty(product))
            {
                MessageBox.Show("Введите продукцию.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbProduct.Focus(); return;
            }
            if (!decimal.TryParse(txtQty.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal qty) || qty <= 0)
            {
                MessageBox.Show("Введите корректное количество.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtQty.Focus(); return;
            }

            Result = new ProductionRecord
            {
                Id          = _original.Id,
                RecordDate  = dpDate.SelectedDate.Value,
                ProductName = product,
                Quantity    = qty,
                Comment     = txtComment.Text.Trim()
            };
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
