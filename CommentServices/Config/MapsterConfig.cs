namespace CommentServices.Config;

public static class MapsterConfig
{
    public static void ConfigureMappings()
    {
        // TypeAdapterConfig<CommentDto, Comment>.NewConfig()
        //     .ConstructUsing(src => new Comment(src.Text, src.CreateId, src.TypeOf, src.EntityId))
        //     .Map(dest => dest.Id, src => src.Id)
        //     .Map(dest => dest.Text, src => src.Text)
        //     .Map(dest => dest.TypeOf, src => src.TypeOf)
        //     .Map(dest => dest.EntityId, src => src.EntityId)
        //     .Map(dest => dest.IsRead, src => src.IsRead)
        //     .Map(dest => dest.Rating, src => src.Rating)
        //     .Map(dest => dest.Active, src => src.Active)
        //     .Map(dest => dest.Anonymous, src => src.Anonymous)
        //     .Map(dest => dest.Like, src => src.Like)
        //     .Map(dest => dest.Dislike, src => src.Dislike)
        //     .Map(dest => dest.CreateDate, src => src.CreateDate)
        //     .Map(dest => dest.CreateId, src => src.CreateId)
        //     .Map(dest => dest.ActivatorId, src => src.ActivatorId)
        //     .Map(dest => dest.SubComments, src => src.SubComments);
        //
        // TypeAdapterConfig<SubCommentDto, SubComment>.NewConfig()
        //     .Map(dest => dest.Id, src => src.Id)
        //     .Map(dest => dest.Text, src => src.Text)
        //     .Map(dest => dest.IsRead, src => src.IsRead)
        //     .Map(dest => dest.Active, src => src.Active)
        //     .Map(dest => dest.Anonymous, src => src.Anonymous)
        //     .Map(dest => dest.Like, src => src.Like)
        //     .Map(dest => dest.Rating, src => src.Rating)
        //     .Map(dest => dest.ParentId, src => src.ParentId)
        //     .Map(dest => dest.Dislike, src => src.Dislike)
        //     .Map(dest => dest.CreateDate, src => src.CreateDate)
        //     .Map(dest => dest.CreateId, src => src.CreateId)
        //     .Map(dest => dest.ActivatorId, src => src.ActivatorId)
        //     .Map(dest => dest.SubComments, src => src.SubComments);
    }
}