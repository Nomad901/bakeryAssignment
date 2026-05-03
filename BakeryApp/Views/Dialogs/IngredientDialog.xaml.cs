// Views/Dialogs/IngredientDialog.xaml.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BakeryApp.Models;

namespace BakeryApp.Views.Dialogs
{
    public partial class IngredientDialog : Window
    {
        public Ingredient Result { get; private set; }
        private readonly Ingredient _original;

        public IngredientDialog(Ingredient item)
        {
            InitializeComponent();
            _original   = item;
            txtName.Text  = item.Name;
            cmbUnit.Text  = item.Unit;
            txtQty.Text   = item.Quantity.ToString("G", CultureInfo.InvariantCulture);
            txtPrice.Text = item.PurchasePrice.ToString("G", CultureInfo.InvariantCulture);
            Loaded += (_, __) => txtName.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name  = txtName.Text.Trim();
            string unit  = (cmbUnit.SelectedItem as ComboBoxItem)?.Content?.ToString()
                           ?? cmbUnit.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus(); return;
            }
            if (string.IsNullOrEmpty(unit))
            {
                MessageBox.Show("Укажите единицу измерения.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbUnit.Focus(); return;
            }
            if (!decimal.TryParse(txtQty.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal qty))
            {
                MessageBox.Show("Введите корректное количество.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtQty.Focus(); return;
            }
            if (!decimal.TryParse(txtPrice.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                MessageBox.Show("Введите корректную цену.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrice.Focus(); return;
            }

            Result = new Ingredient
            {
                Id            = _original.Id,
                Name          = name,
                Unit          = unit,
                Quantity      = qty,
                PurchasePrice = price
            };
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
