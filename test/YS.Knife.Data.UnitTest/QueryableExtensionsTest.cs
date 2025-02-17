﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class QueryableExtensionsTest
    {
        [TestMethod, TestCategory("ListAll")]
        public void ShouldGetListWhenListAllWithOutQueryInfo()
        {
            var users = CreateUsersWithAddress();
            var actual = users.ListAll(null);
            actual.Should().BeEquivalentTo(users.ToList());
        }

        [TestMethod, TestCategory("ListAll")]
        public void ShouldGetListWhenListAllWithFilterInfoAndNoOrderInfo()
        {
            var users = CreateUsersWithAddress();
            var actual = users.ListAll(FilterInfo.CreateItem("Name", FilterType.Contains, "a"), null);
            actual.Should().BeEquivalentTo(users.Where(p => (p.Name != null && p.Name.Contains("a"))).ToList());
        }

        [TestMethod, TestCategory("ListAll")]
        public void ShouldGetOriginListWhenListAllWithNoFilterInfoAndOrderInfo()
        {
            var users = CreateUsersWithAddress();
            var actual = users.ListAll(null, OrderInfo.Create("Name", OrderType.Desc));
            actual.Should().BeEquivalentTo(users.OrderByDescending(p => p.Name).ToList());
        }

        [TestMethod, TestCategory("ListAll")]
        public void ShouldGetListWhenListAllWithFilterInfoAndOrderInfo()
        {
            var users = CreateUsersWithAddress();
            var actual = users.ListAll(FilterInfo.CreateItem("Name", FilterType.Contains, "a"),
                OrderInfo.Create("Name", OrderType.Desc));
            actual.Should().BeEquivalentTo(users.Where(p => (p.Name != null && p.Name.Contains("a")))
                .OrderByDescending(p => p.Name).ToList());
        }

        
        [TestMethod, TestCategory("ListAll2")]
        public void ShouldGetOriginListWhenListAll2WithOutQueryInfo()
        {
            var users = CreateUsersWithAddress();
            var actual = users.ListAll(null,null, UserMapper);
            actual.Should().BeEquivalentTo( UserMapper(users).ToList());
        }

        private IQueryable<User2> UserMapper(IQueryable<User> users)
        {
            return users.Select(p => new User2()
            {
                 Id = p.Id,
                 Name = p.Name,
                 Age = p.Age,
                 FirstAddressCity = p.Addresses.Select(c=>c.City).FirstOrDefault()
            });
        }

        private IQueryable<User> CreateUsersWithAddress()
        {
            return (new List<User>()
            {
                new()
                {
                    Id = "001",
                    Name = "ZhangSan",
                    Age = 19,
                    Score = 61,
                    Addresses = new List<Address>() {new() {City = "xian"}}
                },
                new()
                {
                    Id = "002",
                    Name = "LiSi",
                    Age = 20,
                    Score = 81,
                    Addresses = new List<Address>() {new() {City = "beijing"}, new() {City = "xian"}}
                },
                new()
                {
                    Id = "003",
                    Name = "WangWu",
                    Age = 20,
                    Score = 70,
                    Addresses = new List<Address>() {new() {City = "beijing"}}
                },
                new()
                {
                    Id = "004",
                    Name = "WangMaZi",
                    Age = 19,
                    Addresses = new List<Address>() {new() {City = "xian"}, new() {City = "xian"}}
                },
                new()
                {
                    Id = "005",
                    Name = null,
                    Age = 21,
                    Addresses = new List<Address>() {new() {City = "beijing"}, new() {City = "beijing"}}
                }
            }).AsQueryable();
        }

        class User
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }

            public int? Score { get; set; }

            public List<Address> Addresses { get; set; } = new List<Address>();
        }

        class Address
        {
            public string City { get; set; }
        }

        class User2
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string FirstAddressCity { get; set; }
        }
    }
}
