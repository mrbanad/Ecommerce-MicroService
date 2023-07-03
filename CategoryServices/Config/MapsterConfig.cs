namespace CategoryServices.Config;

public static class MapsterConfig
{
    public static void ConfigureMappings()
    {
        // TypeAdapterConfig<CategoryDto, Category>.NewConfig()
        //     .Map(dest => dest.Id, src => src.Id)
        //     .Map(dest => dest.Title, src => src.Title)
        //     .Map(dest => dest.Description, src => src.Description)
        //     .Map(dest => dest.TypeOf, src => src.TypeOf)
        //     .Map(dest => dest.EntityId, src => src.EntityId)
        //     .Map(dest => dest.Active, src => src.Active)
        //     .Ignore(dest => dest.SubCategories);
        //
        // TypeAdapterConfig<SubCategoryDto, SubCategory>.NewConfig()
        //     .Map(dest => dest.Id, src => src.Id)
        //     .Map(dest => dest.Title, src => src.Title)
        //     .Map(dest => dest.Description, src => src.Description)
        //     .Map(dest => dest.Text, src => src.Text)
        //     .Map(dest => dest.Active, src => src.Active)
        //     .Ignore(dest => dest.SubCategories);
    }
}