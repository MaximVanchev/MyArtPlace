using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.CustomAttributes
{
    public class IsBeforeAttribute : ValidationAttribute
    {
        private readonly string propertyToCompare;

        public IsBeforeAttribute(string _propertyToCompare , string errorMessage = "")
        {
            propertyToCompare = _propertyToCompare;
            ErrorMessage = errorMessage;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            try
            {
                DateTime dateTimeToCompare = (DateTime)validationContext
                    .ObjectType
                    .GetProperty(propertyToCompare)
                    .GetValue(validationContext.ObjectInstance);

                if ((DateTime)value < dateTimeToCompare)
                {
                    return ValidationResult.Success;
                }
            }
            catch (Exception)
            { }

            return new ValidationResult(ErrorMessage);
        }
    }
}
