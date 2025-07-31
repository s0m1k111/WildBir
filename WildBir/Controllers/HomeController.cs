using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using WildBir.Models;

namespace WildBir.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private SqlConnection connection = new("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\User\\source\\repos\\WildBir\\DataBase\\Database1.mdf;Integrated Security=True");
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadPage(int page)
        {
            connection.Open();
            SqlCommand command = new SqlCommand(
                "select * from ( select ROW_NUMBER() over (ORDER by Id) as RowNum,* from Products) as numbers where RowNum between (@begin) and (@end);",
                connection
                );
            command.Parameters.AddWithValue("begin",(page*10));
            command.Parameters.AddWithValue("end", (page * 10)+10);
            SqlDataReader reader = command.ExecuteReader();
            List<ProductModel> products = new List<ProductModel>();
            while (reader.Read())
            {
                products.Add(new ProductModel(
                    Convert.ToString(reader["Id"]),
                    Convert.ToString(reader["Image"]),
                    Convert.ToString(reader["Price"]),
                    Convert.ToString(reader["Name"])
                    ));
            }
            reader.Close();
            return PartialView("Page",products);
        }

        public IActionResult IdProduct(string id)
        {
            Console.WriteLine(id);
            connection.Open();
            SqlCommand command = new SqlCommand(
                "insert into [Cart] (Users, Product) values (@users, @product)",
                connection
                );
            command.Parameters.AddWithValue("users", "Users");
            command.Parameters.AddWithValue("product", id);
            command.ExecuteNonQuery();
            return View("Index");
        }

        public IActionResult Cart()
        {
            connection.Open();
            SqlCommand command = new SqlCommand(
                "select Product from Cart where (Users = @users)",
                connection
                );
            command.Parameters.AddWithValue("users","Users");
            SqlDataReader reader = command.ExecuteReader();
            List<string> products = new List<string>();
            while (reader.Read())
            {
                products.Add(Convert.ToString(reader["Product"]));
            }
            reader.Close();
            List<ProductModel> models = new List<ProductModel>();
            
            foreach (string id in products)
            {
                SqlCommand command1 = new SqlCommand(
                    "select * from Products where (id = @id)",
                    connection
                    );
                command1.Parameters.AddWithValue("id", id);
                SqlDataReader reader2 = command1.ExecuteReader();
                while(reader2.Read())
                {
                    models.Add(new ProductModel(
                        Convert.ToString(reader2["Id"]),
                        Convert.ToString(reader2["Image"]),
                        Convert.ToString(reader2["Price"]),
                        Convert.ToString(reader2["Name"])
                        ));
                }
                reader2.Close();
            }
            int total = 0;
            foreach(var product in models)
            {
                try
                {
                    total += Convert.ToInt32(product.Price);
                }
                catch (Exception ex) { }
                
            }
            ViewData["Total"] = total.ToString();
            return View(models);
        }

        public ActionResult Clear()
        {
            connection.Open();
            SqlCommand command = new SqlCommand(
                "delete from Cart where (Users = @users)",
                connection
                );
            command.Parameters.AddWithValue("users", "Users");
            command.ExecuteNonQuery();
            return View("Cart");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InsertProduct(ProductModel productModel)
        {
            connection.Open();
            SqlCommand command = new SqlCommand(
                "insert into [Products] (Image,Price,Name) values (@image,@price,@name)",
                connection);
            command.Parameters.AddWithValue("image", productModel.Image);
            command.Parameters.AddWithValue("price", productModel.Price);
            command.Parameters.AddWithValue("name", productModel.Name);
            command.ExecuteNonQuery();
            return View("Privacy");
        }
        public IActionResult Search(string search)
        {
            @ViewData["Search"] = search;
            return View();
        }

        ///Home/LoadSearch
        public IActionResult LoadSearch(int page, string search)
        {
            connection.Open();
            SqlCommand command = new SqlCommand(
                "select * from Products where (Name like @search)",
                connection
                );
            command.Parameters.AddWithValue("search", $"%{search}%");
            SqlDataReader reader = command.ExecuteReader();
            List<ProductModel> products = new List<ProductModel>();
            while (reader.Read())
            {
                products.Add(new ProductModel(
                    Convert.ToString(reader["Id"]),
                    Convert.ToString(reader["Image"]),
                    Convert.ToString(reader["Price"]),
                    Convert.ToString(reader["Name"])
                    ));
            }
            reader.Close();
            return PartialView("Page", products);
        }

        public IActionResult InfoProduct(string id)
        {
            Console.WriteLine(id);
            connection.Open();
            SqlCommand command = new SqlCommand(
                "select * from Products where (Id = @id)",
                connection
                );
            command.Parameters.AddWithValue("id",id);
            SqlDataReader reader = command.ExecuteReader();
            ProductModel model = null;
            while(reader.Read())
            {
                model = new(
                    Convert.ToString(reader["Id"]),
                    Convert.ToString(reader["Image"]),
                    Convert.ToString(reader["Price"]),
                    Convert.ToString(reader["Name"])
                    );
            }
            reader.Close();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
