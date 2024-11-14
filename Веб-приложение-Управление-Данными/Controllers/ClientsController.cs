using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileStoreApi.Models;

namespace MobileStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly string _connectionString;

        public ClientsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/clients
        [HttpGet("clients")]
        public async Task<ActionResult<IEnumerable<Client>>> GetAll()
        {
            var items = new List<Client>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM clients", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new Client
                        {
                            IdClient = reader.GetInt32(0),
                            LastName = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            PhoneNumber = reader.GetString(3),
                            Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                            DateOfBirth = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                            Email = reader.IsDBNull(6) ? null : reader.GetString(6)
                        });
                    }
                }
            }

            return Ok(items);
        }

        // GET: api/clients/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(int id)
        {
            Client item = null;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM clients WHERE id_clients=@id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new Client
                            {
                                IdClient = reader.GetInt32(0),
                                LastName = reader.GetString(1),
                                FirstName = reader.GetString(2),
                                PhoneNumber = reader.GetString(3),
                                Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                                DateOfBirth = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                                Email = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };
                        }
                    }
                }
            }

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST: api/clients
        [HttpPost("clients")]
        public async Task<ActionResult<Client>> Create([FromBody] Client client)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("INSERT INTO clients(surname_clients, name_clients, phone_number, address, birth_date, email) VALUES(@surname_clients, @name_clients, @phone_number, @address, @birth_date, @email) RETURNING id_clients;", conn))
                {
                    cmd.Parameters.AddWithValue("@surname_clients", client.LastName);
                    cmd.Parameters.AddWithValue("@name_clients", client.FirstName);
                    cmd.Parameters.AddWithValue("@phone_number", client.PhoneNumber);
                    if (client.Address != null)
                    {
                        cmd.Parameters.AddWithValue("@address", client.Address);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@address", DBNull.Value);
                    }

                    if (client.DateOfBirth != null)
                    {
                        cmd.Parameters.AddWithValue("@birth_date", client.DateOfBirth);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@birth_date", DBNull.Value);
                    }

                    if (client.Email != null)
                    {
                        cmd.Parameters.AddWithValue("@email", client.Email);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@email", DBNull.Value);
                    }

                    client.IdClient = (int)await cmd.ExecuteScalarAsync();
                }
            }
            return CreatedAtAction(nameof(GetById), new { id = client.IdClient }, client);
        }

        // PUT: api/clients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Client updatedClient)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("UPDATE clients SET surname_clients=@surname_clients , name_clients=@name_clients , phone_number=@phone_number , address=@address , birth_date=@birth_date , email=@email WHERE id_clients=@id_clients;", conn))
                {
                    cmd.Parameters.AddWithValue("@surname_clients", updatedClient.LastName);
                    cmd.Parameters.AddWithValue("@name_clients", updatedClient.FirstName);
                    cmd.Parameters.AddWithValue("@phone_number", updatedClient.PhoneNumber);
                    cmd.Parameters.AddWithValue("@address", (object)updatedClient.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@birth_date", (object)updatedClient.DateOfBirth ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", (object)updatedClient.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_clients", id); // Убедитесь, что ID передан правильно

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return NotFound(); // Если записи не найдены
                    }
                }
            }

            return NoContent(); // Возврат успешного ответа
        }

        // DELETE: api/clients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM clients WHERE id_clients=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

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