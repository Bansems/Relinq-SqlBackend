﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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

namespace Remotion.Data.Linq.IntegrationTests.Utilities
{ 
  /// <summary>
  /// Supports better comparability for the output of two results from <see cref="TestResultSerializer"/>
  /// </summary>
  public class TestResultChecker
  {
    public static bool Check (string expected, string actual)
    {
      bool returnValue = expected.Equals(actual);
      if (!returnValue)
      {
        var expectedLines = expected.Split (Environment.NewLine.ToCharArray());
        var actualLines = actual.Split (Environment.NewLine.ToCharArray ());
        
        if (expectedLines.Length == actualLines.Length)
          CheckInLinesDiff (expectedLines, actualLines);
        else
          CheckCompleteDiff (expectedLines, actualLines);
      }
      return returnValue;
    }

    private static void CheckInLinesDiff (string[] expected, string[] actual)
    {
      // TODO: find lines with differencess;
    }

    private static void CheckCompleteDiff (string[] expected, string[] actual)
    {
      // TODO: find missing /additional lines and differences inside
    }
  }
}