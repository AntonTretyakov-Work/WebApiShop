using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModelsApiOrder.Models;
using System.Formats.Tar;
using System.Net.Sockets;

namespace MobileStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly string _connectionString;

        public OrdersController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/orders
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var items = new List<Order>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM orders;", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new Order
                         {
                            IdOrder = reader.GetInt32(0),  
                             IdClient = reader.GetInt32(1),  
                             IdEmployee = reader.GetInt32(2),  
                             IdCart = reader.GetInt32(3)
                         });
                    }
                }
            }

            return Ok(items);
        }
        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            Order item = null;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM orders WHERE id_order=@id;", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new Order
                            {
                                IdOrder = reader.GetInt32(0),
                                IdClient = reader.GetInt32(1),
                                IdEmployee = reader.GetInt32(2),
                                IdCart = reader.GetInt32(3)
                            };
                        }
                    }
                }

                if (item == null) return NotFound();

                return Ok(item);
            }
        }

        // POST: api/orders
        [HttpPost("orders")]
        public async Task<ActionResult<Order>> Create([FromBody] Order order)
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand("INSERT INTO orders(id_client, id_employee, id_cart) VALUES(@id_client,@id_employee,@id_cart) RETURNING id_order;", conn))
                    {
                        cmd.Parameters.AddWithValue("@id_client", order.IdClient);
                        cmd.Parameters.AddWithValue("@id_employee", order.IdEmployee);
                        cmd.Parameters.AddWithValue("@id_cart", order.IdCart);

                        order.IdOrder = (int)await cmd.ExecuteScalarAsync();
                    }
                }

                return CreatedAtAction(nameof(GetById), new { id = order.IdOrder }, order);
            }

            // PUT: api/orders/{id}
            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, [FromBody] Order updatedOrder)
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand("UPDATE orders SET id_client=@id_client,id_employee=@id_employee,id_cart=@id_cart WHERE id_order=@id_order;", conn))
                    {
                        cmd.Parameters.AddWithValue("@id_client", updatedOrder.IdClient);
                        cmd.Parameters.AddWithValue("@id_employee", updatedOrder.IdEmployee);
                        cmd.Parameters.AddWithValue("@id_cart", updatedOrder.IdCart);
                        cmd.Parameters.AddWithValue("@id", id);

                        var rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected == 0) return NotFound();
                    }
                }

                return NoContent();
            }

            // DELETE: api/orders/{id}
            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand("DELETE FROM orders WHERE id_order=@id;", conn))
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