// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.Linq.SqlGeneration.SqlServer
{
  [TestFixture]
  public class BinaryConditionBuilderTest
  {
    [Test]
    public void BuildBinaryConditionPart_BinaryConditions ()
    {
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.Equal),
          "(@1 = @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.NotEqual),
          "(@1 <> @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.LessThan),
          "(@1 < @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.GreaterThan),
          "(@1 > @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.LessThanOrEqual),
          "(@1 <= @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.GreaterThanOrEqual),
          "(@1 >= @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.Like),
          "(@1 LIKE @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.Add),
          "(@1 + @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.Divide),
          "(@1 / @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.Modulo),
          "(@1 % @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.Multiply),
          "(@1 * @2)");
      CheckBuildBinaryConditionPart_Constants (
          new BinaryCondition (new Constant ("foo"), new Constant ("foo"), BinaryCondition.ConditionKind.Subtract),
          "(@1 - @2)");
    }

    [Test]
    public void BuildBinaryConditionPart_BinaryConditions_WithColumns ()
    {
      var c1 = new Column (new Table ("a", "b"), "foo");
      var c2 = new Column (new Table ("c", "d"), "bar");
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, c2, BinaryCondition.ConditionKind.Equal),
          "(([b].[foo] IS NULL AND [d].[bar] IS NULL) OR [b].[foo] = [d].[bar])");
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, c2, BinaryCondition.ConditionKind.NotEqual),
          "(([b].[foo] IS NULL AND [d].[bar] IS NOT NULL) OR ([b].[foo] IS NOT NULL AND [d].[bar] IS NULL) OR [b].[foo] <> [d].[bar])");
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, c2, BinaryCondition.ConditionKind.LessThan),
          "([b].[foo] < [d].[bar])");
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, c2, BinaryCondition.ConditionKind.GreaterThan),
          "([b].[foo] > [d].[bar])");
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, c2, BinaryCondition.ConditionKind.LessThanOrEqual),
          "(([b].[foo] IS NULL AND [d].[bar] IS NULL) OR [b].[foo] <= [d].[bar])");
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, c2, BinaryCondition.ConditionKind.GreaterThanOrEqual),
          "(([b].[foo] IS NULL AND [d].[bar] IS NULL) OR [b].[foo] >= [d].[bar])");
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, c2, BinaryCondition.ConditionKind.Like),
          "([b].[foo] LIKE [d].[bar])");
    }

    [Test]
    public void BuildBinaryConditionPart_BinaryConditions_WithColumn_LeftSide ()
    {
      var c1 = new Column (new Table ("a", "b"), "foo");
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, new Constant ("const"), BinaryCondition.ConditionKind.Equal),
          "([b].[foo] = @1)", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, new Constant ("const"), BinaryCondition.ConditionKind.NotEqual),
          "([b].[foo] IS NULL OR [b].[foo] <> @1)", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, new Constant ("const"), BinaryCondition.ConditionKind.LessThan),
          "([b].[foo] < @1)", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, new Constant ("const"), BinaryCondition.ConditionKind.GreaterThan),
          "([b].[foo] > @1)", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, new Constant ("const"), BinaryCondition.ConditionKind.LessThanOrEqual),
          "([b].[foo] <= @1)", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, new Constant ("const"), BinaryCondition.ConditionKind.GreaterThanOrEqual),
          "([b].[foo] >= @1)", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (c1, new Constant ("const"), BinaryCondition.ConditionKind.Like),
          "([b].[foo] LIKE @1)", new CommandParameter ("@1", "const"));
    }

    [Test]
    public void BuildBinaryConditionPart_BinaryConditions_WithColumn_RightSide ()
    {
      var c1 = new Column (new Table ("a", "b"), "foo");
      CheckBuildBinaryConditionPart (
          new BinaryCondition (new Constant ("const"), c1, BinaryCondition.ConditionKind.Equal),
          "(@1 = [b].[foo])", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (new Constant ("const"), c1, BinaryCondition.ConditionKind.NotEqual),
          "([b].[foo] IS NULL OR @1 <> [b].[foo])", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (new Constant ("const"), c1, BinaryCondition.ConditionKind.LessThan),
          "(@1 < [b].[foo])", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (new Constant ("const"), c1, BinaryCondition.ConditionKind.GreaterThan),
          "(@1 > [b].[foo])", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (new Constant ("const"), c1, BinaryCondition.ConditionKind.LessThanOrEqual),
          "(@1 <= [b].[foo])", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (new Constant ("const"), c1, BinaryCondition.ConditionKind.GreaterThanOrEqual),
          "(@1 >= [b].[foo])", new CommandParameter ("@1", "const"));
      CheckBuildBinaryConditionPart (
          new BinaryCondition (new Constant ("const"), c1, BinaryCondition.ConditionKind.Like),
          "(@1 LIKE [b].[foo])", new CommandParameter ("@1", "const"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The binary condition kind 2147483647 is not supported.")]
    public void BuildBinaryConditionPart_InvalidBinaryConditionKind ()
    {
      CheckBuildBinaryConditionPart (new BinaryCondition (new Constant ("foo"), new Constant ("foo"), (BinaryCondition.ConditionKind) int.MaxValue), null);
    }

    [Test]
    public void BuildBinaryConditionPart_BinaryConditionLeftNull ()
    {
      var binaryCondition = new BinaryCondition (new Constant (null), new Constant ("foo"), BinaryCondition.ConditionKind.Equal);
      CheckBuildBinaryConditionPart (binaryCondition, "@1 IS NULL", new CommandParameter ("@1", "foo"));
    }

    [Test]
    public void BuildBinaryConditionPart_BinaryConditionRightNull ()
    {
      var binaryCondition = new BinaryCondition (new Constant ("foo"), new Constant (null), BinaryCondition.ConditionKind.Equal);
      CheckBuildBinaryConditionPart (binaryCondition, "@1 IS NULL", new CommandParameter ("@1", "foo"));
    }

    [Test]
    public void BuildBinaryConditionPart_BinaryConditionIsNotNull ()
    {
      var binaryCondition = new BinaryCondition (new Constant (null), new Constant ("foo"), BinaryCondition.ConditionKind.NotEqual);
      CheckBuildBinaryConditionPart (binaryCondition, "@1 IS NOT NULL", new CommandParameter ("@1", "foo"));
    }

    [Test]
    public void BuildBinaryConditionPart_BinaryConditionNullIsNotNull ()
    {
      var binaryCondition = new BinaryCondition (new Constant (null), new Constant (null), BinaryCondition.ConditionKind.NotEqual);
      CheckBuildBinaryConditionPart (binaryCondition, "NULL IS NOT NULL");
    }

    [Test]
    [ExpectedException (typeof (NotImplementedException), ExpectedMessage = "The method or operation is not implemented.")]
    public void BuildBinaryConditionPart_InvalidValue ()
    {
      var binaryCondition = new BinaryCondition (new PseudoValue(), new Constant (null), BinaryCondition.ConditionKind.NotEqual);
      CheckBuildBinaryConditionPart (binaryCondition, null);
    }
    
    [Test]
    public void CreateSqlGeneratorForSubQuery ()
    {
      var subQuery = new SubQuery (ExpressionHelper.CreateQueryModel (), ParseMode.SubQueryInWhere, null);
      var commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance, new MethodCallSqlGeneratorRegistry());
      var conditionBuilder = new BinaryConditionBuilder (commandBuilder);
      var subQueryGenerator = (InlineSqlServerGenerator) PrivateInvoke.InvokeNonPublicMethod (conditionBuilder, "CreateSqlGeneratorForSubQuery",
          subQuery, StubDatabaseInfo.Instance, commandBuilder);
      Assert.AreSame (StubDatabaseInfo.Instance, subQueryGenerator.DatabaseInfo);
      Assert.AreEqual (ParseMode.SubQueryInWhere, subQueryGenerator.ParseMode);
    }

    [Test]
    public void BuildBinaryCondition_ContainsFulltext ()
    {
      var column = new Column (new Table ("Student", "s"), "First");
      var constant = new Constant("Test");
      var binaryCondition = new BinaryCondition (column, constant, BinaryCondition.ConditionKind.ContainsFulltext);

      var commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance, new MethodCallSqlGeneratorRegistry());
      var binaryConditionBuilder = new BinaryConditionBuilder (commandBuilder);

      binaryConditionBuilder.BuildBinaryConditionPart (binaryCondition);

      Assert.AreEqual("CONTAINS ([s].[First],@1)", commandBuilder.GetCommandText());
      Assert.That (commandBuilder.GetCommandParameters (), Is.EqualTo (new object[] { new CommandParameter ("@1", "Test") }));
    }

    [Test]
    public void BuildBinaryCondition_Contains_OnCollection ()
    {
      var column = new Column (new Table ("Student", "s"), "First");
      var collection = new [] {"Test1", "Test2"};
      var constantCollection = new Constant(collection);
      var binaryCondition = new BinaryCondition(constantCollection, column, BinaryCondition.ConditionKind.Contains);

      var commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance, new MethodCallSqlGeneratorRegistry ());
      var binaryConditionBuilder = new BinaryConditionBuilder (commandBuilder);

      binaryConditionBuilder.BuildBinaryConditionPart (binaryCondition);

      Assert.AreEqual ("[s].[First] IN (@1, @2)", commandBuilder.GetCommandText ());
      Assert.That (commandBuilder.GetCommandParameters (), 
        Is.EqualTo (new object[] { new CommandParameter ("@1", "Test1"), new CommandParameter ("@2", "Test2") }));
    }

    [Test]
    public void BuildBinaryCondition_Contains_OnEmptyCollection ()
    {
      var column = new Column (new Table ("Student", "s"), "First");
      var collection = new string[] {};
      var constantCollection = new Constant (collection);
      var binaryCondition = new BinaryCondition (constantCollection, column, BinaryCondition.ConditionKind.Contains);

      var commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance, new MethodCallSqlGeneratorRegistry ());
      var binaryConditionBuilder = new BinaryConditionBuilder (commandBuilder);

      binaryConditionBuilder.BuildBinaryConditionPart (binaryCondition);

      Assert.AreEqual ("(1<>1)", commandBuilder.GetCommandText ());
    }

    [Test]
    public void BuildBinaryCondition_MethodCall ()
    {
      var methodInfo = typeof (string).GetMethod ("ToUpper", new Type[] { });
      var column = new Column (new Table ("Student", "s"), "FirstColumn");
      var methodCall = new MethodCall (methodInfo, column, new List<IEvaluation> ());

      var binaryCondition = new BinaryCondition(methodCall,new Constant("Test"),BinaryCondition.ConditionKind.Equal);

      var sqlServerGenerator = new SqlServerGenerator (StubDatabaseInfo.Instance);
      var commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance, sqlServerGenerator.MethodCallRegistry);
      var binaryConditionBuilder = new BinaryConditionBuilder (commandBuilder);

      binaryConditionBuilder.BuildBinaryConditionPart (binaryCondition);

      Assert.AreEqual ("(UPPER([s].[FirstColumn]) = @1)", commandBuilder.GetCommandText ());
      Assert.That (commandBuilder.GetCommandParameters (), Is.EqualTo (new object[] { new CommandParameter ("@1", "Test") }));
    }

    public class PseudoValue : IValue
    {
      public void Accept (IEvaluationVisitor visitor)
      {
        throw new NotImplementedException();
      }
    }


    private static void CheckBuildBinaryConditionPart_Constants (BinaryCondition binaryCondition, string expectedString)
    {
      CheckBuildBinaryConditionPart (binaryCondition, expectedString,
          new CommandParameter ("@1", "foo"), new CommandParameter ("@2", "foo"));
    }

    private static void CheckBuildBinaryConditionPart (BinaryCondition condition, string expectedString,
       params CommandParameter[] expectedParameters)
    {
      var commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance, new MethodCallSqlGeneratorRegistry());
      var binaryConditionBuilder = new BinaryConditionBuilder (commandBuilder);

      binaryConditionBuilder.BuildBinaryConditionPart (condition);

      Assert.AreEqual (expectedString, commandBuilder.GetCommandText());
      Assert.That (commandBuilder.GetCommandParameters(), Is.EqualTo (expectedParameters));
    }
  }
}
