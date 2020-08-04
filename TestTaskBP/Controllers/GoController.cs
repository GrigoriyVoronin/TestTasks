using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTaskBP.Data;
using TestTaskBP.Models;

namespace TestTaskBP.Controllers
{
    public class GoController : Controller
    {
        private readonly UrlDataContext dataContext;

        public GoController(UrlDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<ActionResult> Index(string id)
        {
            if (id == null) return NotFound();

            var url = await dataContext.Urls
                .FirstOrDefaultAsync(urlDB => urlDB.ShortUrl == "https://voronintask.ru/go?id=" + id);

            if (url == null || !await UpdateNumberOfTransitions(url))
                return NotFound();

            return Redirect(url.FullUrl);
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