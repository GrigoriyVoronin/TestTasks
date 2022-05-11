using System.Linq;
using System.Threading.Tasks;
using HashidsNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VoroninTestTask.Data;
using VoroninTestTask.Models;

namespace VoroninTestTask.Controllers
{
    public class URLsController : Controller
    {
        private readonly UrlDataContext dataContext;
        private readonly IConfiguration configuration;

        public URLsController(UrlDataContext dataContext, IConfiguration configuration)
        {
            this.dataContext = dataContext;
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            return View(await dataContext.Urls
                .ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FullUrl,ShortUrl,CreateDate,NumberOfTransitions")] URL url)
        {
            if (ModelState.IsValid && await AddUrlToDb(url))
                return RedirectToAction(nameof(Index));

            return View(url);
        }

        private async Task<bool> AddUrlToDb(URL url)
        {
            var urlInDb = await dataContext.Urls
                .FirstOrDefaultAsync(oldUrl => oldUrl.FullUrl == url.FullUrl);
            if (urlInDb != null)
                return false;

            var hashids = new Hashids($"{url.FullUrl}");
            var id = hashids.Encode(1, 2, 3, 4, 5);
            url.ShortUrl = $"{configuration.GetSection("host").Value}/go?id=" + id;
            dataContext.Add(url);
            await dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var url = await dataContext.Urls.FindAsync(id);
            if (url == null)
                return NotFound();

            return View(url);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FullUrl,ShortUrl,CreateDate,NumberOfTransitions")] URL url)
        {
            if (id != url.ID)
                return NotFound();

            if (await UpdateUrl(url))
                return RedirectToAction(nameof(Index));

            return View(url);
        }

        private async Task<bool> UpdateUrl(URL url)
        {
            if (!ModelState.IsValid)
                return false;

            try
            {
                var hashids = new Hashids($"{url.FullUrl}");
                var id = hashids.Encode(1, 2, 3, 4, 5);
                url.ShortUrl = "https://voronintask.ru/go?id=" + id;
                dataContext.Update(url);
                await dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (dataContext.Urls.Any(e => e.ID == url.ID))
                    return false;
            }

            return true;
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var url = await dataContext.Urls
                .FirstOrDefaultAsync(m => m.ID == id);
            if (url == null)
                return NotFound();

            return View(url);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var url = await dataContext.Urls.FindAsync(id);
            dataContext.Urls.Remove(url);
            await dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}