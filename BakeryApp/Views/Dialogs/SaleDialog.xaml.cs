// Views/Dialogs/SaleDialog.xaml.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BakeryApp.Models;

namespace BakeryApp.Views.Dialogs
{
    public partial class SaleDialog : Window
    {
        public SaleRecord Result { get; private set; }
        private readonly SaleRecord _original;

        public SaleDialog(SaleRecord item, List<string> productNames)
        {
            InitializeComponent();
            _original              = item;
            dpDate.SelectedDate    = item.RecordDate;
            cmbProduct.ItemsSource = productNames;
            cmbProduct.Text        = item.ProductName;
            txtQty.Text            = item.Quantity.ToString("G", CultureInfo.InvariantCulture);
            txtUnitPrice.Text      = item.UnitPrice.ToString("G", CultureInfo.InvariantCulture);
            txtTotal.Text          = item.TotalAmount.ToString("N2", CultureInfo.InvariantCulture);
            txtBuyer.Text          = item.Buyer;
            Loaded += (_, __) => cmbProduct.Focus();
        }

        private void Recalc_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtQty.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal q) &&
                decimal.TryParse(txtUnitPrice.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal p))
            {
                txtTotal.Text = (q * p).ToString("N2", CultureInfo.InvariantCulture);
            }
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
            if (!decimal.TryParse(txtUnitPrice.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Введите корректную цену.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUnitPrice.Focus(); return;
            }

            Result = new SaleRecord
            {
                Id          = _original.Id,
                RecordDate  = dpDate.SelectedDate.Value,
                ProductName = product,
                Quantity    = qty,
                UnitPrice   = unitPrice,
                TotalAmount = qty * unitPrice,
                Buyer       = txtBuyer.Text.Trim()
            };
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
