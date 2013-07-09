using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using NUnit.Framework;
using ReflectionalMapper;
using ReflectionalMapperTest.TestDomain;
using Mapper = ReflectionalMapper.ReflectionalMapper;
namespace ReflectionalMapperTest
{
    [TestFixture]
    public class ReflectionalMapperTest
    {
        [Test]
        public void ReflectionalMapperMapsOkForClientsPhone()
        {
            SqlManager sqlManager = new SqlManager("MapperTestDb");
            IEnumerable<ClientsPhone> clientsPhones = sqlManager.ExecQuery<ClientsPhone>
            (
                "SELECT c.Name AS [ClientName], cp.PhoneNumber, p.AreaCode, p.Number, pt.Name AS [PhoneTypeName] FROM ClientsPhone cp " +
                "INNER JOIN Phone p ON cp.PhoneNumber = p.Number AND cp.PhoneAreaCode = p.AreaCode " +
                "INNER JOIN client c ON c.Name = cp.ClientName " +
                "INNER JOIN phoneType pt ON pt.Name = p.PhoneTypeName"
            );
            Assert.That(clientsPhones, Is.Not.Null);
            Assert.That(clientsPhones.Count(), Is.EqualTo(2));
        }

        [Test]
        public void ReflectionalMapperMapsOkForClients()
        {
            SqlManager sqlManager = new SqlManager("MapperTestDb");
            IEnumerable<Client> clientsPhones = sqlManager.ExecQuery<Client>("SELECT * FROM Client");
            Assert.That(clientsPhones, Is.Not.Null);
            Assert.That(clientsPhones.Count(), Is.EqualTo(3));
        }

        [Test]
        public void ReflectionalMapperMapsOkForPhoneTypes()
        {
            SqlManager sqlManager = new SqlManager("MapperTestDb");
            IEnumerable<PhoneType> clientsPhones = sqlManager.ExecQuery<PhoneType>("SELECT * FROM PhoneType");
            Assert.That(clientsPhones, Is.Not.Null);
            Assert.That(clientsPhones.Count(), Is.EqualTo(3));
        }

        [Test]
        public void ReflectionalMapperMapsOkForPhones()
        {
            SqlManager sqlManager = new SqlManager("MapperTestDb");
            IEnumerable<Phone> clientsPhones = sqlManager.ExecQuery<Phone>("SELECT * FROM Phone");
            Assert.That(clientsPhones, Is.Not.Null);
            Assert.That(clientsPhones.Count(), Is.EqualTo(2));
        }

        [Test]
        public void TestFindByString()
        {
            SqlBuilder builder = new SqlBuilder();
            
            string expression1 = builder.BuildFromExpression<TestEntity>(c => c.Name, "asdfa");
            string expression2 = builder.BuildFromExpression<TestEntity>(c => c.Id, "asdfa");

            Assert.That(expression1, Is.EqualTo("SELECT * FROM TestEntity WHERE Name = @Name"));
            Assert.That(expression2, Is.EqualTo("SELECT * FROM TestEntity WHERE Id = @Id"));
        }

        [Test]
        public void TestFIndBy()
        {
            const string name = "Daniel Mcliver";
            SqlManager sqlManager = new SqlManager("MapperTestDb");
            IEnumerable<Client> clients = sqlManager.FindBy<Client>(c => c.Name, name);

            IEnumerable<Client> allClients = clients as IList<Client> ?? clients.ToList();

            Assert.That(allClients.Count(), Is.EqualTo(1));
            Assert.That(allClients.First().Name, Is.EqualTo(name));
        }

        [Test]
        public void TestFindAll()
        {
            SqlManager sqlManager = new SqlManager("MapperTestDb");
            IEnumerable<Client> clients = sqlManager.FindAll<Client>();

            IEnumerable<Client> allClients = clients as IList<Client> ?? clients.ToList();

            Assert.That(allClients.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestExecQueryWithParams()
        {
            var manager = new SqlManager("MapperTestDb");
            var sqlParams = new Dictionary<string, object> {{"Number", 8462489}, {"AreaCode", 09}};

            IEnumerable<Phone> phones = manager.ExecQuery<Phone>("SELECT * FROM Phone p WHERE p.Number = @Number AND p.AreaCode = @AreaCode", sqlParams);

            IEnumerable<Phone> allPhones = phones as IList<Phone> ?? phones.ToList();

            Assert.That(allPhones.Count(), Is.EqualTo(1));
            Assert.That(allPhones.First().AreaCode, Is.EqualTo(9));
            Assert.That(allPhones.First().Number, Is.EqualTo(8462489));
        }

        [Test]
        public void TestExecQueryWithParamsThatMatchNoDataInDbReturnsEmptyList()
        {
            var manager = new SqlManager("MapperTestDb");
            var sqlParams = new Dictionary<string, object> {{"Number", 8492489}, {"AreaCode", 09}};

            IEnumerable<Phone> phones = manager.ExecQuery<Phone>("SELECT * FROM Phone p WHERE p.Number = @Number AND p.AreaCode = @AreaCode", sqlParams);

            IEnumerable<Phone> allPhones = phones as IList<Phone> ?? phones.ToList();

            Assert.That(allPhones.Count(), Is.EqualTo(0));
        }

        [Test]
        public void BuildInsertStatementWithAreaCodeAsIdentityBuildsSqlWithoutAreaCode()
        {
            SqlBuilder builder = new SqlBuilder();
            string statement = builder.BuildInsertStatement(new Phone {AreaCode = 11, Number = 1234567}, p => p.AreaCode);
            Assert.That(statement, Is.EqualTo("INSERT INTO Phone(Number) VALUES(1234567)"));
        }

        [Test]
        public void BuildInsertStatementWithNoIdentityIncludesAreaCode()
        {
            SqlBuilder builder = new SqlBuilder();
            string statement = builder.BuildInsertStatement(new Phone { AreaCode = 11, Number = 1234567 }, null);
            Assert.That(statement, Is.EqualTo("INSERT INTO Phone(AreaCode,Number) VALUES(11,1234567)"));
        }

        [Test]
        public void BuildInsertStatementWithStringAddsQuotes()
        {
            SqlBuilder builder = new SqlBuilder();
            string statement = builder.BuildInsertStatement(new Client{Name = "Daniel Mcliver"}, null);
            Assert.That(statement, Is.EqualTo("INSERT INTO Client(Name) VALUES('Daniel Mcliver')"));
        }

        [Test]
        public void InsertInsertsOk()
        {
            SqlManager manager = new SqlManager("MapperTestDb");
            using (new TransactionScope())
            {
                manager.Save(new Client { Name = "Randy Roads" });
                int count = manager.FindBy<Client>(c => c.Name, "Randy Roads").Count();
                Assert.That(count, Is.EqualTo(1));
            }
        }

        [Test]
        public void UpdateBuildsUpdateStatementCorrectly()
        {
            SqlBuilder builder = new SqlBuilder();
            string statement = builder.BuildUpdateStatement(new Phone {AreaCode = 11, Number = 223423}, p => p.AreaCode, p => p.Number);
            //This is a crap test but given the current schema the only one applicable unfortunately
            Assert.That(statement, Is.EqualTo("UPDATE Phone SET  WHERE AreaCode=11 AND Number=223423"));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot update an entity whose fields only contain ids")]
        public void BuildsUpdateStatementTryingToUpdateIdThrowsException()
        {
            SqlBuilder builder = new SqlBuilder();
            builder.BuildUpdateStatement(new Client { Name = "Sandy Roads"}, p => p.Name);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Must specify identifier")]
        public void BuildUpdateStatementTryingToUpdateWithNoIdThrowsException()
        {
            SqlBuilder builder = new SqlBuilder();
            builder.BuildUpdateStatement(new Client { Name = "Sandy Roads" });
        }
        
        [Test]
        public void BuildDeleteStatementBuildsCorrectly()
        {
            SqlBuilder builder = new SqlBuilder();
            string statement = builder.BuildDeleteStatement(new Phone { AreaCode = 11, Number = 223423 }, p => p.AreaCode, p => p.Number);
            Assert.That(statement, Is.EqualTo("DELETE FROM Phone WHERE AreaCode=11 AND Number=223423"));
        }

        [Test]
        public void BuildDeleteStatementWithStringIdAppliesQuotes()
        {
            SqlBuilder builder = new SqlBuilder();
            string statement = builder.BuildDeleteStatement(new Client() { Name = "Hello Kitty"}, c => c.Name);
            Assert.That(statement, Is.EqualTo("DELETE FROM Client WHERE Name='Hello Kitty'"));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Must specify identifier")]
        public void BuildDeleteStatementWithNoIdsThrowsException()
        {
            SqlBuilder builder = new SqlBuilder();
            builder.BuildDeleteStatement(new Client());
        }

        [Test]
        public void TestDeleteDeletesFromDatabase()
        {
            SqlManager manager = new SqlManager("MapperTestDb");
            using (new TransactionScope())
            {
                manager.Delete(new Client { Name = "Golly Wog" }, c => c.Name);
                Assert.That(manager.FindAll<Client>().Count(), Is.EqualTo(2));
            }
        }
    }
}

