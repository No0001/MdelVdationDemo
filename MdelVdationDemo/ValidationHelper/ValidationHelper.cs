using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace MdelVdationDemo
{
    public class CustomValidationHelper
    {
        public static ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {

                return new ValidationResult(ex.Message);
            }
        }
    }

    public sealed class OtherRequiredAttribute : ValidationAttribute
    {
        /// <summary>
        /// MannerTypeCheck
        /// </summary>
        /// <param name="otherProperty">MannerType</param>
        /// <param name="methodName">
        /// Method
        /// parameter (value, mannerType) return bool    
        /// </param>
        /// <param name="type">MethodClassType</param>
        public OtherRequiredAttribute(string otherProperty, string methodName, Type type)
        {
            OtherProperty = otherProperty ?? throw new ArgumentNullException("otherProperty");
            MethodName = methodName ?? throw new ArgumentNullException("MethodName");
            ClassType = type ?? throw new ArgumentNullException("ClassType");
        }

        public string OtherProperty { get; private set; }

        public Type ClassType { get; private set; }

        public string OtherPropertyDisplayName { get; internal set; }

        public string MethodName { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                PropertyInfo otherPropertyInfo = GetProertyInfo(validationContext);

                if (otherPropertyInfo == null) { throw new ArgumentNullException("otherPropertyInfo"); }

                object otherPropertyValue = GetPropertyInfoValueAsMannerType(validationContext, otherPropertyInfo);

                var typefullName = ClassType.AssemblyQualifiedName;

                MethodInfo theMethod = GetMethod(typefullName, MethodName);

                bool result = InvokeMethod(value, otherPropertyValue, theMethod);

                if (!result)
                    return new ValidationResult($"{value} => MannerTypeRequiredAttribute");

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult($"{value} => MannerTypeRequiredAttribute => {ex.Message}");
            }
        }

        private static object GetPropertyInfoValueAsMannerType(ValidationContext validationContext, PropertyInfo otherPropertyInfo)
        {
            var result = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            return result;
        }

        private static bool InvokeMethod(object value, object otherPropertyValue, MethodInfo theMethod)
        {
            var result = theMethod.Invoke(null, new object[] { value, otherPropertyValue });

            if (!(result is bool))
                throw new ArgumentNullException("theMethod");

            return (bool)result;
        }

        private PropertyInfo GetProertyInfo(ValidationContext validationContext)
        => validationContext.ObjectType.GetProperty(OtherProperty);

        private static MethodInfo GetMethod(string typefullName, string methodName)
        => Type.GetType(typefullName).GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);

    }
    public static class CheckedField
    {
        public static bool CheckDate(object value, object otherPropertyValue)
        {

            return true;
        }
    }

    public sealed class InsideModelAttribute : ValidationAttribute
    {
        // Methods
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                IList objectList = value as IList;

                if (objectList == null)
                    return new ValidationResult($"傳入資訊不正確 => InsideModelAttribute");

                var str = new StringBuilder();

                foreach (var item in objectList)
                {
                    var context = new ValidationContext(item);

                    var result = new List<ValidationResult>();

                    // Act

                    var valid = Validator.TryValidateObject(item, context, result, true);

                    result.ForEach(x => str.Append($"{x.MemberNames} => {x.ErrorMessage}\n"));
                }

                if (str.Length <= 0)
                    return ValidationResult.Success;
                else
                    return new ValidationResult($"傳入資訊不正確 => InsideModelAttribute => {str.ToString()}");

            }
            catch (Exception ex)
            {
                return new ValidationResult($"傳入資訊不正確{ex.Message} => InsideModelAttribute");
            }
        }
    }

    public sealed class EnumAttribute : ValidationAttribute
    {
        // Fields
        private object[] _IsEnum;

        public EnumAttribute(params object[] IsEnum)
        {
            this._IsEnum = IsEnum;
        }


        // Methods
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value == null)
                return new ValidationResult($"傳入資訊不正確{value} => EnumAttribute");

            if (!_IsEnum.ToList().Contains(value))
                return new ValidationResult($"傳入資訊不正確{value} => EnumAttribute");


            return ValidationResult.Success;
        }
    }
}