using System;
using System.Collections;
using System.Collections.Generic;
using M2SA.AppGenome.Tests.TestObjects;
using NUnit.Framework;
using M2SA.AppGenome.Reflection;

namespace M2SA.AppGenome.Tests
{
    [TestFixture]
    public class ObjectFormatTest : TestBase
    {
        public class TestOrder
        {
            public int OrderId { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public DateTime CreateDate { get; set; }
            public IList<TestProduct> Products { get; set; }
        }

        public class TestProduct
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public IList<TestProduct> SubProducts { get; set; }
        }

        [Test]
        public void SimpleTest()
        {
            var templete = @"@Code @Name @Id";
            var entity = new TestProduct();
            entity.Id = TestHelper.RandomizeInt();
            entity.Code = string.Concat("code-", TestHelper.RandomizeString());
            entity.Name = string.Concat("name-",TestHelper.RandomizeString());
            
            Console.WriteLine(entity.ToText());
            Console.WriteLine(entity.ToText(templete));
        }

        [Test]
        public void NestedTest()
        {
            var templete = @"
[@OrderId,@Price] @Name @CreateDate
@Products

#Products{
    @Id @Code @Name}
#Products$header{head@count }
#Products$footer{
foot@count }";

            var entity = new TestOrder();
            entity.OrderId = TestHelper.RandomizeInt();
            entity.Price = TestHelper.RandomizeInt() * TestHelper.RandomizeInt();
            entity.Name = string.Concat("name-", TestHelper.RandomizeString());
            entity.CreateDate = DateTime.Now;
            entity.Products = new List<TestProduct>()
            {
                new TestProduct() {Id=TestHelper.RandomizeInt(), Code=string.Concat("code-", TestHelper.RandomizeString()),Name = string.Concat("name-",TestHelper.RandomizeString())},
                new TestProduct() {Id=TestHelper.RandomizeInt(), Code=string.Concat("code-", TestHelper.RandomizeString()),Name = string.Concat("name-",TestHelper.RandomizeString())}
            };
            Console.WriteLine(entity.ToText(templete));
            Console.WriteLine("============");
        }

        public void RecursionTest()
        {
            var templete = @"
[@OrderId,@Price] @Name @CreateDate
@Products

#Products{
    @Id @Code @Name @SubProducts}
#Products$header{head@count }
#Products$footer{
foot@count }

#SubProducts{
        sub @Id @Code @Name}";

            var entity = new TestOrder();
            entity.OrderId = TestHelper.RandomizeInt();
            entity.Price = TestHelper.RandomizeInt() * TestHelper.RandomizeInt();
            entity.Name = string.Concat("name-", TestHelper.RandomizeString());
            entity.CreateDate = DateTime.Now;
            entity.Products = new List<TestProduct>()
            {
                new TestProduct()
                {
                    Id=TestHelper.RandomizeInt(), Code=string.Concat("p-code-", TestHelper.RandomizeString()),Name = string.Concat("name-",TestHelper.RandomizeString()),
                    SubProducts = new List<TestProduct>()
                        {
                            new TestProduct() {Id=TestHelper.RandomizeInt(), Code=string.Concat("sub-p-code-", TestHelper.RandomizeString()),Name = string.Concat("name-",TestHelper.RandomizeString())},
                            new TestProduct() {Id=TestHelper.RandomizeInt(), Code=string.Concat("sub-p-code-", TestHelper.RandomizeString()),Name = string.Concat("name-",TestHelper.RandomizeString())}
                        }
                },
                new TestProduct() {Id=TestHelper.RandomizeInt(), Code=string.Concat("p-code-", TestHelper.RandomizeString()),Name = string.Concat("name-",TestHelper.RandomizeString())}
            };
            Console.WriteLine(entity.ToXmlText(templete));
            Console.WriteLine("============");
        }
    }
}
