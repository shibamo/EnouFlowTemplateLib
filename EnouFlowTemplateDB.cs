using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using EnouFlowOrgMgmtLib;

namespace EnouFlowTemplateLib
{
  //TODO: 为实体类实现验证接口 IValidatableObject, 范例如下:
  //public class Product : IValidatableObject
  //{
  //  public int ProductID { get; set; }
  //  public string Name { get; set; }
  //  public decimal Price { get; set; }
  //  public bool IncludeInSale { get; set; }
  //  public IEnumerable<ValidationResult> Validate(ValidationContext
  //  validationContext)
  //  {
  //    List<ValidationResult> errors = new List<ValidationResult>();
  //    if (Name == null || Name == string.Empty)
  //    {
  //      errors.Add(new ValidationResult(
  //      "A value is required for the Name property"));
  //    }
  //    if (Price == 0)
  //    {
  //      errors.Add(new ValidationResult(
  //      "A value is required for the Price property"));
  //    }
  //    else if (Price < 1 || Price > 2000)
  //    {
  //      errors.Add(new ValidationResult("The Price value is out of range"));
  //    }
  //    if (IncludeInSale)
  //    {
  //      errors.Add(new ValidationResult(
  //      "Request cannot contain values for IncludeInSale"));
  //    }
  //    return errors;
  //  }
  //}


  public class EnouFlowTemplateDbContext : DbContext
  {
    #region Some tedious configuration
    public EnouFlowTemplateDbContext()
      : base("name=EnouFlowPlatformDatabase") // DB connection name
    {
    }
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      //去掉系统自带的级联删除
      modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
      modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
    }
    #endregion

    public DbSet<FlowTemplate> flowTemplates { get; set; }
    public DbSet<FlowDynamicUser> flowDynamicUsers { get; set; }
  }

  [Table("Enou_FlowTemplate")]
  public class FlowTemplate
  {
    [Key]
    public int flowTemplateId { get; set; }
    [Required]
    public string guid { get; set; }
    [Required]
    public string name { get; set; }
    [Required]
    public string displayName { get; set; }
    [Required]
    public string version { get; set; }
    public string flowTemplateJson { get; set; }
    public int systemManagerId { get; set; } //Creator

    public string code { get; set; }
    public string indexNumber { get; set; }
    public DateTime createTime { get; set; } = DateTime.Now;

    public bool isVisible { get; set; } = true;
    public bool isValidated { get; set; } = false;
    public bool isPublished { get; set; } = false;
  }

  /// <summary>
  /// 可获得的用户的脚本, 运行script字段将返回User列表,
  /// 主要用于引擎执行时通过流程实例对象,表单对象,组织结构对象集
  /// 以及表里定义的script脚本动态确定活动任务的接收人
  /// </summary>
  [Table("Enou_FlowDynamicUser")]
  public class FlowDynamicUser
  { 
    [Key]
    public int flowDynamicUserId { get; set; }
    public string guid { get; set; } = Guid.NewGuid().ToString();
    [Required]
    public string name { get; set; }
    public string displayName { get; set; } //可用于前台友好显示
    public string memo { get; set; } // 注释
    public string script { get; set; }
    public DateTime createTime { get; set; } = DateTime.Now;
    public bool isVisible { get; set; } = true;
    public bool isValidated { get; set; } = false;
    public bool isPublished { get; set; } = false;
    #region 自定义字段列表
    public int? intField_1 { get; set; }
    public int? intField_2 { get; set; }
    public int? intField_3 { get; set; }
    public int? intField_4 { get; set; }
    public int? intField_5 { get; set; }
    public int? intField_6 { get; set; }
    public int? intField_7 { get; set; }
    public int? intField_8 { get; set; }
    public int? intField_9 { get; set; }
    public int? intField_10 { get; set; }
    public string stringField_1 { get; set; }
    public string stringField_2 { get; set; }
    public string stringField_3 { get; set; }
    public string stringField_4 { get; set; }
    public string stringField_5 { get; set; }
    public string stringField_6 { get; set; }
    public string stringField_7 { get; set; }
    public string stringField_8 { get; set; }
    public string stringField_9 { get; set; }
    public string stringField_10 { get; set; }
    public decimal? decimalField_1 { get; set; }
    public decimal? decimalField_2 { get; set; }
    public decimal? decimalField_3 { get; set; }
    public decimal? decimalField_4 { get; set; }
    public decimal? decimalField_5 { get; set; }
    public decimal? decimalField_6 { get; set; }
    public decimal? decimalField_7 { get; set; }
    public decimal? decimalField_8 { get; set; }
    public decimal? decimalField_9 { get; set; }
    public decimal? decimalField_10 { get; set; }
    public DateTime? dateTimeField_1 { get; set; }
    public DateTime? dateTimeField_2 { get; set; }
    public DateTime? dateTimeField_3 { get; set; }
    public DateTime? dateTimeField_4 { get; set; }
    public DateTime? dateTimeField_5 { get; set; }
    #endregion
  }
}
