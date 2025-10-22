using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Helpers
{
    public class ValidEnumListAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public ValidEnumListAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IEnumerable<MethodType> list)
                return new ValidationResult(ErrorMessage);

            foreach (var item in list)
            {
                if (item == null || !Enum.IsDefined(_enumType, item))
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }
}
