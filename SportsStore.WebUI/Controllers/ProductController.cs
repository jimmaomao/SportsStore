using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository repository;
        public int pageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            this.repository = productRepository;//为何加this？？
        }

        public ViewResult List(string category, int page = 1)//告诉框架为该动作渲染一个默认的视图
        {
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = repository.Products.Where(p => p.Category == null || p.Category == category)
                .OrderBy(p => p.ProductID).Skip((page - 1) * pageSize).Take(pageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemPerPage = pageSize,
                    TotalItems = category == null ?
                    repository.Products.Count() :
                    repository.Products.Where(e => e.Category == category).Count()
                },
                CurrentCategory = category
            };
            //return View(repository.Products.OrderBy(p => p.ProductID).Skip((page - 1) * pageSize).Take(pageSize));//传递products对象，给框架提供数据
            return View(model);
        }

        public FileContentResult GetImage(int productId) {
            Product prod = repository.Products.FirstOrDefault(p => p.ProductID ==productId);
            if (prod != null)
            {
                return File(prod.ImageData, prod.ImageMimeType);
            }
            else {
                return null;
            }
        }
    }
}