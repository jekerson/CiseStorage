using Domain.Enum;

namespace Domain.Extensions
{
    public static class ActionTypeExtensions
    {
        public static string GetActionName(this ActionType actionType) =>
            actionType switch
            {
                ActionType.Insert => "Создан",
                ActionType.Update => "Обновлен",
                ActionType.Delete => "Удален",
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
            };
    }
}