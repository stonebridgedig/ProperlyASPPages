using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ProperlyASPPages.Pages
{
    public class TestConnectionModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public string? ConnectionResult { get; set; }
        public bool IsSuccess { get; set; }

        public TestConnectionModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                
                var serverVersion = connection.ServerVersion;
                var database = connection.Database;
                
                ConnectionResult = $"Connected successfully!\n\n" +
                                 $"Server Version: {serverVersion}\n" +
                                 $"Database: {database}\n" +
                                 $"Connection String: {connectionString.Replace(_configuration["ConnectionStrings:DefaultConnection"]!.Split("Password=")[1].Split(";")[0], "****")}";
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                ConnectionResult = $"Connection failed!\n\n" +
                                 $"Error: {ex.Message}\n\n" +
                                 $"Stack Trace: {ex.StackTrace}\n\n" +
                                 $"Connection String: {connectionString.Replace(_configuration["ConnectionStrings:DefaultConnection"]!.Split("Password=")[1].Split(";")[0], "****")}";
                IsSuccess = false;
            }

            return Page();
        }
    }
}
