using NUnit.Framework;
using SWE3_Zulli.OR.Framework;
using SWE3_Zulli.OR.Framework.Cache;
using SWE3_Zulli.OR.Models;
using System;

namespace TestProject
{
    /*
        delete from public.class;
        delete from public.student_course;
        delete from public.course;
        delete from public.locks;
        delete from public.student;
        delete from public.teacher;
        delete from public.person;
    */


    public class CRUDTests
    {
        private int _stIDCount = 0;
        private int _tcIDCount = 0;
        private int _courseIDCount = 0;
        private int _classIDCount = 0;

        [SetUp]
        public void Setup()
        {
            ORMapper.ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=ORTest;Pooling=true;Include Error Detail=true";
            ORMapper.Cache = new TrackerCache();
            //ORMapper.ClearDB();
        }

        [Test, Order(0)]
        public void ORMapper_InsertStudent()
        {
            Student s = new Student()
            {
                FirstName = "Bongo",
                LastName = "Dogo",
                BirthDate = DateTime.Today,
                ID = _stIDCount,
                Gender = Gender.OTHER,
                Grade = 15
            };
            ORMapper.Save(s);
            Assert.Pass();
        }

        [Test, Order(1)]
        public void ORMapper_GetStudent()
        {
            Student s = ORMapper.Get<Student>(_stIDCount);
            Assert.AreEqual("Bongo", s.FirstName);
            Assert.AreEqual("Dogo", s.LastName);
            Assert.AreEqual(DateTime.Today, s.BirthDate);
            Assert.AreEqual(Gender.OTHER, s.Gender);
            Assert.AreEqual(15, s.Grade);
        }

        [Test, Order(2)]
        public void ORMapper_UpdateStudent()
        {
            Student s = new Student()
            {
                FirstName = "CCCCCC",
                LastName = "KKKKKK",
                BirthDate = DateTime.Today,
                ID = _stIDCount,
                Gender = Gender.FEMALE,
                Grade = 3
            };
            ORMapper.Save(s);

            s = ORMapper.Get<Student>(s.ID);

            Assert.AreEqual("CCCCCC", s.FirstName);
            Assert.AreEqual("KKKKKK", s.LastName);
            Assert.AreEqual(DateTime.Today, s.BirthDate);
            Assert.AreEqual(Gender.FEMALE, s.Gender);
            Assert.AreEqual(3, s.Grade);
        }

        [Test, Order(3)]
        public void ORMapper_DeleteStudent()
        {
            Student s = new Student()
            {
                FirstName = "CCCCCC",
                LastName = "KKKKKK",
                BirthDate = DateTime.Today,
                ID = _stIDCount,
                Gender = Gender.FEMALE,
                Grade = 3
            };
            ORMapper.Delete(s);

            Assert.Throws<ArgumentOutOfRangeException>( () => ORMapper.Get<Student>(s.ID));
        }
    }
}