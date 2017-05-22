using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System.Linq;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //arrange
            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };

            Cart target = new Cart();

            //act
            target.AddItem(p1, 1);
            target.AddItem(p2, 2);
            CartLine[] results = target.Lines.ToArray();

            //assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //arrange
            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };

            Cart target = new Cart();

            //act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductId).ToArray();

            //assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_All_Lines()
        {
            //arrange
            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };
            Product p3 = new Product { ProductId = 3, Name = "P3" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            //act
            target.RemoveLine(p2);

            //assert
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //arrange
            Product p1 = new Product { ProductId = 1, Name = "P1", Price = 100m };
            Product p2 = new Product { ProductId = 2, Name = "P2", Price = 50m };

            Cart target = new Cart();

            //act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal results = target.ComputeTotalValue();

            //assert
            Assert.AreEqual(results, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //arrange
            Product p1 = new Product { ProductId = 1, Name = "P1", Price = 100m };
            Product p2 = new Product { ProductId = 2, Name = "P2", Price = 50m };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            //act
            target.Clear();

            //assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m=>m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Jab" }
            }.AsQueryable());

            Cart cart = new Cart();

            CartController target = new CartController(mock.Object, null);

            //act
            target.AddToCart(cart, 1, null);

            //asser
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductId, 1);
        }

        [TestMethod]
        public void Adding_Products_To_CarT_Goes_To_Cart_Screen()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Jabłka" }
            }.AsQueryable());

            Cart cart = new Cart();

            CartController target = new CartController(mock.Object, null);

            //act
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            //assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();

            ShippingDetails details = new ShippingDetails();

            CartController target = new CartController(null, mock.Object);

            //act
            ViewResult result = target.Checkout(cart, details);

            //asser czy zamowienie przekazano do procesora
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

            //assert czy metoda zwraca domyslny widok
            Assert.AreEqual("", result.ViewName);

            //assert czy przekazujemy prawidlowy model do widoku
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Invalid_ShippingDetails()
        {
            //arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController target = new CartController(null, mock.Object);

            target.ModelState.AddModelError("error", "error");

            //act
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //assert czy czy zamowienie przekazano do procesora
            Assert.AreEqual("", result.ViewName);

            //assert czy przekazujemy nieprawidlowy model do widoku
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            //arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();
            cart.AddItem(new Product(),1);

            CartController target = new CartController(null, mock.Object);

            //act
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //assert sprawdzamy czy zamowienie nie zostalo przekazane do procesora
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());

            //assert czy metoda zwraca widok complete
            Assert.AreEqual("Completed", result.ViewName);

            //assert czy przekazujemy prawidlowy model do widoku
            Assert.AreEqual(true, target.ViewData.ModelState.IsValid);
        }
    }
}
