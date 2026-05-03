// Data/ProductRepository.cs
using System.Collections.Generic;
using System.Data.SqlClient;
using BakeryApp.Models;

namespace BakeryApp.Data
{
    public class ProductRepository
    {
        // ─── Чтение ────────────────────────────────────────────────────────────

        public List<Product> GetAll(string search = "")
        {
            var list = new List<Product>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    cmd.CommandText =
                        "SELECT Id, Name, Type, SalePrice, Notes FROM Products ORDER BY Type, Name";
                }
                else
                {
                    cmd.CommandText = @"
SELECT Id, Name, Type, SalePrice, Notes
FROM   Products
WHERE  Name LIKE @s OR Type LIKE @s OR Notes LIKE @s
ORDER  BY Type, Name";
                    cmd.Parameters.AddWithValue("@s", $"%{search}%");
                }

                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(Map(r));
            }
            return list;
        }

        /// <summary>Возвращает только имена продуктов для ComboBox.</summary>
        public List<string> GetNames()
        {
            var names = new List<string>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Name FROM Products ORDER BY Name";
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        names.Add(r.GetString(0));
            }
            return names;
        }

        public Product GetById(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT Id, Name, Type, SalePrice, Notes FROM Products WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Map(r) : null;
            }
        }

        // ─── Запись ────────────────────────────────────────────────────────────

        public void Add(Product item)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO Products (Name, Type, SalePrice, Notes)
VALUES (@name, @type, @price, @notes);
SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@name",  item.Name);
                cmd.Parameters.AddWithValue("@type",  item.Type);
                cmd.Parameters.AddWithValue("@price", item.SalePrice);
                cmd.Parameters.AddWithValue("@notes", (object)item.Notes ?? System.DBNull.Value);
                item.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void Update(Product item)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE Products
SET    Name = @name, Type = @type, SalePrice = @price, Notes = @notes
WHERE  Id   = @id";
                cmd.Parameters.AddWithValue("@name",  item.Name);
                cmd.Parameters.AddWithValue("@type",  item.Type);
                cmd.Parameters.AddWithValue("@price", item.SalePrice);
                cmd.Parameters.AddWithValue("@notes", (object)item.Notes ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@id",    item.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Products WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // ─── Маппинг ───────────────────────────────────────────────────────────

        private static Product Map(SqlDataReader r) => new Product
        {
            Id        = r.GetInt32(0),
            Name      = r.GetString(1),
            Type      = r.GetString(2),
            SalePrice = r.GetDecimal(3),
            Notes     = r.IsDBNull(4) ? string.Empty : r.GetString(4)
        };
    }
}
