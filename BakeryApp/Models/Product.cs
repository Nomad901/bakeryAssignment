// Models/Product.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BakeryApp.Models
{
    public class Product : INotifyPropertyChanged
    {
        private int _id;
        private string _name = string.Empty;
        private string _type = string.Empty;
        private decimal _salePrice;
        private string _notes = string.Empty;

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

        /// <summary>Хлеб / Булка / Пирожное / ...</summary>
        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        public decimal SalePrice
        {
            get => _salePrice;
            set { _salePrice = value; OnPropertyChanged(); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Product Clone() => new Product
        {
            Id        = Id,
            Name      = Name,
            Type      = Type,
            SalePrice = SalePrice,
            Notes     = Notes
        };
    }
}
