// Models/Ingredient.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BakeryApp.Models
{
    public class Ingredient : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _unit = string.Empty;
        private decimal _quantity;
        private decimal _purchasePrice;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Unit
        {
            get => _unit;
            set { _unit = value; OnPropertyChanged(); }
        }

        public decimal Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public decimal PurchasePrice
        {
            get => _purchasePrice;
            set { _purchasePrice = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Ingredient Clone() => new Ingredient
        {
            Id            = Id,
            Name          = Name,
            Unit          = Unit,
            Quantity      = Quantity,
            PurchasePrice = PurchasePrice
        };
    }
}
