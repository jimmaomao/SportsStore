using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.WebUI.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID=1,Name="P1" },
                new Product {ProductID=2,Name="P2" },
                new Product {ProductID=3,Name="P3" },
                new Product {ProductID=4,Name="P4" },
                new Product {ProductID=5,Name="P5" }
            });
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //动作
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //断言
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");

        }

        [TestMethod]
        public void Can_Generate_Page_links()
        {
            //准备---定义一个html辅助器，这是必须的，目的是运用扩展方法
            HtmlHelper myHelper = null;

            //创建pagingInfo数据
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemPerPage = 10
            };

            //准备用lambda表达式建立委托
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //动作
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //断言
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                            + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1"},
                new Product { ProductID=2,Name="P2"},
                new Product { ProductID=3,Name="P3"},
                new Product { ProductID=4,Name="P4"},
                new Product { ProductID=5,Name="P5"}
            });

            //准备    
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //动作
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //断言
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);

        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1",Category="cat1"},
                new Product { ProductID=2,Name="P2",Category="cat2"},
                new Product { ProductID=3,Name="P3",Category="cat1"},
                new Product { ProductID=4,Name="P4",Category="cat2"},
                new Product { ProductID=5,Name="P5",Category="cat3"},
                new Product { ProductID=6,Name="P6",Category="cat3"},
            });
            //准备创建 控制器，并使页面大小为3个物品
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //动作
            Product[] result = ((ProductsListViewModel)(controller.List("cat2", 1).Model)).Products.ToArray();

            //断言
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //构建存储库
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1",Category="cat1"},
                new Product { ProductID=2,Name="P2",Category="cat2"},
                new Product { ProductID=3,Name="P3",Category="cat2"},
                new Product { ProductID=4,Name="P4",Category="cat3"},
                new Product { ProductID=5,Name="P5",Category="cat4"},
                new Product { ProductID=6,Name="P6",Category="cat4"},
                new Product { ProductID=7,Name="P7",Category="cat3"},
                new Product { ProductID=8,Name="P8",Category="cat1"},
            });

            //准备一个控制器
            NavController controller = new NavController(mock.Object);

            //动作--获取分类集合
            string[] result = ((IEnumerable<string>)controller.Menu().Model).ToArray();

            //断言
            Assert.AreEqual(result.Length, 4);
            Assert.AreEqual(result[0], "cat1");
            Assert.AreEqual(result[1], "cat2");
            Assert.AreEqual(result[2], "cat3");
            Assert.AreEqual(result[3], "cat4");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //准备创建模仿存储库
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1", Category="APP"},
                new Product { ProductID=4,Name="P4",Category="ORA"}
            });
            //准备创建控制器
            NavController controller = new NavController(mock.Object);
            //准备定义已选分类
            string categoryToSelect = "APP";
            //动作
            string result = controller.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //断言
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1",Category="app"},
                new Product { ProductID=2,Name="P2",Category="app1"},
                new Product { ProductID=3,Name="P3",Category="app"},
                new Product { ProductID=4,Name="P4",Category="app1"},
                new Product { ProductID=5,Name="P5",Category="app"}
            });

            ProductController controller = new ProductController(mock.Object);

            controller.pageSize = 3;

            int re1 = ((ProductsListViewModel)(controller.List("app").Model)).PagingInfo.TotalItems;
            int re2 = ((ProductsListViewModel)(controller.List("app1").Model)).PagingInfo.TotalItems;
            int re3 = ((ProductsListViewModel)(controller.List(null).Model)).PagingInfo.TotalItems;

            Assert.AreEqual(re1, 3);
            Assert.AreEqual(re2, 2);
            Assert.AreEqual(re3, 5);
        }

        [TestMethod]
        public void Can_Retrieve_Image_Data() {
            Product prod = new Product {
                ProductID=2,
                Name="test",
                ImageData=new byte[] { },
                ImageMimeType="image/png"
            };

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1"},
                prod,
                new Product { ProductID=3,Name="P3"}

            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);

            ActionResult result = controller.GetImage(2);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Can_Retrieve_Image_Data_For_Invalid_ID()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1"},
                new Product { ProductID=2,Name="P2"}

            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);

            ActionResult result = controller.GetImage(100);

            Assert.IsNull(result);
        }

    }
}
