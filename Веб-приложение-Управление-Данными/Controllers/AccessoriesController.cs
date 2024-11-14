using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileStoreApi.Models;

namespace MobileStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessoriesController : ControllerBase
    {
        private readonly string _connectionString;

        public AccessoriesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/accessories
        [HttpGet("accessories")]
        public async Task<ActionResult<IEnumerable<Accessory>>> GetAll()
        {
            var items = new List<Accessory>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM accessories", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new Accessory
                        {
                            IdAccessories = reader.GetInt32(0), // Предполагаем, что этот столбец не NULL
                            Name = reader.GetString(1),
                            Cost = reader.GetDecimal(2),
                            IdMobileDevice = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3) // Проверка на NULL
                        });
                    }
                }
            }
            return Ok(items);
        }

        // GET: api/accessories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Accessory>> GetById(int id)
        {
            Accessory item = null;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM accessories WHERE id_accessory = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new Accessory
                            {
                                IdAccessories = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Cost = reader.GetDecimal(2),
                                IdMobileDevice = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // POST: api/accessories
        [HttpPost("accessories")]
        public async Task<ActionResult<Accessory>> Create([FromBody] Accessory accessory)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("INSERT INTO accessories (name, cost, id_mobile_device) VALUES (@name, @cost, @id_mobile_device) RETURNING id_accessory;", conn))
                {
                    cmd.Parameters.AddWithValue("@name", accessory.Name);
                    cmd.Parameters.AddWithValue("@cost", accessory.Cost);
                    cmd.Parameters.AddWithValue("@id_mobile_device", (object)accessory.IdMobileDevice ?? DBNull.Value); // Проверка на NULL

                    accessory.IdAccessories = (int)await cmd.ExecuteScalarAsync();
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = accessory.IdAccessories }, accessory);
        }

        // PUT: api/accessories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Accessory updatedAccessory)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("UPDATE accessories SET name = @name, cost = @cost, id_mobile_device = @id_mobile_device WHERE id_accessory = @id_accessory", conn))
                {
                    cmd.Parameters.AddWithValue("name", updatedAccessory.Name);
                    cmd.Parameters.AddWithValue("cost", updatedAccessory.Cost);
                    cmd.Parameters.AddWithValue("id_mobile_device", updatedAccessory.IdMobileDevice);
                    cmd.Parameters.AddWithValue("id_accessory", id);
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }

        // DELETE: api/accessories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM accessories WHERE id_accessory = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                }
            }

            return NoContent();
        }
    }
}