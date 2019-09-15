﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly IProductRepository _productRepository;

        public ProductRepositoryTests()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            var p3ReferentialOptions = new DbContextOptionsBuilder<P3Referential>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _productRepository = new ProductRepository(new P3Referential(p3ReferentialOptions));
            SeedData.Initialize(p3ReferentialOptions);
        }

        [Theory]
        [InlineData(1, false, "Echo Dot", "(2nd Generation) - Black", 10, 9.99)]
        [InlineData(10000, true)]
        [InlineData(0, true)]
        [InlineData(-1, true)]
        public async void GetProductTest(int id, bool expectNull, string name = "", string description = "", int quantity = 0, double price = 0)
        {
            var product = await _productRepository.GetProduct(id);

            if (expectNull)
            {
                Assert.Null(product);
            }
            else
            {
                Assert.Equal(name, product.Name);
                Assert.Equal(description, product.Description);
                Assert.Equal(quantity, product.Quantity);
                Assert.Equal(price, product.Price);
            }
        }

        [Fact]
        public void GetAllProductsTest()
        {
            var products = _productRepository.GetAllProducts();

            Assert.IsType<List<Product>>(products);
            Assert.Equal(5, products.Count());
        }

        [Fact]
        public void UpdateProductStocksTest()
        {
            _productRepository.UpdateProductStocks(1, 3);

            Assert.Equal(7, _productRepository.GetProduct(1).Result.Quantity);
        }
    }
}
