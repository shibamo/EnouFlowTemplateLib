using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using EnouFlowTemplateLib.Tests.Attributes;
using NSubstitute;

using EnouFlowOrgMgmtLib;

namespace EnouFlowTemplateLib.Tests.Integration
{
  [TestFixture]
  public class Template_TestsI
  {
    private EnouFlowTemplateDbContext db = null;

    [SetUp]
    public void Setup()
    {
      db = new EnouFlowTemplateDbContext();
    }

    [TearDown]
    public void TearDown()
    {
      db.Dispose();
    }
  }
}
