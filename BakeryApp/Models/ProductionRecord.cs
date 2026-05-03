// Models/ProductionRecord.cs
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BakeryApp.Models
{
    public class ProductionRecord : INotifyPropertyChanged
    {
        private int _id;
        private DateTime _recordDate = DateTime.Today;
        private string _productName = string.Empty;
        private decimal _quantity;
        private string _comment = string.Empty;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public DateTime RecordDate
        {
            get => _recordDate;
            set { _recordDate = value; OnPropertyChanged(); }
        }

        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(); }
        }

        public decimal Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ProductionRecord Clone() => new ProductionRecord
        {
            Id          = Id,
            RecordDate  = RecordDate,
            ProductName = ProductName,
            Quantity    = Quantity,
            Comment     = Comment
        };
    }
}
