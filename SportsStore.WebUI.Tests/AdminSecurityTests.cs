using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;

namespace SportsStore.WebUI.Tests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Can_Login_With_Valid_Credentials()
        {
            //准备--创建模仿认证提供器
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);

            //准备 --创建视图模型器
            LoginViewModel model = new LoginViewModel
            {
                UserName = "admin",
                Password = "secret"
            };

            //准备-创建控制器
            AccountController controller = new AccountController(mock.Object);

            //动作--使用合法凭证进行认证
            ActionResult result = controller.Login(model, "/MyUrl");

            //断言
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invaild_Credentials()
        {
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("baduser", "badpass")).Returns(false);

            LoginViewModel model = new LoginViewModel
            {
                UserName = "baduser",
                Password = "badpass"
            };

            AccountController controller = new AccountController(mock.Object);

            ActionResult result = controller.Login(model, "/myUrl");

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
