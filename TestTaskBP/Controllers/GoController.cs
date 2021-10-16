using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoroninTestTask.Data;
using VoroninTestTask.Models;

namespace VoroninTestTask.Controllers
{
    public class GoController : Controller
    {
        private readonly UrlDataContext dataContext;
        private readonly IConfiguration configuration;

        public GoController(UrlDataContext dataContext, IConfiguration configuration)
        {
            this.dataContext = dataContext;
            this.configuration = configuration;
        }

        public async Task<ActionResult> Index(string id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var shortUrl = $"{configuration.GetSection("host").Value}/go?id={id}";
                var url = await dataContext.Urls
                    .FirstOrDefaultAsync(urlDB => urlDB.ShortUrl == shortUrl);

                if (url == null || !await UpdateNumberOfTransitions(url))
                    return NotFound();

                return Redirect(url.FullUrl);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        private async Task<bool> UpdateNumberOfTransitions(URL url)
        {
            try
            {
                url.NumberOfTransitions++;
                await dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return !dataContext.Urls.Any(e => e.ID == url.ID);
            }

            return true;
        }
    }
}