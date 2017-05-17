using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
using Microsoft.CSharp;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId =2 , Name= "P1" },
                new Product {ProductId = 3, Name = "P2" },
                new Product {ProductId = 5, Name = "P3" },
                new Product {ProductId = 6, Name = "P4" },
                new Product {ProductId = 7, Name = "P5" }
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {

            // arrange - definiowanie metody pomocniczej HTML — potrzebujemy tego,
            // aby użyć metody rozszerzającej
            HtmlHelper myHelper = null;

            // arrange - tworzenie danych PagingInfo
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // arrange - konfigurowanie delegatu z użyciem wyrażenia lambda
            Func<int, string> pageUrlDelegate = i => "Strona" + i;

            // act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Strona1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Strona2"">2</a>"
                 + @"<a class=""btn btn-default"" href=""Strona3"">3</a>",
                 result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId =2 , Name= "P1" },
                new Product {ProductId = 3, Name = "P2" },
                new Product {ProductId = 5, Name = "P3" },
                new Product {ProductId = 6, Name = "P4" },
                new Product {ProductId = 7, Name = "P5" }
            });

            //arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //assert
            PagingInfo info = result.PagingInfo;
            Assert.AreEqual(info.CurrentPage, 2);
            Assert.AreEqual(info.ItemsPerPage, 3);
            Assert.AreEqual(info.TotalItems, 5);
            Assert.AreEqual(info.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId =2 , Name= "P1",Category = "Cat1" },
                new Product {ProductId = 3, Name = "P2",Category = "Cat2"  },
                new Product {ProductId = 5, Name = "P3",Category = "Cat1"  },
                new Product {ProductId = 6, Name = "P4",Category = "Cat2"  },
                new Product {ProductId = 7, Name = "P5",Category = "Cat5"  }
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //act
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            //assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categoeries()
        {
            //arrange

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
{
                new Product {ProductId =2 , Name= "P1",Category = "Jabłka" },
                new Product {ProductId = 3, Name = "P2",Category = "Jabłka"  },
                new Product {ProductId = 5, Name = "P3",Category = "Śliwki" },
                new Product {ProductId = 6, Name = "P4",Category = "Pomarańcze"  },
                new Product {ProductId = 7, Name = "P5",Category = "Gruszki"  }
});

            NavController target = new NavController(mock.Object);

            //act
            string[] result = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //assert
            Assert.AreEqual(result.Length, 4);
            Assert.AreEqual(result[0], "Gruszki");
            Assert.AreEqual(result[1], "Jabłka");
            Assert.AreEqual(result[2], "Pomarańcze");
            Assert.AreEqual(result[3], "Śliwki");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1,Name = "P1", Category = "Jabłka" },
                new Product {ProductId = 4, Name = "P2", Category = "Pomarańcze" }
            });

            NavController target = new NavController(mock.Object);

            string categoryToSelect = "Jabłka";

            //act 
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //assert
            Assert.AreEqual(categoryToSelect, result);

        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId =2 , Name= "P1",Category = "Cat1" },
                new Product {ProductId = 3, Name = "P2",Category = "Cat2"  },
                new Product {ProductId = 5, Name = "P3",Category = "Cat1"  },
                new Product {ProductId = 6, Name = "P4",Category = "Cat2"  },
                new Product {ProductId = 7, Name = "P5",Category = "Cat3"  }
            });

            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            //act
            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            //assert
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
