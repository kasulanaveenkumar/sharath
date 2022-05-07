using Common.Validations.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Common.Validations.Helper
{
    public class ValidationHelper
    {
        #region Get Model Validation Result

        public static ValidationResultModel GetModelValidationResult(ModelStateDictionary modelStateValues)
        {
            var validationResultModel = new ValidationResultModel();
            validationResultModel.Message = "Validation Failed";
            validationResultModel.Errors = modelStateValues.Keys
                                           .SelectMany(key => modelStateValues[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                                           .ToList();
            return validationResultModel;
        }

        #endregion

        #region Remove Model Error

        public static void RemoveModelError(ModelStateDictionary dic, string modelKey)
        {
            foreach (string key in dic.Keys.Where(k => k.EndsWith(modelKey)).ToList())
            {
                dic.Remove(key);
            }
        }

        #endregion
    }
}
