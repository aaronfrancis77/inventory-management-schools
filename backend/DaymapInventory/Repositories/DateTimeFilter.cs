using DaymapInventory.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;

namespace DaymapInventory.Filters
{
    public class LocalDateTimeFilter : IAsyncResultFilter
    {
        private readonly DateTimeHelper _dateTimeHelper;

        public LocalDateTimeFilter(DateTimeHelper dateTimeHelper)
        {
            _dateTimeHelper = dateTimeHelper;
        }
        // Runs just before the API result is sent back to the client
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                TraverseAndConvert(objectResult.Value);
            }

            await next();
        }
        // Recursively traverses an object and converts all DateTime properties
        private void TraverseAndConvert(object obj)
        {
            if (obj == null) return;

            // Handle Lists/Collections
            if (obj is IEnumerable list && !(obj is string))
            {
                foreach (var item in list) TraverseAndConvert(item);
                return;
            }

            // Use reflection to find DateTime properties
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                {
                    var val = prop.GetValue(obj) as DateTime?;
                    if (val.HasValue)
                    {
                        prop.SetValue(obj, _dateTimeHelper.ConvertToLocal(val.Value));
                    }
                }
                // Recurse into nested objects if necessary (e.g., child models)
                else if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                {
                    var nested = prop.GetValue(obj);
                    TraverseAndConvert(nested);
                }
            }
        }
    }
}