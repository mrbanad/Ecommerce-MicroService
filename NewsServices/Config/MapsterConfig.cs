namespace NewsServices.Config;

public static class MapsterConfig
{
    public static void ConfigureMappings()
    {
        // TypeAdapterConfig<NewsDto, News>.NewConfig()
        //     .Map(dest => dest.Id, src => src.Id)
        //     .Map(dest => dest.Title, src => src.Title)
        //     .Map(dest => dest.Description, src => src.Description)
        //     .Map(dest => dest.Text, src => src.Text)
        //     .Map(dest => dest.Active, src => src.Active)
        //     .Map(dest => dest.CanComment, src => src.CanComment)
        //     .Map(dest => dest.CategoryId, src => src.CategoryId)
        //     .Map(dest => dest.Images, src => src.Images);
    }
}