using System.ComponentModel.DataAnnotations;

namespace CampusLearn_Web_App.Attributes
{
    public class RestrictedRoleAttribute : ValidationAttribute
    {
        private readonly string[] _allowedRoles;

        public RestrictedRoleAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override bool IsValid(object? value)
        {
            if (value == null || !(value is string role))
                return false;

            return _allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field must be one of the following: {string.Join(", ", _allowedRoles)}";
        }
    }

    // Specifically for registration - blocks admin role
    public class RegistrationRoleAttribute : RestrictedRoleAttribute
    {
        public RegistrationRoleAttribute() : base("Student", "Tutor")
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return "Only 'Student' and 'Tutor' roles are allowed for registration.";
        }
    }

    // Belgium Campus email domain validation
    public class BelgiumCampusEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || !(value is string email))
                return false;

            return email.EndsWith("@belgiumcampus.ac.za", StringComparison.OrdinalIgnoreCase);
        }

        public override string FormatErrorMessage(string name)
        {
            return "Only Belgium Campus email addresses (@belgiumcampus.ac.za) are allowed.";
        }
    }
}
