using System.Linq.Expressions;
using CommonClassLibrary.Dto;
using CommonClassLibrary.Model;

namespace CommonServiceLibrary.Service;

public static class DisplayService<T> where T : IBaseWithDisplayModel, IBaseModel
{
    public static Expression<Func<T, bool>> BasePredicate(BaseSearchWithDisplayDto condition)
    {
        Expression<Func<T, bool>> predicate = p => true;

        if (condition.Title != null)
            predicate = p => p.DisplayItems.Any(x => x.Title.Contains(condition.Title));

        if (condition.Description != null)
            predicate = p => p.DisplayItems.Any(x => x.Description.Contains(condition.Description));

        if (condition.Keyword != null)
            predicate = p => p.DisplayItems.Any(x => x.Keyword != null && x.Keyword.Contains(condition.Keyword));

        if (condition.ImageName != null)
            predicate = p => p.ImageName.Contains(condition.ImageName);

        return predicate;
    }
}