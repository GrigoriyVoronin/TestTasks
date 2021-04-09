using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABTestReal.ApiModels;
using ABTestReal.Services;
using DbRepositories;

namespace ABTestReal.Controllers
{
    [Route("api/retention")]
    [ApiController]
    public class RetentionController : Controller
    {
        private readonly RetentionService _retentionService;

        public RetentionController(RetentionService retentionService)
        {
            _retentionService = retentionService;
        }

        [HttpGet]
        public async Task<ActionResult<RetentionInfo>> GetRollingRetention()
        {
            return await _retentionService.CalculateRetentionInfo(7);
        }
    }
}
