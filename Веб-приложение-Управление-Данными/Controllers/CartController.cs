using Microsoft.AspNetCore.Mvc;
using MobileStoreApi.Models;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileStoreApi.Models; // Замените на ваше пространство имен

namespace CartController.Controllers // Замените на ваше пространство имен
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly string _connectionString;

        public CartController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/cart
        [HttpGet("cart")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetAllCarts()
        {
            var carts = new List<Cart>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM cart", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        carts.Add(new Cart
                        {
                            IdCart = reader.GetInt32(0),
                            IdMobileDevice = reader.GetInt32(1),
                            IdService = reader.GetInt32(2),
                            IdAccessory = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3)
                        });
                    }
                }
            }

            return Ok(carts);
        }

        // POST: api/cart
        [HttpPost("cart")]
        public async Task<ActionResult<Cart>> CreateCart([FromBody] Cart cart)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("INSERT INTO cart(id_mobile_device, id_service, id_accessory) VALUES(@id_mobile_device, @id_service, @id_accessory) RETURNING id_cart;", conn))
                {
                    cmd.Parameters.AddWithValue("@id_mobile_device", cart.IdMobileDevice);
                    cmd.Parameters.AddWithValue("@id_service", cart.IdService);
                    cmd.Parameters.AddWithValue("@id_accessory", (object)cart.IdAccessory ?? DBNull.Value);

                    cart.IdCart = (int)await cmd.ExecuteScalarAsync();
                }
            }
            return CreatedAtAction(nameof(GetAllCarts), new { id = cart.IdCart }, cart);
        }

        // PUT: api/cart/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] Cart updatedCart)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("UPDATE cart SET id_mobile_device=@id_mobile_device, id_service=@id_service, id_accessory=@id_accessory WHERE id_cart=@id_cart;", conn))
                {
                    cmd.Parameters.AddWithValue("@id_mobile_device", updatedCart.IdMobileDevice);
                    cmd.Parameters.AddWithValue("@id_service", updatedCart.IdService);
                    cmd.Parameters.AddWithValue("@id_accessory", (object)updatedCart.IdAccessory ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_cart", id);

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected == 0) return NotFound();
                }
            }
            return NoContent();
        }

        // DELETE: api/cart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM cart WHERE id_cart=@id_cart;", conn))
                {
                    cmd.Parameters.AddWithValue("@id_cart", id);
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected == 0) return NotFound();
                }
            }
            return NoContent();
        }
    }
}