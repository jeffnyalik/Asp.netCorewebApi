using System.Net;
using Microsoft.Win32;
using System.Net.Mime;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{  
    [Route("/api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {   
        private readonly ApplicationDbContext _context;
       
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
          
        }
        [HttpGet]
        [Authorize(Policy = "RequiredLoggedIn")]
        public async Task<ActionResult<IEnumerable<ProductModel>>>GetProducts()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequiredLoggedIn")]
        public async Task<ActionResult<ProductModel>>GetProduct(int id)
        {
           var product = await _context.Products.FindAsync(id);
           if(product == null)
           {
               return NotFound("The product ID does not exist");
           }

           return Ok(product);
        }
        
        [HttpPost]
        [Authorize(Policy = "RequiredAdmin")]
        public async Task<IActionResult>Addproduct([FromBody] ProductModel formData)
        {   

            if(!ModelState.IsValid)
               return BadRequest(ModelState);

            var newProduct = new ProductModel
            {
                Name = formData.Name,
                Description = formData.Description,
                ImageUrl = formData.ImageUrl,
                OutOfStock = formData.OutOfStock,
                Price = formData.Price,
            };

            await _context.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            return Ok(new {Success="Data has been Created Successfully"});
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "RequiredAdmin")]
        public async Task<IActionResult>UpdateProduct(int id, [FromBody] ProductModel productModel)
        {
            if(!ModelState.IsValid)
               return BadRequest(ModelState);

            var findProduct =await _context.Products.FirstOrDefaultAsync(p=>p.ProductId == id);
            if(findProduct == null)
            {
                return NotFound("The product does not exist");
            }

            findProduct.Name = productModel.Name;
            findProduct.Description = productModel.Description;
            findProduct.ImageUrl = productModel.ImageUrl;
            findProduct.OutOfStock = productModel.OutOfStock;
            findProduct.Price = productModel.Price;

            _context.Entry(findProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new JsonResult($"The product with ID {id} has been updated successfully"));
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequiredAdmin")]
        public async Task<ActionResult<ProductModel>>DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound($"Product with an ID of {product.ProductId} Does not exist");
            }
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new {msg="Record has been removed."});
        }
    }
}