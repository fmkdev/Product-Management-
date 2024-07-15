using API.Application;
using API.Domain.Entities;
using API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static API.Dtos.ResponseDto;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/product")]
    [ApiController]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepo;

        public ProductController(IGenericRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Product([FromBody] List<CreateProductDto> createProductRequest)
        {
            if(createProductRequest.Count == 0 || createProductRequest == null)
            {
                return BadRequest(new ResponseDto()
                {
                    Status = ResponseStatus.Fail,
                    Message = "Provide new product data"
                });
            }
            else
            {
                foreach (var product in createProductRequest)
                {
                    var newProduct = new Product
                    {
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price
                    };
                    await _productRepo.Create(newProduct);
                }

                await _productRepo.Save();

                return Ok(new ResponseDto()
                {
                    Status = ResponseStatus.Success,
                    Message = "Products successfully created"
                });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Product([FromBody] UpdateProductDto updateProductRequest)
        {
            if (updateProductRequest == null)
            {
                return BadRequest(new ResponseDto()
                {
                    Status = ResponseStatus.Fail,
                    Message = "Provide product data"
                });
            }
            else
            {
                var product = await _productRepo.Get(a => a.Id ==  updateProductRequest.Id);
                if(product == null) return BadRequest(new ResponseDto(){ Status = ResponseStatus.Fail, Message = "product not exist" });

                product.Name = updateProductRequest.Name;
                product.Price = updateProductRequest.Price;
                product.Description = updateProductRequest.Description;

                await _productRepo.Update(product);
              
                await _productRepo.Save();

                return Ok(new ResponseDto()
                {
                    Status = ResponseStatus.Success,
                    Message = "Product successfully updated"
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Product([FromQuery] int productId)
        {
            var product = await _productRepo.Get(a => a.Id ==  productId);
            if (product == null) return BadRequest(new ResponseDto() { Status = ResponseStatus.Fail, Message = "product not exist" });
            return Ok(new ResponseDto()
            {
                Status = ResponseStatus.Success,
                Message = "Success",
                Data = product
            });
        }
        [HttpGet("all")]
        public async Task<IActionResult> Product([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productRepo.Query().OrderByDescending(p => p.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(new ResponseDto()
            {
                Status = ResponseStatus.Success,
                Message = "Success",
                Data = products
            });
        }
        [HttpGet("disabled")]
        public async Task<IActionResult> Product()
        {
            var products = await _productRepo.Query().Where(a=>a.IsActive == false).OrderByDescending(p => p.CreatedOn).ToListAsync();
            return Ok(new ResponseDto()
            {
                Status = ResponseStatus.Success,
                Message = "Success",
                Data = products
            });
        }
        [HttpGet("last_week_prices")]
        public async Task<IActionResult> SumOfLastWeekPices()
        {
            var lastWeek = DateTime.UtcNow.AddDays(-7);

            var products = await _productRepo.Query().Where(a => a.CreatedOn >= lastWeek).SumAsync(a => a.Price);
            return Ok(new ResponseDto()
            {
                Status = ResponseStatus.Success,
                Message = "Success",
                Data = products
            });
        }

        [HttpDelete]
        public async Task<IActionResult> Disable([FromQuery] List<int> productIds)
        {
            var products = await _productRepo.Query().Where(a => productIds.Contains(a.Id)).ToListAsync();
            await _productRepo.Delete(products);
            await _productRepo.Save();
            return Ok(new ResponseDto()
            {
                Status = ResponseStatus.Success,
                Message = "Success"
            });
        }
    }
    public class CreateProductDto
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
    public class UpdateProductDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
