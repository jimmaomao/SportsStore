using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.WebUI.Tests
{
    /// <summary>
    /// UnitTest2 的摘要说明
    /// </summary>
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            //准备--创建模仿存储库
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[]{
                new Product { ProductID=1,Name="p1"},
                new Product { ProductID=2,Name="P2"},
                new Product { ProductID=3,Name="P3"}
            }
                );

            //准备--创建控制器
            AdminController controller = new AdminController(mock.Object);

            //动作
            Product[] result = ((IEnumerable<Product>)controller.Index().ViewData.Model).ToArray();

            //断言
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("p1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1"},
                new Product { ProductID=2,Name="P2"},
                new Product { ProductID=3,Name="P3"},
            });

            AdminController controller = new AdminController(mock.Object);

            Product p1 = controller.Edit(1).ViewData.Model as Product;
            Product p2 = controller.Edit(2).ViewData.Model as Product;
            Product p3 = controller.Edit(3).ViewData.Model as Product;

            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1"},
                new Product { ProductID=2,Name="P2"},
                new Product { ProductID=3,Name="P3"},
            });

            AdminController controller = new AdminController(mock.Object);

            Product result = controller.Edit(4).ViewData.Model as Product;


            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            AdminController controller = new AdminController(mock.Object);

            Product product = new Product { Name = "Test" };

            ActionResult result = controller.Edit(product);

            mock.Verify(m => m.SaveProduct(product));

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Save_Invalid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            AdminController controller = new AdminController(mock.Object);

            Product product = new Product { Name = "Test" };

            controller.ModelState.AddModelError("error", "error");

            ActionResult result = controller.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            Product prod = new Product { ProductID = 2, Name = "Test" };
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product {ProductID=1,Name="P1" },
                prod,
                new Product { ProductID=3,Name="P2"}
            });
            AdminController controller = new AdminController(mock.Object);

            

            controller.Delete(prod.ProductID);

        

            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }
    }
}
