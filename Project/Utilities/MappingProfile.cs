using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Customer, CustomerDetails>();
        CreateMap<Customer, CustomerListItem>();
        CreateMap<CustomerPost, Customer>();
        CreateMap<CustomerPut, Customer>();

        CreateMap<Commande, CommandeDetails>()
        .ForMember(x=>x.Skills,x=>x.MapFrom(x=>x.CommandeSkills))
        .ForMember(x=>x.CustomerName,x=>x.MapFrom(x=>x.Customer.Name))
        .ForMember(x=>x.Models,x=>x.MapFrom(x=>x.CommandeModels));
        
        CreateMap<Commande, CommandeListItem>()
        .ForMember(x=>x.CustomerName,x=>x.MapFrom(x=>x.Customer.Name));

        CreateMap<CommandePost, Commande>();
        CreateMap<CommandePut, Commande>();

        CreateMap<CommandeSkill, SkillListItem>()
        .ForMember(x=>x.Label,x=>x.MapFrom(x=>x.Skill.Label))
        .ForMember(x=>x.Id,x=>x.MapFrom(x=>x.SkillId))
        .ForMember(x=>x.FieldId,x=>x.MapFrom(x=>x.Skill.FieldId))
        .ForMember(x=>x.FieldLabel,x=>x.MapFrom(x=>x.Skill.Field.Label));

        CreateMap<CommandeModel, ModelListItem>()
        .ForMember(x=>x.Label,x=>x.MapFrom(x=>x.Model.Label))
        .ForMember(x=>x.Id,x=>x.MapFrom(x=>x.ModelId))
        .ForMember(x=>x.BrandId,x=>x.MapFrom(x=>x.Model.BrandId))
        .ForMember(x=>x.BrandLabel,x=>x.MapFrom(x=>x.Model.Brand.Label)); 
        
        CreateMap<Skill, SkillListItem>()
        .ForMember(x=>x.FieldLabel,x=>x.MapFrom(x=>x.Field.Label));
         CreateMap<Skill, SkillListItemContractor>()
        .ForMember(x=>x.FieldLabel,x=>x.MapFrom(x=>x.Field.Label));
        CreateMap<ContractorSkill, SkillListItem>()
        .ForMember(x=>x.Id,x=>x.MapFrom(x=>x.SkillId))
        .ForMember(x=>x.FieldId,x=>x.MapFrom(x=>x.Skill.FieldId))
        .ForMember(x=>x.Label,x=>x.MapFrom(x=>x.Skill.Label))
        .ForMember(x=>x.FieldLabel,x=>x.MapFrom(x=>x.Skill.Field.Label));
        CreateMap<ContractorSkill, SkillListItemContractor>()
        .ForMember(x=>x.Id,x=>x.MapFrom(x=>x.SkillId))
        .ForMember(x=>x.FieldId,x=>x.MapFrom(x=>x.Skill.FieldId))
        .ForMember(x=>x.Label,x=>x.MapFrom(x=>x.Skill.Label))
        .ForMember(x=>x.FieldLabel,x=>x.MapFrom(x=>x.Skill.Field.Label));

        
        CreateMap<Model, ModelListItem>()
        .ForMember(x=>x.BrandLabel,x=>x.MapFrom(x=>x.Brand.Label));
        CreateMap<Model, ModelListItemContractor>()
        .ForMember(x=>x.BrandLabel,x=>x.MapFrom(x=>x.Brand.Label));
        CreateMap<ContractorModel, ModelListItem>()
        .ForMember(x=>x.Id,x=>x.MapFrom(x=>x.ModelId))
        .ForMember(x=>x.BrandId,x=>x.MapFrom(x=>x.Model.BrandId))
        .ForMember(x=>x.Label,x=>x.MapFrom(x=>x.Model.Label))
        .ForMember(x=>x.BrandLabel,x=>x.MapFrom(x=>x.Model.Brand.Label)); 
         CreateMap<ContractorModel, ModelListItemContractor>()
        .ForMember(x=>x.Id,x=>x.MapFrom(x=>x.ModelId))
        .ForMember(x=>x.BrandId,x=>x.MapFrom(x=>x.Model.BrandId))
        .ForMember(x=>x.Label,x=>x.MapFrom(x=>x.Model.Label))
        .ForMember(x=>x.BrandLabel,x=>x.MapFrom(x=>x.Model.Brand.Label));    
        


    }
}