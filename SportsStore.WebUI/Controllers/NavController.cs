using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {

        private IProductRepository repository;

        public NavController(IProductRepository repo)//DI依赖解析，已实现接口
        {
            this.repository = repo;
        }

        //public PartialViewResult Menu(string category = null, bool horizontalLayout=false)
        public PartialViewResult Menu(string category = null)
        {
            ViewBag.SelectedCategory = category;
            IEnumerable<string> categories = repository.Products.Select(x => x.Category).Distinct().OrderBy(x => x);
            //string viewName = horizontalLayout ? "MenuHorizontal" : "Menu";
            return PartialView("FlexMenu", categories);
        }
    }
}