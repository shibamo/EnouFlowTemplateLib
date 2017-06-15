using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


using EnouFlowOrgMgmtLib;

namespace EnouFlowTemplateLib
{
  public class FlowTemplateDef
  {
    public BasicInfo basicInfo { get; set; }
    public AdvancedInfo advancedInfo { get; set; }
    public Dictionary<string, object> customData { get; set; }
    public ActivityNodes activityNodes { get; set; }
    public ActivityConnections activityConnections { get; set; }
  }

  public class BasicInfo
  {
    public string name { get; set; }
    public string displayName { get; set; }
    public string version { get; set; }
    public string code { get; set; }
    public string guid { get; set; }
    public string desc { get; set; }
    public FlowTemplateCreator creator { get; set; }
    public DateTime? createTime { get; set; }
    public DateTime? lastUpdateTime { get; set; }
    public int? isPopular { get; set; }

  }

  public class FlowTemplateCreator
  {
    public string name { get; set; }
    public string guid { get; set; }
  }

  public class AdvancedInfo
  {
    public int? arbitraryJumpAllowed { get; set; }
    public string managers { get; set; } /// TODO: 需要更新为带guid的人员信息
  }

  public class ActivityNodes
  {
    public List<ActivityNode> nodes { get; set; }
  }

  public class ActivityNode
  {
    public string name { get; set; }
    public string type { get; set; }
    public string guid { get; set; }
    public int[] size { get; set; }
    public int[] position { get; set; }
    public Dictionary<string, string> customData { get; set; }
    public string linkForm { get; set; }
    public List<string> beforeActions { get; set; }
    public List<string> afterActions { get; set; }
    public List<Paticipant> roles { get; set; }
    public List<ConditionRule> autoRules { get; set; }
  }

  public class ActivityConnections
  {
    public List<ActivityConnection> connections { get; set; }
  }

  public class ActivityConnection
  {
    public string guid { get; set; }
    public string name { get; set; }
    public string fromGuid { get; set; }
    public string toGuid { get; set; }
  }

  public class Paticipant
  {
    public string PaticipantType { get; set; } //"role" or "user" or "dynamic"
    public PaticipantDigest PaticipantObj { get; set; }

    public Paticipant(string paticipantType, PaticipantDigest paticipantObj)
    {
      this.PaticipantType = paticipantType;
      this.PaticipantObj = paticipantObj;
    }
  }

  public class PaticipantDigest
  {
    public string name { get; set; }
    public string guid { get; set; }
    public int? userId { get; set; }
    public int? roleId { get; set; }
    public int? flowDynamicUserId { get; set; }

    public PaticipantDigest(string name, string guid, 
      int? userId, int? roleId, int? flowDynamicUserId)
    {
      this.name = name;
      this.guid = guid;
      this.userId = userId;
      this.roleId = roleId;
      this.flowDynamicUserId = flowDynamicUserId;
    }
  }

  public class ConditionRule
  {
    public string name { get; set; }
    public string guid { get; set; }
    public bool isDefault { get; set; }
    public string code { get; set; }
    public string connectionGuid { get; set; }
    public List<Paticipant> paticipants { get; set; }
  }

  public static class ActivityTypeString
  {
    public const string standard_Start = "st-start";
    public const string standard_End = "st-end";
    public const string standard_SingleHuman = "st-singleHumanActivity";
    public const string standard_MultiHuman = "st-multiHumanActivity";
    public const string standard_Auto = "st-autoActivity";

  }
}
