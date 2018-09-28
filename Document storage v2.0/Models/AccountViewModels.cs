using System.ComponentModel.DataAnnotations;


namespace Document_storage_v2.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Имя")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string UserLogin { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string UserPassword { get; set; }

        [Required]
        [Compare("UserPassword", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string ConfirmPassword { get; set; }

    }

    public class LoginViewModel
    {

        [Required]
        [Display(Name = "Логин")]
        public string UserLogin { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string UserPassword { get; set; }

        [Display(Name = "Запомнить")]
        public bool RememberMe { get; set; }
    }

    public class RestorePasswordViewModel
    {

        [Required]
        [Display(Name = "Логин")]
        public string UserLogin { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }
}