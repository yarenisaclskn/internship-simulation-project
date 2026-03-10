
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Npgsql;
using HaritaUygulamasi.Models;

namespace HaritaUygulamasi.Controllers
{
    public class HaritaNesnesiController : Controller
    {
        private readonly string _connectionString;

        public HaritaNesnesiController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PostgreConnection"].ConnectionString;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetObjects()
        {
            var objects = new List<HaritaNesnesi>();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT id, name, lat, lon, alt, pitch, glb_file, isvisible, \"RotaId\" FROM harita_nesneleri ORDER BY id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                objects.Add(new HaritaNesnesi
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.IsDBNull(1) ? "" : reader.GetString(1).Trim(),
                                    Latitude = reader.IsDBNull(2) ? 0 : reader.GetDouble(2),
                                    Longitude = reader.IsDBNull(3) ? 0 : reader.GetDouble(3),
                                    Altitude = reader.IsDBNull(4) ? 0 : reader.GetDouble(4),
                                    Pitch = reader.IsDBNull(5) ? (double?)null : reader.GetDouble(5),
                                    glb_file = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    IsVisible = reader.IsDBNull(7) ? true : reader.GetBoolean(7),
                                    RotaId = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8)
                                });
                            }
                        }
                    }
                }
                return Json(objects, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetOneObject(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT id, name, lat, lon, alt, pitch, glb_file, isvisible, \"RotaId\" FROM harita_nesneleri WHERE id = @id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var nesne = new HaritaNesnesi
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetValue(1).ToString().Trim(),
                                    Latitude = reader.IsDBNull(2) ? (double?)null : reader.GetDouble(2),
                                    Longitude = reader.IsDBNull(3) ? (double?)null : reader.GetDouble(3),
                                    Altitude = reader.IsDBNull(4) ? (double?)null : reader.GetDouble(4),
                                    Pitch = reader.IsDBNull(5) ? (double?)null : reader.GetDouble(5),
                                    glb_file = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    IsVisible = reader.IsDBNull(7) ? true : reader.GetBoolean(7),
                                    RotaId = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8)
                                };
                                return Json(nesne, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                return Json(new { error = "Nesne bulunamadı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateObject(HaritaNesnesi updatedObject)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { error = "Geçersiz model verisi" });
            }
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "UPDATE harita_nesneleri SET name = @name, lat = @lat, lon = @lon, alt = @alt, pitch = @pitch, glb_file = @glb_file, \"RotaId\" = @rotaId WHERE id = @id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("id", updatedObject.Id);
                        command.Parameters.AddWithValue("name", updatedObject.Name ?? "");
                        command.Parameters.AddWithValue("lat", (object)updatedObject.Latitude ?? DBNull.Value);
                        command.Parameters.AddWithValue("lon", (object)updatedObject.Longitude ?? DBNull.Value);
                        command.Parameters.AddWithValue("alt", (object)updatedObject.Altitude ?? DBNull.Value);
                        command.Parameters.AddWithValue("pitch", (object)updatedObject.Pitch ?? DBNull.Value);
                        command.Parameters.AddWithValue("glb_file", (object)updatedObject.glb_file ?? DBNull.Value);
                        command.Parameters.AddWithValue("rotaId", (object)updatedObject.RotaId ?? DBNull.Value);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Json(new { success = true, id = updatedObject.Id });
                        }
                        else
                        {
                            return Json(new { error = "Güncellenecek nesne bulunamadı" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteObject(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "DELETE FROM harita_nesneleri WHERE id = @id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("id", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { error = "Silinecek nesne bulunamadı" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostObject(HaritaNesnesi newObject)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { error = "Geçersiz model verisi" });
            }
            if (string.IsNullOrWhiteSpace(newObject.Name))
            {
                return Json(new { error = "İsim alanı zorunludur" });
            }

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "INSERT INTO harita_nesneleri (name, lat, lon, alt, pitch, glb_file, \"RotaId\") VALUES (@name, @lat, @lon, @alt, @pitch, @glb_file, @rotaId) RETURNING id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("name", newObject.Name);
                        command.Parameters.AddWithValue("lat", (object)newObject.Latitude ?? DBNull.Value);
                        command.Parameters.AddWithValue("lon", (object)newObject.Longitude ?? DBNull.Value);
                        command.Parameters.AddWithValue("alt", (object)newObject.Altitude ?? DBNull.Value);
                        command.Parameters.AddWithValue("pitch", (object)newObject.Pitch ?? DBNull.Value);
                        command.Parameters.AddWithValue("glb_file", (object)newObject.glb_file ?? DBNull.Value);
                        command.Parameters.AddWithValue("rotaId", (object)newObject.RotaId ?? DBNull.Value);

                        var newId = (int)await command.ExecuteScalarAsync();
                        newObject.Id = newId;
                        return Json(new { success = true, id = newObject.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> ToggleVisibility(int id, bool isVisible)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "UPDATE harita_nesneleri SET isvisible = @isvisible WHERE id = @id";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("isvisible", isVisible);
                        command.Parameters.AddWithValue("id", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false, error = "Nesne bulunamadı" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + "_" + System.IO.Path.GetFileName(file.FileName);
                    var path = Server.MapPath("~/Models/3D/");
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    var fullPath = System.IO.Path.Combine(path, fileName);
                    file.SaveAs(fullPath);
                    return Json(new { success = true, filePath = "/Models/3D/" + fileName });
                }
            }
            return Json(new { success = false, error = "Dosya yükleme başarısız." });
        }

        [HttpGet]
        public async Task<ActionResult> GetRotalar()
        {
            var rotalar = new List<HaritaNesnesiRota>();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = "SELECT \"RotaId\", \"RotaAdi\", \"RouteData\" FROM harita_nesneleri_rota ORDER BY \"RotaAdi\"";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                rotalar.Add(new HaritaNesnesiRota
                                {
                                    RotaId = reader.GetInt32(0),
                                    RotaAdi = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    RouteData = reader.IsDBNull(2) ? null : reader.GetString(2)
                                });
                            }
                        }
                    }
                }
                return Json(rotalar, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public async Task<ActionResult> GetRouteData(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var rotaIdSql = "SELECT \"RotaId\" FROM harita_nesneleri WHERE id = @id";
                    int? rotaId = null;

                    using (var rotaIdCommand = new NpgsqlCommand(rotaIdSql, connection))
                    {
                        rotaIdCommand.Parameters.AddWithValue("id", id);
                        var result = await rotaIdCommand.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            rotaId = Convert.ToInt32(result);
                        }
                    }

                    if (!rotaId.HasValue)
                    {
                        return Json(new List<object>(), JsonRequestBehavior.AllowGet);
                    }

                    var routeDataSql = "SELECT \"RouteData\" FROM harita_nesneleri_rota WHERE \"RotaId\" = @rotaId";
                    string routeDataJson = null;

                    using (var routeDataCommand = new NpgsqlCommand(routeDataSql, connection))
                    {
                        routeDataCommand.Parameters.AddWithValue("rotaId", rotaId.Value);
                        var result = await routeDataCommand.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            routeDataJson = result.ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(routeDataJson))
                    {
                        return Content(routeDataJson, "application/json");
                    }
                }
            }
            catch (Exception )
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            return Json(new List<object>(), JsonRequestBehavior.AllowGet);
        }
    }
    }