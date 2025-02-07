﻿using System.ComponentModel.DataAnnotations;

namespace Server.Data.Models.Validation
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[] AllowableValues { get; set; } = new string[0];

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        { 
            if (AllowableValues.Contains(value?.ToString()) == true)
            {
                return ValidationResult.Success!;
            }

            var msg = $"Please enter one of the allowable values: {string.Join(", ", (AllowableValues ?? new string[] { "No allowable values found" }))}.";
            return new ValidationResult(msg);
        }
    }
}
