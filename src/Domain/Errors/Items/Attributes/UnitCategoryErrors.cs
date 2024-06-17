using Domain.Abstraction;

namespace Domain.Errors.Items.Attributes
{
    public static class UnitCategoryErrors
    {
        public static Error UnitCategoryAlreadyExist(string name) =>
            Error.Conflict(
                "UnitCategory.AlreadyExist",
                $"Unit category with name '{name}' already exists.");

        public static Error UnitCategoryNotFoundById(int id) =>
            Error.NotFound(
                "UnitCategory.NotFoundById",
                $"Unit category with ID '{id}' not found.");

        public static Error UnitCategoryNotFoundByName(string name) =>
            Error.NotFound(
                "UnitCategory.NotFoundByName",
                $"Unit category with name '{name}' not found.");
    }
}
