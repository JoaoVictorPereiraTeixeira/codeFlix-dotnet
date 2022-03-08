using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class BaseFixture
{
    public Faker Faker { get; set; }
    protected BaseFixture()
    {
        Faker = new Faker("pt_BR");
    }

}
