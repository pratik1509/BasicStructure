using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Basic.WebAPI.Helpers
{
    public static class WebHelper
    {
        public static List<string> GetCustomModelErrores(ModelStateDictionary ModelState)
        {
            var Errors = new List<string>();
            foreach (var error in ModelState.Where(x => x.Value.Errors.Count > 0))
            {
                foreach (var err in error.Value.Errors)
                {
                    Errors.Add(err.ErrorMessage);
                }
            }
           
            return Errors;
        }

        
    }
}
