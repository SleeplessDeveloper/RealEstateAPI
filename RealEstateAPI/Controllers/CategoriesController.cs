using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Data;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        ApiDBCOntext _dbContext = new ApiDBCOntext();

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(_dbContext.Categories);
        }
    }
}
