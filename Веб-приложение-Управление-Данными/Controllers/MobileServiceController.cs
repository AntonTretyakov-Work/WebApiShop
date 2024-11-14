using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileStoreApi.Models;

namespace MobileStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileOperatorServiceController : ControllerBase
    {
        private readonly string _connectionString;

        public MobileOperatorServiceController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/mobileoperatorservice
        [HttpGet("mobileoperatorservice")]
        public async Task<ActionResult<IEnumerable<MobileOperatorService>>> GetAll()
        {
            var items = new List<MobileOperatorService>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM mobile_operator_service", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new MobileOperatorService
                        {
                            IdService = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Cost = reader.GetDecimal(2)
                        });
                    }
                }
            }
            return Ok(items);
        }

        // GET: api/mobileoperatorservice/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MobileOperatorService>> GetById(int id)
        {
            MobileOperatorService item = null; // Исправлено: добавлен пробел между типом и именем переменной
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM mobile_operator_service WHERE id_service=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new MobileOperatorService
                            {
                                IdService = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Cost = reader.GetDecimal(2)
                            };
                        }
                    }
                }
            }

            if (item == null) return NotFound();

            return Ok(item);
        }

        // POST: api/mobileoperatorservice
        [HttpPost("mobileoperatorservice")]
        public async Task<ActionResult<MobileOperatorService>> Create([FromBody] MobileOperatorService service)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("INSERT INTO mobile_operator_service(name, cost) VALUES(@name, @cost) RETURNING id_service;", conn))
                {
                    cmd.Parameters.AddWithValue("@name", service.Name);
                    cmd.Parameters.AddWithValue("@cost", service.Cost);

                    service.IdService = (int)await cmd.ExecuteScalarAsync();
                }
            }
            return CreatedAtAction(nameof(GetById), new { id = service.IdService }, service);
        }

        // PUT: api/mobileoperatorservice/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MobileOperatorService updatedService)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("UPDATE mobile_operator_service SET name=@name, cost=@cost WHERE id_service=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@name", updatedService.Name);
                    cmd.Parameters.AddWithValue("@cost", updatedService.Cost);
                    cmd.Parameters.AddWithValue("@id", id);

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected == 0) return NotFound();
                }
            }
            return NoContent();
        }

        // DELETE: api/mobileoperatorservice/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM mobile_operator_service WHERE id_service=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected == 0) return NotFound();
                }
            }
            return NoContent();
        }
    }
}