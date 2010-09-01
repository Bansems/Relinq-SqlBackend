﻿'Microsoft Public License (Ms-PL)

'This license governs use of the accompanying software. If you use the software, you
'accept this license. If you do not accept the license, do not use the software.

'1. Definitions
'The terms "reproduce," "reproduction," "derivative works," and "distribution" have the
'same meaning here as under U.S. copyright law.
'A "contribution" is the original software, or any additions or changes to the software.
'A "contributor" is any person that distributes its contribution under this license.
'"Licensed patents" are a contributor's patent claims that read directly on its contribution.

'2. Grant of Rights
'(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
'each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, 
'prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
'(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
'each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, 
'sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

'3. Conditions and Limitations
'(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
'(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from 
'such contributor to the software ends automatically.
'(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present 
'in the software.
'(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of 
'this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a 
'license that complies with this license.
'(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. 
'You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws,
'the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.

Option Infer On
Option Strict On

Imports NUnit.Framework

Imports System.Linq.Expressions
Imports Remotion.Data.Linq.IntegrationTests
Imports Remotion.Data.Linq.IntegrationTests.TestDomain.Northwind
Imports System.Reflection

Namespace LinqSamples101
  <TestFixture()>
  Public Class AdvancedTests
    Inherits TestBase

    'This sample builds a query dynamically to return the contact name of each customer.
    <Test()>
    Public Sub LinqToSqlAdvanced01()
      Dim param = Expression.Parameter(GetType(Customer), "c")
      Dim selector = Expression.Property(param, GetType(Customer).GetProperty("ContactName"))
      Dim pred = Expression.Lambda(selector, param)

      Dim custs = db.Customers
      Dim _
        expr = _
          Expression.Call(GetType(Queryable), "Select", New Type() {GetType(Customer), GetType(String)}, _
                           Expression.Constant(custs), pred)
      Dim query = custs.AsQueryable().Provider.CreateQuery(Of String)(expr)

      TestExecutor.Execute(query, MethodBase.GetCurrentMethod())
    End Sub

    'This sample builds a query dynamically to filter for Customers in London.
    <Test()>
    Public Sub LinqToSqlAdvanced02()

      Dim custs = db.Customers
      Dim param = Expression.Parameter(GetType(Customer), "c")
      Dim right = Expression.Constant("London")
      Dim left = Expression.Property(param, GetType(Customer).GetProperty("City"))
      Dim filter = Expression.Equal(left, right)
      Dim pred = Expression.Lambda(filter, param)

      Dim _
        expr = _
          Expression.Call(GetType(Queryable), "Where", New Type() {GetType(Customer)}, Expression.Constant(custs), _
                           pred)
      Dim query = DB.Customers.AsQueryable().Provider.CreateQuery(Of Customer)(expr)

      TestExecutor.Execute(query, MethodBase.GetCurrentMethod())
    End Sub


    'This sample builds a query dynamically to filter for Customers in London and order them by ContactName.
    <Test()>
    Public Sub LinqToSqlAdvanced03()

      Dim param = Expression.Parameter(GetType(Customer), "c")

      Dim left = Expression.Property(param, GetType(Customer).GetProperty("City"))
      Dim right = Expression.Constant("London")
      Dim filter = Expression.Equal(left, right)
      Dim pred = Expression.Lambda(filter, param)

      Dim custs As IQueryable = db.Customers

      Dim expr = Expression.Call(GetType(Queryable), "Where", _
                                  New Type() {GetType(Customer)}, _
                                  Expression.Constant(custs), pred)

      expr = Expression.Call(GetType(Queryable), "OrderBy", _
                              New Type() {GetType(Customer), GetType(String)}, _
                              custs.Expression, _
                              Expression.Lambda(Expression.Property(param, "ContactName"), param))


      Dim query = db.Customers.AsQueryable().Provider.CreateQuery(Of Customer)(expr)

      TestExecutor.Execute(query, MethodBase.GetCurrentMethod())
    End Sub

    'This sample dynamically builds a Union to return a sequence of all countries where either a customer or an employee live.
    <Test()>
    <Ignore("Bug or missing feature in Relinq. System.NotSupportedException : The handler type ResultOperatorBase is not supported by this registry")>
    Public Sub LinqToSqlAdvanced04()

      Dim custs = DB.Customers
      Dim param1 = Expression.Parameter(GetType(Customer), "c")
      Dim left1 = Expression.Property(param1, GetType(Customer).GetProperty("City"))
      Dim pred1 = Expression.Lambda(left1, param1)

      Dim employees = DB.Employees
      Dim param2 = Expression.Parameter(GetType(Employee), "e")
      Dim left2 = Expression.Property(param2, GetType(Employee).GetProperty("City"))
      Dim pred2 = Expression.Lambda(left2, param2)

      Dim _
        expr1 = _
          Expression.Call(GetType(Queryable), "Select", New Type() {GetType(Customer), GetType(String)}, _
                           Expression.Constant(custs), pred1)
      Dim _
        expr2 = _
          Expression.Call(GetType(Queryable), "Select", New Type() {GetType(Employee), GetType(String)}, _
                           Expression.Constant(employees), pred2)

      Dim custQuery1 = DB.Customers.AsQueryable().Provider.CreateQuery(Of String)(expr1)
      Dim empQuery1 = DB.Employees.AsQueryable().Provider.CreateQuery(Of String)(expr2)

      Dim finalQuery = custQuery1.Union(empQuery1)

      TestExecutor.Execute(finalQuery, MethodBase.GetCurrentMethod())
    End Sub

    'This sample uses OrderByDescending and Take to return the discontinued products of the top 10 most expensive products.
    <Test()>
    Public Sub LinqToSqlAdvanced06()
      Dim prods = From prod In DB.Products.OrderByDescending(Function(p) p.UnitPrice) _
            Take 10 _
            Where prod.Discontinued

      TestExecutor.Execute(prods, MethodBase.GetCurrentMethod())
    End Sub
  End Class
End Namespace
