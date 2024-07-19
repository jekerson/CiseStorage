using Domain.Enum;

namespace Domain.Extensions
{
    public static class AttributeValueDataTypeExtensions
    {
        public static string GetDatatypeDisplayName(this AttributeValueDataType attributeValueType)
        {
            return attributeValueType switch
            {
                AttributeValueDataType.String => "Строка",
                AttributeValueDataType.Int => "Целое число",
                AttributeValueDataType.Float => "Число с плавающей запятой",
                AttributeValueDataType.Date => "Дата",
                AttributeValueDataType.Boolean => "Да/Нет",
                _ => throw new ArgumentOutOfRangeException(nameof(attributeValueType), attributeValueType, null)
            };
        }
    }
}