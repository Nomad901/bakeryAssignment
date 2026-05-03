// Data/SaleRepository.cs
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BakeryApp.Models;

namespace BakeryApp.Data
{
    public class SaleRepository
    {
        // ─── Чтение ────────────────────────────────────────────────────────────

        public List<SaleRecord> GetAll(DateTime? from = null, DateTime? to = null, string search = "")
        {
            var list = new List<SaleRecord>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT Id, RecordDate, ProductName, Quantity, UnitPrice, TotalAmount, Buyer
FROM   SaleRecords
WHERE  (@from IS NULL OR RecordDate >= @from)
  AND  (@to   IS NULL OR RecordDate <= @to)
  AND  (@s    = ''    OR ProductName LIKE @sl OR Buyer LIKE @sl)
ORDER  BY RecordDate DESC, Id DESC";

                cmd.Parameters.AddWithValue("@from", from.HasValue ? (object)from.Value.Date : DBNull.Value);
                cmd.Parameters.AddWithValue("@to",   to.HasValue   ? (object)to.Value.Date   : DBNull.Value);
                cmd.Parameters.AddWithValue("@s",    search ?? "");
                cmd.Parameters.AddWithValue("@sl",   $"%{search}%");

                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(Map(r));
            }
            return list;
        }

        public SaleRecord GetById(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT Id, RecordDate, ProductName, Quantity, UnitPrice, TotalAmount, Buyer FROM SaleRecords WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Map(r) : null;
            }
        }

        // ─── Запись ────────────────────────────────────────────────────────────

        public void Add(SaleRecord item)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO SaleRecords (RecordDate, ProductName, Quantity, UnitPrice, TotalAmount, Buyer)
VALUES (@date, @name, @qty, @uprice, @total, @buyer);
SELECT SCOPE_IDENTITY();";
                Bind(cmd, item);
                item.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void Update(SaleRecord item)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE SaleRecords
SET    RecordDate  = @date,  ProductName = @name,
       Quantity    = @qty,   UnitPrice   = @uprice,
       TotalAmount = @total, Buyer       = @buyer
WHERE  Id = @id";
                Bind(cmd, item);
                cmd.Parameters.AddWithValue("@id", item.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM SaleRecords WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ─── Маппинг ───────────────────────────────────────────────────────────

        private static void Bind(SqlCommand cmd, SaleRecord item)
        {
            cmd.Parameters.AddWithValue("@date",   item.RecordDate.Date);
            cmd.Parameters.AddWithValue("@name",   item.ProductName);
            cmd.Parameters.AddWithValue("@qty",    item.Quantity);
            cmd.Parameters.AddWithValue("@uprice", item.UnitPrice);
            cmd.Parameters.AddWithValue("@total",  item.TotalAmount);
            cmd.Parameters.AddWithValue("@buyer",  (object)item.Buyer ?? DBNull.Value);
        }

        private static SaleRecord Map(SqlDataReader r) => new SaleRecord
        {
            Id          = r.GetInt32(0),
            RecordDate  = r.GetDateTime(1),
            ProductName = r.GetString(2),
            Quantity    = r.GetDecimal(3),
            UnitPrice   = r.GetDecimal(4),
            TotalAmount = r.GetDecimal(5),
            Buyer       = r.IsDBNull(6) ? string.Empty : r.GetString(6)
        };
    }
}
