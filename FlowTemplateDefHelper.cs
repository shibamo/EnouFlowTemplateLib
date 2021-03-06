﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

using EnouFlowOrgMgmtLib;
using EnouFlowTemplateLib;

namespace EnouFlowTemplateLib
{
  public class FlowTemplateDefHelper
  {
    public FlowTemplateDef def { get; set; }

    internal static Dictionary<string, FlowTemplateDef> _defs = new Dictionary<string, FlowTemplateDef>();

    public FlowTemplateDefHelper(string json)
    {
      Contract.Requires<DataLogicException>(json != null, "流程模板Json不能为空");

      if (!_defs.ContainsKey(json))
      {
        _defs.Add(json, JsonHelper.DeserializeJsonToObject<FlowTemplateDef>(
          json));
      }
      def = _defs[json];
    }

    private FlowTemplateDefHelper() { }

    public Tuple<ActivityNode, ActivityNode, ActivityConnection>
      getNodesOfConnection(string connGuid)
    {
      var conn = def.activityConnections.connections.Find(
         con => con.guid == connGuid);
      return new Tuple<ActivityNode, ActivityNode, ActivityConnection>(
        def.activityNodes.nodes.Find(node => node.guid == conn.fromGuid),
        def.activityNodes.nodes.Find(node => node.guid == conn.toGuid),
        conn);
    }

    public ActivityNode getNodeFromGuid(string guid)
    {
      return def.activityNodes.nodes.Find(node => node.guid == guid);
    }

    public List<ActivityNode> getNodesOfStartType()
    {
      return def.activityNodes.nodes.Where(node =>
        node.type == ActivityTypeString.standard_Start).ToList();
    }

    public List<ActivityConnection> getOutboundConnectionsOfNode(ActivityNode node)
    {
      return def.activityConnections.connections.Where(
        conn => conn.fromGuid == node.guid).ToList();
    }

    public static Paticipant getPaticipantFromGuid(string guid)
    {
      using (EnouFlowOrgMgmtContext db = new EnouFlowOrgMgmtContext())
      {
        #region 如果为用户
        var user = new UserHelper(db).getObject(guid);
        if (user != null)
        {
          return new Paticipant("user",
            new PaticipantDigest(
              user.name,
              user.guid,
              user.userId,
              null,
              null
            )
          );
        }
        #endregion

        #region 如果为角色
        var role = new RoleHelper(db).getObject(guid);
        if (role != null)
        {
          return new Paticipant("role",
            new PaticipantDigest(
              role.name,
              role.guid,
              null,
              role.roleId,
              null
            )
          );
        }
        #endregion

        #region 如果为流程动态用户
        using (var flowTemplateDb = new EnouFlowTemplateDbContext())
        {
          var flowDynamicUser = flowTemplateDb.flowDynamicUsers.Where(
            obj => obj.guid == guid).FirstOrDefault();
          if (flowDynamicUser != null)
          {
            return new Paticipant("dynamic",
              new PaticipantDigest(
                flowDynamicUser.name,
                flowDynamicUser.guid,
                null,
                null,
                flowDynamicUser.flowDynamicUserId
              )
            );
          }
        }
        #endregion
      }
      return null;
    }
  }
}
