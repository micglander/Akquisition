using System;
using System.Globalization;
using System.Web.Mvc;

namespace Akquisition.MyStuff
{
    public class NullableDecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                if (valueResult.AttemptedValue == null || valueResult.AttemptedValue.Equals(String.Empty))
                {
                    actualValue = null;
                }
                else
                {
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
                        CultureInfo.CurrentCulture);
                }
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}