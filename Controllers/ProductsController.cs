using ProductsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProductsApp.Controllers
{
    public class ProductsController : ApiController
    {
		public IEnumerable<Product> GetAllProducts()
		{
			return ProductRepository.Products;
		}

		public IHttpActionResult GetProduct(int id)
		{
			var product = (from p in ProductRepository.Products
						   where p.Id == id
						   select p).SingleOrDefault();

			if (product != null)
			{
				return Ok(product);
			}
			else
			{
				return NotFound();
			}
		}
    }


}
