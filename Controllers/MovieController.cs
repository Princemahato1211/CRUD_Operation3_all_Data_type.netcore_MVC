using All_type_input_database.Data;
using All_type_input_database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;
using System.Net;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Data.SqlClient;

namespace All_type_input_database.Controllers
{
    public class MovieController : Controller
    {
        public readonly AppDbContext _context;
        public MovieController(AppDbContext context)
        {
            _context = context;
        }


        private List<SelectListItem> GetPageSizes(int selectedPageSize = 10)
        {
            var pageSizes = new List<SelectListItem>();

            if(selectedPageSize == 5)
            {
                pageSizes.Add(new SelectListItem("5", "5", true));
            }
            else
            {
                pageSizes.Add(new SelectListItem("5", "5"));
            }

            for(int lp =10; lp<=100; lp += 10)
            {
                if(lp == selectedPageSize)
                {
                    pageSizes.Add(new SelectListItem(lp.ToString(), lp.ToString(), true));
                }
                else
                {
                    pageSizes.Add(new SelectListItem(lp.ToString(), lp.ToString()));
                }
            }

            return pageSizes;
        }

        
        // displaying all data
        [HttpGet]
        public async Task<IActionResult> Index(int pg = 1)
        {
            //try
            //{
                List<Movie_data_model> data = await _context.Movie_Table.ToListAsync();
                const int pageSize = 3;
                if (pg < 1)
                {
                    pg = 1;
                }
                int rescount = data.Count();
                var pager = new Pager(rescount, pg, pageSize);
                int recSkip = (pg - 1) * pageSize;
                var data1 = data.Skip(recSkip).Take(pager.PageSize).ToList();
                this.ViewBag.Pager = pager;

                return View(data1);
            //}
            //catch(SqlException ex)
            //{
                //return View();
            }

            //this.ViewBag.PageSizes = GetPageSizes(pageSize
        }
        [HttpPost]
        public async Task<IActionResult> Index(string SearchText="", int pg = 1)
        {
            try
            {
                List<Movie_data_model> data;
                if (SearchText != "" && SearchText != null)
                {
                    data = _context.Movie_Table.Where(p => p.MovieName.Contains(SearchText)).ToList();
                }
                else
                {
                    data = await _context.Movie_Table.ToListAsync();
                }

                const int pageSize = 3;
                if (pg < 1)
                {
                    pg = 1;
                }
                int rescount = data.Count();
                var pager = new Pager(rescount, pg, pageSize);
                int recSkip = (pg - 1) * pageSize;
                var data1 = data.Skip(recSkip).Take(pager.PageSize).ToList();
                this.ViewBag.Pager = pager;

                //this.ViewBag.PageSizes = GetPageSizes(pageSize);

                return View(data1);
            }
            catch(SqlException ex)
            {
                return View();
            }
        }

        // Create 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie_data_model movie,IFormFile Imagefile)
        {
            if(movie.MovieName == null || movie.MovieDateTime == DateTime.MinValue || Imagefile == null || movie.email == null)  
            {
                return View(movie);
            }
            if (Imagefile.Length > 0 && Imagefile.Length <= (2 * 1024 * 1024)) // 2mb limit
            {
                var imagedata = new byte[Imagefile.Length];
                movie.Image = imagedata;
            }


            var moviedetail = new Movie_data_model()
            {
                MovieName = movie.MovieName,
                MovieDateTime = movie.MovieDateTime,
                email = movie.email
            };

            using (var stream = new MemoryStream())
            {
                Imagefile.CopyTo(stream);
                moviedetail.Image = stream.ToArray();
            }
            await _context.Movie_Table.AddAsync(moviedetail);
            await _context.SaveChangesAsync();

            // code to send email
            string fromMail = "princemahato1211@gmail.com";
            string fromPassword = "ubkpnhacfnnvozst";
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Registration Successful";
            message.To.Add(new MailAddress(movie.email));
            message.Body = string.Format("Movie Ticket is Succefully booked \n Movie Name : {0} \n Email : {1} \n Movie Time:{2}", movie.MovieName ,movie.email, movie.MovieDateTime);
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);

            return RedirectToAction("Index");
        }

        //Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var data = await _context.Movie_Table.FirstOrDefaultAsync(x => x.Id == id);
            return View(data);
        }

        //Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var data = await _context.Movie_Table.FirstOrDefaultAsync(x => x.Id == id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Movie_data_model movie, IFormFile Imagefile)
        {
            if (Imagefile != null && Imagefile.Length > 0 && Imagefile.Length <= (2 * 1024 * 1024)) // 2mb limit
            {
                var imagedata = new byte[Imagefile.Length];
                movie.Image = imagedata;
            }

            var moviedetail = new Movie_data_model()
            {
                Id=movie.Id,
                MovieName = movie.MovieName,
                MovieDateTime = movie.MovieDateTime,
                email = movie.email
            };

            using (var stream = new MemoryStream())
            {
                Imagefile.CopyTo(stream);
                moviedetail.Image = stream.ToArray();
            }
            _context.Update(moviedetail);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.Movie_Table.FirstOrDefaultAsync(x => x.Id == id);
            return View(data);
        }

        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var data = await _context.Movie_Table.FirstOrDefaultAsync(x => x.Id == id);
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
