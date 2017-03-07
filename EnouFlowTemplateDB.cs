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


}
