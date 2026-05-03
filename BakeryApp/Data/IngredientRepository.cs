// Data/IngredientRepository.cs
using System.Collections.Generic;
using System.Data.SqlClient;
using BakeryApp.Models;

namespace BakeryApp.Data
{
    public class IngredientRepository
    {
        // ─── Чтение ────────────────────────────────────────────────────────────

        public List<Ingredient> GetAll(string search = "")
        {
            var list = new List<Ingredient>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    cmd.CommandText = "SELECT Id, Name, Unit, Quantity, PurchasePrice FROM Ingredients ORDER BY Name";
                }
                else
                {
                    cmd.CommandText = @"
SELECT Id, Name, Unit, Quantity, PurchasePrice
FROM   Ingredients
WHERE  Name LIKE @s
ORDER  BY Name";
                    cmd.Parameters.AddWithValue("@s", $"%{search}%");
                }

                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(Map(r));
            }
            return list;
        }

        public Ingredient GetById(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT Id, Name, Unit, Quantity, PurchasePrice FROM Ingredients WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Map(r) : null;
            }
        }

        // ─── Запись ────────────────────────────────────────────────────────────

        public void Add(Ingredient item)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO Ingredients (Name, Unit, Quantity, PurchasePrice)
VALUES (@name, @unit, @qty, @price);
SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@name",  item.Name);
                cmd.Parameters.AddWithValue("@unit",  item.Unit);
                cmd.Parameters.AddWithValue("@qty",   item.Quantity);
                cmd.Parameters.AddWithValue("@price", item.PurchasePrice);
                item.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void Update(Ingredient item)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE Ingredients
SET    Name = @name, Unit = @unit, Quantity = @qty,
       PurchasePrice = @price, UpdatedAt = GETDATE()
WHERE  Id = @id";
                cmd.Parameters.AddWithValue("@name",  item.Name);
                cmd.Parameters.AddWithValue("@unit",  item.Unit);
                cmd.Parameters.AddWithValue("@qty",   item.Quantity);
                cmd.Parameters.AddWithValue("@price", item.PurchasePrice);
                cmd.Parameters.AddWithValue("@id",    item.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Ingredients WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Быстрый приход/расход: прибавляет delta к текущему количеству.</summary>
        public void AdjustQuantity(int id, decimal delta)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
UPDATE Ingredients
SET    Quantity   = Quantity + @delta,
       UpdatedAt  = GETDATE()
WHERE  Id = @id";
                cmd.Parameters.AddWithValue("@delta", delta);
                cmd.Parameters.AddWithValue("@id",    id);
                cmd.ExecuteNonQuery();
            }
        }

        // ─── Маппинг ───────────────────────────────────────────────────────────

        private static Ingredient Map(SqlDataReader r) => new Ingredient
        {
            Id            = r.GetInt32(0),
            Name          = r.GetString(1),
            Unit          = r.GetString(2),
            Quantity      = r.GetDecimal(3),
            PurchasePrice = r.GetDecimal(4)
        };
    }
}
