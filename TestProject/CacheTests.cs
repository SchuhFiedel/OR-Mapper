using NUnit.Framework;
using SWE3_Zulli.OR.Framework.Cache;
using SWE3_Zulli.OR.Framework.Interfaces;
using SWE3_Zulli.OR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class CacheTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test, Order(0)]
        public void Cache_Put()
        {
            ICache cache = new TrackerCache();
            Teacher t = new()
            {
                ID = 0,
                Salary = 55000,
                BirthDate = DateTime.Today,
                HireDate = DateTime.Now,
                FirstName = "Hans",
                Gender = Gender.MALE,
                LastName = "Rudolf"
            };
            cache.PutObject(t);
            Teacher retT = (Teacher)cache.GetObject(typeof(Teacher), t.ID);
            Assert.AreEqual(t, retT);
        }

        [Test, Order(1)]
        public void Cache_Contains_True()
        {
            ICache cache = new TrackerCache();
            Teacher t = new()
            {
                ID = 0,
                Salary = 55000,
                BirthDate = DateTime.Today,
                HireDate = DateTime.Now,
                FirstName = "Hans",
                Gender = Gender.MALE,
                LastName = "Rudolf"
            };
            cache.PutObject(t);

            Assert.IsTrue(cache.ContainsObject(t));
        }

        [Test, Order(2)]
        public void Cache_Contains_False()
        {
            ICache cache = new TrackerCache();
            Teacher t = new()
            {
                ID = 0,
                Salary = 55000,
                BirthDate = DateTime.Today,
                HireDate = DateTime.Now,
                FirstName = "Hans",
                Gender = Gender.MALE,
                LastName = "Rudolf"
            };

            Assert.IsFalse(cache.ContainsObject(t));
        }

        [Test, Order(3)]
        public void Cache_HasChanged_True()
        {
            ICache cache = new TrackerCache();
            Teacher t = new()
            {
                ID = 0,
                Salary = 55000,
                BirthDate = DateTime.Today,
                HireDate = DateTime.Now,
                FirstName = "Hans",
                Gender = Gender.MALE,
                LastName = "Rudolf"
            };
            cache.PutObject(t);
            t.Salary = 10000000;
            t.LastName = "iiiiii";
            Assert.IsTrue(cache.ObjectHasChanged(t));
        }

        [Test, Order(4)]
        public void Cache_HasChanged_False()
        {
            ICache cache = new TrackerCache();
            Teacher t = new()
            {
                ID = 0,
                Salary = 55000,
                BirthDate = DateTime.Today,
                HireDate = DateTime.Now,
                FirstName = "Hans",
                Gender = Gender.MALE,
                LastName = "Rudolf"
            };
            cache.PutObject(t);

            Assert.IsFalse(cache.ObjectHasChanged(t));
        }

        [Test, Order(5)]
        public void Cache_Delete_ObjectInCache()
        {
            ICache cache = new TrackerCache();
            Teacher t = new()
            {
                ID = 0,
                Salary = 55000,
                BirthDate = DateTime.Today,
                HireDate = DateTime.Now,
                FirstName = "Hans",
                Gender = Gender.MALE,
                LastName = "Rudolf"
            };
            cache.PutObject(t);

            Teacher retT = (Teacher)cache.GetObject(typeof(Teacher), t.ID);
            Assert.AreEqual(t, retT);

            cache.RemoveObject(t);

            Assert.IsFalse(cache.ContainsObject(t));
        }

        [Test, Order(6)]
        public void Cache_Delete_NoObjectInCache()
        {
            ICache cache = new TrackerCache();
            Teacher t = new()
            {
                ID = 0,
                Salary = 55000,
                BirthDate = DateTime.Today,
                HireDate = DateTime.Now,
                FirstName = "Hans",
                Gender = Gender.MALE,
                LastName = "Rudolf"
            };

            cache.RemoveObject(t);

            Assert.Pass();
        }
    }
}
