#region using

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TestMVideo.Models;

#endregion

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestMVideo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService productsService;

        public ProductsController(ProductsService productsService)
        {
            this.productsService = productsService;
        }

        [HttpGet("/request")]
        public IActionResult Get([FromQuery] ProductRecommendationsRequest request)
        {
            var t = new Stopwatch();
            t.Start();
            var x = Ok(request.MinRank == 0
                ? productsService.GetRecommendedProductsIds(request.ProductId)
                : productsService.GetRecommendedProductsIdsWithMinRank(request.ProductId, request.MinRank));
            Debug.Print(t.Elapsed.TotalMilliseconds.ToString());
            return x;
        }
    }
}