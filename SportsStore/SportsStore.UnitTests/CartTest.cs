﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System.Linq;

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
    }
}