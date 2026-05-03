// Models/SaleRecord.cs
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BakeryApp.Models
{
    public class SaleRecord : INotifyPropertyChanged
    {
        private int _id;
        private DateTime _recordDate = DateTime.Today;
        private string _productName = string.Empty;
        private decimal _quantity;
        private decimal _unitPrice;
        private decimal _totalAmount;
        private string _buyer = string.Empty;

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

        public decimal UnitPrice
        {
            get => _unitPrice;
            set { _unitPrice = value; OnPropertyChanged(); }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }

        /// <summary>Покупатель / точка сбыта (необязательно)</summary>
        public string Buyer
        {
            get => _buyer;
            set { _buyer = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public SaleRecord Clone() => new SaleRecord
        {
            Id          = Id,
            RecordDate  = RecordDate,
            ProductName = ProductName,
            Quantity    = Quantity,
            UnitPrice   = UnitPrice,
            TotalAmount = TotalAmount,
            Buyer       = Buyer
        };
    }
}
