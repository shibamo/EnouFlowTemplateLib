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
  public class FlowTemplateDefHelperTestsI
  {
    private EnouFlowTemplateDbContext db = null;

    [SetUp]
    public void Setup()
    {
      db = new EnouFlowTemplateDbContext();
    }

    [TestCase("Integration_Test_XXXYYYZZZ")]
    [TestCase(null)]
    [Category("Integration")]
    [Rollback]
    public void getPaticipantFromGuid_invalidGuid_returnNull(string guid)
    {
      Paticipant result = FlowTemplateDefHelper.getPaticipantFromGuid(guid);

      Assert.Null(result);
    }

    [Test]
    [Category("Integration")]
    [Rollback]
    public void getPaticipantFromGuid_validUserGuid_returnUserPaticipant()
    {
      using (EnouFlowOrgMgmtContext dbOrg = new EnouFlowOrgMgmtContext())
      {
        UserHelper userHelper = new UserHelper(dbOrg);
        var user = userHelper.createObject();
        user.name = "Integration_Test_XXXYYYZZZ";
        userHelper.saveCreatedObject(user);

        Paticipant result = FlowTemplateDefHelper.getPaticipantFromGuid(user.guid);

        Assert.NotNull(result);
        Assert.AreEqual(result.PaticipantType, "user");
        Assert.AreEqual(result.PaticipantObj.guid, user.guid);
      }
    }

    [Test]
    [Category("Integration")]
    [Rollback]
    public void getPaticipantFromGuid_validRoleGuid_returnRolePaticipant()
    {
      using (EnouFlowOrgMgmtContext dbOrg = new EnouFlowOrgMgmtContext())
      {
        RoleHelper roleHelper = new RoleHelper(dbOrg);
        var role = roleHelper.createObject();
        role.name = "Integration_Test_XXXYYYZZZ";
        roleHelper.saveCreatedObject(role);

        Paticipant result = FlowTemplateDefHelper.getPaticipantFromGuid(role.guid);

        Assert.NotNull(result);
        Assert.AreEqual(result.PaticipantType, "role");
        Assert.AreEqual(result.PaticipantObj.guid, role.guid);
      }
    }

    [Test]
    [Category("Integration")]
    [Rollback]
    public void getPaticipantFromGuid_validDynamicUserGuid_returnDynamicUserPaticipant()
    {
      FlowDynamicUser flowDynamicUser = db.flowDynamicUsers.Create();
      flowDynamicUser.name = "Integration_Test_XXXYYYZZZ";
      db.flowDynamicUsers.Add(flowDynamicUser);
      db.SaveChanges();

      Paticipant result = FlowTemplateDefHelper.getPaticipantFromGuid(flowDynamicUser.guid);

      Assert.NotNull(result);
      Assert.AreEqual(result.PaticipantType, "dynamic");
      Assert.AreEqual(result.PaticipantObj.guid, flowDynamicUser.guid);
    }

    [Test]
    [Category("Integration")]
    [Rollback]
    public void FlowTemplateDefHelper_repeatSameJson_returnSameObj()
    {
      FlowTemplateDefHelper FlowTemplateDefHelper1 = new FlowTemplateDefHelper("{'AmountTotal': 49999}");
      FlowTemplateDefHelper FlowTemplateDefHelper0 = new FlowTemplateDefHelper("{'AmountTotal': 500000}");

      FlowTemplateDefHelper FlowTemplateDefHelper2 = new FlowTemplateDefHelper("{'AmountTotal': 49999}");

      Assert.AreSame(FlowTemplateDefHelper1.def, FlowTemplateDefHelper2.def);
      Assert.AreNotSame(FlowTemplateDefHelper1.def, FlowTemplateDefHelper0.def);
    }

    [Test]
    [Category("Integration")]
    [Rollback]
    public void FlowTemplateDefHelper_invalidJson_throws()
    {
      Assert.Catch(() =>{ new FlowTemplateDefHelper("Integration_Test_XXXYYYZZZ"); });
    }

    [TearDown]
    public void TearDown()
    {
      db.Dispose();
    }
  }
}
