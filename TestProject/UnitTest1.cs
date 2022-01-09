using NUnit.Framework;

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


    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}