// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.MappingResolution
{
  [TestFixture]
  public class GroupAggregateSimplifierTest
  {
    private SqlStatement _simplifiableSqlStatement;
    private StreamedScalarValueInfo _dataInfo;
    private SqlGroupingSelectExpression _associatedGroupingSelectExpression;
    private SqlStatement _joinedGroupingSubStatement;
    private SqlTable _joinedGroupingTable;
    private AggregationExpression _selectProjection;

    [SetUp]
    public void SetUp ()
    {
      _dataInfo = new StreamedScalarValueInfo (typeof (int));
      _associatedGroupingSelectExpression = new SqlGroupingSelectExpression (Expression.Constant ("key"), Expression.Constant ("value"));
      _joinedGroupingSubStatement = SqlStatementModelObjectMother.CreateSqlStatement (_associatedGroupingSelectExpression);
      _joinedGroupingTable = new SqlTable (
          new ResolvedJoinedGroupingTableInfo (
              "q0",
              _joinedGroupingSubStatement,
              _associatedGroupingSelectExpression));
      _selectProjection = new AggregationExpression (
          typeof (int), new SqlTableReferenceExpression (_joinedGroupingTable), AggregationModifier.Min);

      _simplifiableSqlStatement = new SqlStatement (
          _dataInfo,
          _selectProjection,
          new[] { _joinedGroupingTable },
          null,
          null,
          new Ordering[0],
          null,
          false,
          Expression.Constant (0),
          Expression.Constant (0));
    }
    
    [Test]
    public void IsSimplifiableGroupAggregate_True_WithReferenceToJoinGroupInAggregationExpression ()
    {
      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (_simplifiableSqlStatement), Is.True);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_True_WithNamedExpressionInAggregationExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableSqlStatement)
      {
        SelectProjection = new AggregationExpression (
          typeof (int), new NamedExpression ("value", new SqlTableReferenceExpression (_joinedGroupingTable)), AggregationModifier.Min)
      }.GetSqlStatement();
      
      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.True);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_True_WithNonReferenceExpressionInAggregationExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableSqlStatement)
      {
        SelectProjection = new AggregationExpression (typeof (int), Expression.Constant (0), AggregationModifier.Min)
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.True);
    }

    [Test]
    [Ignore ("TODO 2993")]
    public void IsSimplifiableGroupAggregate_False_WithInvalidReferenceExpressionInAggregationExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableSqlStatement)
      {
        SelectProjection = new AggregationExpression (
            typeof (int), 
            new SqlTableReferenceExpression (SqlStatementModelObjectMother.CreateSqlTable()), 
            AggregationModifier.Min)
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_NoAggregationExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableSqlStatement)
      {
        SelectProjection = new SqlTableReferenceExpression (_joinedGroupingTable)
      }.GetSqlStatement ();
      
      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_WhereExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableSqlStatement)
      {
        WhereCondition = Expression.Constant (true)
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_OrderingExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableSqlStatement)
      {
        Orderings = { new Ordering(Expression.Constant("order"), OrderingDirection.Asc) }
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_GroupByExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableSqlStatement)
      {
        GroupByExpression = Expression.Constant(0)
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_TooManySqlTables ()
    {
      var sqlStatementBuilder = new SqlStatementBuilder (_simplifiableSqlStatement);
      sqlStatementBuilder.SqlTables.Add (SqlStatementModelObjectMother.CreateSqlTable());
      var sqlStatement = sqlStatementBuilder.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_NoResolvedJoinedGroupingTableInfo ()
    {
      var sqlStatementBuilder = new SqlStatementBuilder (_simplifiableSqlStatement);
      sqlStatementBuilder.SqlTables.Clear();
      sqlStatementBuilder.SqlTables.Add (SqlStatementModelObjectMother.CreateSqlTable (new ResolvedSimpleTableInfo (typeof (int), "table", "t0")));
      var sqlStatement = sqlStatementBuilder.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_TopExpression ()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableSqlStatement)
      {
        TopExpression = Expression.Constant (0)
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }

    [Test]
    public void IsSimplifiableGroupAggregate_False_DistinctQuery()
    {
      var sqlStatement = new SqlStatementBuilder (_simplifiableSqlStatement)
      {
        IsDistinctQuery = true
      }.GetSqlStatement ();

      Assert.That (GroupAggregateSimplifier.IsSimplifiableGroupAggregate (sqlStatement), Is.False);
    }
  }
}