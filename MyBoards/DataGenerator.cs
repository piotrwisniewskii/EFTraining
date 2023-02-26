using Bogus;
using MyBoards.Entities;

namespace MyBoards
{
    public class DataGenerator
    {
        public void Seed(MyBoardsContext context)
        {
            var addressGenerator = new Faker<Adress>()
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.Country, f => f.Address.Country())
                .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
                .RuleFor(a => a.Street, f => f.Address.StreetName());

            var userGenerator = new Faker<User>()
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.FullName, f => f.Person.FullName)
                .RuleFor(u => u.Adress, f => addressGenerator.Generate());
        }
    }
}
