using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RushTodo.Api.Entities;

public class EntityType : Enummy
{
    [NotMapped] public override int Id => (int)EntityTypeId;

    public EntityTypeId EntityTypeId { get; private set; }

    [MaxLength(255)]
    public string EntityTypeName { get; private set; } = "";

    public override string GetText() => EntityTypeName;
}


public enum EntityTypeId
{
    AppUser = 1,
    Gardener = 2,
    WorkItem = 3,
}


public static class EntityTypeIdExtensions
{
    private static Dictionary<Type, EntityTypeId> EntityTypes { get; } = new()
    {
        [typeof(AppUser)] = EntityTypeId.AppUser,
        [typeof(Gardener)] = EntityTypeId.Gardener,
        [typeof(WorkItem)] = EntityTypeId.WorkItem,
    };

    public static EntityTypeId GetEntityTypeId(this Type entityType)
    {
        var entityTypeName = entityType.Name
            .TrimSuffix("Model")
            .TrimSuffix("Row");

        return EntityTypes.TryGetValue(entityType, out var entityTypeEnum)
            ? entityTypeEnum
            : Enum.TryParse<EntityTypeId>(entityTypeName, out entityTypeEnum)
                ? entityTypeEnum
                : throw new InvalidOperationException($"Entity type {entityType.Name} is not registered for audit.");
    }

    public static EntityTypeId GetEntityTypeId(this IModelEntity entity) => entity.GetType().GetEntityTypeId();

    public static Type GetEntityType(this IModelEntity entity)
    {
        var entityTypeId = entity.GetEntityTypeId();
        return EntityTypes.Single(pair => pair.Value == entityTypeId).Key;
    }
}


public static class EntityTypeNameExtensions
{
    public static string TrimSuffix(this string value, string suffix)
    {
        return value.EndsWith(suffix, StringComparison.Ordinal)
            ? value[..^suffix.Length]
            : value;
    }
}
