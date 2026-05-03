// Views/Tabs/IngredientsTab.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BakeryApp.Data;
using BakeryApp.Models;
using BakeryApp.Views.Dialogs;

namespace BakeryApp.Views.Tabs
{
    public partial class IngredientsTab : UserControl
    {
        private readonly IngredientRepository _repo = new IngredientRepository();
        private List<Ingredient> _items = new List<Ingredient>();

        public IngredientsTab()
        {
            InitializeComponent();

            // Не обращаться к БД в режиме дизайнера Visual Studio
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            Loaded += (_, __) => LoadData();
        }

        // ─── Загрузка данных ─────────────────────────────────────────────────

        private void LoadData()
        {
            try
            {
                _items = _repo.GetAll(txtSearch.Text.Trim());
                dgIngredients.ItemsSource = null;
                dgIngredients.ItemsSource = _items;
                lblCount.Text = $"Записей: {_items.Count}";
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // ─── Добавление ──────────────────────────────────────────────────────

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm(out string name, out string unit, out decimal qty, out decimal price))
                return;

            try
            {
                var item = new Ingredient
                {
                    Name          = name,
                    Unit          = unit,
                    Quantity      = qty,
                    PurchasePrice = price
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
            var selected = dgIngredients.SelectedItem as Ingredient;
            if (selected == null) return;

            var dlg = new IngredientDialog(selected.Clone()) { Owner = Window.GetWindow(this) };
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
            var selected = dgIngredients.SelectedItem as Ingredient;
            if (selected == null) return;

            var result = MessageBox.Show(
                $"Удалить ингредиент «{selected.Name}»?\nЭто действие необратимо.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _repo.Delete(selected.Id);
                    LoadData();
                }
                catch (Exception ex) { ShowError(ex); }
            }
        }

        // ─── Быстрый Приход ──────────────────────────────────────────────────

        private void BtnIncoming_Click(object sender, RoutedEventArgs e)
        {
            AdjustQuantity(positive: true);
        }

        // ─── Быстрый Расход ──────────────────────────────────────────────────

        private void BtnOutgoing_Click(object sender, RoutedEventArgs e)
        {
            AdjustQuantity(positive: false);
        }

        private void AdjustQuantity(bool positive)
        {
            var selected = dgIngredients.SelectedItem as Ingredient;
            if (selected == null) return;

            string title  = positive ? "Приход" : "Расход";
            string prompt = positive
                ? $"Введите количество для прихода\n{selected.Name} ({selected.Unit}):"
                : $"Введите количество для расхода\n{selected.Name} ({selected.Unit}):";

            var dlg = new QuickQuantityDialog(title, prompt, selected.Unit)
            {
                Owner = Window.GetWindow(this)
            };

            if (dlg.ShowDialog() == true && dlg.Value > 0)
            {
                try
                {
                    decimal delta = positive ? dlg.Value : -dlg.Value;
                    _repo.AdjustQuantity(selected.Id, delta);
                    LoadData();
                }
                catch (Exception ex) { ShowError(ex); }
            }
        }

        // ─── UI-события ─────────────────────────────────────────────────────

        private void DgIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = dgIngredients.SelectedItem != null;
            btnEdit.IsEnabled     = hasSelection;
            btnDelete.IsEnabled   = hasSelection;
            btnIncoming.IsEnabled = hasSelection;
            btnOutgoing.IsEnabled = hasSelection;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        // ─── Вспомогательные ─────────────────────────────────────────────────

        private bool ValidateForm(out string name, out string unit, out decimal qty, out decimal price)
        {
            name  = txtIngName.Text.Trim();
            unit  = (cmbUnit.SelectedItem as ComboBoxItem)?.Content?.ToString()
                    ?? cmbUnit.Text.Trim();
            qty   = 0;
            price = 0;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название ингредиента.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIngName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(unit))
            {
                MessageBox.Show("Выберите или введите единицу измерения.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbUnit.Focus();
                return false;
            }
            if (!decimal.TryParse(txtIngQty.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out qty))
            {
                MessageBox.Show("Введите корректное количество.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIngQty.Focus();
                return false;
            }
            if (!decimal.TryParse(txtIngPrice.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out price))
            {
                MessageBox.Show("Введите корректную цену.", "Проверка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIngPrice.Focus();
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            txtIngName.Text  = string.Empty;
            cmbUnit.Text     = string.Empty;
            cmbUnit.SelectedIndex = -1;
            txtIngQty.Text   = "0";
            txtIngPrice.Text = "0";
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
