using System;
using System.Linq;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Linq.Clauses;
using Rubicon.Data.DomainObjects.Linq.UnitTests.Parsing;

namespace Rubicon.Data.DomainObjects.Linq.UnitTests.ParsingTest.QueryParserTest
{
  [TestFixture]
  public class SimpleQueryTest : QueryTestBase<Student>
  {
    protected override IQueryable<Student> CreateQuery ()
    {
      return TestQueryGenerator.CreateSimpleQuery (QuerySource);
    }

    [Test]
    public void ParseResultIsNotNull()
    {
      Assert.IsNotNull (ParsedQuery);
    }

    [Test]
    public void HasFromClause ()
    {
      Assert.IsNotNull (ParsedQuery.FromClause);
      Assert.AreEqual ("s", ParsedQuery.FromClause.Identifier.Name);
      Assert.AreSame (typeof (Student), ParsedQuery.FromClause.Identifier.Type);
      Assert.AreSame (QuerySource, ParsedQuery.FromClause.QuerySource);
      Assert.AreEqual (0, ParsedQuery.FromClause.JoinClauseCount);
    }

    [Test]
    public void HasQueryBody()
    {
      Assert.IsNotNull (ParsedQuery.QueryBody);
      Assert.AreEqual (0, ParsedQuery.QueryBody.FromLetWhereClauseCount);
      Assert.IsNull (ParsedQuery.QueryBody.OrderByClause);
      Assert.IsNotNull (ParsedQuery.QueryBody.SelectOrGroupClause);
    }

    [Test]
    public void HasSelectClause()
    {
      SelectClause clause = ParsedQuery.QueryBody.SelectOrGroupClause as SelectClause;
      Assert.IsNotNull (clause);
      Assert.IsNotNull (clause.Expression);
      Assert.AreSame (ParsedQuery.FromClause.Identifier, clause.Expression,
          "from s in ... select s => select expression must be same as from-identifier"); 
    }

    [Test]
    public void OutputResult()
    {
      Console.WriteLine (ParsedQuery);
    }
  }
}