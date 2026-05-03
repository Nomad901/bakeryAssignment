// Data/ProductionRepository.cs
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BakeryApp.Models;

namespace BakeryApp.Data
{
    public class ProductionRepository
    {
        // ─── Чтение ────────────────────────────────────────────────────────────

        public List<ProductionRecord> GetAll(DateTime? from = null, DateTime? to = null, string search = "")
        {
            var list = new List<ProductionRecord>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT Id, RecordDate, ProductName, Quantity, Comment
FROM   ProductionRecords
WHERE  (@from IS NULL OR RecordDate >= @from)
  AND  (@to   IS NULL OR RecordDate <= @to)
  AND  (@s    = ''    OR ProductName LIKE @sl OR Comment LIKE @sl)
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

        public ProductionRecord GetById(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT Id, RecordDate, ProductName, Quantity, Comment FROM ProductionRecords WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Map(r) : null;
            }
        }

        // ─── Запись ────────────────────────────────────────────────────────────

        public void Add(ProductionRecord item)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO ProductionRecords (RecordDate, ProductName, Quantity, Comment)
VALUES (@date, @name, @qty, @comment);
SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@date",    item.RecordDate.Date);
                cmd.Parameters.AddWithValue("@name",    item.ProductName);
                cmd.Parameters.AddWithValue("@qty",     item.Quantity);
                cmd.Parameters.AddWithValue("@comment", (object)item.Comment ?? DBNull.Value);
                item.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void Update(ProductionRecord item)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE ProductionRecords
SET    RecordDate = @date, ProductName = @name, Quantity = @qty, Comment = @comment
WHERE  Id = @id";
                cmd.Parameters.AddWithValue("@date",    item.RecordDate.Date);
                cmd.Parameters.AddWithValue("@name",    item.ProductName);
                cmd.Parameters.AddWithValue("@qty",     item.Quantity);
                cmd.Parameters.AddWithValue("@comment", (object)item.Comment ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id",      item.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM ProductionRecords WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ─── Маппинг ───────────────────────────────────────────────────────────

        private static ProductionRecord Map(SqlDataReader r) => new ProductionRecord
        {
            Id          = r.GetInt32(0),
            RecordDate  = r.GetDateTime(1),
            ProductName = r.GetString(2),
            Quantity    = r.GetDecimal(3),
            Comment     = r.IsDBNull(4) ? string.Empty : r.GetString(4)
        };
    }
}
