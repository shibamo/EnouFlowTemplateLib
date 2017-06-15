﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace EnouFlowTemplateLib.Tests.Attributes
{
  /// <summary>
  /// Rollback Attribute wraps test execution into a transaction and cancels the transaction once the test is finished.
  /// You can use this attribute on single test methods or test classes/suites
  /// </summary>
  public class RollbackAttribute : Attribute, ITestAction
  {
    private TransactionScope transaction;

    public void BeforeTest(ITest testDetails)
    {
      transaction = new TransactionScope();
    }

    public void AfterTest(ITest testDetails)
    {
      transaction.Dispose();
    }

    public ActionTargets Targets
    {
      get { return ActionTargets.Test; }
    }
  }
}
