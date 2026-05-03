// Views/Dialogs/QuickQuantityDialog.xaml.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace BakeryApp.Views.Dialogs
{
    public partial class QuickQuantityDialog : Window
    {
        public decimal Value { get; private set; }

        public QuickQuantityDialog(string title, string prompt, string unit)
        {
            InitializeComponent();
            lblTitle.Text  = title;
            lblPrompt.Text = prompt;
            lblUnit.Text   = $"Значение ({unit}):";
            txtValue.Text  = "0";
            Loaded += (_, __) => { txtValue.Focus(); txtValue.SelectAll(); };
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e) => Confirm();
        private void BtnCancel_Click(object sender, RoutedEventArgs e) { DialogResult = false; }

        private void TxtValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)  Confirm();
            if (e.Key == Key.Escape) { DialogResult = false; }
        }

        private void Confirm()
        {
            if (!decimal.TryParse(txtValue.Text.Replace(',', '.'),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val) || val <= 0)
            {
                MessageBox.Show("Введите положительное число.", "Проверка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtValue.Focus();
                return;
            }
            Value = val;
            DialogResult = true;
        }
    }
}
