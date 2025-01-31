using MasterLabel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace MasterLabel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "https://azuappsrvuat01v.mcquay.com/SupportDashboard/Home/";

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> GetReportData([FromBody] SearchParameters parameters)
        {
            try
            {
                var requestUrl = $"{BaseUrl}GetReportData?id=2154&parameters={{\"serial_number\":\"{parameters.SerialNumber}\"}}";
                var response = await _httpClient.PostAsync(requestUrl, null);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return Json(jsonResult);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return Json(new { error = $"Error al obtener datos. Status: {response.StatusCode}, Content: {errorContent}" });
            }
            catch (Exception ex)
            {
                return Json(new { error = $"Error detallado: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult SaveLabelData([FromBody] ReportData data)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("SqlConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"INSERT INTO LabelData 
                                (SerialNumber, Job, Item, Description, OrderNumber, OrderLine, 
                                 LPN, TagNumber, ShipCode, IRNO, Subinv, FullAddress, CreatedDate)
                                VALUES 
                                (@SerialNumber, @Job, @Item, @Description, @OrderNumber, @OrderLine,
                                 @LPN, @TagNumber, @ShipCode, @IRNO, @Subinv, @Address, @Date)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@SerialNumber", data.SerialNumber ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Job", data.Job ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Item", data.Item ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Description", data.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@OrderNumber", string.IsNullOrEmpty(data.OrderNumber) ? (object)DBNull.Value : decimal.Parse(data.OrderNumber));
                        command.Parameters.AddWithValue("@OrderLine", data.OrderLine ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@LPN", data.LPN ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TagNumber", data.TagNumber ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ShipCode", data.ShipCode ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IRNO", data.IRNO ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Subinv", data.Subinv ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Address", data.Address ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Date", data.Date ?? DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true, message = "Datos guardados correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar los datos en la base de datos");
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class SearchParameters
    {
        public string SerialNumber { get; set; }
    }
}