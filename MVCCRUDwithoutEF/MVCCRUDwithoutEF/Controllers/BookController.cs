using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCCRUDwithoutEF.Data;
using MVCCRUDwithoutEF.Models;

namespace MVCCRUDwithoutEF.Controllers
{
    public class BookController : Controller
    {
        private readonly IConfiguration _configuration;

        public BookController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // GET: Book
        public IActionResult Index()
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("BookViewAll", sqlConnection);
                sqlDataAdapter.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlDataAdapter.Fill(dataTable);
            }
            return View(dataTable);
        }

        // GET: Book/AddOrEdit/5
        public IActionResult AddOrEdit(int? id)
        {
            BookViewModel bookViewModel = new BookViewModel();
            if (id > 0)
            {
                bookViewModel = FetchBookByID(id);
            }
            return View(bookViewModel);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(int id, [Bind("BookID,Title,Author,Price")] BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand("BookAddOrEdit", sqlConnection);
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("BookID", bookViewModel.BookID);
                    sqlCommand.Parameters.AddWithValue("Title", bookViewModel.Title);
                    sqlCommand.Parameters.AddWithValue("Author", bookViewModel.Author);
                    sqlCommand.Parameters.AddWithValue("Price", bookViewModel.Price);
                    sqlCommand.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bookViewModel);
        }

        // GET: Book/Delete/5
        public IActionResult Delete(int? id)
        {
            BookViewModel bookViewModel = FetchBookByID(id);
            return View(bookViewModel);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("BookDeleteByID", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("BookID", id);
                sqlCommand.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }

        public BookViewModel FetchBookByID(int? id)
        {
            BookViewModel bookViewModel = new BookViewModel();
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("BookViewByID", sqlConnection);
                sqlDataAdapter.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("BookID", id);
                sqlDataAdapter.Fill(dataTable);
                if (dataTable.Rows.Count == 1)
                {
                    bookViewModel.BookID = Convert.ToInt32(dataTable.Rows[0]["BookID"].ToString());
                    bookViewModel.Title = dataTable.Rows[0]["Title"].ToString();
                    bookViewModel.Author = dataTable.Rows[0]["Author"].ToString();
                    bookViewModel.Price = Convert.ToInt32(dataTable.Rows[0]["Price"].ToString());
                }
                return bookViewModel;
            }
        }
    }
}
