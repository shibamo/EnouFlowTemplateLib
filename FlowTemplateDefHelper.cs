using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnouFlowOrgMgmtLib;

namespace EnouFlowTemplateLib
{
  public class FlowTemplateDefHelper
  {
    public FlowTemplateDef def { get; set; }

    public FlowTemplateDefHelper(string json)
    {
      def = JsonHelper.DeserializeJsonToObject<FlowTemplateDef>(
          json);
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
      return def.activityNodes.nodes.Where(
        node => node.type == ActivityTypeString.standard_Start)
        .ToList();
    }

    public List<ActivityConnection> getOutboundConnectionsOfNode(ActivityNode node)
    {
      return def.activityConnections.connections.Where(
        conn => conn.fromGuid == node.guid).ToList();
    }

    public static List<UserDTO> getUserDTOsFromPaticipantList(
      List<Paticipant> paticipants)
    {
      List<UserDTO> result = new List<UserDTO>();

      if (paticipants?.Count() > 0)
      {
        paticipants.ForEach(p =>
        {
          if (p != null) { 
            result.AddRange(p.resolveToUserDTOs());
          }
        });
        result = result.Distinct().ToList();
      }

      return result;
    }

    public static Paticipant getPaticipantFromGuid(string guid)
    {
      using (EnouFlowOrgMgmtContext db = new EnouFlowOrgMgmtContext())
      {
        var user = OrgMgmtDBHelper.getUser(guid, db);
        if (user != null)
        {
          return new Paticipant("user",
            new PaticipantDigest(
              user.name,
              user.guid,
              user.userId,
              null
            )
          );
        }
        else
        {
          var role = OrgMgmtDBHelper.getRole(guid, db);
          if (role != null)
          { 
            return new Paticipant("role",
              new PaticipantDigest(
                role.name,
                role.guid,
                null,
                role.roleId
              )
            );
          }
        }
      }
      return null;
    }
  }
}
