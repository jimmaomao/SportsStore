using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SportsStore.Domain.Entities;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Tests
{
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void Add_New_Lines()
        {
            //准备--创建一些测试产品
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //准备--创建一个新的购物车
            Cart cart = new Cart();

            //动作
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            CartLine[] results = cart.Lines.ToArray();

            //断言
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //准备--创建一些测试产品
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //准备--创建一个新的购物车
            Cart cart = new Cart();

            //动作
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 2);
            CartLine[] results = cart.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //断言
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quentity, 3);
            Assert.AreEqual(results[1].Quentity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //准备--创建一些测试产品
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            //准备--创建一个新的购物车
            Cart cart = new Cart();

            //准备--对购物车添加一些产品
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 3);
            cart.AddItem(p3, 5);
            cart.AddItem(p2, 1);

            //动作
            cart.RemoveLine(p2);
            // CartLine[] results = cart.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //断言
            Assert.AreEqual(cart.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(cart.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //准备--创建一些测试产品
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //准备--创建一个新的购物车
            Cart cart = new Cart();

            //动作
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 3);
            decimal result = cart.ComputeTotalValue();

            //断言
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //准备--创建一些测试产品
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //准备--创建一个新的购物车
            Cart cart = new Cart();


            //准备--对购物车添加一些产品
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);

            //动作--重置购物车
            cart.Clear();

            //断言
            Assert.AreEqual(cart.Lines.Count(), 0);
        }


        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //准备--创建模仿存储库
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1",Category="app",Price=10M}
            }.AsQueryable());

            //准备--创建Cart
            Cart cart = new Cart();

            //准备--创建控制器
            CartController controller = new CartController(mock.Object, null);

            //动作 对cart添加一个产品
            controller.AddToCart(cart, 1, null);

            //断言
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Gose_To_Cart_Screen()
        {
            //准备-创建模仿存储库
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductID=1,Name="P1" ,Category="APP" }
            }.AsQueryable());

            //准备--创建Cart
            Cart cart = new Cart();

            //准备--创建控制器
            CartController controller = new CartController(mock.Object, null);

            //动作--向cart添加产品
            RedirectToRouteResult result = controller.AddToCart(cart, 2, "myUrl");

            //断言
            Assert.AreEqual(result.RouteValues["action"], "Index");            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //准备--创建Cart
            Cart cart = new Cart();

            //准备--创建控制器
            CartController controller = new CartController(null, null);

            //动作--挑用Index动作方法
            CartIndexViewModel result = (CartIndexViewModel)controller.Index(cart, "myUrl").ViewData.Model;//为什么用ViewData.Model而不用Model？？

            //断言
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannnot_Checkout_Empty_Cart()
        {
            //准备-创建一个模仿的订单处理器
            Mock<IOderProcessor> mock = new Mock<IOderProcessor>();

            //准备--创建一个空的购物车
            Cart cart = new Cart();

            //准备--创建一个控制器实例
            ShippingDetails shippingDetails = new ShippingDetails();

            //准备--创建一个控制器实例
            CartController controller = new CartController(null, mock.Object);

            //动作
            ViewResult result = controller.Checkout(cart, shippingDetails);

            //断言--检查。订单尚未传给处理器
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(),
                It.IsAny<ShippingDetails>()), Times.Never());

            //断言--检查，该方法返回的hi默认视图
            Assert.AreEqual("", result.ViewName);

            //Assert--check that i am passing an invalid model to the view  
            //断言-检查。给视图传递一个非法的模型
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannnot_Checkout_Invalid_ShippingDetails()
        {
            //准备-创建一个模仿的订单处理器
            Mock<IOderProcessor> mock = new Mock<IOderProcessor>();

            //准备--创建一个空的购物车
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //准备--创建一个控制器实例
            CartController controller = new CartController(null, mock.Object);

            //准备--把一个错误添加到模型
            controller.ModelState.AddModelError("error", "errror");

            //动作-试图结算
            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            //断言--检查。订单尚未传给处理器
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(),
                It.IsAny<ShippingDetails>()), Times.Never());

            //断言--检查，该方法返回的hi默认视图
            Assert.AreEqual("", result.ViewName);

            //Assert--check that i am passing an invalid model to the view  
            //断言-检查。给视图传递一个非法的模型
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannnot_Checkout_And_Submit_Order()
        {
            //准备-创建一个模仿的订单处理器
            Mock<IOderProcessor> mock = new Mock<IOderProcessor>();

            //准备--创建一个空的购物车
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //准备--创建一个控制器实例
            CartController controller = new CartController(null, mock.Object);

            //动作-试图结算
            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            //断言--检查。订单尚未传给处理器
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(),
                It.IsAny<ShippingDetails>()), Times.Once());

            //断言--检查，该方法返回的"Completed"默认视图
            Assert.AreEqual("Completed", result.ViewName);

            //Assert--check that i am passing an invalid model to the view  
            //断言-检查。给视图传递一个有效的模型
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
