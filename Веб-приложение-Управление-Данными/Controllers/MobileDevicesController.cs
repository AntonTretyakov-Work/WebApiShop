using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileStoreApi.Models;

namespace MobileStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileDevicesController : ControllerBase
    {
        private readonly string _connectionString;

        public MobileDevicesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/mobiledevices
        [HttpGet("mobiledevices")]
        public async Task<ActionResult<IEnumerable<MobileDevice>>> GetAll()
        {
            var items = new List<MobileDevice>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM mobile_devices", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new MobileDevice
                        {
                            IdDevice = reader.GetInt32(0),
                            Manufacturer = reader.GetString(1),
                            Model = reader.GetString(2),
                            Color = reader.GetString(3),
                            BuiltInMemory = reader.GetInt32(4),
                            YearRelease = reader.GetInt32(5),
                            CountryOfManufacturer = reader.GetString(6),
                            DeviceType = reader.GetString(7),
                            DeviceCost = reader.GetDecimal(8)
                        });
                    }
                }
            }

            return Ok(items);
        }

        // GET: api/mobiledevices/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MobileDevice>> GetById(int id)
        {
            MobileDevice item = null;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM mobile_devices WHERE Id_device = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item = new MobileDevice
                            {
                                IdDevice = reader.GetInt32(0),
                                Manufacturer = reader.GetString(1),
                                Model = reader.GetString(2),
                                Color = reader.GetString(3),
                                BuiltInMemory = reader.GetInt32(4),
                                YearRelease = reader.GetInt32(5),
                                CountryOfManufacturer = reader.GetString(6),
                                DeviceType = reader.GetString(7),
                                DeviceCost = reader.GetDecimal(8)
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

        // POST: api/mobiledevices
        [HttpPost("mobiledevices")]
        public async Task<ActionResult<MobileDevice>> Create([FromBody] MobileDevice device)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("INSERT INTO mobile_devices (manufacturer, model, color, built_in_memory, year_release, country_of_manufacturer, device_type, device_cost) VALUES (@manufacturer, @model, @color, @built_in_memory, @year_release, @country_of_manufacturer, @device_type, @device_cost) RETURNING Id_device", conn))
                {
                    cmd.Parameters.AddWithValue("manufacturer", device.Manufacturer);
                    cmd.Parameters.AddWithValue("model", device.Model);
                    cmd.Parameters.AddWithValue("color", device.Color);
                    cmd.Parameters.AddWithValue("built_in_memory", device.BuiltInMemory);
                    cmd.Parameters.AddWithValue("year_release", device.YearRelease);
                    cmd.Parameters.AddWithValue("country_of_manufacturer", device.CountryOfManufacturer);
                    cmd.Parameters.AddWithValue("device_type", device.DeviceType);
                    cmd.Parameters.AddWithValue("device_cost", device.DeviceCost);

                    device.IdDevice = (int)await cmd.ExecuteScalarAsync();
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = device.IdDevice }, device);
        }

        // PUT: api/mobiledevices/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MobileDevice updatedDevice)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("UPDATE mobile_devices SET manufacturer = @manufacturer, model = @model, color = @color, built_in_memory = @built_in_memory, year_release = @year_release, country_of_manufacturer = @country_of_manufacturer, device_type = @device_type, device_cost = @device_cost WHERE Id_device = @id", conn))
                {
                    cmd.Parameters.AddWithValue("manufacturer", updatedDevice.Manufacturer);
                    cmd.Parameters.AddWithValue("model", updatedDevice.Model);
                    cmd.Parameters.AddWithValue("color", updatedDevice.Color);
                    cmd.Parameters.AddWithValue("built_in_memory", updatedDevice.BuiltInMemory);
                    cmd.Parameters.AddWithValue("year_release", updatedDevice.YearRelease);
                    cmd.Parameters.AddWithValue("country_of_manufacturer", updatedDevice.CountryOfManufacturer);
                    cmd.Parameters.AddWithValue("device_type", updatedDevice.DeviceType);
                    cmd.Parameters.AddWithValue("device_cost", updatedDevice.DeviceCost);
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

        // DELETE: api/mobiledevices/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("DELETE FROM mobile_devices WHERE Id_device = @id", conn))
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