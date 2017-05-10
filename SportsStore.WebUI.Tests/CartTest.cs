using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SportsStore.Domain.Entities;

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
    }
}
