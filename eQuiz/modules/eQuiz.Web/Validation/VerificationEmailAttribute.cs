using eQuiz.Entities;
using eQuiz.Repositories.Abstract;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eQuiz.Web.Validation
{
    public class VerificationEmailAttribute : ValidationAttribute
    {
        #region Fields

        private readonly IRepository _repository;

        #endregion


        #region Constructors

        public VerificationEmailAttribute()
        {
            this._repository = DependencyResolver.Current.GetService<IRepository>(); ;
            this.ErrorMessage = "Provided Email is not eligible for registration. Please try another one.";
        }

        #endregion
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var importedUser = _repository.GetSingle<User>(el => el.Email == value.ToString());
            if (importedUser == null)
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
            return null;
        }
    }
}