// Views/Dialogs/ProductDialog.xaml.cs
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BakeryApp.Models;

namespace BakeryApp.Views.Dialogs
{
    public partial class ProductDialog : Window
    {
        public Product Result { get; private set; }
        private readonly Product _original;

        public ProductDialog(Product item)
        {
            InitializeComponent();
            _original     = item;
            txtName.Text  = item.Name;
            cmbType.Text  = item.Type;
            txtPrice.Text = item.SalePrice.ToString("G", CultureInfo.InvariantCulture);
            txtNotes.Text = item.Notes;
            Loaded += (_, __) => txtName.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name  = txtName.Text.Trim();
            string type  = (cmbType.SelectedItem as ComboBoxItem)?.Content?.ToString()
                           ?? cmbType.Text.Trim();
            string notes = txtNotes.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus(); return;
            }
            if (string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Укажите тип продукта.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbType.Focus(); return;
            }
            if (!decimal.TryParse(txtPrice.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                MessageBox.Show("Введите корректную цену.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrice.Focus(); return;
            }

            Result = new Product
            {
                Id        = _original.Id,
                Name      = name,
                Type      = type,
                SalePrice = price,
                Notes     = notes
            };
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
