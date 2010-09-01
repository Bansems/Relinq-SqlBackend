﻿//Microsoft Public License (Ms-PL)

//This license governs use of the accompanying software. If you use the software, you
//accept this license. If you do not accept the license, do not use the software.

//1. Definitions
//The terms "reproduce," "reproduction," "derivative works," and "distribution" have the
//same meaning here as under U.S. copyright law.
//A "contribution" is the original software, or any additions or changes to the software.
//A "contributor" is any person that distributes its contribution under this license.
//"Licensed patents" are a contributor's patent claims that read directly on its contribution.

//2. Grant of Rights
//(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
//each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, 
//prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
//(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
//each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, 
//sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

//3. Conditions and Limitations
//(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
//(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from 
//such contributor to the software ends automatically.
//(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present 
//in the software.
//(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of 
//this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a 
//license that complies with this license.
//(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. 
//You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws,
//the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.Linq.IntegrationTests.TestDomain.Northwind;

namespace Remotion.Data.Linq.IntegrationTests.CSharp.LinqSamples101
{
  public class InheritanceTests:TestBase
  {
    /// <summary>
    /// This sample returns all contacts where the city is London.
    /// </summary>
    [Test]
    public void LinqToSqlInheritance01 ()
    {
      var cons = from c in DB.Contacts
                 select c;

      TestExecutor.Execute (cons, MethodBase.GetCurrentMethod ());

    }

    /// <summary>
    /// This sample uses OfType to return all customer contacts.
    /// </summary>
    [Test]
    public void LinqToSqlInheritance02 ()
    {
      var cons = from c in DB.Contacts.OfType<CustomerContact> ()
                 select c;

      TestExecutor.Execute (cons, MethodBase.GetCurrentMethod ());
    }

    /// <summary>
    /// This sample uses IS to return all shipper contacts.
    /// </summary>
    [Test]
    public void LinqToSqlInheritance03 ()
    {
      var cons = from c in DB.Contacts
                 where c is ShipperContact
                 select c;

      TestExecutor.Execute (cons, MethodBase.GetCurrentMethod ());
    }

    /// <summary>
    /// This sample uses AS to return FullContact or null.
    /// </summary>
    [Test]
    [Ignore ("Bug or missing feature in Relinq - Selecting as FullContact is not support")]
    public void LinqToSqlInheritance04 ()
    {
      var cons = from c in DB.Contacts
                 select c as FullContact;

      TestExecutor.Execute (cons, MethodBase.GetCurrentMethod ());
    }

    /// <summary>
    /// This sample uses a cast to retrieve customer contacts who live in London.
    /// </summary>
    [Test]
    public void LinqToSqlInheritance05 ()
    {
      var cons = from c in DB.Contacts
                 where c.ContactType == "Customer" && ((CustomerContact) c).City == "London"
                 select c;

      TestExecutor.Execute (cons, MethodBase.GetCurrentMethod ());
    }
  }
}