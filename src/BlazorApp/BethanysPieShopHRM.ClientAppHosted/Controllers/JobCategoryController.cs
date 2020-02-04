using BethanysPieShopHRM.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BethanysPieShopHRM.ClientAppHosted.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobCategoryController : Controller
    {
        private readonly ILogger<JobCategoryController> logger;

        public JobCategoryController(ILogger<JobCategoryController> logger)
        {
            this.logger = logger;
        }


        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<JobCategory> GetJobCategories()
        {
           return new JobCategory[]{
                new JobCategory() { JobCategoryId = 1, JobCategoryName = "Pie research" },
                new JobCategory() { JobCategoryId = 2, JobCategoryName = "Sandwich research" }
            };
        }     
    }
}
