using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EnouFlowTemplateLib
{
  public static partial class FlowTemplateDBHelper
  {
    #region FlowTemplate
    // Create FlowTemplate
    public static FlowTemplate createFlowTemplate()
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        return db.flowTemplates.Create();
      }
    }

    public static Tuple<bool, FlowTemplate, List<string>> createFlowTemplate(
      string guid, string name, string displayName, 
      string version, string code, string flowTemplateJson, 
      bool isPublished = false,bool isValidated=false)
    {
      List<string> errors = new List<string>();
      var basicCheckResult = validateFlowTemplateBasic(
        guid, name, displayName, version, code, flowTemplateJson, 
        isPublished, isValidated);
      if (basicCheckResult.Item1==false)
      {
        return new Tuple<bool, FlowTemplate, List<string>>(
          false, null, basicCheckResult.Item2);
      }

      bool result = true;

      using (var db = new EnouFlowTemplateDbContext())
      {
        #region 创建流程模板记录的检查
        if (db.flowTemplates.ToList().Exists(
          obj => obj.code == code &&
          obj.version == version))
        {
          result = false;
          errors.Add(string.Format(
            @"不能多次创建相同代码'{0}'相同版本'{1}'的流程模板;", 
            code,version));
        }

        if (db.flowTemplates.ToList().Exists(obj => obj.guid == guid))
        {
          result = false;
          errors.Add(string.Format(
            @"不能多次创建相同guid'{0}'的流程模板;", guid));
        }
        if (!result) { 
          return new Tuple<bool, FlowTemplate, List<string>>
            (false, null, errors);
        }
        #endregion

        var flowTemplate = db.flowTemplates.Create();
        flowTemplate.guid = guid;
        flowTemplate.name = name;
        flowTemplate.displayName = displayName;
        flowTemplate.version = version;
        flowTemplate.code = code;
        flowTemplate.flowTemplateJson = flowTemplateJson;
        flowTemplate.isPublished = isPublished;
        flowTemplate.isValidated = isValidated;
        db.flowTemplates.Add(flowTemplate);
        db.SaveChanges();
        return new Tuple<bool,FlowTemplate, List<string>>(
          true,flowTemplate,null);
      }
    }

    public static Tuple<bool, FlowTemplate, List<string>> updateFlowTemplate(
      string guid, string name, string displayName,
      string version, string code, string flowTemplateJson,
      bool isPublished = false, bool isValidated = false)
    {
      List<string> errors = new List<string>();
      var basicCheckResult = validateFlowTemplateBasic(
        guid, name, displayName, version, code, flowTemplateJson,
        isPublished, isValidated);
      if (basicCheckResult.Item1 == false)
      {
        return new Tuple<bool, FlowTemplate, List<string>>(
          false, null, basicCheckResult.Item2);
      }
      using (var db = new EnouFlowTemplateDbContext())
      {
        var flowTemplate = getFlowTemplate(guid);
        #region 修改流程模板记录的检查
        if (flowTemplate==null)
        {
          errors.Add("该流程模板在数据库中不存在,是否尚未在数据库中创建?");
          return new Tuple<bool, FlowTemplate, List<string>>(
            false, flowTemplate, errors);
        }
        if (flowTemplate.isPublished)
        {
          errors.Add("为不影响业务,不能更改已被发布的流程模板;");
          return new Tuple<bool, FlowTemplate, List<string>>(
            false, flowTemplate, errors);
        }
        #endregion
        flowTemplate.name = name;
        flowTemplate.displayName = displayName;
        flowTemplate.version = version;
        flowTemplate.code = code; 
        flowTemplate.flowTemplateJson = flowTemplateJson;
        flowTemplate.isPublished = isPublished;
        flowTemplate.isValidated = isValidated;
        db.SaveChanges();
        return new Tuple<bool, FlowTemplate, List<string>>(
          true, flowTemplate, null);
      }
    }

    private static Tuple<bool, List<string>> validateFlowTemplateBasic(
      string guid, string name, string displayName,
      string version, string code, string flowTemplateJson,
      bool isPublished = false, bool isValidated = false)
    {
      bool result = true;
      List<string> errors = new List<string>();

      if (string.IsNullOrWhiteSpace(guid))
      {
        result = false;
        errors.Add("流程模板guid不能为空;");
      }

      if (string.IsNullOrWhiteSpace(version))
      {
        result = false;
        errors.Add("流程模板version不能为空;");
      }

      if (!new Regex(@"\d+").Match(version).Success)
      {
        result = false;
        errors.Add(@"流程模板version必须为类似'1'(\d+)的正整数格式;");
      }

      if (string.IsNullOrWhiteSpace(code))
      {
        result = false;
        errors.Add("流程模板code不能为空;");
      }

      if (string.IsNullOrWhiteSpace(flowTemplateJson))
      {
        result = false;
        errors.Add("流程模板flowTemplateJson不能为空;");
      }

      var flowTemplateDefTest = 
        JsonHelper.DeserializeJsonToObject<FlowTemplateDef>(
          flowTemplateJson);
      if (flowTemplateDefTest == null)
      {
        result = false;
        errors.Add("流程模板flowTemplateJson解析失败;");
      }

      return new Tuple<bool, List<string>>(result, errors);
    }

    public static Tuple<bool, FlowTemplate, List<string>> setPublishStateOfFlowTemplate(
      string guid, bool isPublished = true)
    {
      List<string> errors = new List<string>();
      using (var db = new EnouFlowTemplateDbContext())
      {
        var flowTemplate = getFlowTemplate(guid);
        #region 修改流程模板记录的检查,暂无
        #endregion
        flowTemplate.isPublished = isPublished;
        db.SaveChanges();
        return new Tuple<bool, FlowTemplate, List<string>>(
          true, flowTemplate, null);
      }
    }

    public static void saveCreatedFlowTemplate(FlowTemplate flowTemplate)
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        //不能创建同名同版本且已被发布使用的流程模板
        if (db.flowTemplates.ToList().Exists(
        obj => obj.name == flowTemplate.name &&
        obj.version == flowTemplate.version &&
        obj.isPublished == true))
        {
          throw new DataLogicException("不能创建同名同版本且已被发布使用的流程模板.");
        }

        // TODO: 加入其它验证和字段的自动设置
        db.flowTemplates.Add(flowTemplate);
        db.SaveChanges();
      }
    }
    // List all FlowTemplates()

    // Query one 
    public static FlowTemplate getFlowTemplate(int flowTemplateId)
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        return db.flowTemplates.Find(flowTemplateId);
      }
    }

    public static FlowTemplate getFlowTemplate(string flowTemplateGuid)
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        return db.flowTemplates.Where(
          tpl => tpl.guid == flowTemplateGuid).FirstOrDefault();
      }
    }

    // Query list by code
    public static List<FlowTemplate> getAvailableFlowTemplatesByCode(string code)
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        return db.flowTemplates.Where(
          tpl => tpl.code == code && tpl.isVisible && tpl.isPublished).
          OrderByDescending(tpl=>tpl.flowTemplateId).ToList();
      }
    }

    // Get list
    public static List<FlowTemplate> getAllFlowTemplates()
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        return db.flowTemplates.ToList();
      }
    }

    public static void togglePublishOfFlowTemplate(int flowTemplateId)
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        var tpl = db.flowTemplates.Find(flowTemplateId);
        tpl.isPublished = !tpl.isPublished;
        db.SaveChanges();
      }
    }
    #endregion

    #region FlowDynamicUser
    public static Tuple<bool, FlowDynamicUser, List<string>> createFlowDynamicUser(
      string guid, string name, string displayName, string script, string memo,
      bool isPublished = false, bool isValidated = false)
    {
      List<string> errors = new List<string>();
      var basicCheckResult = validateFlowDynamicUserBasic(
        guid, name, displayName, script, memo, isPublished, isValidated);
      if (basicCheckResult.Item1 == false)
      {
        return new Tuple<bool, FlowDynamicUser, List<string>>(
          false, null, basicCheckResult.Item2);
      }

      bool result = true;

      using (var db = new EnouFlowTemplateDbContext())
      {
        #region 创建流程动态用户的检查
        if (db.flowDynamicUsers.ToList().Exists(
          obj => obj.name == name))
        {
          result = false;
          errors.Add(string.Format(
            @"不能多次创建相同名称'{0}'的流程动态用户;",
            name));
        }

        if (db.flowDynamicUsers.ToList().Exists(obj => obj.guid == guid))
        {
          result = false;
          errors.Add(string.Format(
            @"不能多次创建相同guid'{0}'的流程动态用户;", guid));
        }
        if (!result)
        {
          return new Tuple<bool, FlowDynamicUser, List<string>>
            (false, null, errors);
        }
        #endregion

        var flowDynamicUser = db.flowDynamicUsers.Create();
        if (!string.IsNullOrWhiteSpace(guid))
        { // 支持自动生成guid
          flowDynamicUser.guid = guid;
        }
        flowDynamicUser.name = name;
        flowDynamicUser.displayName = displayName;
        flowDynamicUser.script = script;
        flowDynamicUser.memo = memo;
        flowDynamicUser.isPublished = isPublished;
        flowDynamicUser.isValidated = isValidated;
        db.flowDynamicUsers.Add(flowDynamicUser);
        db.SaveChanges();
        return new Tuple<bool, FlowDynamicUser, List<string>>(
          true, flowDynamicUser, null);
      }
    }

    public static Tuple<bool, FlowDynamicUser, List<string>> updateFlowDynamicUser(
      string guid, string name, string displayName, string script, string memo,
      bool isPublished = false, bool isValidated = false, bool forceChangeName = false)
    {
      List<string> errors = new List<string>();
      var basicCheckResult = validateFlowDynamicUserBasic(
        guid, name, displayName, script, memo ,isPublished, isValidated);
      if (basicCheckResult.Item1 == false)
      {
        return new Tuple<bool, FlowDynamicUser, List<string>>(
          false, null, basicCheckResult.Item2);
      }
      using (var db = new EnouFlowTemplateDbContext())
      {
        var flowDynamicUser = db.flowDynamicUsers.Where(
          obj => obj.guid == guid).FirstOrDefault();

        #region 修改记录的检查
        if (flowDynamicUser == null)
        {
          errors.Add("该流程动态用户在数据库中不存在,是否尚未在数据库中创建?");
          return new Tuple<bool, FlowDynamicUser, List<string>>(
            false, flowDynamicUser, errors);
        }
        if (flowDynamicUser.isPublished)
        {
          errors.Add("为不影响业务,不能直接更改已被发布的流程动态用户;");
          return new Tuple<bool, FlowDynamicUser, List<string>>(
            false, flowDynamicUser, errors);
        }

        if(!forceChangeName && flowDynamicUser.name != name)
        {
          errors.Add("如果需要对流程动态用户更名,需要指定forceChangeName参数为true;");
          return new Tuple<bool, FlowDynamicUser, List<string>>(
            false, flowDynamicUser, errors);
        }
        #endregion
        flowDynamicUser.name = name;
        flowDynamicUser.displayName = displayName;
        flowDynamicUser.script = script;
        flowDynamicUser.memo = memo;
        flowDynamicUser.isPublished = isPublished;
        flowDynamicUser.isValidated = isValidated;
        db.SaveChanges();

        return new Tuple<bool, FlowDynamicUser, List<string>>(
          true, flowDynamicUser, null);
      }
    }

    private static Tuple<bool, List<string>> validateFlowDynamicUserBasic(
      string guid, string name, string displayName, string script, string memo,
      bool isPublished = false, bool isValidated = false)
    {
      bool result = true;
      List<string> errors = new List<string>();

      if (string.IsNullOrWhiteSpace(name))
      {
        result = false;
        errors.Add("流程动态用户name不能为空;");
      }

      if (string.IsNullOrWhiteSpace(script))
      {
        result = false;
        errors.Add("流程动态用户script不能为空;");
      }

      return new Tuple<bool, List<string>>(result, errors);
    }

    // Query one 
    public static FlowDynamicUser getFlowDynamicUser(int flowDynamicUserId)
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        return db.flowDynamicUsers.Find(flowDynamicUserId);
      }
    }

    public static FlowDynamicUser getFlowDynamicUser(string flowDynamicUserGuid)
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        return db.flowDynamicUsers.Where(
          obj => obj.guid == flowDynamicUserGuid).FirstOrDefault();
      }
    }

    // Get list
    public static List<FlowDynamicUser> getAllFlowDynamicUsers(
      bool isPublished = false, bool isValidated = false)
    {
      using (var db = new EnouFlowTemplateDbContext())
      {
        var result = db.flowDynamicUsers.ToList();
        #region 根据条件筛选
        if (isPublished && result!=null && result.Count()>0)
        {
          result = result.Where(obj => obj.isPublished).ToList();
        }
        if (isValidated && result != null && result.Count() > 0)
        {
          result = result.Where(obj => obj.isValidated).ToList();
        }
        #endregion
        return result;
      }
    }

    #endregion
  }
}
